using GuildManagementWoW.DTO.Character;
using GuildManagementWoW.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace GuildManagementWoW.Service
{
    public class CharacterService
    {
        private GuildDbContext _context;

        public CharacterService(GuildDbContext context)
        {
            _context = context;
        }

        private static Character DTOToModel(CharacterCreateDTO dto)
        {
            return new Character()
            {
                Nick = dto.Nick,
                Class = dto.Class,
                MainSpec = dto.MainSpec,
                MainRole = dto.MainRole,
                OffSpec = dto.OffSpec,
                OffRole = dto.OffRole,
                IsMainCharacter = dto.IsMainCharacter,
                UserId = dto.UserId
            };
        }

        private static Character DTOToModel(CharacterEditDTO dto)
        {
            return new Character()
            {
                Id = dto.Id,
                MainSpec = dto.MainSpec,
                MainRole = dto.MainRole,
                OffSpec = dto.OffSpec,
                OffRole = dto.OffRole
            };
        }

        private static CharacterDetailDTO ModelToDetailDTO(Character character)
        {
            return new CharacterDetailDTO()
            {
                Id = character.Id,
                Nick = character.Nick,
                Class = character.Class,
                MainSpec = character.MainSpec,
                MainRole = character.MainRole,
                OffSpec = character.OffSpec,
                OffRole = character.OffRole,
                IsMainCharacter = character.IsMainCharacter,
                UserName = character.User.UserName,
                UserId = character.UserId

            };

        }

        private static CharacterListDTO ModelToListDTO(Character character)
        {
            return new CharacterListDTO()
            {
                Id = character.Id,
                Nick = character.Nick,
                Class = character.Class,
                MainSpec = character.MainSpec,
                UserId = character.UserId,
                IsMainCharacter = character.IsMainCharacter,
                UserName = character.User.UserName,

            };
        }

        public IEnumerable<CharacterListDTO> GetAll()
        {
            List<CharacterListDTO> characterListDTOs = new List<CharacterListDTO>();
            var characters = _context.Characters
                .Include(c => c.User)
                .Where(c => !c.IsDeleted).ToList();
            foreach (var character in characters)
            {
                CharacterListDTO characterListDTO = ModelToListDTO(character);
                characterListDTOs.Add(characterListDTO);
            }
            return characterListDTOs;
        }

        internal async Task<CharacterDetailDTO> CreateCharacterAsync(CharacterCreateDTO newCharacter)
        {
            Character character = DTOToModel(newCharacter);

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            var characterWithUser = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == character.Id);

            return ModelToDetailDTO(characterWithUser);
        }

        internal async Task<CharacterDetailDTO> GetByIdAsync(int id)
        {
            var characterToEdit = await _context.Characters.Include(c=> c.User).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (characterToEdit == null)
            {
                return null;
            }
            return ModelToDetailDTO(characterToEdit);
        }

        public IEnumerable<Character> GetByUserId(int userId)
        {
            return _context.Characters.Where(c => c.UserId == userId).ToList();
        }


        internal async Task EditCharacterAsync(CharacterEditDTO character)
        {
            var model = await _context.Characters.FirstOrDefaultAsync(c => c.Id == character.Id);
            if (model == null)
            {
                return;
            }
            model.MainSpec = character.MainSpec;
            model.MainRole = character.MainRole;
            model.OffSpec = character.OffSpec;
            model.OffRole = character.OffRole;
            model.IsMainCharacter = character.IsMainCharacter;

            await _context.SaveChangesAsync();
        }

        internal async Task SoftDeleteCharacterAsync(int id)
        {
            var characterToDelete = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            if (characterToDelete == null)
            {
                return;
            }
            characterToDelete.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

    }
}
