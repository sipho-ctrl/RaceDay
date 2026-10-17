using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get current participant's enrolments
        /// </summary>
        /// <returns>List of enrolments for the logged-in participant</returns>
        /// <response code="200">Returns list of enrolments</response>
        /// <response code="403">User is not a Participant</response>
        [HttpGet]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(typeof(IEnumerable<EnrolmentResponseDto>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = GetCurrentUserId();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .Select(e => new EnrolmentResponseDto
                {
                    EnrolmentID = e.EnrolmentID,
                    ParticipantID = e.ParticipantID,
                    ParticipantName = e.Participant != null ? e.Participant.FullName : null,
                    CategoryID = e.CategoryID,
                    CategoryName = e.Category != null ? e.Category.CategoryName : null,
                    CategoryDistance = e.Category != null ? e.Category.Distance : 0,
                    EventID = e.Category != null && e.Category.Event != null ? e.Category.Event.EventID : 0,
                    EventName = e.Category != null && e.Category.Event != null ? e.Category.Event.EventName : null,
                    EventDate = e.Category != null && e.Category.Event != null ? e.Category.Event.EventDate : DateTime.MinValue,
                    RegistrationDate = e.RegistrationDate,
                    PaymentStatus = e.PaymentStatus,
                    StartNumber = e.StartNumber
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        /// <summary>
        /// Get all enrolments for a specific event (Organiser only, must own the event)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of enrolments for the event</returns>
        /// <response code="200">Returns list of enrolments</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Event not found</response>
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(IEnumerable<EnrolmentResponseDto>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEnrolmentsByEvent(int eventId)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found" });

            if (eventItem.OrganiserID != organiser.OrganiserID)
                return Forbid();

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .Where(e => e.Category != null && e.Category.EventID == eventId)
                .Select(e => new EnrolmentResponseDto
                {
                    EnrolmentID = e.EnrolmentID,
                    ParticipantID = e.ParticipantID,
                    ParticipantName = e.Participant != null ? e.Participant.FullName : null,
                    CategoryID = e.CategoryID,
                    CategoryName = e.Category != null ? e.Category.CategoryName : null,
                    CategoryDistance = e.Category != null ? e.Category.Distance : 0,
                    EventID = eventId,
                    EventName = eventItem.EventName,
                    EventDate = eventItem.EventDate,
                    RegistrationDate = e.RegistrationDate,
                    PaymentStatus = e.PaymentStatus,
                    StartNumber = e.StartNumber
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        /// <summary>
        /// Get a single enrolment by ID (Participant can view own, Organiser can view for their events)
        /// </summary>
        /// <param name="id">Enrolment ID</param>
        /// <returns>Enrolment details</returns>
        /// <response code="200">Returns the enrolment</response>
        /// <response code="403">Not authorized to view this enrolment</response>
        /// <response code="404">Enrolment not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnrolmentResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEnrolmentById(int id)
        {
            var userId = GetCurrentUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var enrolment = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found" });

            // Check authorization
            if (role == "Participant")
            {
                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (participant == null || enrolment.ParticipantID != participant.ParticipantID)
                    return Forbid();
            }
            else if (role == "Organiser")
            {
                var organiser = await _context.Organisers
                    .FirstOrDefaultAsync(o => o.UserID == userId);

                if (organiser == null || enrolment.Category?.Event?.OrganiserID != organiser.OrganiserID)
                    return Forbid();
            }

            var response = new EnrolmentResponseDto
            {
                EnrolmentID = enrolment.EnrolmentID,
                ParticipantID = enrolment.ParticipantID,
                ParticipantName = enrolment.Participant?.FullName,
                CategoryID = enrolment.CategoryID,
                CategoryName = enrolment.Category?.CategoryName,
                CategoryDistance = enrolment.Category?.Distance ?? 0,
                EventID = enrolment.Category?.Event?.EventID ?? 0,
                EventName = enrolment.Category?.Event?.EventName,
                EventDate = enrolment.Category?.Event?.EventDate ?? DateTime.MinValue,
                RegistrationDate = enrolment.RegistrationDate,
                PaymentStatus = enrolment.PaymentStatus,
                StartNumber = enrolment.StartNumber
            };

            return Ok(response);
        }

        /// <summary>
        /// Enrol in an event category (Participant only)
        /// </summary>
        /// <param name="dto">Enrolment details</param>
        /// <returns>Created enrolment</returns>
        /// <response code="201">Enrolment successful</response>
        /// <response code="400">Invalid input, already enrolled, or category full</response>
        /// <response code="403">User is not a Participant</response>
        [HttpPost]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(typeof(EnrolmentResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentDto dto)
        {
            var userId = GetCurrentUserId();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .Include(c => c.Enrolments)
                .FirstOrDefaultAsync(c => c.CategoryID == dto.CategoryID);

            if (category == null)
                return BadRequest(new { Message = "Category not found" });

            // Check if event registration is still open
            if (category.Event != null && category.Event.EventStatus != "Open")
                return BadRequest(new { Message = "Event is not open for registration" });

            // Check if already enrolled in this category
            var existingEnrolment = await _context.Enrolments
                .AnyAsync(e => e.ParticipantID == participant.ParticipantID && e.CategoryID == dto.CategoryID);

            if (existingEnrolment)
                return BadRequest(new { Message = "You are already enrolled in this category" });

            // Check if category is full
            if (category.MaxParticipants.HasValue && category.Enrolments.Count >= category.MaxParticipants.Value)
                return BadRequest(new { Message = "Category is full" });

            // Generate start number
            var existingCount = await _context.Enrolments.CountAsync();
            var startNumber = 100 + existingCount + 1;

            var newEnrolment = new Enrolment
            {
                ParticipantID = participant.ParticipantID,
                CategoryID = dto.CategoryID,
                RegistrationDate = DateTime.Now,
                PaymentStatus = dto.PaymentStatus ?? "Pending",
                StartNumber = startNumber
            };

            _context.Enrolments.Add(newEnrolment);
            await _context.SaveChangesAsync();

            var response = new EnrolmentResponseDto
            {
                EnrolmentID = newEnrolment.EnrolmentID,
                ParticipantID = newEnrolment.ParticipantID,
                ParticipantName = participant.FullName,
                CategoryID = newEnrolment.CategoryID,
                CategoryName = category.CategoryName,
                CategoryDistance = category.Distance,
                EventID = category.Event?.EventID ?? 0,
                EventName = category.Event?.EventName,
                EventDate = category.Event?.EventDate ?? DateTime.MinValue,
                RegistrationDate = newEnrolment.RegistrationDate,
                PaymentStatus = newEnrolment.PaymentStatus,
                StartNumber = newEnrolment.StartNumber
            };

            return CreatedAtAction(nameof(GetEnrolmentById), new { id = newEnrolment.EnrolmentID }, response);
        }

        /// <summary>
        /// Cancel an enrolment (Participant only, own enrolments)
        /// </summary>
        /// <param name="id">Enrolment ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Enrolment cancelled</response>
        /// <response code="403">Not authorized to cancel this enrolment</response>
        /// <response code="404">Enrolment not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var userId = GetCurrentUserId();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
                return Forbid();

            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
                return NotFound(new { Message = "Enrolment not found" });

            if (enrolment.ParticipantID != participant.ParticipantID)
                return Forbid();

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}