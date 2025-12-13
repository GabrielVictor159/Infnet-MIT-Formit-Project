using Formit.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formit.Infraestructure.Data.Seader;
public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_DEFAULT_EMAIL");
        var adminUser = await userManager.FindByEmailAsync(adminEmail!);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail, 
                Email = adminEmail,
                FullName = "Administrador do Sistema",
                EmailConfirmed = true
            };

            var createAdmin = await userManager.CreateAsync(newAdmin, Environment.GetEnvironmentVariable("ADMIN_DEFAULT_PASSWORD")!);

            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}
