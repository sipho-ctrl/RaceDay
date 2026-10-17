using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        /// <returns>User profile details</returns>
        /// <response code="200">Returns user profile</response>
        /// <response code="404">User not found</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            var profile = new UserProfileDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };

            if (user.Role == "Organiser" && user.Organiser != null)
            {
                profile.OrganisationName = user.Organiser.OrganisationName;
                profile.ContactNumber = user.Organiser.ContactNumber;
            }
            else if (user.Role == "Participant" && user.Participant != null)
            {
                profile.FullName = user.Participant.FullName;
                profile.DateOfBirth = user.Participant.DateOfBirth;
                profile.Gender = user.Participant.Gender;
                profile.ContactNumber = user.Participant.ContactNumber;
                profile.EmergencyContact = user.Participant.EmergencyContact;
            }

            return Ok(profile);
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        /// <param name="dto">Updated profile details</param>
        /// <returns>Updated profile</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">User not found</response>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            // Check if email is already used by another user
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.UserID != userId);

            if (emailExists)
                return BadRequest(new { Message = "Email is already in use" });

            // Update user email
            user.Email = dto.Email;

            // Update role-specific fields
            if (user.Role == "Organiser" && user.Organiser != null)
            {
                if (!string.IsNullOrEmpty(dto.OrganisationName))
                    user.Organiser.OrganisationName = dto.OrganisationName;
                if (!string.IsNullOrEmpty(dto.ContactNumber))
                    user.Organiser.ContactNumber = dto.ContactNumber;
            }
            else if (user.Role == "Participant" && user.Participant != null)
            {
                if (!string.IsNullOrEmpty(dto.FullName))
                    user.Participant.FullName = dto.FullName;
                if (dto.DateOfBirth.HasValue)
                    user.Participant.DateOfBirth = dto.DateOfBirth.Value;
                if (!string.IsNullOrEmpty(dto.Gender))
                    user.Participant.Gender = dto.Gender;
                if (!string.IsNullOrEmpty(dto.ContactNumber))
                    user.Participant.ContactNumber = dto.ContactNumber;
                if (!string.IsNullOrEmpty(dto.EmergencyContact))
                    user.Participant.EmergencyContact = dto.EmergencyContact;
            }

            await _context.SaveChangesAsync();

            // Return updated profile
            var profile = new UserProfileDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };

            if (user.Role == "Organiser" && user.Organiser != null)
            {
                profile.OrganisationName = user.Organiser.OrganisationName;
                profile.ContactNumber = user.Organiser.ContactNumber;
            }
            else if (user.Role == "Participant" && user.Participant != null)
            {
                profile.FullName = user.Participant.FullName;
                profile.DateOfBirth = user.Participant.DateOfBirth;
                profile.Gender = user.Participant.Gender;
                profile.ContactNumber = user.Participant.ContactNumber;
                profile.EmergencyContact = user.Participant.EmergencyContact;
            }

            return Ok(profile);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}