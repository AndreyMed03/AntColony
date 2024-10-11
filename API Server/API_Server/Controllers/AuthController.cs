using API_Server.Contexts;
using API_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API_Server.DtoClasses;

namespace API_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;

        public AuthController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister userRegisterDto)
        {
            if (_context.Users.Any(u => u.Email.ToLower() == userRegisterDto.Email.ToLower() || u.Username.ToLower() == userRegisterDto.Username.ToLower()))
            {
                return BadRequest(new AuthResponse { Message = "User already exists!", Success = false });
            }

            var hashedPassword = HashPassword(userRegisterDto.Password);

            var user = new User
            {
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                HashedPassword = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse { Message = "Account created!", Success = true, Username = user.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLoginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userLoginDto.Username || u.Email == userLoginDto.Username);

            if (user == null)
            {
                return BadRequest(new AuthResponse { Message = "User not found!", Success = false });
            }

            var hashedPassword = HashPassword(userLoginDto.Password);
            if (user.HashedPassword != hashedPassword)
            {
                return BadRequest(new AuthResponse { Message = "Incorrect password!", Success = false });
            }

            return Ok(new AuthResponse { Message = "Sign in successful!", Success = true, Username = user.Username });
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}