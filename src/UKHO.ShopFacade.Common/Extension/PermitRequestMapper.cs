using System.Diagnostics.CodeAnalysis;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.Extension
{
    [ExcludeFromCodeCoverage]
    public static class PermitRequestMapper
    {
        public static PermitRequest MapToPermitRequest(List<Products> productModel, List<UserPermit> UserPermits,int PermitExpiryDays)
        {
            return new PermitRequest
            {
                Products = productModel.SelectMany(p => p.MapToProductModel(PermitExpiryDays)).ToList(),
                UserPermits = UserPermits
            };
        }
    }
}
