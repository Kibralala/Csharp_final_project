namespace GuildManagementWoW.DTO.RaidSignUp
{
    public class RaidSignUpListDTO
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string ClassName { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
