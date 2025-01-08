using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.API.Services
{
    public interface IUpnService
    {
        Task<UpnServiceResult> GetUpnDetails(int licenceId, string correlationId);
    }
}
