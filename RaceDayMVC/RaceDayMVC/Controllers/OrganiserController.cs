using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.DTOs;
using RaceDayMVC.Helpers;
using RaceDayMVC.Services;
using RaceDayMVC.ViewModels;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IApiService _api;

        public OrganiserController(IApiService api)
        {
            _api = api;
        }

        private string? Token => HttpContext.Session.GetString(SessionHelper.TokenKey);
        private string? Role => HttpContext.Session.GetString(SessionHelper.RoleKey);

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            var profile = await _api.GetProfileAsync(Token!);
            var events = await _api.GetMyEventsAsync(Token!);

            var vm = new OrganiserDashboardViewModel
            {
                OrganisationName = profile?.OrganisationName ?? "Organiser",
                Username = profile?.Username ?? "Organiser",
                TotalEvents = events.Count,
                Events = new List<OrganiserDashboardViewModel.EventSummary>()
            };

            var totalEnrolments = 0;
            foreach (var ev in events)
            {
                var enrolments = await _api.GetEnrolmentsByEventAsync(Token!, ev.EventID);
                totalEnrolments += enrolments.Count;

                vm.Events.Add(new OrganiserDashboardViewModel.EventSummary
                {
                    EventID = ev.EventID,
                    EventName = ev.EventName,
                    EventDate = ev.EventDate,
                    Location = ev.Location,
                    EventStatus = ev.EventStatus,
                    EnrolmentCount = enrolments.Count
                });
            }

            vm.TotalEnrolments = totalEnrolments;

            return View(vm);
        }

        // ============= CREATE EVENT =============
        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            return View(new CreateEventViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            // Build payload for API
            var payload = new
            {
                eventName = model.EventName,
                eventDate = model.EventDate,
                location = model.Location,
                description = model.Description,
                maxParticipants = model.MaxParticipants,
                registrationDeadline = model.RegistrationDeadline,
                distance = model.Distance,
                eventType = model.EventType
            };

            var created = await _api.CreateEventAsync(Token!, payload);

            if (created == null)
            {
                ModelState.AddModelError("", "Failed to create event. Please check your inputs.");
                return View(model);
            }

            // TODO: Upload banner image (Azure Blob) — Step 11
            // if (model.BannerImage != null) { ... }

            TempData["Success"] = $"Event '{created.EventName}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ============= PLACEHOLDERS =============
        public IActionResult EditEvent(int id)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");
            return Content($"Edit Event {id} — coming in Step 6");
        }

        public IActionResult DeleteEvent(int id)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");
            return Content($"Delete Event {id} — coming in Step 7");
        }

        public IActionResult Enrolments(int eventId)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");
            return Content($"Enrolments for Event {eventId} — coming in Step 8");
        }

        public IActionResult Results(int eventId)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");
            return Content($"Results for Event {eventId} — coming in Step 9");
        }

        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login", "Auth");
        }
    }
}