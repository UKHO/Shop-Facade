using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public class UpnDataProvider : IUpnDataProvider
    {
        public readonly IGraphClient _graphClient;
        private readonly SharePointSiteConfiguration _sharePointSiteConfiguration;

        public UpnDataProvider(IGraphClient graphClient, IOptions<SharePointSiteConfiguration> sharePointSiteConfiguration)
        {
            _graphClient = graphClient;
            _sharePointSiteConfiguration = sharePointSiteConfiguration.Value;
        }

        public async Task<UpnDataProviderResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId)
        {
            const string expandFields = "fields($select=Title,UPN1_Title,ECDIS_UPN_1,UPN2_Title,ECDIS_UPN_2,UPN3_Title,ECDIS_UPN_3,UPN4_Title,ECDIS_UPN_4,UPN5_Title,ECDIS_UPN_5)";
            var filterCondition = $"fields/Title eq '{licenceId}'";

            var graphClient = _graphClient.GetGraphServiceClient();

            var items = await graphClient.Sites[_sharePointSiteConfiguration.SiteId]
               .Lists[_sharePointSiteConfiguration.ListId]
               .Items
               .GetAsync(requestConfiguration =>
               {
                   requestConfiguration.QueryParameters.Expand = new string[] { expandFields };
                   requestConfiguration.QueryParameters.Filter = filterCondition;
               });

            return HandleResponseAsync(items!, correlationId);
        }

        private static UpnDataProviderResult HandleResponseAsync(ListItemCollectionResponse s100UpnCollection, string correlationId)
        {
            if (s100UpnCollection.Value!.Count > 0)
            {
                return UpnDataProviderResult.Success(GetS100UpnRecord(s100UpnCollection)!);
            }
            else
            {
                return UpnDataProviderResult.NotFound(UpnDataProviderResult.SetErrorResponse(correlationId, "licenceId", "Licence not found."));
            }
        }

        private static S100UpnRecord GetS100UpnRecord(ListItemCollectionResponse s100UpnCollection) => s100UpnCollection.Value!.Select(item => new S100UpnRecord
        {
            LicenceId = item.Fields.AdditionalData.TryGetValue(UpnSchema.Title, out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            UPN1_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.UPN1_Title, out var upn1Title) ? upn1Title?.ToString() ?? string.Empty : string.Empty,
            ECDIS_UPN_1 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_1, out var upn1) ? upn1?.ToString() ?? string.Empty : string.Empty,
            UPN2_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.UPN2_Title, out var upn2Title) ? upn2Title?.ToString() : string.Empty,
            ECDIS_UPN_2 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_2, out var upn2) ? upn2?.ToString() : string.Empty,
            UPN3_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.UPN3_Title, out var upn3Title) ? upn3Title?.ToString() : string.Empty,
            ECDIS_UPN_3 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_3, out var upn3) ? upn3?.ToString() : string.Empty,
            UPN4_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.UPN4_Title, out var upn4Title) ? upn4Title?.ToString() : string.Empty,
            ECDIS_UPN_4 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_4, out var upn4) ? upn4?.ToString() : string.Empty,
            UPN5_Title = item.Fields.AdditionalData.TryGetValue(UpnSchema.UPN5_Title, out var upn5Title) ? upn5Title?.ToString() : string.Empty,
            ECDIS_UPN_5 = item.Fields.AdditionalData.TryGetValue(UpnSchema.ECDIS_UPN_5, out var upn5) ? upn5?.ToString() : string.Empty
        }).FirstOrDefault()!;
    }
}
