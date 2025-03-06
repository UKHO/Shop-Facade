// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.Common.Extension
{
    public static class ProductModelMapper
    {
      public static IEnumerable<ProductModel> MapToProductModel(this Products product,int expireDays)
        {
            yield return  new  ProductModel
            {
                ProductName = product.ProductName,
                EditionNumber = product.LatestEditionNumber,
                PermitExpiryDate = DateTime.UtcNow.AddDays(expireDays).ToString("yyyy-MM-dd")
            };
        }
    }
}
