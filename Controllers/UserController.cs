﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace MinimalChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ChatContext _context;
        
        private readonly IConfiguration _configuration;
        public UserController(ChatContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("/api/register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto user) {

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("Registration failed due to validation errors");
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict("Email is already registered");
            }

            // Create a new user
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = EncodePassword(user.Password)
            };

            // Save the user to the database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Return the user's information in the response body
            var response = new RegisterResponseDto
            {
                UserId = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email
            };

            return Ok(response);

        }


        //  hash the password
        private string EncodePassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            Console.WriteLine("hash",hash);
            return hash;
        }

        [HttpPost("/api/login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginDto)
        {

            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Login failed due to validation errors");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            //check email
            if (user == null) return Unauthorized("Login failed due to incorrect credentials");


            //verify password
            bool isPasswordValid = DecodePassword(loginDto.Password, user.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Login failed due to incorrect credentials");
            }

            var token = GenerateJwtToken(user);

            // Construct the response body
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
            return Ok(responseDto);
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

        [Authorize]
        [HttpGet("/api/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() {
            // Get the current user
            var currentUser = HttpContext.User;

            // Access user properties
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = currentUser.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            await Console.Out.WriteLineAsync(userId);
       

            // Retrieve the list of users from the database, excluding the current user
            var users = await _context.Users
                .Where(u => u.Id != Convert.ToInt32(userId))
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();

            // Construct the response body
            var responseDto = new UserListResponseDto
            {
                Users = users
            };

            return Ok(users);
        }
 
    }
}
