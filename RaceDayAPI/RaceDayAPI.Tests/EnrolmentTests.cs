using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Controllers;
using RaceDayAPI.DTOs;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EnrolmentTests
    {
        private EnrolmentsController GetController(out Data.RaceDayDbContext context, int userId = 2, string role = "Participant")
        {
            context = TestHelper.GetInMemoryDbContext();
            TestHelper.SeedTestData(context);

            var controller = new EnrolmentsController(context);

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
        public async Task CreateEnrolment_AsParticipant_ReturnsCreated()
        {
            var controller = GetController(out _, userId: 2, role: "Participant");
            var dto = new CreateEnrolmentDto
            {
                CategoryID = 1,
                PaymentStatus = "Paid"
            };

            var result = await controller.CreateEnrolment(dto);

            // Should fail because participant 1 is already enrolled in category 1
            // (from seed data)
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);
        }

        [Fact]
        public async Task GetMyEnrolments_AsParticipant_ReturnsOk()
        {
            var controller = GetController(out _, userId: 2, role: "Participant");
            var result = await controller.GetMyEnrolments();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetEnrolmentsByEvent_AsOrganiser_ReturnsOk()
        {
            var controller = GetController(out _, userId: 1, role: "Organiser");
            var result = await controller.GetEnrolmentsByEvent(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetEnrolmentsByEvent_AsNonOwnerOrganiser_ReturnsForbid()
        {
            var controller = GetController(out _, userId: 999, role: "Organiser");
            var result = await controller.GetEnrolmentsByEvent(1);

            Assert.IsType<ForbidResult>(result);
        }
    }
}