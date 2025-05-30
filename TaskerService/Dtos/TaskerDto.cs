namespace TaskerService.Dtos
{
    public class TaskerDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public List<int> CategoryIds { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
