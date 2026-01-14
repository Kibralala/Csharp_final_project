namespace GuildManagementWoW.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
