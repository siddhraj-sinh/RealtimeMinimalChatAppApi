using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalChatAppApi.Services
{
    public class MessageService : IMessageService
    {

        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MessageService(IRepository<Message> messageRepository, IRepository<User> userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SendMessageResponseDto> SendMessagesAsync(SendMessageDto message)
        {
            if (message.ReceiverId <= 0)
            {
                throw new ArgumentException("ReceiverId is required and must be a positive integer.");
            }

            if (string.IsNullOrEmpty(message.Content))
            {
                throw new ArgumentException("Content is required and must not be empty.");
            }

            // Get the current user's SenderId from the token
            var currentUser = _httpContextAccessor.HttpContext.User;
            var senderId = Convert.ToInt32(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Check if the receiver user exists
            var receiverUser = await _userRepository.GetByIdAsync(message.ReceiverId);
            if (receiverUser == null)
            {
                throw new ArgumentException("Receiver user not found");
            }

            // Create the message object
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = message.ReceiverId,
                MessageContent = message.Content,
                Timestamp = DateTime.Now
            };

            // Save the message to the database
            await _messageRepository.AddAsync(newMessage);

            // Construct the response body
            var responseDto = new SendMessageResponseDto
            {
                MessageId = newMessage.Id,
                SenderId = newMessage.SenderId,
                ReceiverId = newMessage.ReceiverId,
                Content = newMessage.MessageContent,
                Timestamp = newMessage.Timestamp
            };

            return responseDto;
        }



        public async Task<IActionResult> EditMessageAsync(int messageId, EditMessageDto messageDto)
        {
            var currentUserId = GetCurrentUserId();

            // Find the message in the database based on messageId and currentUserId
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                return new NotFoundObjectResult(new { error = "Message not found" });
            }

            // Check if the current user is the sender of the message
            if (message.SenderId != currentUserId)
            {
                return new UnauthorizedObjectResult(new { error = "You can only edit your own messages" });
            }

            // Check if the content is provided and not empty
            if (string.IsNullOrEmpty(messageDto.Content))
            {
                return new BadRequestObjectResult(new { error = "Content is required and must not be empty." });
            }

            // Update the message content
            message.MessageContent = messageDto.Content;
            message.Timestamp = DateTime.Now;

            // Save the changes to the database
            await _messageRepository.UpdateAsync(message);

            // Return 200 OK with a success message
            return new OkObjectResult(new { message = "Message edited successfully" });
        }

        public async Task<IActionResult> DeleteMessageAsync(int messageId)
        {
            var currentUserId = GetCurrentUserId();

            // Find the message in the database based on messageId and currentUserId
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                return new NotFoundObjectResult(new { error = "Message not found" });
            }

            // Check if the current user is the sender of the message
            if (message.SenderId != currentUserId)
            {
                return new UnauthorizedObjectResult(new { error = "You can only delete your own messages" });
            }

            // Remove the message from the database
            await _messageRepository.DeleteAsync(message);

            // Return 200 OK with a success message
            return new OkObjectResult(new { message = "Message deleted successfully" });
        }

        public async Task<IActionResult> GetConversationHistoryAsync(int userId, DateTime? before, int count = 20, string sort = "asc")
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == userId)
            {
                //  return new BadRequestObjectResult(new { error = "You cannot retrieve your own conversation history." });
            }

            // Find the conversation between the current user and the specified user
            var conversation = await _messageRepository.GetAllAsync();

            conversation = conversation.Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                               (m.SenderId == userId && m.ReceiverId == currentUserId)).ToList();

            // Check if the conversation exists
            if (!conversation.Any())
            {
                return new NotFoundObjectResult(new { error = "Conversation not found" });
            }

            // Apply filters if provided
            if (before.HasValue)
            {
                conversation = conversation.Where(m => m.Timestamp < before.Value).ToList();
            }

            // Apply sorting
            if (sort.ToLower() == "desc")
            {
                conversation = conversation.OrderByDescending(m => m.Timestamp).ToList();
            }
            else
            {
                conversation = conversation.OrderBy(m => m.Timestamp).ToList();
            }

            // Limit the number of messages to be retrieved
            conversation = conversation.Take(count).ToList();

            // Select only the required properties for the response and map to the DTO
            var messages = conversation.Select(m => new ConversationResponseDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.MessageContent,
                Timestamp = m.Timestamp
            }).ToList();

            return new OkObjectResult(new ConversationHistoryResponseDto { Messages = messages });
        }
        // Helper method to get the current user's ID from the token
        private int GetCurrentUserId()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            var currentUserId = Convert.ToInt32(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return currentUserId;
        }

    }
}
