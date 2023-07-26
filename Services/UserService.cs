using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalChatAppApi.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IRepository<User> userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<UserProfileDto>> GetUsersAsync()
        {
            var currentUserId = GetCurrentUserId();

            var users = await _userRepository.GetAllAsync(u => u.Id != currentUserId);

            var responseDto = new List<UserProfileDto>();
            foreach (var user in users)
            {
                responseDto.Add(new UserProfileDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }

            return responseDto;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                throw new ArgumentException("Login failed due to validation errors");
            }


            var user = await GetUserByEmailAsync(loginDto.Email);
            if (user == null || !DecodePassword(loginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Login failed due to incorrect credentials");
            }

            var token = GenerateJwtToken(user);

            var responseDto = new LoginResponseDto
            {
                Token = token,
                Profile = new UserProfileDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };

            return responseDto;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto user)
        {

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Name))
            {
                throw new ArgumentException("Registration failed due to validation errors");
            }
        
            if (await GetUserByEmailAsync(user.Email) != null)
            {

                await Console.Out.WriteLineAsync("User already exists");
                // return new ConflictResult("Email is already registered");
            }   
            
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = EncodePassword(user.Password)
            };

            await _userRepository.AddAsync(newUser);

            var responseDto = new RegisterResponseDto
            {
                UserId = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email
            };

            return responseDto;
        }
        private int GetCurrentUserId()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            var currentUserId = Convert.ToInt32(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return currentUserId;
        }
        private string EncodePassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }
        private bool DecodePassword(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
        // Helper method to generate a JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
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
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetAllAsync(u => u.Email == email);
            return user.FirstOrDefault();
        }
    }
}
