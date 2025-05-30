namespace MessageBus.IntegrationEvents
{
    public class PaymentCompletedEvent
    {
        public long BookingId { get; set; }
        public bool IsSuccess { get; set; }
        public string PaymentMethod { get; set; }
        public string Message { get; set; }
    }
}
