using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public class UpnDataProvider : IUpnDataProvider
    {
        private readonly ILogger<UpnDataProvider> _logger;
        private readonly IGraphClient _graphClient;

        public UpnDataProvider(ILogger<UpnDataProvider> logger, IGraphClient graphClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        }

        public async Task<UpnDataProviderResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId)
        {
            // The filterCondition is used to filter the Sharepoint list based on the licenceId.
            var filterCondition = $"fields/Title eq '{licenceId}'";

            _logger.LogInformation(EventIds.GraphClientCallStarted.ToEventId(), ErrorDetails.GraphClientCallStartedMessage);

            var listItemCollectionResponse = await _graphClient.GetListItemCollectionResponse(UpnDataProviderConstants.ExpandFields, filterCondition);

            return HandleResponseAsync(listItemCollectionResponse!, correlationId);
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
                // When the licence is not found in Sharepoint list then it will return Not Found response with custom message.
                upnDataProviderResult = UpnDataProviderResult.NotFound(UpnDataProviderResult.SetErrorResponse(correlationId, ErrorDetails.Source, ErrorDetails.LicenceNotFoundMessage));
            }

            _logger.LogInformation(EventIds.GraphClientCallCompleted.ToEventId(), ErrorDetails.GraphClientCallCompletedMessage);

            return upnDataProviderResult;
        }

        private static S100UpnRecord GetS100UpnRecord(ListItemCollectionResponse s100UpnCollection)
        {
            return s100UpnCollection.Value!.Select(item => new S100UpnRecord
            {
                ECDIS_UPN1_Title = GetFieldValue(item, UpnSchema.ECDIS_UPN1_Title),
                ECDIS_UPN_1 = GetFieldValue(item, UpnSchema.ECDIS_UPN_1),
                ECDIS_UPN2_Title = GetFieldValue(item, UpnSchema.ECDIS_UPN2_Title),
                ECDIS_UPN_2 = GetFieldValue(item, UpnSchema.ECDIS_UPN_2),
                ECDIS_UPN3_Title = GetFieldValue(item, UpnSchema.ECDIS_UPN3_Title),
                ECDIS_UPN_3 = GetFieldValue(item, UpnSchema.ECDIS_UPN_3),
                ECDIS_UPN4_Title = GetFieldValue(item, UpnSchema.ECDIS_UPN4_Title),
                ECDIS_UPN_4 = GetFieldValue(item, UpnSchema.ECDIS_UPN_4),
                ECDIS_UPN5_Title = GetFieldValue(item, UpnSchema.ECDIS_UPN5_Title),
                ECDIS_UPN_5 = GetFieldValue(item, UpnSchema.ECDIS_UPN_5)
            }).FirstOrDefault()!;
        }

        private static string GetFieldValue(ListItem item, string fieldName)
        {
            return item.Fields.AdditionalData.TryGetValue(fieldName, out var fieldValue) ? fieldValue?.ToString() ?? string.Empty : string.Empty;
        }
    }
}
