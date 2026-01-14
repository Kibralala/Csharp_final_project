using GuildManagementWoW.DTO.Raid;
using GuildManagementWoW.DTO.RaidSignUp;
using GuildManagementWoW.DTO.User;
using GuildManagementWoW.Models;
using Microsoft.EntityFrameworkCore;

namespace GuildManagementWoW.Service
{
    public class RaidSignUpService
    {
        private GuildDbContext _context;
        public RaidSignUpService(GuildDbContext context)
        {
            _context = context;
        }

        private static RaidSignUp DTOToModel(RaidSignUpCreateDTO dto)
        {
            return new RaidSignUp()
            {
                RaidId = dto.RaidId,
                CharacterId = dto.CharacterId,
            };
        }

        private static RaidSignUpListDTO ModelToListDTO(RaidSignUp raidSignUp)
        {
            return new RaidSignUpListDTO()
            {
                Id = raidSignUp.Id,
                CharacterId = raidSignUp.CharacterId,
                CharacterName = raidSignUp.Character.Nick,
                ClassName = raidSignUp.Character.Class,
                SignedAt = raidSignUp.SignedAt
            };
        }

        public IEnumerable<RaidSignUpListDTO> GetAll()
        {
            List<RaidSignUpListDTO> signUpListDTOs = new List<RaidSignUpListDTO>();
            var signups = _context.RaidSignUps.Where(r => !r.IsCancelled).Include(r => r.Character).ToList();
            foreach (var signup in signups)
            {
                RaidSignUpListDTO RaidSignUpListDTO = ModelToListDTO(signup);
                signUpListDTOs.Add(RaidSignUpListDTO);
            }
            return signUpListDTOs;
        }

        public async Task<RaidSignUpListDTO> CreateAsync(RaidSignUpCreateDTO newSignUp)
        {
            RaidSignUp raidSignUp = DTOToModel(newSignUp);
            await _context.RaidSignUps.AddAsync(raidSignUp);
            await _context.SaveChangesAsync();

            var signUpWithCharacter = _context.RaidSignUps
                .Include(r => r.Character).FirstOrDefault(r => r.Id == raidSignUp.Id);


            return ModelToListDTO(signUpWithCharacter);
        }

        public IEnumerable<RaidSignUpListDTO> GetByRaidId(int raidId)
        {
            List<RaidSignUpListDTO> signUpListDTOs = new List<RaidSignUpListDTO>();
            var signups = _context.RaidSignUps
                .Where(r => r.RaidId == raidId && !r.IsCancelled)
                .Include(r => r.Character)
                .ToList();

            foreach (var signup in signups)
            {
                RaidSignUpListDTO dto = ModelToListDTO(signup);
                signUpListDTOs.Add(dto);
            }
            return signUpListDTOs;
        }

        public RaidSignUpListDTO? GetByRaidIdAndUserId(int raidId, int userId)
        {
            List<RaidSignUpListDTO> signUpListDTOs = new List<RaidSignUpListDTO>();

            var signup = _context.RaidSignUps
                .Where(r => r.RaidId == raidId && !r.IsCancelled)
                .Include(r => r.Character)
                .FirstOrDefault(r => r.Character.UserId == userId);

            if (signup == null)
            {
                return null;
            }
         
            return ModelToListDTO(signup);



        }


        public async Task CancelAsync(int id)
        {
            var signup = _context.RaidSignUps.FirstOrDefault(r => r.Id == id && !r.IsCancelled);
            if (signup == null)
            {
                return;
            }
            signup.IsCancelled = true;
            await _context.SaveChangesAsync();
        }
    }
}
