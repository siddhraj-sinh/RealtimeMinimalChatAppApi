using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("/api/messages")]
        [Authorize]
        public async Task<IActionResult> SendMessages([FromBody] SendMessageDto message)
        {
            try
            {
                var responseDto = await _messageService.SendMessagesAsync(message);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
