using AuthenticationAndAuthorization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace AuthenticationAndAuthorization.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Login(Login model)
        {

            if (model.UserName == "admin" && model.Password == "haris")
            {
                var jwtSection = _configuration.GetSection("Jwt");
                var issuer = jwtSection["Issuer"];
                var audience = jwtSection["Audience"];
                var key = Encoding.UTF8.GetBytes(jwtSection["Key"]); ;

                var claims = new[] { new Claim(ClaimTypes.Name, model.UserName), new Claim("role", "admin") };
                var jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwt) });
            }
            return Unauthorized();

        }

        [HttpGet]
        [Authorize(Roles = "admin")]        
        public IActionResult Testing(string id)
        {

            return Ok();
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult TestUser()
        {

            return Ok();
        }

        
    }
}
