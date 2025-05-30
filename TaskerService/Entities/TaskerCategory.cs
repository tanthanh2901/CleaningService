namespace TaskerService.Entities
{
    public class TaskerCategory
    {
        public int TaskerId { get; set; }
        public Tasker Tasker { get; set; }

        public int CategoryId { get; set; }
    }
}