using Microsoft.AspNetCore.Mvc;
using MinimalChatAppApi.Models;
using System.Security.Claims;

namespace MinimalChatAppApi.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto user);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<IEnumerable<UserProfileDto>> GetUsersAsync();

        Task<bool> GoogleSignIn(ClaimsPrincipal externalClaims);
        public string GetGoogleAuthenticationUrl();
    }
}
