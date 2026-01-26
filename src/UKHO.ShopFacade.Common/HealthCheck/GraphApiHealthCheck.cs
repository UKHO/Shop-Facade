using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.Common.HealthCheck
{
    public class GraphApiHealthCheck(ILogger<GraphApiHealthCheck> logger, IGraphClient graphClient) : IHealthCheck
    {
        private readonly ILogger<GraphApiHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IGraphClient _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _graphClient.HealthCheck();

                _logger.LogInformation(EventIds.GraphApiIsHealthy.ToEventId(), "Graph Api is healthy");
                return HealthCheckResult.Healthy("Graph api is healthy");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GraphApiIsUnhealthy.ToEventId(), ex, "Health check for the Graph Api threw an exception");
                return HealthCheckResult.Unhealthy("Graph api is unhealthy");
            }
        }
    }
}
