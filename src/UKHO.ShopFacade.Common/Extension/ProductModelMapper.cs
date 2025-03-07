using System.Diagnostics.CodeAnalysis;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.Common.Extension
{
    [ExcludeFromCodeCoverage]
    public static class ProductModelMapper
    {
      public static IEnumerable<S100Product> MapToProductModel(this Products product,int expireDays)
        {
            yield return  new  S100Product
            {
                ProductName = product.ProductName,
                EditionNumber = product.LatestEditionNumber,
                PermitExpiryDate = DateTime.UtcNow.AddDays(expireDays).ToString("yyyy-MM-dd")
            };
        }
    }
}
