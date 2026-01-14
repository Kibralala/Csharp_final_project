namespace GuildManagementWoW.DTO.Raid
{
    public class RaidEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BeginAt { get; set; }
        public string? Description { get; set; }
    }
}
