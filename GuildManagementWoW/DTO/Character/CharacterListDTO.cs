namespace GuildManagementWoW.DTO.Character
{
    public class CharacterListDTO
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public string Class { get; set; }
        public string? MainSpec { get; set; }
        public bool IsMainCharacter { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
