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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all categories for a specific event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of categories</returns>
        /// <response code="200">Returns list of categories</response>
        /// <response code="404">Event not found</response>
        [HttpGet("event/{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
                return NotFound(new { Message = "Event not found" });

            var categories = await _context.Categories
                .Include(c => c.Event)
                .Where(c => c.EventID == eventId)
                .Select(c => new CategoryResponseDto
                {
                    CategoryID = c.CategoryID,
                    EventID = c.EventID,
                    CategoryName = c.CategoryName,
                    Distance = c.Distance,
                    AgeGroup = c.AgeGroup,
                    GenderGroup = c.GenderGroup,
                    EntryFee = c.EntryFee,
                    MaxParticipants = c.MaxParticipants,
                    EventName = c.Event != null ? c.Event.EventName : null,
                    CurrentEnrolments = c.Enrolments.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Get a single category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        /// <response code="200">Returns the category</response>
        /// <response code="404">Category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Event)
                .Include(c => c.Enrolments)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { Message = "Category not found" });

            var response = new CategoryResponseDto
            {
                CategoryID = category.CategoryID,
                EventID = category.EventID,
                CategoryName = category.CategoryName,
                Distance = category.Distance,
                AgeGroup = category.AgeGroup,
                GenderGroup = category.GenderGroup,
                EntryFee = category.EntryFee,
                MaxParticipants = category.MaxParticipants,
                EventName = category.Event?.EventName,
                CurrentEnrolments = category.Enrolments.Count
            };

            return Ok(response);
        }

        /// <summary>
        /// Create a new category for an event (Organiser only, must own the event)
        /// </summary>
        /// <param name="dto">Category details</param>
        /// <returns>Created category</returns>
        /// <response code="201">Category created successfully</response>
        /// <response code="400">Invalid input or event not found</response>
        /// <response code="403">User is not the event organiser</response>
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(CategoryResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var eventItem = await _context.Events.FindAsync(dto.EventID);

            if (eventItem == null)
                return BadRequest(new { Message = "Event not found" });

            if (eventItem.OrganiserID != organiser.OrganiserID)
                return Forbid();

            var newCategory = new Category
            {
                EventID = dto.EventID,
                CategoryName = dto.CategoryName,
                Distance = dto.Distance,
                AgeGroup = dto.AgeGroup,
                GenderGroup = dto.GenderGroup,
                EntryFee = dto.EntryFee,
                MaxParticipants = dto.MaxParticipants
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            var response = new CategoryResponseDto
            {
                CategoryID = newCategory.CategoryID,
                EventID = newCategory.EventID,
                CategoryName = newCategory.CategoryName,
                Distance = newCategory.Distance,
                AgeGroup = newCategory.AgeGroup,
                GenderGroup = newCategory.GenderGroup,
                EntryFee = newCategory.EntryFee,
                MaxParticipants = newCategory.MaxParticipants,
                EventName = eventItem.EventName,
                CurrentEnrolments = 0
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryID }, response);
        }

        /// <summary>
        /// Update a category (Organiser only, must own the event)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="dto">Updated category details</param>
        /// <returns>Updated category</returns>
        /// <response code="200">Category updated successfully</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Category not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(typeof(CategoryResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var existingCategory = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (existingCategory == null)
                return NotFound(new { Message = "Category not found" });

            if (existingCategory.Event == null || existingCategory.Event.OrganiserID != organiser.OrganiserID)
                return Forbid();

            existingCategory.CategoryName = dto.CategoryName;
            existingCategory.Distance = dto.Distance;
            existingCategory.AgeGroup = dto.AgeGroup;
            existingCategory.GenderGroup = dto.GenderGroup;
            existingCategory.EntryFee = dto.EntryFee;
            existingCategory.MaxParticipants = dto.MaxParticipants;

            await _context.SaveChangesAsync();

            var response = new CategoryResponseDto
            {
                CategoryID = existingCategory.CategoryID,
                EventID = existingCategory.EventID,
                CategoryName = existingCategory.CategoryName,
                Distance = existingCategory.Distance,
                AgeGroup = existingCategory.AgeGroup,
                GenderGroup = existingCategory.GenderGroup,
                EntryFee = existingCategory.EntryFee,
                MaxParticipants = existingCategory.MaxParticipants,
                EventName = existingCategory.Event.EventName,
                CurrentEnrolments = await _context.Enrolments.CountAsync(e => e.CategoryID == id)
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete a category (Organiser only, must own the event)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Category deleted successfully</response>
        /// <response code="400">Cannot delete category with enrolments</response>
        /// <response code="403">User is not the event organiser</response>
        /// <response code="404">Category not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetCurrentUserId();

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
                return Forbid();

            var existingCategory = await _context.Categories
                .Include(c => c.Event)
                .Include(c => c.Enrolments)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (existingCategory == null)
                return NotFound(new { Message = "Category not found" });

            if (existingCategory.Event == null || existingCategory.Event.OrganiserID != organiser.OrganiserID)
                return Forbid();

            if (existingCategory.Enrolments.Any())
                return BadRequest(new { Message = "Cannot delete category with existing enrolments" });

            _context.Categories.Remove(existingCategory);
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