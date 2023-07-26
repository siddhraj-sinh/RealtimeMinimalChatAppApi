using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Interfaces;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MinimalChatAppApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? startTime = null, [FromQuery] DateTime? endTime = null)
        {
            var logs = await _logService.GetLogsAsync(startTime, endTime);

            // If no logs found based on the provided filter, return 404 Not Found
            if (!logs.Any())
            {
                return NotFound(new { error = "No logs found." });
            }

            return Ok(new { Logs = logs });
        }
    }
}
