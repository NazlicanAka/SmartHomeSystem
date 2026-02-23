using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartHome.API.Infrastructure.Data; // Veritabanı için ekledik
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SmartHomeDbContext _context; // Veritabanı köprümüz

        public AuthController(IConfiguration configuration, SmartHomeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Veritabanından kullanıcıyı ve şifresini kontrol et
            var user = _context.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);

            if (user != null)
            {
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                // kullanıcının adını ve veritabanındaki "Rolünü" (Parent/Child) yaz
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return Ok(new { Token = jwtToken });
            }

            return Unauthorized("Kullanıcı adı veya şifre hatalı!");
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel register)
        {
            // Bu kullanıcı adı daha önce alınmış mı kontrol et
            if (_context.Users.Any(u => u.Username == register.Username))
                return BadRequest("Bu kullanıcı adı zaten sistemde kayıtlı!");

            var newUser = new UserEntity
            {
                Username = register.Username,
                Password = register.Password, // normalde hashlenmesi gerekiyor
                Role = register.Role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { Message = "Kayıt başarıyla oluşturuldu! Artık giriş yapabilirsiniz." });
        }

        
    }
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}