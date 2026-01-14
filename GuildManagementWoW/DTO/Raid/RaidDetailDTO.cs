using GuildManagementWoW.DTO.RaidSignUp;

namespace GuildManagementWoW.DTO.Raid
{
    public class RaidDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BeginAt { get; set; }
        public string? Description { get; set; }
        public List<RaidSignUpListDTO> SignUps { get; set; } = new List<RaidSignUpListDTO>();
    }
}
