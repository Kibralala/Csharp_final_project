namespace GuildManagementWoW.Models
{
    public class RaidSignUp
    {
        public int Id { get; set; }
        public int RaidId { get; set; }
        public Raid Raid { get; set; }
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public DateTime SignedAt { get; set; } = DateTime.Now;
        public bool IsCancelled { get; set; } = false;
    }
}
