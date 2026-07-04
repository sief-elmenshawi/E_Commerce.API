using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.DataSeeding;
using E_Commerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastrucreServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddScoped<IDataSeeder,CatalogDataSeeder>();
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");
            services.AddScoped<IUnitOfWork , UnitOfWork>();

            services.AddSingleton<IConnectionMultiplexer>(Config =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("Redis connection string is not configured."));
            });
            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }
    }
}
