namespace MessageBus.IntegrationEvents
{
    public class BookingStatusChangedEvent
    {
        public int BookingId { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
