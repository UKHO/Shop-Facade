// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.Common.HealthCheck
{
    public class GraphApiHealthCheck : IHealthCheck
    {
        private readonly ILogger<UpnDataProvider> _logger;
        private readonly IGraphClient _graphClient;

        public GraphApiHealthCheck(ILogger<UpnDataProvider> logger, IGraphClient graphClient)
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

                var listItemCollectionResponse = await _graphClient.GetListItemCollectionResponse(UpnDataProviderConstants.ExpandFields, filterCondition);
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
