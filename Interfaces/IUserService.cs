using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Interfaces
{
    public interface IUserService
    {
        Task<ActionResult<RegisterResponseDto>> RegisterAsync(RegisterRequestDto user);
        Task<ActionResult<LoginResponseDto>> LoginAsync(LoginRequestDto loginDto);
        Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsersAsync();
    }
}
