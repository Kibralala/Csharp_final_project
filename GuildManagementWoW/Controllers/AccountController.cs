using GuildManagementWoW.Models;
using GuildManagementWoW.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GuildManagementWoW.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly GuildDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            GuildDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var appUser = new AppUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, "Raider");
                var user = new User { UserName = model.UserName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(appUser, isPersistent: false);

                HttpContext.Session.SetInt32("UserId", user.Id);

                TempData["SuccessMessage"] = "Registration successful. You have been logged in.";
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var appUser = await _userManager.FindByNameAsync(model.UserName);
            if (appUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, model.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                }
                TempData["SuccessMessage"] = "Login successful.";
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Wrong login credentials.");
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("UserId");
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}
