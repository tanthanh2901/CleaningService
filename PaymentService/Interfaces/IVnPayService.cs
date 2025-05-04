using PaymentService.Dtos;
using PaymentService.Services.VnPay;

namespace PaymentService.Interfaces
{
    public interface IVnPayService
    {
        string CreateVnPayPaymentUrl(OrderDto order, HttpContext context);
        PaymentVnPayResponseModel PaymentExecute(IQueryCollection collections);
    }
}
