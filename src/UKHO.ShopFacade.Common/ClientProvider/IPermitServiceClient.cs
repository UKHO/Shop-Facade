using UKHO.ShopFacade.Common.Models.Response.S100Permit;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    public interface IPermitServiceClient
    {
        public Task<HttpResponseMessage> CallPermitServiceApiAsync(PermitRequest requestBody, string correlationId);
    }
}
