namespace UKHO.ShopFacade.Common.ClientProvider
{
    public interface IS100PermitServiceClient
    {
        public Task<HttpResponseMessage> CallPermitServiceApiAsync(object requestBody);
    }
}
