using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.Upn
{
    [ExcludeFromCodeCoverage]
    public class PermitServiceResult : ServiceResponseResult<Stream>
    {
        public new ErrorResponse ErrorResponse { get; }
        public new HttpStatusCode StatusCode { get; }

        private PermitServiceResult(Stream? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse!;
        }

        public static new PermitServiceResult Success() => new(null, HttpStatusCode.OK);

        public static new PermitServiceResult NoContent() => new(null, HttpStatusCode.NoContent);

        public static new PermitServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);

        public static PermitServiceResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
