using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class SalesCatalogueResult : ServiceResponseResult<List<Products>>
    {
        private SalesCatalogueResult(List<Products> value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
    : base(value!, statusCode, errorResponse!)
        {
        }

        public static new SalesCatalogueResult Success(List<Products> value) => new(value, HttpStatusCode.OK);

        public static SalesCatalogueResult NotModified(List<Products> value) => new(value, HttpStatusCode.NotModified);

        public static new SalesCatalogueResult InternalServerError(ErrorResponse errorResponse) => new(default!, HttpStatusCode.InternalServerError, errorResponse);
    }
}
