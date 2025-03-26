using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.Upn
{
    [ExcludeFromCodeCoverage]
    public class UpnDataProviderResult : ServiceResponseResult<S100UpnRecord>
    {
        private UpnDataProviderResult(S100UpnRecord? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value!, statusCode, errorResponse!)
        {
        }

        public static new UpnDataProviderResult Success(S100UpnRecord value) => new(value, HttpStatusCode.OK);
        public static new UpnDataProviderResult NoContent() => new(null, HttpStatusCode.NoContent);
        public static new UpnDataProviderResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);
    }
}
