using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Services;
using AljasAuthApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AljasAuthApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            await _userService.CreateUserAsync(request);
            return Ok(new { message = "User created successfully" });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetUserSummary()
        {
            var users = await _userService.GetUserSummaryAsync();
            return Ok(users);
        }
    }
}
