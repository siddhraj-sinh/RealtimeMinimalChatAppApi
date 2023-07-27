namespace MinimalChatAppApi.Models
{
    public class ConversationResponseDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class ConversationHistoryResponseDto
    {
        public IEnumerable<ConversationResponseDto> Messages { get; set; }
    }
}
