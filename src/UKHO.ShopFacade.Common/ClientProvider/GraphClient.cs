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
        private readonly IOptions<SharePointSiteConfiguration> _sharePointSiteConfiguration;
        private readonly IOptions<GraphApiConfiguration> _graphApiConfiguration;
        private readonly IAuthenticationProvider _authenticationProvider;

        public GraphClient(IOptions<SharePointSiteConfiguration> sharePointSiteConfiguration, IAuthenticationProvider authenticationProvider, IOptions<GraphApiConfiguration> graphApiConfiguration)
        {
            _sharePointSiteConfiguration = sharePointSiteConfiguration ?? throw new ArgumentNullException(nameof(sharePointSiteConfiguration));
            _authenticationProvider = authenticationProvider;
            _graphApiConfiguration = graphApiConfiguration;
        }

        public async Task<ListItemCollectionResponse> GetListItemCollectionResponse(string expandFields, string filterCondition)
        {
            var graphClient = new GraphServiceClient(_authenticationProvider, _graphApiConfiguration.Value.GraphApiBaseUrl);

            var listItemCollectionResponse = await graphClient.Sites[_sharePointSiteConfiguration.Value.SiteId]
               .Lists[_sharePointSiteConfiguration.Value.ListId]
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
