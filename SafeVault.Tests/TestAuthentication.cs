using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestAuthentication
    {
        private readonly string _baseAddress = "http://localhost:5111";

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            var payload = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "username", "invalidUser" },
                    { "password", "wrongPass" }
                });

            // Act
            var response = await client.PostAsync("/auth/login", payload);

            // Assert
            Assert.That(response.StatusCode,
                        Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CheckAdmin_WithUnauthorizedUser_ReturnsForbidden()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };

            // Act
            var response = await client.GetAsync("/auth/checkadmin");

            // Assert
            Assert.That(response.StatusCode,
                        Is.EqualTo(System.Net.HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            var payload = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "username", "Paconi" },
                    { "password", "Password" }
                });

            // Act
            var response = await client.PostAsync("/auth/login", payload);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        private async Task<string> LoginAndGetTokenAsync(string username, string password)
        {
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };

            var loginRequest = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password }
                });

            var loginResponse = await client.PostAsync("/auth/login", loginRequest);

            if (!loginResponse.IsSuccessStatusCode)
            {
                var errorContent = await loginResponse.Content.ReadAsStringAsync();
                throw new Exception($"Login failed: {errorContent}");
            }

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResult!.Token!;
        }

        [Test]
        public async Task CheckAdmin_WithAuthorizedAdmin_ReturnsOk()
        {
            // Arrange
            var token = await LoginAndGetTokenAsync("Admin", "Password");

            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/auth/checkadmin");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        public class LoginResponse
        {
            public string? Token { get; set; }
        }
    }
}
