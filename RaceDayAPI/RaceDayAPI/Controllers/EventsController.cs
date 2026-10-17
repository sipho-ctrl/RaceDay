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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all events (accessible to all authenticated users)
        /// </summary>
        /// <returns>List of all events</returns>
        /// <response code="200">Returns list of events</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventResponseDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events
                .Include(e => e.Organiser)
                .Select(e => new EventResponseDto
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Description = e.Description,
                    MaxParticipants = e.MaxParticipants,
                    RegistrationDeadline = e.RegistrationDeadline,
                    EventStatus = e.EventStatus,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    CreatedDate = e.CreatedDate,
                    OrganiserID = e.OrganiserID,
                    OrganiserName = e.Organiser != null ? e.Organiser.OrganisationName : null
                })
                .ToListAsync();

            return Ok(events);
        }

        /// <summary>
        /// Get a single event by ID
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details</returns>
        /// <response code="200">Returns the event</response>
        /// <response code="404">Event not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Organiser)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound(new { Message = "Event not found" });

            var response = new EventResponseDto
            {
                EventID = eventItem.EventID,
                EventName = eventItem.EventName,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                Description = eventItem.Description,
                MaxParticipants = eventItem.MaxParticipants,
                RegistrationDeadline = eventItem.RegistrationDeadline,
                EventStatus = eventItem.EventStatus,
                Distance = eventItem.Distance,
                EventType = eventItem.EventType,
                CreatedDate = eventItem.CreatedDate,
                OrganiserID = eventItem.OrganiserID,
                OrganiserName = eventItem.Organiser?.OrganisationName
            };

            return Ok(response);
        }

        /// <summary>
        /// Get all events created by the current organiser
        /// </summary>
        /// <returns>List of organiser's events</returns>
        /// <response code="200">Returns list of events</response>
        /// <response code="403">User is not an Organiser</response>
        [HttpGet("organiser")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(IEnumerable<EventResponseDto>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var events = await _context.Events
                .Where(e => e.OrganiserID == organiser.OrganiserID)
                .Select(e => new EventResponseDto
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Description = e.Description,
                    MaxParticipants = e.MaxParticipants,
                    RegistrationDeadline = e.RegistrationDeadline,
                    EventStatus = e.EventStatus,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    CreatedDate = e.CreatedDate,
                    OrganiserID = e.OrganiserID,
                    OrganiserName = organiser.OrganisationName
                })
                .ToListAsync();

            return Ok(events);
        }

        /// <summary>
        /// Create a new event (Organiser only)
        /// </summary>
        /// <param name="dto">Event details</param>
        /// <returns>Created event</returns>
        /// <response code="201">Event created successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="403">User is not an Organiser</response>
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(EventResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var newEvent = new Event
            {
                OrganiserID = organiser.OrganiserID,
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Description = dto.Description,
                MaxParticipants = dto.MaxParticipants,
                RegistrationDeadline = dto.RegistrationDeadline,
                Distance = dto.Distance,
                EventType = dto.EventType,
                EventStatus = "Open",
                CreatedDate = DateTime.Now
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            var response = new EventResponseDto
            {
                EventID = newEvent.EventID,
                EventName = newEvent.EventName,
                EventDate = newEvent.EventDate,
                Location = newEvent.Location,
                Description = newEvent.Description,
                MaxParticipants = newEvent.MaxParticipants,
                RegistrationDeadline = newEvent.RegistrationDeadline,
                EventStatus = newEvent.EventStatus,
                Distance = newEvent.Distance,
                EventType = newEvent.EventType,
                CreatedDate = newEvent.CreatedDate,
                OrganiserID = newEvent.OrganiserID,
                OrganiserName = organiser.OrganisationName
            };

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventID }, response);
        }

        /// <summary>
        /// Update an event (Organiser only, must own the event)
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <param name="dto">Updated event details</param>
        /// <returns>Updated event</returns>
        /// <response code="200">Event updated successfully</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Event not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(EventResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
                return NotFound(new { Message = "Event not found" });

            if (existingEvent.OrganiserID != organiser.OrganiserID)
                return Forbid();

            existingEvent.EventName = dto.EventName;
            existingEvent.EventDate = dto.EventDate;
            existingEvent.Location = dto.Location;
            existingEvent.Description = dto.Description;
            existingEvent.MaxParticipants = dto.MaxParticipants;
            existingEvent.RegistrationDeadline = dto.RegistrationDeadline;
            existingEvent.Distance = dto.Distance;
            existingEvent.EventType = dto.EventType;
            existingEvent.EventStatus = dto.EventStatus;

            await _context.SaveChangesAsync();

            var response = new EventResponseDto
            {
                EventID = existingEvent.EventID,
                EventName = existingEvent.EventName,
                EventDate = existingEvent.EventDate,
                Location = existingEvent.Location,
                Description = existingEvent.Description,
                MaxParticipants = existingEvent.MaxParticipants,
                RegistrationDeadline = existingEvent.RegistrationDeadline,
                EventStatus = existingEvent.EventStatus,
                Distance = existingEvent.Distance,
                EventType = existingEvent.EventType,
                CreatedDate = existingEvent.CreatedDate,
                OrganiserID = existingEvent.OrganiserID,
                OrganiserName = organiser.OrganisationName
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete an event (Organiser only, must own the event)
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Event deleted successfully</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Event not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
                return NotFound(new { Message = "Event not found" });

            if (existingEvent.OrganiserID != organiser.OrganiserID)
                return Forbid();

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to get current user ID from JWT
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}