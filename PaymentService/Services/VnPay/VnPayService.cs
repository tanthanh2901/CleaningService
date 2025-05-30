using PaymentService.Dtos;
using PaymentService.Interfaces;

namespace PaymentService.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration configuration;

        public VnPayService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateVnPayPaymentUrl(BookingDto order)
        {
            var tick = ((long)DateTime.Now.Ticks).ToString();

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", configuration["VnPay:vnp_TmnCode"]);

            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount * 100)).ToString());
            vnpay.AddRequestData("vnp_BankCode", "");

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1"); // Default IP for background processing
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + order.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", configuration["VnPay:vnp_Returnurl"]);
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddDays(1).ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_TxnRef", tick); 

            string paymentUrl = vnpay.CreateRequestUrl(configuration["VnPay:vnp_Url"], configuration["VnPay:vnp_HashSecret"]);
            return paymentUrl;
        }

        public PaymentVnPayResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, configuration["VnPay:vnp_HashSecret"]);

            return response;
        }
    }
}
