using Shared.Enums;

namespace OrderService.Dtos
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ScheduleTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        //public int TaskerId { get; set; }
        //public TaskerDto Tasker { get; set; }
    }
}
