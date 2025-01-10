using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public class UpnDataProvider : IUpnDataProvider
    {
        private readonly ILogger<UpnDataProvider> _logger;
        private readonly IGraphClient _graphClient;
        private readonly IOptions<SharePointSiteConfiguration> _sharePointSiteConfiguration;

        public UpnDataProvider(ILogger<UpnDataProvider> logger, IGraphClient graphClient, IOptions<SharePointSiteConfiguration> sharePointSiteConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
            _sharePointSiteConfiguration = sharePointSiteConfiguration ?? throw new ArgumentNullException(nameof(sharePointSiteConfiguration));
        }

        public async Task<UpnDataProviderResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId)
        {
            const string expandFields = "fields($select=Title,ECDIS_UPN1_Title,ECDIS_UPN_1,ECDIS_UPN2_Title,ECDIS_UPN_2,ECDIS_UPN3_Title,ECDIS_UPN_3,ECDIS_UPN4_Title,ECDIS_UPN_4,ECDIS_UPN5_Title,ECDIS_UPN_5)";
            var filterCondition = $"fields/Title eq '{licenceId}'";

            _logger.LogInformation(EventIds.GraphClientCallStarted.ToEventId(), ErrorDetails.GraphClientCallStartedMessage);

            var graphClient = _graphClient.GetGraphServiceClient();

            var items = await graphClient.Sites[_sharePointSiteConfiguration.Value.SiteId]
               .Lists[_sharePointSiteConfiguration.Value.ListId]
               .Items
               .GetAsync(requestConfiguration =>
               {
                   requestConfiguration.QueryParameters.Expand = new string[] { expandFields };
                   requestConfiguration.QueryParameters.Filter = filterCondition;
               });

            return HandleResponseAsync(items!, correlationId);
        }

        private UpnDataProviderResult HandleResponseAsync(ListItemCollectionResponse s100UpnCollection, string correlationId)
        {
            UpnDataProviderResult upnDataProviderResult;

            if (s100UpnCollection.Value!.Count > 0)
            {
                upnDataProviderResult = UpnDataProviderResult.Success(GetS100UpnRecord(s100UpnCollection)!);
            }
            else
            {
                upnDataProviderResult = UpnDataProviderResult.NotFound(UpnDataProviderResult.SetErrorResponse(correlationId, ErrorDetails.Source, ErrorDetails.LicenceNotFoundMessage));
            }

            _logger.LogInformation(EventIds.GraphClientCallCompleted.ToEventId(), ErrorDetails.GraphClientCallCompletedMessage);

            return upnDataProviderResult;
        }

        private static S100UpnRecord GetS100UpnRecord(ListItemCollectionResponse s100UpnCollection) => s100UpnCollection.Value!.Select(item => new S100UpnRecord
        {
            LicenceId = item.Fields.AdditionalData.TryGetValue(UpnSchema.Title, out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            ECDIS_UPN1_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN1_Title, out var upn1Title) ? upn1Title?.ToString() ?? string.Empty : string.Empty,
            ECDIS_UPN_1 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_1, out var upn1) ? upn1?.ToString() ?? string.Empty : string.Empty,
            ECDIS_UPN2_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN2_Title, out var upn2Title) ? upn2Title?.ToString() : string.Empty,
            ECDIS_UPN_2 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_2, out var upn2) ? upn2?.ToString() : string.Empty,
            ECDIS_UPN3_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN3_Title, out var upn3Title) ? upn3Title?.ToString() : string.Empty,
            ECDIS_UPN_3 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_3, out var upn3) ? upn3?.ToString() : string.Empty,
            ECDIS_UPN4_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN4_Title, out var upn4Title) ? upn4Title?.ToString() : string.Empty,
            ECDIS_UPN_4 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_4, out var upn4) ? upn4?.ToString() : string.Empty,
            ECDIS_UPN5_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN5_Title, out var upn5Title) ? upn5Title?.ToString() : string.Empty,
            ECDIS_UPN_5 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_5, out var upn5) ? upn5?.ToString() : string.Empty
        }).FirstOrDefault()!;
    }
}
