using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using week_19.Api.Models;
using week_19.Api.Models.Dto;
using week_19.Api.Services;

namespace week_19.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UsersService _db;

        public LoginController(
            IConfiguration config,
            UsersService db)
        {
            _config = config;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await Authenticate(loginDto);

            if (user is null)
                return NotFound("User not found");

            var token = GenerateToken(user);
            return Ok(
                new
                {
                    email = user.Email,
                    accessToken = token,
                    role = user.Role,
                    id = user.Id,
                });
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                  _config["Jwt:Issuer"],
                  _config["Jwt:Audience"],
                  claims,
                  expires: DateTime.Now.AddHours(12),
                  signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private async Task<User> Authenticate(LoginDto loginDto) =>
            await _db.ValidateUserAsync(loginDto);
    }
}
