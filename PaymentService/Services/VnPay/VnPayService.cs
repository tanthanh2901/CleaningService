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

        public string CreateVnPayPaymentUrl(OrderDto order, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", configuration["VnPay:vnp_TmnCode"]);

            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_BankCode", "");

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", configuration["VnPay:vnp_Returnurl"]);
            vnpay.AddRequestData("vnp_ExpireDate", order.OrderDate.AddDays(1).ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            //vnpay.AddRequestData("vnp_SecureHash", configuration["VnPay:vnp_Returnurl"]);

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
