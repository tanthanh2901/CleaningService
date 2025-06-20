namespace TaskerService.Entities
{
    public class TaskerAvailabilitySlot
    {
        public int Id { get; set; }
        public int TaskerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AvailabilitySlotType SlotType { get; set; }
        public int? BookingId { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
