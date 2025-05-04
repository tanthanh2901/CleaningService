namespace OrderService.Models
{
    public class CheckoutRequest
    {
        public int ServiceId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PaymentMethod { get; set; }
    }
}
