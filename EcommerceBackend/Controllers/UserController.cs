using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Bcrypt;

namespace Controller {
    [ApiController]
    [Route("/api/user")]
    public class UserController : ControllerBase {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context) {
            _context = context;
        }

        //Tao moi nguoi dung
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser ([FromBody] CreateUserRequest request) {
            // Kiểm tra username tồn tại hay chưa
            if (await _context.Users.AnyAsync(u => u.Username == request.Username)) {
                return BadRequest("Username đã tồn tại");
            }

            var user = new User {
                Username = request.Username,
                Password = Bcrypt.Net.Bcrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                Role = "NhanVien",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Sửa thành SaveChangesAsync
            return CreatedAtAction(nameof(GetUser ), new { id = user.Id }, user); // Sửa cú pháp
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser (int id) {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(id);
            return user;
        }
    }
}



public class CreateUserRequest {
    public string Username {get; set;}
    public string Password {get; set;}
    public string Fullname {get; set;}
    public string Phone {get; set;}
}