namespace UKHO.ShopFacade.Common.ClientProvider
{
    public interface ISalesCatalogueClient
    {
        public Task<HttpResponseMessage> CallSalesCatalogueServiceApi();
    }
}
