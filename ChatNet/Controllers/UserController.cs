using ChatNet.Abstractions;
using ChatNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { ErrorMessage = "Invalid email address" });
            }
            try
            {
                var userId = await _userService.RegisterUserAsync(request.Email, request.Password);
                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
