namespace MinimalChatAppApi.Models
{
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UserListResponseDto
    {
        public List<UserProfileDto> Users { get; set; }
    }
}
