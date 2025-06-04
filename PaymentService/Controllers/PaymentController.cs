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
                var bookingId = long.Parse(response.BookingId);

                var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.BookingId == long.Parse(response.BookingId));

                if (payment == null)
                {
                    return NotFound($"Payment with BookingId {bookingId} not found.");
                }

                payment.Status = "Paid";

                await _dbContext.SaveChangesAsync();

                await eventBus.PublishAsync(new PaymentCompletedEvent
                {
                    BookingId = long.Parse(response.BookingId),
                    IsSuccess = response.Success,
                    PaymentMethod = "vnpay",
                });
                return Ok("Thanh toán thanh cong");
            }
            return BadRequest("Thanh toán thất bại");
        }

        [HttpGet("{bookingId}/payment-status")]
        public async Task<IActionResult> GetPaymentStatus(int bookingId, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _dbContext.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId, cancellationToken);

                if (payment == null)
                {
                    return Ok(new
                    {
                        bookingId = bookingId,
                        status = "processing",
                        message = "Payment is being prepared"
                    });
                }

                return Ok(new
                {
                    bookingId = payment.BookingId,
                    status = payment.Status.ToLower(),
                    amount = payment.Amount,
                    paymentMethod = payment.PaymentMethod,
                    paymentUrl = payment.PaymentUrl,
                    createdAt = payment.CreatedAt,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{bookingId}/payment-url")]
        public async Task<IActionResult> GetPaymentUrl(int bookingId)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
            {
                // Return a specific status indicating payment is still being processed
                return StatusCode(202, new
                {
                    message = "Payment is being prepared. Please try again in a moment.",
                    bookingId = bookingId,
                    status = "processing"
                });
            }

            if (string.IsNullOrEmpty(payment.PaymentUrl))
            {
                if (payment.PaymentMethod?.ToLower() == "vnpay")
                {
                    return StatusCode(202, new
                    {
                        message = "Payment URL is being generated. Please try again in a moment.",
                        bookingId = bookingId,
                        status = "url_generating"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Payment URL not available for this payment method",
                        paymentMethod = payment.PaymentMethod
                    });
                }
            }

            return Ok(new { url = payment.PaymentUrl });
        }
    }
}
