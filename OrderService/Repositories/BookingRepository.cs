using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.DbContexts;
using OrderService.Dtos;
using OrderService.Interface;
using OrderService.Entities;
using OrderService.Models;
using System.Security.Claims;
using OrderService.Services;
using OrderService.Enums;
using MessageBus.IntegrationEvents;
using EventBus;

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

            // 2. Validate các option
            var optionsDict = checkoutRequest.Options.ToDictionary(o => o.OptionKey, o => o.Value);
            foreach (var requiredOption in service.Options)
            {
                if (!optionsDict.ContainsKey(requiredOption.OptionKey))
                    throw new Exception($"Missing required option: {requiredOption.OptionKey}");

                string value = optionsDict[requiredOption.OptionKey];
                ValidateOptionDataType(requiredOption.DataType, value);
            }
            
            //tinh tien
            var totalAmount = service.BasePrice * 26000;

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
                BookingStatus = Enums.BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = checkoutRequest.PaymentMethod,
                TotalAmount = totalAmount,
                CreateAt = DateTime.UtcNow,
                Options = checkoutRequest.Options.Select(o => new BookingOption
                {
                    OptionKey = o.OptionKey,
                    Value = o.Value
                }).ToList()
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
                    .ToListAsync();

            var bookingDtos = mapper.Map<IEnumerable<BookingDto>>(bookings);

            // Fetch tasker information for each order
            foreach (var bookingDto in bookingDtos)
            {
                if (bookingDto.TaskerId > 0)
                {
                    var tasker = await taskerService.GetTaskerById(bookingDto.TaskerId);
                    if (tasker != null)
                    {
                        bookingDto.Tasker = mapper.Map<TaskerDto>(tasker);
                    }
                }
            }

            return bookingDtos;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            var orders = await dbContext.Bookings
                    .ToListAsync();
            return mapper.Map<IEnumerable<BookingDto>>(orders);
        }

        public async Task<BookingDto> GetBookingByIdAsync(int orderId)
        {
            var order = await dbContext.Bookings.FirstOrDefaultAsync(o => o.BookingId == orderId);
            return mapper.Map<BookingDto>(order);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingByStatus(int userId, Enums.BookingStatus bookingStatus)
        {
            var orders = await GetBookings(userId);
            var filteredOrders = orders.Where(o => o.BookingStatus == bookingStatus);
            return mapper.Map<IEnumerable<BookingDto>>(filteredOrders);
        }

        public async Task<bool> UpdateBookingStatusAsync(int orderId, Enums.BookingStatus? bookingStatus, PaymentStatus? paymentStatus)
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
            await dbContext.SaveChangesAsync();
            return true;
        }


        private void ValidateOptionDataType(string dataType, string value)
        {
            try
            {
                switch (dataType.ToLower())
                {
                    case "int": int.Parse(value); break;
                    case "decimal": decimal.Parse(value); break;
                    case "bool": bool.Parse(value); break;
                    case "string": break; // luôn hợp lệ
                    default: throw new Exception($"Unsupported data type: {dataType}");
                }
            }
            catch
            {
                throw new Exception($"Invalid value '{value}' for type {dataType}");
            }
        }

        public async Task<bool> CancelBooking(int orderId)
        {
            var order = await dbContext.Bookings.FindAsync(orderId);

            if (order == null || order.BookingStatus != BookingStatus.Pending) 
                return false;

            order.BookingStatus = BookingStatus.Canceled;
           
            await dbContext.SaveChangesAsync();

            eventBus.PublishAsync(new 
                BookingStatusChangedEvent
                {
                    BookingId = orderId,
                    ChangedAt = DateTime.UtcNow,
                    NewStatus = "Cancled"
                });
            return true;
        }
    }
}
