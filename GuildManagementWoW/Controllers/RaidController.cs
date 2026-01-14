using GuildManagementWoW.DTO.Raid;
using GuildManagementWoW.Models;
using GuildManagementWoW.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuildManagementWoW.Controllers
{
    [Authorize]
    public class RaidController : Controller
    {
        private RaidService _raidService;
        private RaidSignUpService _raidSignUpService;

        public RaidController(RaidService raidService, RaidSignUpService raidSignUpService)
        {
            _raidService = raidService;
            _raidSignUpService = raidSignUpService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int actualUserId = userId.Value;

            var allRaids = _raidService.GetAll();

            List<RaidIndexDTO> model = new List<RaidIndexDTO>();

            foreach (var raid in allRaids)
            {
                var signup = _raidSignUpService.GetByRaidIdAndUserId(raid.Id, actualUserId);

                RaidIndexDTO raidIndexDTO = new RaidIndexDTO()
                {
                    Id = raid.Id,
                    Name = raid.Name,
                    BeginAt = raid.BeginAt,
                    Description = raid.Description,

                };

                if (signup != null)
                {
                    raidIndexDTO.IsSignedUp = true;
                    raidIndexDTO.SignUpId = signup.Id;
                }
                else
                {
                    raidIndexDTO.IsSignedUp = false;
                    raidIndexDTO.SignUpId = 0;
                }

                model.Add(raidIndexDTO);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Officer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Officer")]
        public async Task<IActionResult> Create(RaidCreateDTO newRaid)
        {
            if (ModelState.IsValid)
            {
                await _raidService.CreateAsync(newRaid);
                TempData["SuccessMessage"] = "The raid was successfully created.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(newRaid);
            }
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Officer")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var raidToEdit = await _raidService.GetByIdForEditAsync(id);
            if (raidToEdit == null)
            {
                TempData["ErrorMessage"] = "This raid does not exist.";
                return RedirectToAction("Index");
            }
            return View(raidToEdit);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Officer")]
        public async Task<IActionResult> EditAsync(RaidEditDTO raid)
        {
            if (ModelState.IsValid)
            {
                await _raidService.EditRaidAsync(raid);
                TempData["SuccessMessage"] = "The raid was successfully updated.";
                return RedirectToAction("Index");
            }
            return View(raid);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Officer")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _raidService.SoftDeleteRaidAsync(id);
            TempData["SuccessMessage"] = "The raid was successfully deleted.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var raidDetail = await _raidService.GetDetailByIdAsync(id);
            if (raidDetail == null)
            {
                TempData["ErrorMessage"] = "This raid does not exist.";
                return RedirectToAction("Index");
            }

            return View(raidDetail);
        }


    }
}
