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

        // Регистрация
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister userRegisterDto)
        {
            // Проверка на существующего пользователя с таким же Email или Username
            if (_context.Users.Any(u => u.Email == userRegisterDto.Email || u.Username == userRegisterDto.Username))
            {
                return BadRequest(new AuthResponse { Message = "Пользователь уже существует", Success = false });
            }

            // Хеширование пароля
            var hashedPassword = HashPassword(userRegisterDto.Password);

            // Создание нового пользователя
            var user = new User
            {
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                HashedPassword = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse { Message = "Регистрация успешна", Success = true, Username = user.Username });
        }

        // Аутентификация
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLoginDto)
        {
            // Поиск пользователя по Email или Username
            var user = _context.Users.FirstOrDefault(u => u.Username == userLoginDto.Username || u.Email == userLoginDto.Username);

            if (user == null)
            {
                return BadRequest(new AuthResponse { Message = "Пользователь не найден", Success = false });
            }

            // Хеширование пароля и сравнение
            var hashedPassword = HashPassword(userLoginDto.Password);
            if (user.HashedPassword != hashedPassword)
            {
                return BadRequest(new AuthResponse { Message = "Неверный пароль", Success = false });
            }

            return Ok(new AuthResponse { Message = "Вход успешен", Success = true, Username = user.Username });
        }

        // Метод для хеширования пароля
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