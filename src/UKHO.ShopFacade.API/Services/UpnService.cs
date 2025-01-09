using System.Net;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response;

namespace UKHO.ShopFacade.API.Services
{
    public class UpnService : IUpnService
    {
        public readonly IUpnDataProvider _upnDataProvider;
        public UpnService(IUpnDataProvider upnDataProvider)
        {
            _upnDataProvider = upnDataProvider;
        }

        public async Task<UpnServiceResult> GetUpnDetails(int licenceId, string correlationId)
        {
            var upnDataProviderResult = await _upnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId);

            return upnDataProviderResult.StatusCode switch
            {
                HttpStatusCode.OK => UpnServiceResult.Success(GetUpnDetail(upnDataProviderResult)!),
                HttpStatusCode.NotFound => UpnServiceResult.NotFound(upnDataProviderResult.ErrorResponse),
                _ => UpnServiceResult.InternalServerError()
            };
        }

        private static UpnDetail GetUpnDetail(UpnDataProviderResult upnDataProviderResult)
        {
            var upnDetail = new UpnDetail();
            int.TryParse(upnDataProviderResult.Value.LicenceId, out var licid);
            var userPermits = new List<UserPermit>{
                new()
                {
                    Title = upnDataProviderResult.Value.UPN1_Title,
                    Upn = upnDataProviderResult.Value.UPN1
                }
            };

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN2))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN2_Title,
                    Upn = upnDataProviderResult.Value.UPN2
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN3))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN3_Title,
                    Upn = upnDataProviderResult.Value.UPN3
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN4))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN4_Title,
                    Upn = upnDataProviderResult.Value.UPN4
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN5))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN5_Title,
                    Upn = upnDataProviderResult.Value.UPN5
                });
            }

            upnDetail.LicenceId = licid;
            upnDetail.UserPermits = userPermits;

            return upnDetail;
        }
    }
}
