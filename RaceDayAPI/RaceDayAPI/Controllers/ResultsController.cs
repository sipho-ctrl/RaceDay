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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get current participant's personal results
        /// </summary>
        /// <returns>List of results for the logged-in participant</returns>
        /// <response code="200">Returns list of results</response>
        /// <response code="403">User is not a Participant</response>
        [HttpGet]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(typeof(IEnumerable<ResultResponseDto>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = GetCurrentUserId();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
                return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                        .ThenInclude(c => c!.Event)
                .Where(r => r.Enrolment != null && r.Enrolment.ParticipantID == participant.ParticipantID)
                .Select(r => new ResultResponseDto
                {
                    ResultID = r.ResultID,
                    EnrolmentID = r.EnrolmentID,
                    ParticipantID = r.Enrolment!.ParticipantID,
                    ParticipantName = r.Enrolment.Participant != null ? r.Enrolment.Participant.FullName : null,
                    EventID = r.Enrolment.Category != null && r.Enrolment.Category.Event != null ? r.Enrolment.Category.Event.EventID : 0,
                    EventName = r.Enrolment.Category != null && r.Enrolment.Category.Event != null ? r.Enrolment.Category.Event.EventName : null,
                    EventDate = r.Enrolment.Category != null && r.Enrolment.Category.Event != null ? r.Enrolment.Category.Event.EventDate : DateTime.MinValue,
                    CategoryID = r.Enrolment.CategoryID,
                    CategoryName = r.Enrolment.Category != null ? r.Enrolment.Category.CategoryName : null,
                    CategoryDistance = r.Enrolment.Category != null ? r.Enrolment.Category.Distance : 0,
                    FinishTime = r.FinishTime,
                    OverallPosition = r.OverallPosition,
                    CategoryPosition = r.CategoryPosition,
                    Pace = r.Pace,
                    Status = r.Status,
                    RecordedDate = r.RecordedDate
                })
                .ToListAsync();

            return Ok(results);
        }

        /// <summary>
        /// Get all results for a specific event (accessible to all authenticated users)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of results for the event</returns>
        /// <response code="200">Returns list of results</response>
        /// <response code="404">Event not found</response>
        [HttpGet("event/{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<ResultResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetResultsByEvent(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
                return NotFound(new { Message = "Event not found" });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r => r.Enrolment != null && r.Enrolment.Category != null && r.Enrolment.Category.EventID == eventId)
                .Select(r => new ResultResponseDto
                {
                    ResultID = r.ResultID,
                    EnrolmentID = r.EnrolmentID,
                    ParticipantID = r.Enrolment!.ParticipantID,
                    ParticipantName = r.Enrolment.Participant != null ? r.Enrolment.Participant.FullName : null,
                    EventID = eventId,
                    EventName = r.Enrolment.Category != null && r.Enrolment.Category.Event != null ? r.Enrolment.Category.Event.EventName : null,
                    EventDate = r.Enrolment.Category != null && r.Enrolment.Category.Event != null ? r.Enrolment.Category.Event.EventDate : DateTime.MinValue,
                    CategoryID = r.Enrolment.CategoryID,
                    CategoryName = r.Enrolment.Category != null ? r.Enrolment.Category.CategoryName : null,
                    CategoryDistance = r.Enrolment.Category != null ? r.Enrolment.Category.Distance : 0,
                    FinishTime = r.FinishTime,
                    OverallPosition = r.OverallPosition,
                    CategoryPosition = r.CategoryPosition,
                    Pace = r.Pace,
                    Status = r.Status,
                    RecordedDate = r.RecordedDate
                })
                .ToListAsync();

            return Ok(results);
        }

        /// <summary>
        /// Get a single result by ID
        /// </summary>
        /// <param name="id">Result ID</param>
        /// <returns>Result details</returns>
        /// <response code="200">Returns the result</response>
        /// <response code="403">Not authorized to view this result</response>
        /// <response code="404">Result not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetResultById(int id)
        {
            var userId = GetCurrentUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                        .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound(new { Message = "Result not found" });

            // Check authorization
            if (role == "Participant")
            {
                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (participant == null || result.Enrolment?.ParticipantID != participant.ParticipantID)
                    return Forbid();
            }
            else if (role == "Organiser")
            {
                var organiser = await _context.Organisers
                    .FirstOrDefaultAsync(o => o.UserID == userId);

                if (organiser == null || result.Enrolment?.Category?.Event?.OrganiserID != organiser.OrganiserID)
                    return Forbid();
            }

            var response = new ResultResponseDto
            {
                ResultID = result.ResultID,
                EnrolmentID = result.EnrolmentID,
                ParticipantID = result.Enrolment?.ParticipantID ?? 0,
                ParticipantName = result.Enrolment?.Participant?.FullName,
                EventID = result.Enrolment?.Category?.Event?.EventID ?? 0,
                EventName = result.Enrolment?.Category?.Event?.EventName,
                EventDate = result.Enrolment?.Category?.Event?.EventDate ?? DateTime.MinValue,
                CategoryID = result.Enrolment?.CategoryID ?? 0,
                CategoryName = result.Enrolment?.Category?.CategoryName,
                CategoryDistance = result.Enrolment?.Category?.Distance ?? 0,
                FinishTime = result.FinishTime,
                OverallPosition = result.OverallPosition,
                CategoryPosition = result.CategoryPosition,
                Pace = result.Pace,
                Status = result.Status,
                RecordedDate = result.RecordedDate
            };

            return Ok(response);
        }

        /// <summary>
        /// Capture a result for a participant (Organiser only, must own the event)
        /// </summary>
        /// <param name="dto">Result details</param>
        /// <returns>Created result</returns>
        /// <response code="201">Result captured successfully</response>
        /// <response code="400">Invalid input or result already exists</response>
        /// <response code="403">User is not the event organiser</response>
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(ResultResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateResult([FromBody] CreateResultDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var enrolment = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == dto.EnrolmentID);

            if (enrolment == null)
                return BadRequest(new { Message = "Enrolment not found" });

            if (enrolment.Category?.Event == null || enrolment.Category.Event.OrganiserID != organiser.OrganiserID)
                return Forbid();

            // Check if result already exists
            var existingResult = await _context.Results
                .AnyAsync(r => r.EnrolmentID == dto.EnrolmentID);

            if (existingResult)
                return BadRequest(new { Message = "Result already exists for this enrolment" });

            var newResult = new Result
            {
                EnrolmentID = dto.EnrolmentID,
                FinishTime = dto.FinishTime,
                OverallPosition = dto.OverallPosition,
                CategoryPosition = dto.CategoryPosition,
                Pace = dto.Pace,
                Status = dto.Status,
                RecordedDate = DateTime.Now
            };

            _context.Results.Add(newResult);
            await _context.SaveChangesAsync();

            var response = new ResultResponseDto
            {
                ResultID = newResult.ResultID,
                EnrolmentID = newResult.EnrolmentID,
                ParticipantID = enrolment.ParticipantID,
                ParticipantName = enrolment.Participant?.FullName,
                EventID = enrolment.Category.Event.EventID,
                EventName = enrolment.Category.Event.EventName,
                EventDate = enrolment.Category.Event.EventDate,
                CategoryID = enrolment.CategoryID,
                CategoryName = enrolment.Category.CategoryName,
                CategoryDistance = enrolment.Category.Distance,
                FinishTime = newResult.FinishTime,
                OverallPosition = newResult.OverallPosition,
                CategoryPosition = newResult.CategoryPosition,
                Pace = newResult.Pace,
                Status = newResult.Status,
                RecordedDate = newResult.RecordedDate
            };

            return CreatedAtAction(nameof(GetResultById), new { id = newResult.ResultID }, response);
        }

        /// <summary>
        /// Update a result (Organiser only, must own the event)
        /// </summary>
        /// <param name="id">Result ID</param>
        /// <param name="dto">Updated result details</param>
        /// <returns>Updated result</returns>
        /// <response code="200">Result updated successfully</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Result not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(ResultResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] UpdateResultDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var existingResult = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                        .ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (existingResult == null)
                return NotFound(new { Message = "Result not found" });

            if (existingResult.Enrolment?.Category?.Event == null ||
                existingResult.Enrolment.Category.Event.OrganiserID != organiser.OrganiserID)
                return Forbid();

            existingResult.FinishTime = dto.FinishTime;
            existingResult.OverallPosition = dto.OverallPosition;
            existingResult.CategoryPosition = dto.CategoryPosition;
            existingResult.Pace = dto.Pace;
            existingResult.Status = dto.Status;

            await _context.SaveChangesAsync();

            var response = new ResultResponseDto
            {
                ResultID = existingResult.ResultID,
                EnrolmentID = existingResult.EnrolmentID,
                ParticipantID = existingResult.Enrolment?.ParticipantID ?? 0,
                ParticipantName = existingResult.Enrolment?.Participant?.FullName,
                EventID = existingResult.Enrolment?.Category?.Event?.EventID ?? 0,
                EventName = existingResult.Enrolment?.Category?.Event?.EventName,
                EventDate = existingResult.Enrolment?.Category?.Event?.EventDate ?? DateTime.MinValue,
                CategoryID = existingResult.Enrolment?.CategoryID ?? 0,
                CategoryName = existingResult.Enrolment?.Category?.CategoryName,
                CategoryDistance = existingResult.Enrolment?.Category?.Distance ?? 0,
                FinishTime = existingResult.FinishTime,
                OverallPosition = existingResult.OverallPosition,
                CategoryPosition = existingResult.CategoryPosition,
                Pace = existingResult.Pace,
                Status = existingResult.Status,
                RecordedDate = existingResult.RecordedDate
            };

            return Ok(response);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}