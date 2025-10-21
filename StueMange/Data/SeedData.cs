using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StueMange.Models;

namespace StueMange.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Make sure the database is created
            context.Database.EnsureCreated();

            // Seed Roles
            string[] roleNames = { "Admin", "Teacher", "Parent" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed a default Admin User
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    FullName = "Admin User"
                };
                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
