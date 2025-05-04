using System.Formats.Asn1;
using EventBus;
using MassTransit;
using MassTransit.Transports;
using MessageBus.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Interfaces;

namespace PaymentService.Controllers
{
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService vnPayService;
        private readonly IEventBus eventBus;

        public PaymentController(IVnPayService vnPayService, IEventBus eventBus)
        {
            this.vnPayService = vnPayService;
            this.eventBus = eventBus;
        }

        [HttpGet("vnpayCallback")]
        public async Task<IActionResult> VnPayCallback()
        {
            var response = vnPayService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                await eventBus.PublishAsync(new PaymentCompletedEvent
                {
                    OrderId = int.Parse(response.OrderId),
                    IsSuccess = response.Success,
                    PaymentMethod = "vnpay",
                });
                return Ok("Thanh toán thanh cong");
            }
            return BadRequest("Thanh toán thất bại");
        }
    }
}
