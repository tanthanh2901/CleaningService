using AutoMapper;
using EventBus;
using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using PaymentService.DbContexts;
using PaymentService.Dtos;
using PaymentService.Entities;
using PaymentService.Interfaces;

namespace PaymentService.Consumer
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IEventBus eventBus;
        private readonly IVnPayService vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly PaymentDbContext dbContext;

        public OrderCreatedConsumer(IEventBus eventBus, IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor, IMapper mapper, PaymentDbContext dbContext)
        {
            this.eventBus = eventBus;
            this.vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;

            if (await dbContext.Payments.AnyAsync(p => p.OrderId == message.OrderId))
                return;

            var orderDto = mapper.Map<OrderDto>(message);

            var payment = new Payment
            {
                OrderId = orderDto.OrderId,
                Amount = orderDto.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                PaymentMethod = orderDto.PaymentMethod
            };

            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync();

            switch (orderDto.PaymentMethod?.ToLower())
            {
                case "payafterservice":
                    await eventBus.PublishAsync(new PaymentCompletedEvent
                    {
                        OrderId = orderDto.OrderId,
                        IsSuccess = true,
                        PaymentMethod = "payafterservice",
                        Message = "Payment will be collected after service."
                    });
                    break;

                case "vnpay":
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext == null)
                        throw new InvalidOperationException("HttpContext is not available.");

                    var vnpayUrl = vnPayService.CreateVnPayPaymentUrl(orderDto, httpContext);
                    break;

                default:
                    await eventBus.PublishAsync(new PaymentCompletedEvent
                    {
                        OrderId = orderDto.OrderId,
                        IsSuccess = false,
                        PaymentMethod = orderDto.PaymentMethod,
                        Message = $"Unknown payment method: {orderDto.PaymentMethod}"
                    });
                    break;
            }
        }
    }
}
