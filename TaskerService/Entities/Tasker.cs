using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskerService.Entities
{
    public class Tasker
    {
        [Key]
        public int TaskerId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<TaskerCategory> TaskerCategories { get; set; } = new List<TaskerCategory>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public float RatingAverage { get; set; }
        public int TotalRatings { get; set; }
        public int CompletedTasks { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
