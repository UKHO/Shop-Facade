using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using UKHO.ShopFacade.Common.Configuration;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    [ExcludeFromCodeCoverage]
    public class GraphClient : IGraphClient
    {
        private readonly IOptions<GraphApiConfiguration> _graphApiConfiguration;
        private readonly IAuthenticationProvider _authenticationProvider;

        public GraphClient(IAuthenticationProvider authenticationProvider, IOptions<GraphApiConfiguration> graphApiConfiguration)
        {
            _authenticationProvider = authenticationProvider;
            _graphApiConfiguration = graphApiConfiguration;
        }

        /// <summary>
        /// Run a health check against Graph API by attempting to get one item from the configured SharePoint list
        /// If the list is empty this will still return a 200 response
        /// </summary>
        /// <returns></returns>
        public async Task HealthCheck()
        {
            var graphClient = new GraphServiceClient(_authenticationProvider, _graphApiConfiguration.Value.GraphApiBaseUrl);
            await graphClient
                    .Sites[_graphApiConfiguration.Value.SiteId]
                    .Lists[_graphApiConfiguration.Value.ListId]
                    .Items
                    .GetAsync(rc =>
                    {
                        rc.QueryParameters.Top = 1;
                        rc.QueryParameters.Select = ["id"];
                    });

            //Any 200 response is acceptable, if not exception is thrown
        }

        public async Task<ListItemCollectionResponse> GetListItemCollectionResponse(string expandFields, string filterCondition)
        {
            var graphClient = new GraphServiceClient(_authenticationProvider, _graphApiConfiguration.Value.GraphApiBaseUrl);

            var listItemCollectionResponse = await graphClient.Sites[_graphApiConfiguration.Value.SiteId]
               .Lists[_graphApiConfiguration.Value.ListId]
               .Items
               .GetAsync(requestConfiguration =>
               {
                   requestConfiguration.QueryParameters.Expand = [expandFields];
                   requestConfiguration.QueryParameters.Filter = filterCondition;
               });

            return listItemCollectionResponse!;
        }
    }
}
