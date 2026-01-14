namespace GuildManagementWoW.DTO.Raid
{
    public class RaidIndexDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BeginAt { get; set; }
        public string? Description { get; set; }
        public bool IsSignedUp { get; set; }
        public int SignUpId { get; set; }
    }
}
