using System.Net;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class UpnService(IUpnDataProvider upnDataProvider, ILogger<UpnService> logger) : IUpnService
    {
        private readonly IUpnDataProvider _upnDataProvider = upnDataProvider;
        private readonly ILogger<UpnService> _logger = logger;

        public async Task<UpnServiceResult> GetUpnDetails(int licenceId, string correlationId)
        {
            _logger.LogInformation(EventIds.UPNServiceCallStarted.ToEventId(), ErrorDetails.UpnServiceCallStartedMessage);

            var upnDataProviderResult = await _upnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId);

            switch (upnDataProviderResult.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation(EventIds.UPNServiceCallCompleted.ToEventId(), ErrorDetails.UpnServiceCallCompletedMessage);
                    return UpnServiceResult.Success(SetUpnDetailResponse(upnDataProviderResult)!);
                case HttpStatusCode.NoContent:
                    _logger.LogWarning(EventIds.NoContentFound.ToEventId(), ErrorDetails.NoContentMessage);
                    return UpnServiceResult.NoContent();
                case HttpStatusCode.NotFound:
                    _logger.LogWarning(EventIds.LicenceNotFound.ToEventId(), ErrorDetails.LicenceNotFoundMessage);
                    return UpnServiceResult.NotFound(upnDataProviderResult.ErrorResponse);
                default:
                    _logger.LogError(EventIds.InternalError.ToEventId(), ErrorDetails.InternalErrorMessage);
                    return UpnServiceResult.InternalServerError();
            }
        }

        private static List<UserPermit> SetUpnDetailResponse(UpnDataProviderResult upnDataProviderResult)
        {
            var userPermits = new List<UserPermit>{
                    new()
                    {
                        Title = upnDataProviderResult.Value.ECDIS_UPN1_Title,
                        Upn = upnDataProviderResult.Value.ECDIS_UPN_1
                    }
                };

            // Include the UPN details in the response model when both the title and its corresponding UPN values are not null.
            AddUserPermitIfNotNull(upnDataProviderResult.Value.ECDIS_UPN2_Title, upnDataProviderResult.Value.ECDIS_UPN_2, userPermits);
            AddUserPermitIfNotNull(upnDataProviderResult.Value.ECDIS_UPN3_Title, upnDataProviderResult.Value.ECDIS_UPN_3, userPermits);
            AddUserPermitIfNotNull(upnDataProviderResult.Value.ECDIS_UPN4_Title, upnDataProviderResult.Value.ECDIS_UPN_4, userPermits);
            AddUserPermitIfNotNull(upnDataProviderResult.Value.ECDIS_UPN5_Title, upnDataProviderResult.Value.ECDIS_UPN_5, userPermits);

            return userPermits;
        }

        private static void AddUserPermitIfNotNull(string? title, string? upn, List<UserPermit> userPermits)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(upn))
            {
                userPermits.Add(new UserPermit { Title = title, Upn = upn });
            }
        }
    }
}
