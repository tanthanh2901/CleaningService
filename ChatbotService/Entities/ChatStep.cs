namespace ChatbotService.Entities
{
    public class ChatStep
    {
        public string StepId { get; set; }
        public string Question { get; set; }
        public List<ChatOption> Options { get; set; }
    }
}
