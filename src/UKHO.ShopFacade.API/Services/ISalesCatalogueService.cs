using UKHO.ShopFacade.Common.Models.SalesCatalogue;

namespace UKHO.ShopFacade.API.Services
{
    public interface ISalesCatalogueService
    {
        public Task<SalesCatalogueResponse> GetProductsFromSpecificDateAsync();
    }
}
