
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Data;
using ProductApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // REGISTER
        // POST: api/Auth/register

        //[HttpPost("register")]
        //public IActionResult Register(User user)
        //{
        //    var existingUser = _context.Users
        //        .FirstOrDefault(x =>
        //            x.Username == user.Username);

        //    if (existingUser != null)
        //    {
        //        return BadRequest("Username already exists");
        //    }

        //    _context.Users.Add(user);

        //    _context.SaveChanges();

        //    return Ok("User registered successfully");
        //}

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            try
            {
                var existingUser = _context.Users
                    .FirstOrDefault(x => x.Name == user.Name);

                if (existingUser != null)
                {
                    return BadRequest("Username already exists");
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // LOGIN
        // POST: api/Auth/login

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users
                .FirstOrDefault(x =>
                    x.Name == user.Name &&
                    x.Password == user.Password);

            if (existingUser == null)
            {
                return Unauthorized(
                    "Invalid username or password");
            }

            // Create claims
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    existingUser.Name)
            };


            // Get JWT key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );


            // Credentials
            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );


            // Create token
            var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    signingCredentials: credentials
);


            // Convert token to string
            var tokenString =
                new JwtSecurityTokenHandler()
                .WriteToken(token);


            return Ok(new
            {
                token = tokenString
            });
        }
    }
}

