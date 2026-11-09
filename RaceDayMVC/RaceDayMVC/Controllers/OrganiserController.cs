using Microsoft.AspNetCore.Mvc;
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

            TempData["Success"] = $"Event '{created.EventName}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ============= EDIT EVENT =============
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            var ev = await _api.GetEventByIdAsync(Token!, id);

            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new EditEventViewModel
            {
                EventID = ev.EventID,
                EventName = ev.EventName,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Description = ev.Description,
                MaxParticipants = ev.MaxParticipants,
                RegistrationDeadline = ev.RegistrationDeadline,
                Distance = ev.Distance ?? string.Empty,
                EventType = ev.EventType ?? "run",
                EventStatus = ev.EventStatus
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(EditEventViewModel model)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var payload = new
            {
                eventName = model.EventName,
                eventDate = model.EventDate,
                location = model.Location,
                description = model.Description,
                maxParticipants = model.MaxParticipants,
                registrationDeadline = model.RegistrationDeadline,
                distance = model.Distance,
                eventType = model.EventType,
                eventStatus = model.EventStatus
            };

            var success = await _api.UpdateEventAsync(Token!, model.EventID, payload);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to update event.");
                return View(model);
            }

            TempData["Success"] = $"Event '{model.EventName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ============= DELETE EVENT =============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (string.IsNullOrEmpty(Token) || Role != "Organiser")
                return RedirectToAction("Login", "Auth");

            var success = await _api.DeleteEventAsync(Token!, id);

            if (success)
                TempData["Success"] = "Event deleted successfully.";
            else
                TempData["Error"] = "Failed to delete event.";

            return RedirectToAction(nameof(Index));
        }

        // ============= PLACEHOLDERS =============
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