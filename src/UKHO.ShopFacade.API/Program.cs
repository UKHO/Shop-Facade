using System.Diagnostics.CodeAnalysis;
using UKHO.ShopFacade.API.Middleware;

namespace UKHO.ShopFacade.API
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureConfiguration(builder);
            ConfigureServices(builder);

            // Add services to the container.

            var app = builder.Build();

            app.UseCorrelationIdMiddleware();
            app.UseExceptionHandlingMiddleware();
            app.MapControllers();

            app.Run();
        }

        private static void ConfigureConfiguration(WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json", false, true);
#if DEBUG
            builder.Configuration.AddJsonFile("appsettings.local.overrides.json", true, true);
#endif
            builder.Configuration.AddEnvironmentVariables();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();
        }
    }
}
