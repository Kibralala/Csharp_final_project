using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuildManagementWoW.ViewModels
{
    public class RaidSignUpCreateVM
    {
        public int RaidId { get; set; }
        public int CharacterId { get; set; }
        [BindNever]
        public IEnumerable<SelectListItem> Characters {  get; set; }
    }
}
