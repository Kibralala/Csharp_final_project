namespace GuildManagementWoW.Models
{
    public class Raid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BeginAt { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<RaidSignUp> RaidSignUps { get; set; } = new List<RaidSignUp>();
    }
}
