using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto user);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<IEnumerable<UserProfileDto>> GetUsersAsync();
    }
}
