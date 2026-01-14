using GuildManagementWoW.Models;

namespace GuildManagementWoW.DTO.Character
{
    public class CharacterCreateDTO
    {
        public string Nick { get; set; }
        public string Class { get; set; }
        public string? MainSpec { get; set; }
        public string? MainRole { get; set; }
        public string? OffSpec { get; set; }
        public string? OffRole { get; set; }
        public bool IsMainCharacter { get; set; }
        public int UserId { get; set; }

    }
}
