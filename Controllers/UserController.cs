using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Exceptions;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager, IUserService userService, SignInManager<IdentityUser> signInManager)
        {
           _userService=userService;
            _signInManager = signInManager;
            _userManager = userManager;

        }

        [HttpPost("/api/register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto model)
        {
            try
            {
                var response = await _userService.RegisterAsync(model);
                return Ok(response);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }


        }


        [HttpPost("/api/login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginDto)
        {

            try
            {
                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("/api/users")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet("/api/signin-google")]
        public IActionResult GoogleSignIn()
        {
            var redirectUrl = Url.Action("GoogleSignInCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("/api/signin-google-callback")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return BadRequest("Google authentication failed");
            }

            // Find or create the user based on the external login info
            var user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            if (user == null)
            {
                // Create a new user in your application if it doesn't exist
                user = new IdentityUser
                {
                    UserName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value,
                    Email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value
                };
                await _userManager.CreateAsync(user);

                // Add the external login information to the user
                await _userManager.AddLoginAsync(user, externalLoginInfo);
            }

            // Sign in the user using the external login
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new { message = "Google authentication successful" });
        }
    }
    }
