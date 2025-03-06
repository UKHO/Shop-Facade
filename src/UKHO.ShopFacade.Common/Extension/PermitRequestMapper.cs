// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.Extension
{
    public static class PermitRequestMapper
    {
        public static PermitRequest MapToPermitRequest(List<ProductModel> productModel, List<UserPermit> UserPermits)
        {
            return new PermitRequest
            {
                Products = productModel,
                UserPermits = UserPermits
            };
        }
    }
}
