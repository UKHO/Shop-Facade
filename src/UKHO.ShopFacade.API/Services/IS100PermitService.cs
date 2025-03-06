using UKHO.ShopFacade.Common.Models.Response.S100Permit;

namespace UKHO.ShopFacade.API.Services
{
    public interface IS100PermitService
    {
        public Task<S100PermitServiceResult> GetS100PermitZipFileAsync(PermitRequest permitRequest);
    }
}
