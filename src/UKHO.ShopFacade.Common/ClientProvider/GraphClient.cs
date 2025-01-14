using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.Configuration;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    [ExcludeFromCodeCoverage]
    public class GraphClient : IGraphClient
    {
        private readonly IOptions<SharePointSiteConfiguration> _sharePointSiteConfiguration;

        public GraphClient(IOptions<SharePointSiteConfiguration> sharePointSiteConfiguration)
        {
            _sharePointSiteConfiguration = sharePointSiteConfiguration ?? throw new ArgumentNullException(nameof(sharePointSiteConfiguration));
        }

        public async Task<ListItemCollectionResponse> GetListItemCollectionResponse(string expandFields, string filterCondition)
        {
            var graphClient = new GraphServiceClient(new DefaultAzureCredential(), new[] { "https://graph.microsoft.com/.default" });

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
