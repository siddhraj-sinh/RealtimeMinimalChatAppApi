using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Services
{
    public class MessageService : IMessageService
    {
        public Task<IActionResult> DeleteMessageAsync(int messageId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> EditMessageAsync(int messageId, EditMessageDto messageDto)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetConversationHistoryAsync(int userId, DateTime? before, int count = 20, string sort = "asc")
        {
            throw new NotImplementedException();
        }

        public Task<SendMessageResponseDto> SendMessagesAsync(SendMessageDto message)
        {

        }
    }
}
