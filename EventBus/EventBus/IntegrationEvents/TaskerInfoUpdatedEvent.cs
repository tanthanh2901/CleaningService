namespace MessageBus.IntegrationEvents
{
    public class TaskerInfoUpdatedEvent
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? Avatar { get; set; }
    }
}
