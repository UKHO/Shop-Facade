using UKHO.ShopFacade.Common.Models.Response.Permit;

namespace UKHO.ShopFacade.API.Services
{
    public interface IPermitService
    {
        Task<PermitResult> GetPermitDetails(int licenceId, string correlationId);
    }
}
