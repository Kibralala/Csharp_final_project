using GuildManagementWoW.DTO.Raid;
using GuildManagementWoW.DTO.RaidSignUp;
using GuildManagementWoW.Models;
using GuildManagementWoW.Service;
using GuildManagementWoW.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuildManagementWoW.Controllers
{

    [Authorize]
    public class RaidSignUpController : Controller
    {
        private RaidSignUpService _raidSignUpService;
        private RaidService _raidService;
        private CharacterService _characterService;
        public RaidSignUpController(RaidSignUpService raidSignUpservice, RaidService raidService, CharacterService characterService)
        {
            _raidSignUpService = raidSignUpservice;
            _raidService = raidService;
            _characterService = characterService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int raidId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = userId.Value;

            var characters = _characterService.GetByUserId(currentUserId)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nick + "(" + c.Class + ")"
            })
            .ToList();

            var vm = new RaidSignUpCreateVM
            {
                RaidId = raidId,
                Characters = characters
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RaidSignUpPostVM newSignUp)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = userId.Value;

            if (ModelState.IsValid)
            {
                var dto = new RaidSignUpCreateDTO
                {
                    RaidId = newSignUp.RaidId,
                    CharacterId = newSignUp.CharacterId
                };

                await _raidSignUpService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Your sign up was successfull.";
                return RedirectToAction("Index", "Raid");
            }
            else //znovu naplnění dropdownu, když to selže
            {
                var vm = new RaidSignUpCreateVM
                {
                    RaidId = newSignUp.RaidId,
                    Characters = _characterService.GetByUserId(currentUserId)
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.Nick + " (" + c.Class + ")"
                        }).ToList()
                };

                return View("~/Views/RaidSignUp/Create.cshtml", vm);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _raidSignUpService.CancelAsync(id);
            TempData["SuccessMessage"] = "Your sign up was successfully cancelled.";
            return RedirectToAction("Index", "Raid");
        }
    }
}
