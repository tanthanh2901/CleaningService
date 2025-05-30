namespace UserService.Models
{
    public class PromoteToTaskerRequest
    {
        public int UserId { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
