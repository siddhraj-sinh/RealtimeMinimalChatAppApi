using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using MinimalChatAppApi.Services;

namespace MinimalChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService) { 
         _messageService=messageService;
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

        [HttpPut("/api/messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] EditMessageDto messageDto)
        {
            var result = await _messageService.EditMessageAsync(messageId, messageDto);
            return result;
        }

        [HttpDelete("/api/messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var result = await _messageService.DeleteMessageAsync(messageId);
            return result;
        }

        [HttpGet("/api/messages")]
        [Authorize]
        public async Task<IActionResult> GetConversationHistory([FromQuery]  int userId, [FromQuery] DateTime? before, [FromQuery] int count = 20, [FromQuery] string sort = "asc")
        {
            var result = await _messageService.GetConversationHistoryAsync(userId, before, count, sort);
            return result;
        }
    }
}
