using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.DTOs;
using RaceDayMVC.Helpers;
using RaceDayMVC.Services;
using RaceDayMVC.ViewModels;

namespace RaceDayMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _api;

        public AuthController(IApiService api)
        {
            _api = api;
        }

        // ============= LOGIN =============
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionHelper.TokenKey)))
                return RedirectToRoleHome();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _api.LoginAsync(new LoginDto
            {
                Username = model.Username,
                Password = model.Password
            });

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            SessionHelper.SetUser(HttpContext.Session, result.Token, result.Role, result.Username, result.UserID);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToRoleHome();
        }

        // ============= REGISTER =============
        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionHelper.TokenKey)))
                return RedirectToRoleHome();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Role != "Organiser" && model.Role != "Participant")
            {
                ModelState.AddModelError("Role", "Role must be Organiser or Participant");
                return View(model);
            }

            var dto = new RegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                ContactNumber = model.ContactNumber,
                EmergencyContact = model.EmergencyContact,
                OrganisationName = model.OrganisationName
            };

            var result = await _api.RegisterAsync(dto);

            if (result == null)
            {
                ModelState.AddModelError("", "Registration failed. Username or email may already exist.");
                return View(model);
            }

            SessionHelper.SetUser(HttpContext.Session, result.Token, result.Role, result.Username, result.UserID);
            return RedirectToRoleHome();
        }

        // ============= LOGOUT =============
        [HttpPost]
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login", "Auth");
        }

        // ============= HELPERS =============
        private IActionResult RedirectToRoleHome()
        {
            var role = HttpContext.Session.GetString(SessionHelper.RoleKey);
            return role switch
            {
                "Organiser" => RedirectToAction("Index", "Organiser"),
                "Participant" => RedirectToAction("Index", "Participant"),
                _ => RedirectToAction("Login", "Auth")
            };
        }
    }
}