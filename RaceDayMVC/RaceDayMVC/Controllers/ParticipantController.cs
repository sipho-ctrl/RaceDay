using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Helpers;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(SessionHelper.RoleKey) != "Participant")
                return RedirectToAction("Login", "Auth");

            return Content("Participant Home — placeholder");
        }
    }
}