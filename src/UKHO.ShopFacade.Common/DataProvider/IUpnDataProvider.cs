using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public interface IUpnDataProvider
    {
        Task<UpnDataProviderResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId);
    }
}
