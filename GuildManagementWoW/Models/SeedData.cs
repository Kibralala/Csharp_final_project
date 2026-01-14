using Microsoft.AspNetCore.Identity;

namespace GuildManagementWoW.Models
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<GuildDbContext>();
            string[] roles = new string[] { "SuperAdmin", "Admin", "Officer", "Raider", "Alt", "Social" };

            // Vytvoření rolí
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            string superAdminName = "AllMighto";
            string password = "MyHero123"; // rychlý fix pro školní projekt

            var superAdmin = await userManager.FindByNameAsync("AllMighto");
            if (superAdmin == null)
            {
                superAdmin = new AppUser { UserName = "AllMighto" };
                await userManager.CreateAsync(superAdmin, "MyHero123");
            }

            var userInDb = context.Users.FirstOrDefault(u => u.UserName == superAdminName);
            if (userInDb == null)
            {
                context.Users.Add(new User { UserName = superAdminName });
                await context.SaveChangesAsync();
            }

            // Odstranit všechny aktuální role (pro jistotu)
            var currentRoles = await userManager.GetRolesAsync(superAdmin);
            foreach (var role in currentRoles)
            {
                await userManager.RemoveFromRoleAsync(superAdmin, role);
            }

            // Přidat roli SuperAdmin
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

        }
    }
}
