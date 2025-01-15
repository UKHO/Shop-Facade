using UKHO.GraphApi.MockService.Configuration;
using UKHO.GraphApi.MockService.StubSetup;
using WireMock.Settings;

namespace UKHO.GraphApi.MockService
{
    internal static class Program
    {
        public static void Main(string[] args)
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
