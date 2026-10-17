using Microsoft.Extensions.Configuration;
using RaceDayAPI.DTOs;
using RaceDayAPI.Services;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class RoleBasedAccessTests
    {
        [Fact]
        public async Task Register_WithParticipantRole_CreatesParticipantProfile()
        {
            var context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "ThisIsMySuperSecretKeyForRaceDayAPI2026MakeItLongEnough123!"},
                    {"Jwt:Issuer", "RaceDayAPI"},
                    {"Jwt:Audience", "RaceDayClient"}
                })
                .Build();

            var service = new AuthService(context, config);

            var dto = new RegisterDto
            {
                Username = "newpart",
                Email = "newpart@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "New Participant"
            };

            var result = await service.RegisterAsync(dto);

            Assert.Equal("Participant", result.Role);

            var participant = context.Participants.FirstOrDefault(p => p.UserID == result.UserID);
            Assert.NotNull(participant);
            Assert.Equal("New Participant", participant.FullName);
        }

        [Fact]
        public async Task Register_WithOrganiserRole_CreatesOrganiserProfile()
        {
            var context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "ThisIsMySuperSecretKeyForRaceDayAPI2026MakeItLongEnough123!"},
                    {"Jwt:Issuer", "RaceDayAPI"},
                    {"Jwt:Audience", "RaceDayClient"}
                })
                .Build();

            var service = new AuthService(context, config);

            var dto = new RegisterDto
            {
                Username = "neworg",
                Email = "neworg@test.com",
                Password = "password123",
                Role = "Organiser",
                OrganisationName = "New Organisation"
            };

            var result = await service.RegisterAsync(dto);

            Assert.Equal("Organiser", result.Role);

            var organiser = context.Organisers.FirstOrDefault(o => o.UserID == result.UserID);
            Assert.NotNull(organiser);
            Assert.Equal("New Organisation", organiser.OrganisationName);
        }

        [Fact]
        public async Task Login_ReturnsCorrectRoleInToken()
        {
            var context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "ThisIsMySuperSecretKeyForRaceDayAPI2026MakeItLongEnough123!"},
                    {"Jwt:Issuer", "RaceDayAPI"},
                    {"Jwt:Audience", "RaceDayClient"}
                })
                .Build();

            var service = new AuthService(context, config);

            var organiserLogin = await service.LoginAsync(new LoginDto
            {
                Username = "testorganiser",
                Password = "password123"
            });

            Assert.Equal("Organiser", organiserLogin.Role);

            var participantLogin = await service.LoginAsync(new LoginDto
            {
                Username = "testparticipant",
                Password = "password123"
            });

            Assert.Equal("Participant", participantLogin.Role);
        }
    }
}