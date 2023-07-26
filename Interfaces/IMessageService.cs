using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Interfaces
{
    public interface IMessageService
    {
        Task<SendMessageResponseDto> SendMessagesAsync(SendMessageDto message);
        Task<IActionResult> EditMessageAsync(int messageId, EditMessageDto messageDto);
        Task<IActionResult> DeleteMessageAsync(int messageId);
        Task<IActionResult> GetConversationHistoryAsync(int userId, DateTime? before, int count = 20, string sort = "asc");
    }
}
