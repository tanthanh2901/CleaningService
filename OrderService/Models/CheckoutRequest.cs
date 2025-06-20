using Shared.Enums;

namespace OrderService.Models
{
    public class CheckoutRequest
    {
        public int ServiceId { get; set; }
        public int TaskerId { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public PaymentMethodType PaymentMethod { get; set; }
        public int SelectedDurationConfigId { get; set; }
        public List<int> SelectedPremiumServiceIds { get; set; } = new();
    }
}
