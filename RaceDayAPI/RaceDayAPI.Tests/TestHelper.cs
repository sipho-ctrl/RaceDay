using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Tests
{
    public static class TestHelper
    {
        public static RaceDayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void SeedTestData(RaceDayDbContext context)
        {
            // Seed Users
            var organiserUser = new User
            {
                UserID = 1,
                Username = "testorganiser",
                Email = "organiser@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Organiser",
                IsActive = true
            };

            var participantUser = new User
            {
                UserID = 2,
                Username = "testparticipant",
                Email = "participant@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Participant",
                IsActive = true
            };

            context.Users.AddRange(organiserUser, participantUser);

            // Seed Organiser profile
            context.Organisers.Add(new Organiser
            {
                OrganiserID = 1,
                UserID = 1,
                OrganisationName = "Test Events",
                ContactNumber = "0821234567"
            });

            // Seed Participant profile
            context.Participants.Add(new Participant
            {
                ParticipantID = 1,
                UserID = 2,
                FullName = "Test Participant",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                ContactNumber = "0829876543"
            });

            // Seed Event
            context.Events.Add(new Event
            {
                EventID = 1,
                OrganiserID = 1,
                EventName = "Test Marathon",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Cape Town",
                Description = "A test marathon",
                EventStatus = "Open",
                EventType = "run",
                Distance = "42km"
            });

            // Seed Category
            context.Categories.Add(new Category
            {
                CategoryID = 1,
                EventID = 1,
                CategoryName = "Senior",
                Distance = 42.2m,
                AgeGroup = "18-39",
                GenderGroup = "All",
                EntryFee = 300.00m,
                MaxParticipants = 1000
            });

            // Seed Enrolment
            context.Enrolments.Add(new Enrolment
            {
                EnrolmentID = 1,
                ParticipantID = 1,
                CategoryID = 1,
                RegistrationDate = DateTime.Now,
                PaymentStatus = "Paid",
                StartNumber = 101
            });

            context.SaveChanges();
        }
    }
}