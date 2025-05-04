using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.DbContexts;
using OrderService.Dtos;
using OrderService.Interface;
using OrderService.Entities;
using OrderService.Models;
using System.Security.Claims;
using OrderService.Services;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext dbContext;
        private readonly ICatalogService catalogService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderRepository(OrderDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICatalogService catalogService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this.catalogService = catalogService;
        }

        public async Task<OrderDto> Checkout(CheckoutRequest checkoutRequest)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // get service by id
            var service = await catalogService.GetService(checkoutRequest.ServiceId);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Address = checkoutRequest.Address,
                PhoneNumber = checkoutRequest.PhoneNumber,
                ServiceId = checkoutRequest.ServiceId,
                Name = service.Name,
                ImageUrl = service.ImageUrl,
                Price = service.Price,
                BookingStatus = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return mapper.Map<OrderDto>(order);
        }

        public async  Task<Order> CreateOrder(Order order)
        {
            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders(int userId)
        {
            var orders = await dbContext.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            return mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByStatus(int userId, BookingStatus status)
        {
            var orders = await GetAllOrders(userId);
            var filteredOrders = orders.Where(o => o.BookingStatus == status);
            return mapper.Map<IEnumerable<OrderDto>>(filteredOrders);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, BookingStatus status)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.BookingStatus = status;
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
