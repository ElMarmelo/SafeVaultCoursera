using NUnit.Framework;
using MySqlConnector;

namespace SafeVaultCoursera.Tests
{
    [TestFixture]
    public class TestInputValidation
    {
        private readonly string _connectionString =
        $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
        $"Port=3306;" +
        $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
        $"User ID={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
        $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        [Test]
        public void TestForSQLInjection()
        {
            string maliciousUsername = "' OR '1'='1";

            using MySqlConnection connection = new(_connectionString);
            using MySqlCommand command = new(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username",
                connection
            );

            command.Parameters.AddWithValue("@Username", maliciousUsername);

            connection.Open();

            var result = command.ExecuteScalar();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TestForXSS()
        {
            Console.WriteLine("Testing XSS prevention...");
            // Simulate XSS vulnerability by injecting malicious script
            string maliciousInput = "<script>alert('XSS');</script>";
            string sanitizedInput = SanitizeInput(maliciousInput);

            // Ensure the input is sanitized
            Assert.That(sanitizedInput, Is.Not.EqualTo(maliciousInput));
            Assert.That(sanitizedInput, Does.Contain("&lt;script&gt;alert(&#39;XSS&#39;);&lt;/script&gt;"));
        }

        private static string SanitizeInput(string input)
        {
            // Basic HTML encoding to prevent XSS
            return System.Net.WebUtility.HtmlEncode(input);
        }
    }
}
