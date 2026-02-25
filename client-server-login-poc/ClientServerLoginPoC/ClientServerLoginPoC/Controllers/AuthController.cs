using ClientServerLoginPoC.Models;
using ClientServerLoginPoC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientServerLoginPoC.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserStore _users;
        private readonly SessionStore _sessions;

        public AuthController(UserStore users, SessionStore sessions)
        {
            _users = users;
            _sessions = sessions;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var valid = _users.Validate(request.Email, request.Password);
            if (!valid)
                return Unauthorized();

            var token = _sessions.CreateSession(request.Email);
            return Ok(new LoginResponse(token));
        }

        [HttpGet("me")]
        public ActionResult<MeResponse> Me()
        {
            var token = Request.Headers["X-Session-Token"].ToString();
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Missing X-Session-Token.");

            if (!_sessions.TryGetEmail(token, out var email))
                return Unauthorized("Invalid token.");

            return Ok(new MeResponse(email));
        }
    }

}
