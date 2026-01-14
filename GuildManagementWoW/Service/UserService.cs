using GuildManagementWoW.DTO.User;
using GuildManagementWoW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuildManagementWoW.Service
{
    public class UserService
    {
        private GuildDbContext _context;
        public UserService(GuildDbContext context)
        {
            _context = context;
        }

        private static User DTOToModel(UserCreateDTO dto)
        {
            return new User()
            {
                UserName = dto.UserName,
            };
        }

        private static User DTOToModel(UserEditDTO dto)
        {
            return new User()
            {
                Id = dto.Id,
                UserName = dto.UserName,
            };
        }

        private static UserDetailDTO ModelToDetailDTO(User user)
        {
            return new UserDetailDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                CharacterNames = user.Characters
                .Where(c => !c.IsDeleted)
                .Select(c => c.Nick)
                .ToList()

            };
        }

        private static UserListDTO ModelToListDTO(User user)
        {
            return new UserListDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                CharacterCount = user.Characters.Count(c => !c.IsDeleted)
            };
        }

        private static UserEditDTO ModelToEditDTO(User user)
        {
            return new UserEditDTO()
            {
                Id = user.Id,
                UserName = user.UserName,

            };
        }

        public IEnumerable<UserListDTO> GetAll()
        {
            List<UserListDTO> userListDTOs = new List<UserListDTO>();
            var users = _context.Users.Include(u=>u.Characters).Where(r => !r.IsDeleted).ToList();
            foreach (var user in users)
            {
                UserListDTO userListDTO = ModelToListDTO(user);
                userListDTOs.Add(userListDTO);
            }
            return userListDTOs;
        }

        public async Task<UserDetailDTO> CreateAsync(UserCreateDTO newUser)
        {
            User user = DTOToModel(newUser);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return ModelToDetailDTO(user);
        }

        public async Task<UserDetailDTO> GetByIdAsync(int id)
        {
            var userToEdit = await _context.Users.Include(u=> u.Characters).FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (userToEdit == null)
            {
                return null;
            }
            return new UserDetailDTO
            {
                Id = userToEdit.Id,
                UserName = userToEdit.UserName,
                CharacterNames = userToEdit.Characters.Where(c=>!c.IsDeleted).Select(c=>c.Nick).ToList()
            };
        }

        public async Task<UserEditDTO> GetByIdForEditAsync(int id)
        {
            var userToEdit = await _context.Users.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (userToEdit == null)
            {
                return null;
            }

            return ModelToEditDTO(userToEdit);
        }

        public async Task EditUserAsync(UserEditDTO user)
        {
            var model = await _context.Users.FirstOrDefaultAsync(r => r.Id == user.Id);

            if (model == null)
            {
                return;
            }

            model.UserName = user.UserName;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteUserAsync(int id)
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(r => r.Id == id);

            if (userToDelete == null)
            {
                return;
            }
            userToDelete.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

    }
}
