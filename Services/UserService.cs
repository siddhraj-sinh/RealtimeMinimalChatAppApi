using Microsoft.AspNetCore.Identity;
using MinimalChatAppApi.Exceptions;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;

namespace MinimalChatAppApi.Services
{
    public class UserService : IUserService
    {


        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, SignInManager<IdentityUser> signInManager)
        {
            _userManager= userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
       

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Login failed due to incorrect credentials");
            }

            // Generate JWT token upon successful login (same as before)
            var token = GenerateJwtToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                Profile = new UserProfileDto
                {
                    UserId = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                }
            };

            return response;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Registration failed due to validation errors");
            }

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                throw new ConflictException("Email is already registered");
            }

            // Create a new user
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Name,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception("User registration failed");
            }

            // Create and return the response DTO
            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            return response;
        }
        public async Task<IEnumerable<UserProfileDto>> GetUsersAsync()
        {
            var currentUserId = GetCurrentUserId();
            await Console.Out.WriteLineAsync("Current"+currentUserId);
            // Get all users from UserManager<IdentityUser>
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter out the current user from the list of all users
            var otherUsers = allUsers.Where(u => u.Id != currentUserId);

            var responseDto = new List<UserProfileDto>();
            foreach (var user in otherUsers)
            {
                responseDto.Add(new UserProfileDto
                {
                    UserId = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                });
            }

            return responseDto;
        }

        private string GetCurrentUserId()
        {
            var currentUser = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return currentUserId;
        }

        // Helper method to generate a JWT token
        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            // string _jwtSecret = _configuration.GetSection("AppSettings:Token").Value;
            string _jwtSecret = _configuration["AppSettings:Token"];
            Console.WriteLine(_jwtSecret);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "JWTAuthenticationServer",
                audience: "JWTServicePostmanClient",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GetGoogleAuthenticationUrl()
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, "/api/signin-google-callback");
            return properties.RedirectUri;
        }

        public async Task<bool> GoogleSignIn(ClaimsPrincipal externalUser)
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return false;
            }

            // You can access the user's information from the externalUser.Claims

            // Use the information to find or create a user in your application
            // For example:
            var user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            if (user == null)
            {
                // Create a new user in your application if it doesn't exist
                // For example:
                user = new IdentityUser
                {
                    UserName = externalUser.FindFirst(ClaimTypes.Email)?.Value,
                    Email = externalUser.FindFirst(ClaimTypes.Email)?.Value
                };
                await _userManager.CreateAsync(user);

                // Add the external login information to the user
                await _userManager.AddLoginAsync(user, externalLoginInfo);
            }

            // Sign in the user using the external login
            await _signInManager.SignInAsync(user, isPersistent: false);

            return true;
        }
    }
}
