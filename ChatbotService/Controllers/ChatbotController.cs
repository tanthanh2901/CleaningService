using Microsoft.AspNetCore.Mvc;
using ChatbotService.Services;

namespace ChatbotService.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatFlowService _chatService;

        public ChatbotController(ChatFlowService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("step/{stepId}")]
        public IActionResult GetStep(string stepId)
        {
            var step = _chatService.GetStep(stepId);
            if (step == null) return NotFound();
            return Ok(step);
        }
    }
} 