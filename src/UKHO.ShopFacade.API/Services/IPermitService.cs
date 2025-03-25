using UKHO.ShopFacade.Common.Models.Response.Permit;

namespace UKHO.ShopFacade.API.Services
{
    public interface IPermitService
    {
        Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId);
    }
}
