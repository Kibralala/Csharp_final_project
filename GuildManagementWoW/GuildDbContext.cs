using GuildManagementWoW.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuildManagementWoW
{
    public class GuildDbContext :IdentityDbContext<AppUser>
    {
        public GuildDbContext(DbContextOptions<GuildDbContext> options):base(options)
        {
            
        }

        public DbSet<Character> Characters {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Raid> Raids { get; set; }
        public DbSet<RaidSignUp> RaidSignUps { get; set; }

    }
}
