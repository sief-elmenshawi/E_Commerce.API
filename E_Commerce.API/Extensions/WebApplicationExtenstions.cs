using E_Commerce.Domain.Contracts;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace E_Commerce.API.Extensions
{
    public static class WebApplicationExtenstions
    {
        public static async Task<WebApplication> SeedAndMigrateDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Catalog");
            await seeder.SeedDataAsync();

            var IdentitySeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Identity");
            await IdentitySeeder.SeedDataAsync();


            return app;
        }
        public static WebApplication UseApiHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return app;
        }
    }
}
