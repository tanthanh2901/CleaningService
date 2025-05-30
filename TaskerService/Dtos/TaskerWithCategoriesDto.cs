namespace TaskerService.Dtos
{
    public class TaskerWithCategoriesDto
    {
        public int TaskerId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public float RatingAverage { get; set; }
        public bool IsAvailable { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
