using E_Commerce.API.Extensions;
using E_Commerce.Application;
using E_Commerce.Application.Profiles;
using E_Commerce.Infrastructure;
using E_Commerce.Infrastructure.Identity.Services;
using Serilog;
using Microsoft.Extensions.FileProviders;

namespace E_Commerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
               .CreateBootstrapLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
                 loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddInfrastrucreServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            builder.Services.AddApiExceptionHandling();
            builder.Services.AddApiValidation();
            builder.Services.AddApiHealthChecks(builder.Configuration);
            builder.Services.AddApiCors();
            builder.Services.AddApiVersioningConfig();
            builder.Services.AddApiRateLimiting();

            builder.Services.Configure<UrlSettings>(builder.Configuration.GetSection("UrlSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseCors("AllowFrontend");
            app.UseApiHealthChecks();

            app.UseSerilogRequestLogging();

            await app.SeedAndMigrateDataAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Files")),
                RequestPath = "/Files"
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}