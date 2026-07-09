using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataSeeder> logger;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<IdentityDataSeeder> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (PendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);
                if (!await roleManager.Roles.AnyAsync())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }
                if (!await userManager.Users.AnyAsync(ct))
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "Serag Ibrahim",
                        Email = "Serag@gmail.com",
                        UserName = "Serag",
                        PhoneNumber = "0123456789",

                    };
                    var result = await userManager.CreateAsync(admin, "P@ssw0rd");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "SuperAdmin");
                    }
                    else
                    {    var Errors = string.Join(";", result.Errors.Select(e => e.Description));
                        logger.LogWarning($"The User did not Create{Errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding identity data.");
                return;
            }
            
        }
    }
}
