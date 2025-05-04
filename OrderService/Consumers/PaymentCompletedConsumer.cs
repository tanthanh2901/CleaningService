using MassTransit;
using MessageBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using OrderService.DbContexts;

namespace OrderService.Consumers
{
    public sealed class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly OrderDbContext _context;

        public PaymentCompletedConsumer(OrderDbContext dbContext)
        {
            this._context = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _context
                .Orders
                .FirstOrDefaultAsync(order => order.OrderId == context.Message.OrderId);

            if (order is null)
            {
                return;
            }

            var paymentStatus = context.Message.IsSuccess;

            if (paymentStatus)
                order.PaymentStatus = Entities.PaymentStatus.Paid;
            else
                order.PaymentStatus = Entities.PaymentStatus.Fail;

            await _context.SaveChangesAsync();
        }
    }
}
