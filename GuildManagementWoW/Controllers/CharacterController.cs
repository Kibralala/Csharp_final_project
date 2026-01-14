using GuildManagementWoW.DTO.Character;
using GuildManagementWoW.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuildManagementWoW.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private CharacterService _characterService;

        public CharacterController(CharacterService characterService)
        {
         _characterService = characterService;   
        }

        [HttpGet]
        public IActionResult Index()
        {
            var allCharacters = _characterService.GetAll();
            return View(allCharacters);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CharacterCreateDTO newCharacter)
        {
            if (!ModelState.IsValid)
            {
                return View(newCharacter);
            }
                
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a new character.";
                return RedirectToAction("Login", "Account");
            }

            newCharacter.UserId = userId.Value;

            await _characterService.CreateCharacterAsync(newCharacter);
            TempData["SuccessMessage"] = "Character created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var characterToEdit = await _characterService.GetByIdAsync(id);
            if (characterToEdit == null)
            {
                TempData["ErrorMessage"] = "This character does not exist.";
                return RedirectToAction("Index");
            }

            var editDTO = new CharacterEditDTO()
            {
                Id = characterToEdit.Id,
                Nick = characterToEdit.Nick,
                Class = characterToEdit.Class,
                MainSpec = characterToEdit.MainSpec,
                MainRole = characterToEdit.MainRole,
                OffSpec = characterToEdit.OffSpec,
                OffRole = characterToEdit.OffRole,
                IsMainCharacter = characterToEdit.IsMainCharacter
            };

            return View(editDTO);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(CharacterEditDTO character)
        {
            if (ModelState.IsValid)
            {
                await _characterService.EditCharacterAsync(character);
                return RedirectToAction("Index");
            }
            else
                return View(character);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _characterService.SoftDeleteCharacterAsync(id);
            TempData["SuccessMessage"] = "The character was deleted.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailAsync(int id)
        {
            var CharacterDetail = await _characterService.GetByIdAsync(id);
            if (CharacterDetail == null)
            {
                TempData["ErrorMessage"] = "This raid does not exist.";
                return RedirectToAction("Index");
            }
            return View(CharacterDetail);
        }

    }
}
