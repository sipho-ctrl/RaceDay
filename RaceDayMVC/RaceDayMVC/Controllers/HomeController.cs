using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Helpers;

namespace RaceDayMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString(SessionHelper.TokenKey);

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var role = HttpContext.Session.GetString(SessionHelper.RoleKey);
            return role switch
            {
                "Organiser" => RedirectToAction("Index", "Organiser"),
                "Participant" => RedirectToAction("Index", "Participant"),
                _ => RedirectToAction("Login", "Auth")
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}