using UKHO.ShopFacade.MockService.Configuration;
using UKHO.ShopFacade.MockService.StubSetup;
using WireMock.Settings;

namespace UKHO.ShopFacade.MockService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
       => Host.CreateDefaultBuilder(args)
           .ConfigureServices((host, services) => ConfigureServices(services, host.Configuration));

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WireMockServerSettings>(configuration.GetSection("WireMockServerSettings"));

            services.Configure<GraphApiConfiguration>(configuration.GetSection("GraphApiConfiguration"));
            services.AddSingleton<StubFactory>();
            services.AddHostedService<StubManagerHostedService>();
        }
    }
}
