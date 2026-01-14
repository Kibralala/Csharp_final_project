using GuildManagementWoW.DTO.Raid;
using GuildManagementWoW.DTO.RaidSignUp;
using GuildManagementWoW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuildManagementWoW.Service
{
    public class RaidService
    {
        private GuildDbContext _context;
        public RaidService(GuildDbContext context)
        {
            _context = context;
        }

        private static Raid DTOToModel(RaidCreateDTO dto)
        {
            return new Raid()
            {
                Name = dto.Name,
                BeginAt = dto.BeginAt,
                Description = dto.Description
            };
        }

        private static Raid DTOToModel(RaidEditDTO dto)
        {
            return new Raid()
            {
                Id = dto.Id,
                Name = dto.Name,
                BeginAt = dto.BeginAt,
                Description = dto.Description
            };
        }

        private static RaidDetailDTO ModelToDetailDTO(Raid raid)
        {
            return new RaidDetailDTO()
            {
                Id = raid.Id,
                Name = raid.Name,
                BeginAt = raid.BeginAt,
                Description = raid.Description
            };
        }

        private static RaidListDTO ModelToListDTO(Raid raid)
        {
            return new RaidListDTO()
            {
                Id = raid.Id,
                Name = raid.Name,
                BeginAt = raid.BeginAt,
                Description = raid.Description
            };
        }

        private static RaidEditDTO ModelToEditDTO(Raid raid)
        {
            return new RaidEditDTO()
            {
                Id = raid.Id,
                Name = raid.Name,
                BeginAt = raid.BeginAt,
                Description = raid.Description
            };
        }

        public IEnumerable<RaidListDTO> GetAll()
        {
            List<RaidListDTO> raidListDTOs = new List<RaidListDTO>();
            var raids = _context.Raids.Where(r => !r.IsDeleted).ToList();
            foreach (var raid in raids)
            {
                RaidListDTO raidListDTO = ModelToListDTO(raid);
                raidListDTOs.Add(raidListDTO);
            }
            return raidListDTOs;
        }

        public async Task<RaidDetailDTO> CreateAsync(RaidCreateDTO newRaid)
        {
            Raid raid = DTOToModel(newRaid);

            await _context.Raids.AddAsync(raid);
            await _context.SaveChangesAsync();

            return ModelToDetailDTO(raid);
        }

        public async Task<RaidDetailDTO> GetByIdAsync(int id)
        {
            var raidToEdit = await _context.Raids.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (raidToEdit == null)
            {
                return null;
            }
            return ModelToDetailDTO(raidToEdit);
        }

        public async Task<RaidEditDTO> GetByIdForEditAsync(int id)
        {
            var raidToEdit = await _context.Raids.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (raidToEdit == null)
            {
                return null;
            }

            return ModelToEditDTO(raidToEdit);
        }

        public async Task EditRaidAsync(RaidEditDTO raid)
        {
            var model = await _context.Raids.FirstOrDefaultAsync(r => r.Id == raid.Id);

            if (model == null)
            {
                return;
            }

            model.Name = raid.Name;
            model.Description = raid.Description;
            model.BeginAt = raid.BeginAt;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteRaidAsync(int id)
        {
            var raidToDelete = await _context.Raids.FirstOrDefaultAsync(r => r.Id == id);

            if (raidToDelete == null)
            {
                return;
            }
            raidToDelete.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<RaidDetailDTO> GetDetailByIdAsync(int raidId)
        {
            var raid = await _context.Raids
                .Where(r => r.Id == raidId && !r.IsDeleted)
                .Include(r => r.RaidSignUps)
                    .ThenInclude(rs => rs.Character)
                .FirstOrDefaultAsync();

            if (raid == null)
                return null;

            return new RaidDetailDTO
            {
                Id = raid.Id,
                Name = raid.Name,
                BeginAt = raid.BeginAt,
                Description = raid.Description,
                SignUps = raid.RaidSignUps
                    .Where(rs => !rs.IsCancelled)
                    .Select(rs => new RaidSignUpListDTO
                        {
                            Id = rs.Id,
                            CharacterId = rs.CharacterId,
                            CharacterName = rs.Character.Nick,
                            ClassName = rs.Character.Class
                        })
                        .ToList()
            };
        }

    }
}
