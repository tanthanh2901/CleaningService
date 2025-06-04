namespace OrderService.Entities
{
    public class BookingOption
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ServiceOptionId { get; set; }
        public string OptionKey { get; set; }
        public string Value { get; set; }

        public Booking Booking { get; set; }
    }
}