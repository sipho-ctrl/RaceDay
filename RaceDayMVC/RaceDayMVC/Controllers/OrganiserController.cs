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

        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login", "Auth");
        }
    }
}