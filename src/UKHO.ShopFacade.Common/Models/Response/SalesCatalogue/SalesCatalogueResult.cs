using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class SalesCatalogueResult : ServiceResponseResult<List<Products>>
    {
        private SalesCatalogueResult(List<Products> value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null, string? origin = null)
    : base(value!, statusCode, errorResponse!, origin!)
        {
        }

        public static new SalesCatalogueResult Success(List<Products> value) => new(value, HttpStatusCode.OK);

        public static SalesCatalogueResult NotModified(List<Products> value) => new(value, HttpStatusCode.NotModified);

        public static new SalesCatalogueResult InternalServerError(ErrorResponse errorResponse, string origin) => new(default!, HttpStatusCode.InternalServerError, errorResponse, origin);
    }
}
