namespace MessageBus.IntegrationEvents
{
    public class TaskerCreatedEvent
    {
        public int UserId { get; set; }
        public List<int> CategoryIds { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
