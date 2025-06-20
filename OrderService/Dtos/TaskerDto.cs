namespace OrderService.Dtos
{
    public class TaskerDto
    {
        public int TaskerId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int CompletedTasks { get; set; }
        public string Avatar { get; set; }
    }
} 