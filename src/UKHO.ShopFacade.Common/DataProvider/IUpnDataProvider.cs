using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public interface IUpnDataProvider
    {
        Task<UpnServiceResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId);
    }
}
