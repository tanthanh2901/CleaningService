namespace UserService.Models
{
    public class CreateTaskerRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "tasker";
        public List<int> CategoryIds { get; set; }
    }
}
