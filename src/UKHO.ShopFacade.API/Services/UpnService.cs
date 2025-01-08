using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Models;

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
            return await _upnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId);
        }
    }
}
