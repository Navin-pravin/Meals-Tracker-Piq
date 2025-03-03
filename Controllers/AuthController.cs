using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Services;
using AljasAuthApi.Models;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var isSent = await _authService.AuthenticateUser(request);
            if (!isSent) return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { message = "OTP sent to email" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest request)
        {
            var result = await _authService.VerifyOTP(request);

            if (result == "Invalid OTP")
                return BadRequest(new { message = "Invalid OTP" });

            if (result == "OTP Expired")
                return BadRequest(new { message = "OTP Expired" });

            return Ok(new { message = "Successful Login", token = result });
        }
    }
}
