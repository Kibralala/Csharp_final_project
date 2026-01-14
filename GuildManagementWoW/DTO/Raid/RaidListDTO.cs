namespace GuildManagementWoW.DTO.Raid
{
    public class RaidListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BeginAt { get; set; }
        public string? Description { get; set; }
    }
}
