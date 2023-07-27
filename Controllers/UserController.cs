using Azure;
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
        public UserController(IUserService userService)
        {
           _userService=userService;
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

        }
    }
