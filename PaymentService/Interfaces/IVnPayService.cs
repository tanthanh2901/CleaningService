using PaymentService.Dtos;
using PaymentService.Services.VnPay;

namespace PaymentService.Interfaces
{
    public interface IVnPayService
    {
        string CreateVnPayPaymentUrl(BookingDto order);
        PaymentVnPayResponseModel PaymentExecute(IQueryCollection collections);
    }
}
