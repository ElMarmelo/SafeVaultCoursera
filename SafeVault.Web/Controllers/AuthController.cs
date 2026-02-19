using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Linq;
using System.Security.Cryptography;
using System.Diagnostics;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SafeVault.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _connectionString =
            $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
            $"User ID={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(
                "SELECT Email FROM Users WHERE Username = @Username",
                connection
            );

            command.Parameters.AddWithValue("@Username", username);

            var result = await command.ExecuteScalarAsync();

            string? email = result as string;

            return email != null
                ? Ok(new { UserId = 1, Username = username, Email = email })
                : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            Debug.WriteLine($"Received: Username={username}, Email={email}, Password={password}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username, email, and password are required.");
            }

            // Hash the password
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(
                "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)",
                connection
            );

            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", hashedPassword);

            var result = await command.ExecuteNonQueryAsync();

            return result > 0 ? Ok(new { UserId = 1, Username = username, Email = email }) : BadRequest("Failed to register.");
        }
    }
}
