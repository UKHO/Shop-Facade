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

            if (upnDataProviderResult.IsSuccess)
            {
                UpnServiceResult.Success(GetUpnDetail(upnDataProviderResult)!);
            }
            return upnDataProviderResult;
        }

        private UpnDetail GetUpnDetail(UpnServiceResult upnDataProviderResult)
        {
            var upnDetail = new UpnDetail();
            int.TryParse(upnDataProviderResult.Value.LicenceId, out var licid);
            var userPermits = new List<UserPermit>{
                new()
                {
                    Title = upnDataProviderResult.Value.UPN1_Title,
                    Upn = upnDataProviderResult.Value.ECDIS_UPN_1
                }
            };

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN2_Title) && !string.IsNullOrEmpty(upnDataProviderResult.Value.ECDIS_UPN_2))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN2_Title,
                    Upn = upnDataProviderResult.Value.ECDIS_UPN_2
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN3_Title) && !string.IsNullOrEmpty(upnDataProviderResult.Value.ECDIS_UPN_3))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN3_Title,
                    Upn = upnDataProviderResult.Value.ECDIS_UPN_3
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN4_Title) && !string.IsNullOrEmpty(upnDataProviderResult.Value.ECDIS_UPN_4))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN4_Title,
                    Upn = upnDataProviderResult.Value.ECDIS_UPN_4
                });
            }

            if (!string.IsNullOrEmpty(upnDataProviderResult.Value.UPN5_Title) && !string.IsNullOrEmpty(upnDataProviderResult.Value.ECDIS_UPN_5))
            {
                userPermits.Add(new UserPermit
                {
                    Title = upnDataProviderResult.Value.UPN5_Title,
                    Upn = upnDataProviderResult.Value.ECDIS_UPN_5
                });
            }

            upnDetail.LicenceId = licid;
            upnDetail.UserPermits = userPermits;

            return upnDetail;
        }
    }
}
