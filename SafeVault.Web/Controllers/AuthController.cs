using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using SafeVault.Web.Services;

namespace SafeVault.Web.Controllers
{
    public class AuthController(TokenService tokenService) : Controller
    {
        private readonly TokenService _tokenService = tokenService;
        private readonly string _connectionString =
            $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
            $"User ID={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("All fields are required.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(
                "INSERT INTO Users (Username, Email, Password, Role) VALUES (@Username, @Email, @Password, 'User')",
                connection
            );

            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", passwordHash);

            try
            {
                var result = await command.ExecuteNonQueryAsync();
                return result > 0 ? Ok(new { Username = username, Email = email }) : BadRequest("Registration failed.");
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry error code
            {
                return BadRequest("Username or Email already exists.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(
                "SELECT Password, Role FROM Users WHERE Username = @Username",
                connection
            );
            cmd.Parameters.AddWithValue("@Username", username);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return Unauthorized("Invalid credentials");

            var storedHash = reader.GetString("Password");
            var role = reader.GetString("Role");

            bool isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);

            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = _tokenService.GenerateToken(username, role);

            return Ok(new { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> CheckAdmin()
        {
            return Ok("This is an admin only route!");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> CheckUser()
        {
            return Ok("This is a user only route!");
        }
    }
}
