using ChatbotService.Entities;

namespace ChatbotService.Services
{
    public class ChatFlowService
    {
        private readonly Dictionary<string, ChatStep> _steps;

        public ChatFlowService()
        {
            _steps = new Dictionary<string, ChatStep>
            {
                ["start"] = new ChatStep
                {
                    StepId = "start",
                    Question = "Chào bạn! Bạn cần hỗ trợ về vấn đề gì?",
                    Options = new List<ChatOption>
                {
                    new ChatOption { Label = "Dịch vụ", NextStepId = "service" },
                    new ChatOption { Label = "Quy trình sử dụng dịch vụ", NextStepId = "" },
                    new ChatOption { Label = "Thông tin liên hệ", NextStepId = "contact" },
                }
                },
                ["service"] = new ChatStep
                {
                    StepId = "service",
                    Question = "Bạn quan tâm đến loại dịch vụ nào?",
                    Options = new List<ChatOption>
                {
                        //dịch vụ
                    new ChatOption { Label = "Điện thoại", NextStepId = "phone" },
                    new ChatOption { Label = "Laptop", NextStepId = "laptop" }
                }
                },
                ["contact"] = new ChatStep
                {
                    StepId = "contact",
                    Question = "Email: nguyentanthanh29012003@gmail.com"
                    
                },
                // và các bước tiếp theo...
            };
        }

        public ChatStep GetStep(string stepId)
        {
            return _steps.ContainsKey(stepId) ? _steps[stepId] : null;
        }
    }

}
