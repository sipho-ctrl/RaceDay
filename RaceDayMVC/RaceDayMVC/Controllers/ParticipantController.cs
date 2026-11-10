using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Helpers;
using RaceDayMVC.Services;
using RaceDayMVC.ViewModels;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IApiService _api;

        public ParticipantController(IApiService api)
        {
            _api = api;
        }

        private string? Token => HttpContext.Session.GetString(SessionHelper.TokenKey);
        private string? Role => HttpContext.Session.GetString(SessionHelper.RoleKey);
        private string? Username => HttpContext.Session.GetString(SessionHelper.UsernameKey);

        // ============= HOME — BROWSE EVENTS =============
        [HttpGet]
        public async Task<IActionResult> Index(string? eventType, string? location)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");

            var events = await _api.GetAllEventsAsync(Token!);
            var myEnrolments = await _api.GetMyEnrolmentsAsync(Token!);

            // Apply filters
            var filtered = events.Where(e => e.EventStatus == "Open").ToList();

            if (!string.IsNullOrEmpty(eventType))
                filtered = filtered.Where(e => e.EventType == eventType).ToList();

            if (!string.IsNullOrEmpty(location))
                filtered = filtered.Where(e => e.Location.Contains(location, StringComparison.OrdinalIgnoreCase)).ToList();

            var vm = new ParticipantHomeViewModel
            {
                Events = filtered.OrderBy(e => e.EventDate).ToList(),
                EventTypeFilter = eventType,
                LocationFilter = location,
                Username = Username ?? "Participant",
                MyEnrolmentCount = myEnrolments.Count
            };

            return View(vm);
        }

        // ============= EVENT DETAIL =============
        [HttpGet]
        public async Task<IActionResult> EventDetail(int id)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");

            var ev = await _api.GetEventByIdAsync(Token!, id);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _api.GetCategoriesByEventAsync(Token!, id);
            var myEnrolments = await _api.GetMyEnrolmentsAsync(Token!);

            var alreadyEnrolled = myEnrolments
                .Where(e => e.EventID == id)
                .Select(e => e.CategoryID)
                .ToList();

            var vm = new EventDetailViewModel
            {
                Event = ev,
                Categories = categories,
                AlreadyEnrolledCategoryIds = alreadyEnrolled
            };

            return View(vm);
        }

        // ============= ENROL IN CATEGORY =============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");

            var payload = new
            {
                categoryID = categoryId,
                paymentStatus = "Pending"
            };

            var created = await _api.CreateEnrolmentAsync(Token!, payload);

            if (created == null)
            {
                TempData["Error"] = "Failed to enrol. You may already be enrolled in this category, or the category is full.";
            }
            else
            {
                TempData["Success"] = $"Successfully enrolled! Your start number: {created.StartNumber}";
            }

            return RedirectToAction(nameof(EventDetail), new { id = eventId });
        }

        // ============= PLACEHOLDERS (Steps 11) =============
        public IActionResult MyEvents()
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");
            return Content("My Events — coming in Step 11");
        }

        public IActionResult MyResults()
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");
            return Content("My Results — coming in Step 11");
        }

        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(Token) || Role != "Participant")
                return RedirectToAction("Login", "Auth");
            return Content("Profile — coming in Step 11");
        }

        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login", "Auth");
        }
    }
}