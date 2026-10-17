using Microsoft.Extensions.Configuration;
using RaceDayAPI.DTOs;
using RaceDayAPI.Services;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthServiceTests
    {
        private AuthService GetService(out Data.RaceDayDbContext context)
        {
            context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "ThisIsMySuperSecretKeyForRaceDayAPI2026MakeItLongEnough123!"},
                    {"Jwt:Issuer", "RaceDayAPI"},
                    {"Jwt:Audience", "RaceDayClient"}
                })
                .Build();

            return new AuthService(context, config);
        }

        [Fact]
        public async Task Register_WithValidOrganiserData_ReturnsToken()
        {
            var service = GetService(out _);
            var dto = new RegisterDto
            {
                Username = "neworganiser",
                Email = "neworganiser@test.com",
                Password = "password123",
                Role = "Organiser",
                OrganisationName = "New Events",
                ContactNumber = "0821112222"
            };

            var result = await service.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Organiser", result.Role);
        }

        [Fact]
        public async Task Register_WithValidParticipantData_ReturnsToken()
        {
            var service = GetService(out _);
            var dto = new RegisterDto
            {
                Username = "newparticipant",
                Email = "newparticipant@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "New Participant",
                DateOfBirth = new DateTime(1995, 5, 5)
            };

            var result = await service.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Participant", result.Role);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ThrowsException()
        {
            var service = GetService(out _);
            var dto = new RegisterDto
            {
                Username = "testorganiser",
                Email = "different@test.com",
                Password = "password123",
                Role = "Organiser"
            };

            await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(dto));
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ThrowsException()
        {
            var service = GetService(out _);
            var dto = new RegisterDto
            {
                Username = "differentuser",
                Email = "organiser@test.com",
                Password = "password123",
                Role = "Organiser"
            };

            await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(dto));
        }

        [Fact]
        public async Task Register_WithInvalidRole_ThrowsException()
        {
            var service = GetService(out _);
            var dto = new RegisterDto
            {
                Username = "invalidrole",
                Email = "invalid@test.com",
                Password = "password123",
                Role = "Admin"
            };

            await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(dto));
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var service = GetService(out _);
            var dto = new LoginDto
            {
                Username = "testorganiser",
                Password = "password123"
            };

            var result = await service.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Organiser", result.Role);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ThrowsException()
        {
            var service = GetService(out _);
            var dto = new LoginDto
            {
                Username = "testorganiser",
                Password = "wrongpassword"
            };

            await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(dto));
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ThrowsException()
        {
            var service = GetService(out _);
            var dto = new LoginDto
            {
                Username = "nonexistent",
                Password = "password123"
            };

            await Assert.ThrowsAsync<Exception>(() => service.LoginAsync(dto));
        }
    }
}