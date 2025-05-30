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
    public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
    {
        private readonly IEventBus eventBus;
        private readonly IVnPayService vnPayService;
        private readonly IMapper mapper;
        private readonly PaymentDbContext dbContext;

        public BookingCreatedConsumer(IEventBus eventBus, IVnPayService vnPayService, IMapper mapper, PaymentDbContext dbContext)
        {
            this.eventBus = eventBus;
            this.vnPayService = vnPayService;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
        {
            var message = context.Message;

            if (await dbContext.Payments.AnyAsync(p => p.BookingId == message.BookingId))
                return;

            var orderDto = mapper.Map<BookingDto>(message);

            var payment = new Payment
            {
                BookingId = orderDto.BookingId,
                Amount = orderDto.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                PaymentMethod = orderDto.PaymentMethod
            };

            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync();

            var paymentMethod = orderDto.PaymentMethod?.ToLower();

            switch (paymentMethod)
            {
                case "cod":
                    await eventBus.PublishAsync(new PaymentCompletedEvent
                    {
                        BookingId = orderDto.BookingId,
                        IsSuccess = true,
                        PaymentMethod = "cod",
                        Message = "Payment will be collected after service."
                    });
                    break;

                case "vnpay":
                    var vnpayUrl = vnPayService.CreateVnPayPaymentUrl(orderDto);
                    payment.PaymentUrl = vnpayUrl;
                    await dbContext.SaveChangesAsync();

                    break;

                default:
                    await eventBus.PublishAsync(new PaymentCompletedEvent
                    {
                        BookingId = orderDto.BookingId,
                        IsSuccess = false,
                        PaymentMethod = orderDto.PaymentMethod,
                        Message = $"Unknown payment method: {orderDto.PaymentMethod}"
                    });
                    break;
            }
        }
    }
}
