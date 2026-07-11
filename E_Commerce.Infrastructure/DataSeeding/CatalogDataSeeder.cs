using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext ,ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
			try
			{
				var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
				if (PendingMigrations.Any())
					await dbContext.Database.MigrateAsync(ct);

				// Seeding

				var seedRoot = Path.Combine(AppContext.BaseDirectory, "DataSeed");

				await seedIfEmptyAsync<ProductBrand,int>(seedRoot, "brands.json",ct);
				await seedIfEmptyAsync<ProductType,int>(seedRoot, "types.json", ct);
				await seedIfEmptyAsync<Product,int>(seedRoot, "products.json", ct);
				await seedIfEmptyAsync<DeliveryMethod,int>(seedRoot, "delivery.json", ct);

                int result = await dbContext.SaveChangesAsync(ct);

				if (result > 0)
					logger.LogInformation($"{result} Rows Add");
				else
					logger.LogInformation($"Database Already Seeded");

            }
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed To Seed Data");
				return;
			}
        }

		private async Task seedIfEmptyAsync<T,Tkey>(string rootPath , string fileName , CancellationToken ct) where T : BaseEntity<Tkey>
		{
			if ( await dbContext.Set<T>().AnyAsync())
			{
				logger.LogInformation("Table Already Seeded");
				return;
			}

			var filePath = Path.Combine(rootPath, fileName);
			if (!File.Exists(filePath))
			{
				logger.LogWarning($"File {fileName} Is Not Found");
				return;
			}

			using var fileStream = File.OpenRead(filePath);

			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
			};

			var item = await JsonSerializer.DeserializeAsync<List<T>>(fileStream,options,ct);
			if (item?.Any() ?? false) 
				dbContext.Set<T>().AddRange(item);
		}
    }
}
