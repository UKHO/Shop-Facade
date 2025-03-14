using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.API.Services
{
    public interface ISalesCatalogueService
    {
        public Task<SalesCatalogueResult> GetProductsCatalogueAsync(string correlationId);
    }
}
