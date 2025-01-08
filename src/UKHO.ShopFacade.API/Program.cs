using System.Diagnostics.CodeAnalysis;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using UKHO.ShopFacade.API.Middleware;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Options;
using System.Reflection;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;
using UKHO.Logging.EventHubLogProvider;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UKHO.ShopFacade.API
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private const string EventHubLoggingConfiguration = "EventHubLoggingConfiguration";
        private const string AzureAdScheme = "AzureAd";
        private const string AzureAdConfiguration = "AzureAdConfiguration";
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureConfiguration(builder);
            ConfigureServices(builder);

            // Add services to the container.

            var app = builder.Build();

            app.UseCorrelationIdMiddleware();
            app.UseExceptionHandlingMiddleware();

            ConfigureLogging(app);

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

            var configuration = builder.Configuration;
            var kvServiceUri = configuration["KeyVaultSettings:ServiceUri"];

            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                var secretClient = new SecretClient(new Uri(kvServiceUri), new DefaultAzureCredential(new DefaultAzureCredentialOptions()));
                builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            }
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
#if DEBUG
                loggingBuilder.AddSerilog(new LoggerConfiguration()
                    .WriteTo.File("Logs/UKHO.ShopFacade.API-Logs-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("UKHO", LogEventLevel.Debug)
                    .CreateLogger(), dispose: true);
#endif
                loggingBuilder.AddAzureWebAppDiagnostics();
            });
            var options = new ApplicationInsightsServiceOptions { ConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString") };
            builder.Services.AddApplicationInsightsTelemetry(options);
            builder.Services.AddControllers();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.Configure<EventHubLoggingConfiguration>(builder.Configuration.GetSection(EventHubLoggingConfiguration));

            var azureAdConfiguration = builder.Configuration.GetSection(AzureAdConfiguration).Get<AzureAdConfiguration>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(AzureAdScheme, options =>
                {
                    options.Audience = azureAdConfiguration.ClientId;
                    options.Authority = $"{azureAdConfiguration.MicrosoftOnlineLoginUrl}{azureAdConfiguration.TenantId}";
                });

            builder.Services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(AzureAdScheme)
                    .Build())
                .AddPolicy(ShopFacadeConstants.ShopFacadePolicy, policy => policy.RequireRole(ShopFacadeConstants.ShopFacadePolicy));
        }

        private static void ConfigureLogging(WebApplication webApplication)
        {
            var loggerFactory = webApplication.Services.GetRequiredService<ILoggerFactory>();
            var eventHubLoggingConfiguration = webApplication.Services.GetRequiredService<IOptions<EventHubLoggingConfiguration>>();
            var httpContextAccessor = webApplication.Services.GetRequiredService<IHttpContextAccessor>();

            if (!string.IsNullOrWhiteSpace(eventHubLoggingConfiguration?.Value.ConnectionString))
            {
                void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
                {
                    if (httpContextAccessor.HttpContext != null)
                    {
                        additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress!.ToString();
                        additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers.UserAgent.FirstOrDefault() ?? string.Empty;
                        additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
                        additionalValues["_X-Correlation-ID"] =
                            httpContextAccessor.HttpContext.Request.Headers?[ApiHeaderKeys.XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;
                    }
                }

                loggerFactory.AddEventHub(config =>
                {
                    config.Environment = eventHubLoggingConfiguration.Value.Environment;
                    config.DefaultMinimumLogLevel =
                        (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.MinimumLoggingLevel!, true);
                    config.MinimumLogLevels["UKHO"] =
                        (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.UkhoMinimumLoggingLevel!, true);
                    config.EventHubConnectionString = eventHubLoggingConfiguration.Value.ConnectionString;
                    config.EventHubEntityPath = eventHubLoggingConfiguration.Value.EntityPath;
                    config.System = eventHubLoggingConfiguration.Value.System;
                    config.Service = eventHubLoggingConfiguration.Value.Service;
                    config.NodeName = eventHubLoggingConfiguration.Value.NodeName;
                    config.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
                });
            }
        }
    }
}
