using AutoMapper;
using EventBus;
using MassTransit;
using MessageBus;
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
            var cancellationToken = context.CancellationToken;

            if (await dbContext.Payments.AnyAsync(p => p.BookingId == message.BookingId))
                return;

            var orderDto = mapper.Map<BookingDto>(message);

            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var payment = new Payment
                {
                    BookingId = orderDto.BookingId,
                    Amount = orderDto.TotalAmount,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    PaymentMethod = orderDto.PaymentMethod
                };

                dbContext.Payments.Add(payment);
                await dbContext.SaveChangesAsync(cancellationToken);

                await ProcessPaymentMethod(orderDto, payment, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch(Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
        }

        private async Task ProcessPaymentMethod(BookingDto orderDto, Payment payment, CancellationToken cancellationToken)
        {
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
                    }, cancellationToken);
                    break;

                case "vnpay":
                    try
                    {
                        var vnpayUrl = vnPayService.CreateVnPayPaymentUrl(orderDto);
                        payment.PaymentUrl = vnpayUrl;
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                    catch
                    {
                        await eventBus.PublishAsync(new PaymentCompletedEvent
                        {
                            BookingId = orderDto.BookingId,
                            IsSuccess = false,
                            PaymentMethod = "vnpay",
                            Message = "Failed to initialize VNPay payment"
                        }, cancellationToken);
                    }
                    break;

                default:
                    await eventBus.PublishAsync(new PaymentCompletedEvent
                    {
                        BookingId = orderDto.BookingId,
                        IsSuccess = false,
                        PaymentMethod = orderDto.PaymentMethod,
                        Message = $"Unknown payment method: {orderDto.PaymentMethod}"
                    }, cancellationToken);
                    break;
            }

        }
    }
}
