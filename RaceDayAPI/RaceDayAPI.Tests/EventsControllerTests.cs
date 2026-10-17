using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.DTOs;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EventsControllerTests
    {
        private EventsController GetController(out Data.RaceDayDbContext context, int userId = 1, string role = "Organiser")
        {
            context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var controller = new EventsController(context);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, "testuser")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            return controller;
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOkWithEvents()
        {
            var controller = GetController(out _);
            var result = await controller.GetAllEvents();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetEventById_WithValidId_ReturnsOk()
        {
            var controller = GetController(out _);
            var result = await controller.GetEventById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetEventById_WithInvalidId_ReturnsNotFound()
        {
            var controller = GetController(out _);
            var result = await controller.GetEventById(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var controller = GetController(out _, userId: 1, role: "Organiser");
            var dto = new CreateEventDto
            {
                EventName = "New Test Event",
                EventDate = DateTime.Now.AddDays(60),
                Location = "Durban",
                Description = "New test event",
                MaxParticipants = 500,
                Distance = "21km",
                EventType = "run"
            };

            var result = await controller.CreateEvent(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<EventResponseDto>(createdResult.Value);
            Assert.Equal("New Test Event", response.EventName);
        }

        [Fact]
        public async Task GetMyEvents_AsOrganiser_ReturnsOk()
        {
            var controller = GetController(out _, userId: 1, role: "Organiser");
            var result = await controller.GetMyEvents();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}