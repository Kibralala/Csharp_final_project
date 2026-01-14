namespace GuildManagementWoW.DTO.User
{
    public class UserDetailDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public List<string> CharacterNames { get; set; } = new List<string>();
    }
}
