using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.DbContexts;
using OrderService.Dtos;
using OrderService.Interface;
using OrderService.Entities;
using OrderService.Models;
using System.Security.Claims;
using OrderService.Services;
using MessageBus.IntegrationEvents;
using EventBus;
using Shared.Enums;

namespace OrderService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext dbContext;
        private readonly ICatalogService catalogService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEventBus eventBus;
        private readonly ITaskerService taskerService;

        public BookingRepository(
            BookingDbContext dbContext, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            ICatalogService catalogService, 
            IEventBus eventBus,
            ITaskerService taskerService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this.catalogService = catalogService;
            this.eventBus = eventBus;
            this.taskerService = taskerService;
        }

        public async Task<BookingDto> Checkout(CheckoutRequest checkoutRequest)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var service = await catalogService.GetService(checkoutRequest.ServiceId);

            if (service == null)
                throw new Exception("Service not found from Catalog Service");

            var durations = await catalogService.GetDurationConfig(checkoutRequest.SelectedDurationConfigId);
            var premiums = await catalogService.GetPremiumServicesByIdsAsync(checkoutRequest.SelectedPremiumServiceIds);

            decimal basePrice = service.BasePrice;
            decimal total = basePrice;

            total *= durations.PriceMultiplier;

            foreach (var p in premiums)
            {
                total += p.IsPercentage ? (basePrice * p.AdditionalPrice / 100) : p.AdditionalPrice;
            }

            if(checkoutRequest.PaymentMethod == PaymentMethodType.VNPAY)
            {
                total *= 26000;
            }

            var order = new Booking
            {
                UserId = userId,
                Address = checkoutRequest.Address,
                PhoneNumber = checkoutRequest.PhoneNumber,
                ServiceId = checkoutRequest.ServiceId,
                TaskerId = checkoutRequest.TaskerId,
                ScheduleTime = checkoutRequest.ScheduleTime,
                Name = service.Name,
                ImageUrl = service.ImageUrl,
                BookingStatus = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = checkoutRequest.PaymentMethod,
                TotalAmount = total,
                CreateAt = DateTime.Now,
                BookingPremiums = premiums.Select(p => new BookingPremium
                {
                    Name = p.Name,
                    AdditionalPrice = p.AdditionalPrice,
                    IsPercentage = p.IsPercentage
                }).ToList(),
                BookingDurations = new List<BookingDuration>
                {
                    new BookingDuration
                    {
                        DurationHours = durations.DurationHours,
                        MaxRooms = durations.MaxRooms,
                        MaxAreaSqm = durations.MaxAreaSqm,
                        PriceMultiplier = durations.PriceMultiplier
                    }
                }
            };

            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();

            await eventBus.PublishAsync(
                new BookingCreatedEvent
                {
                    BookingId = order.BookingId,
                    UserId = order.UserId,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = (MessageBus.PaymentMethodType)order.PaymentMethod,
                    TaskerId = checkoutRequest.TaskerId,
                    ScheduleTime = checkoutRequest.ScheduleTime,
                    Address = checkoutRequest.Address,
                    PhoneNumber = checkoutRequest.PhoneNumber,
                    ServiceId = order.ServiceId,
                    ServiceName = order.Name,
                    CreatedAt = (DateTime)order.CreateAt
                }
            );

            return mapper.Map<BookingDto>(order);
        }

        public async Task<IEnumerable<BookingDto>> GetBookings(int userId)
        {
            var bookings = await dbContext.Bookings
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(b => b.CreateAt)
                    .ToListAsync();

            var bookingDtos = mapper.Map<IEnumerable<BookingDto>>(bookings);

            // Fetch tasker information for each order
            //foreach (var bookingDto in bookingDtos)
            //{
            //    if (bookingDto.TaskerId > 0)
            //    {
            //        var tasker = await taskerService.GetTaskerById(bookingDto.TaskerId);
            //        if (tasker != null)
            //        {
            //            bookingDto.Tasker = mapper.Map<TaskerDto>(tasker);
            //        }
            //    }
            //}

            return bookingDtos;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByTaskerId(int taskerId)
        {
            var bookings = await dbContext.Bookings
                    .Where(o => o.TaskerId == taskerId)
                    .OrderByDescending(b => b.CreateAt)
                    .ToListAsync();

            var bookingDtos = mapper.Map<IEnumerable<BookingDto>>(bookings);

            return bookingDtos;
        }


        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            var orders = await dbContext.Bookings
                    .OrderByDescending(b => b.CreateAt)
                    .ToListAsync();
            return mapper.Map<IEnumerable<BookingDto>>(orders);
        }

        public async Task<BookingDetailsDto> GetBookingByIdAsync(int orderId)
        {
            var booking = await dbContext.Bookings
                .Include(o => o.BookingDurations)
                .Include(o => o.BookingPremiums)
                .FirstOrDefaultAsync(o => o.BookingId == orderId);

            var bookingDtos = mapper.Map<BookingDetailsDto>(booking);

            var tasker = await taskerService.GetTaskerById(bookingDtos.TaskerId);
            if (tasker != null)
            {
                bookingDtos.Tasker = mapper.Map<TaskerDto>(tasker);
            }
            return bookingDtos;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingByStatus(int userId, BookingStatus bookingStatus)
        {
            var orders = await GetBookings(userId);
            var filteredOrders = orders.Where(o => o.BookingStatus == bookingStatus);
            return mapper.Map<IEnumerable<BookingDto>>(filteredOrders);
        }

        public async Task<bool> UpdateBookingStatusAsync(int orderId, BookingStatus? bookingStatus, PaymentStatus? paymentStatus, DateTime? updatedAt = null)
        {
            var order = await dbContext.Bookings.FindAsync(orderId);
            if (order == null) return false;

            if (bookingStatus.HasValue)
            {
                order.BookingStatus = bookingStatus.Value;
            }

            if (paymentStatus.HasValue)
            {
                order.PaymentStatus = paymentStatus.Value;
            }

            order.UpdateAt = updatedAt ?? DateTime.Now;

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBooking(int orderId)
        {
            var order = await dbContext.Bookings.FindAsync(orderId);

            if (order == null || order.BookingStatus != BookingStatus.Pending) 
                return false;

            order.BookingStatus = BookingStatus.Canceled;
            order.UpdateAt = DateTime.Now;

            await dbContext.SaveChangesAsync();

            eventBus.PublishAsync(new
                BookingStatusChangedEvent
            {
                BookingId = orderId,
                ChangedAt = DateTime.UtcNow,
                NewStatus = BookingStatus.Canceled.ToString()
            }); 
            return true;
        }
    }
}
