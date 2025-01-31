using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class UpnDataProviderResult : ServiceResponseResult<S100UpnRecord>
    {
        public new ErrorResponse ErrorResponse { get; }
        public new HttpStatusCode StatusCode { get; }

        private UpnDataProviderResult(S100UpnRecord? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value!, statusCode, errorResponse!)
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse!;
        }

        public static new UpnDataProviderResult Success(S100UpnRecord value) => new(value, HttpStatusCode.OK);

        public static new UpnDataProviderResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);
    }
}
