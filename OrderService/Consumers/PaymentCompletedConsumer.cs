using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using OrderService.DbContexts;
using OrderService.Enums;

namespace OrderService.Consumers
{
    public sealed class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly BookingDbContext _context;

        public PaymentCompletedConsumer(BookingDbContext dbContext)
        {
            this._context = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _context
                .Bookings
                .FirstOrDefaultAsync(order => order.BookingId == context.Message.BookingId);

            if (order is null)
            {
                return;
            }

            var paymentStatus = context.Message.IsSuccess;

            if (paymentStatus)
                order.PaymentStatus = PaymentStatus.Paid;
            else
                order.PaymentStatus = PaymentStatus.Fail;

            await _context.SaveChangesAsync();
        }
    }
}
