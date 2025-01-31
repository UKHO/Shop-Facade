using System.Net;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response;

namespace UKHO.ShopFacade.API.Services
{
    public class UpnService : IUpnService
    {
        private readonly IUpnDataProvider _upnDataProvider;
        public UpnService(IUpnDataProvider upnDataProvider)
        {
            _upnDataProvider = upnDataProvider ?? throw new ArgumentNullException(nameof(upnDataProvider));
        }

        public async Task<UpnServiceResult> GetUpnDetails(int licenceId, string correlationId)
        {
            var upnDataProviderResult = await _upnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId);

            return upnDataProviderResult.StatusCode switch
            {
                HttpStatusCode.OK => UpnServiceResult.Success(SetUpnDetailResponse(upnDataProviderResult)!),
                HttpStatusCode.NotFound => UpnServiceResult.NotFound(upnDataProviderResult.ErrorResponse),
                _ => UpnServiceResult.InternalServerError()
            };
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
