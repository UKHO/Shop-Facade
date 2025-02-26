using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public interface IUpnService
    {
        Task<UpnServiceResult> GetUpnDetails(int licenceId, string correlationId);
    }
}
