using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ODataErrors;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.Common.HealthCheck
{
    public class GraphApiHealthCheck : IHealthCheck
    {
        private readonly ILogger<GraphApiHealthCheck> _logger;
        private readonly IGraphClient _graphClient;

        public GraphApiHealthCheck(ILogger<GraphApiHealthCheck> logger, IGraphClient graphClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // This is just a sample filter condition for graph api health check.
                const string filterCondition = $"fields/Title eq '1'";

                _ = await _graphClient.GetListItemCollectionResponse(UpnDataProviderConstants.ExpandFields, filterCondition);
                _logger.LogInformation(EventIds.GraphApiIsHealthy.ToEventId(), "Graph Api is healthy");
                return HealthCheckResult.Healthy("Graph api is healthy");
            }
            catch (ODataError odataEx)
            {
                _logger.LogError(EventIds.GraphApiIsUnhealthy.ToEventId(), odataEx, 
                    "Graph API ODataError: {Code} - {Message}", odataEx.Error?.Code, odataEx.Error?.Message);
                return HealthCheckResult.Unhealthy($"Graph api is unhealthy: {odataEx.Error?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GraphApiIsUnhealthy.ToEventId(), ex, "Health check for the Graph Api threw an exception");
                return HealthCheckResult.Unhealthy("Graph api is unhealthy");
            }
        }
    }
}
