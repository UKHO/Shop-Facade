using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public interface IUpnDataProvider
    {
        Task<UpnDataProviderResult> GetUpnDetailsByLicenseId(int licenceId, string correlationId);
    }
}
