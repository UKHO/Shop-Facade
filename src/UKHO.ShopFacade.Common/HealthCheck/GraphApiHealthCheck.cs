// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.DataProvider;

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
                // This is just a sample expand fields for graph api health check.
                const string expandFields = "fields($select=Title)";

                // This is just a sample filter condition for graph api health check.
                const string filterCondition = $"fields/Title eq '1'";

                var listItemCollectionResponse = await _graphClient.GetListItemCollectionResponse(expandFields, filterCondition);

                return HealthCheckResult.Healthy("Graph api is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Graph api is unhealthy");
            }
        }
    }
}
