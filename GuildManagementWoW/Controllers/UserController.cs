using GuildManagementWoW.DTO.User;
using GuildManagementWoW.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuildManagementWoW.Controllers
{
    public class UserController : Controller
    {
        private UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            return View("Detail", user);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _userService.GetByIdForEditAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Index", "Home");

            return View("Edit", user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(UserEditDTO user)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || user.Id != userId.Value)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                await _userService.EditUserAsync(user);
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }

            return View("Edit", user);
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index()
        {
            var allUsers = _userService.GetAll();
            return View(allUsers);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateAsync(UserCreateDTO newUser)
        {
            if (ModelState.IsValid)
            {
                await _userService.CreateAsync(newUser);
                TempData["SuccessMessage"] = "The user was successfully created.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(newUser);
            }
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> EditAsync(int id)
        {
            var userToEdit = await _userService.GetByIdForEditAsync(id);
            if (userToEdit == null)
            {
                TempData["ErrorMessage"] = "This user does not exist.";
                return RedirectToAction("Index");
            }
            return View(userToEdit);
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> EditAsync(UserEditDTO user)
        {
            if (ModelState.IsValid)
            {
                await _userService.EditUserAsync(user);
                TempData["SuccessMessage"] = "The user was successfully updated.";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _userService.SoftDeleteUserAsync(id);
            TempData["SuccessMessage"] = "The user was successfully deleted.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> DetailAsync(int id)
        {
            var userForDetail = await _userService.GetByIdAsync(id);
            if (userForDetail == null)
            {
                TempData["ErrorMessage"] = "This user does not exist.";
                return RedirectToAction("Index");
            }
            return View(userForDetail);
        }

    }
}
