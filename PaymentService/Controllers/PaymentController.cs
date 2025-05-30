using EventBus;
using MessageBus.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.DbContexts;
using PaymentService.Interfaces;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService vnPayService;
        private readonly IEventBus eventBus;
        private readonly PaymentDbContext _dbContext;

        public PaymentController(IVnPayService vnPayService, IEventBus eventBus, PaymentDbContext context)
        {
            this.vnPayService = vnPayService;
            this.eventBus = eventBus;
            _dbContext = context;
        }

        [HttpGet("vnpayCallback")]
        public async Task<IActionResult> VnPayCallback()
        {
            var response = vnPayService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                await eventBus.PublishAsync(new PaymentCompletedEvent
                {
                    BookingId = long.Parse(response.OrderId),
                    IsSuccess = response.Success,
                    PaymentMethod = "vnpay",
                });
                return Ok("Thanh toán thanh cong");
            }
            return BadRequest("Thanh toán thất bại");
        }

        [HttpGet("{orderId}/redirect")]
        public async Task<IActionResult> RedirectToPaymentUrl(int orderId)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.BookingId == orderId);

            if (payment == null || string.IsNullOrEmpty(payment.PaymentUrl))
            {
                return NotFound($"No VNPay payment URL found for order {orderId}.");
            }

            // Perform HTTP redirect to VNPay URL
            return Redirect(payment.PaymentUrl);
        }
    }
}
