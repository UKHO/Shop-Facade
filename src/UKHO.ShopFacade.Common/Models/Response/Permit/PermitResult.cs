using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.Permit
{
    [ExcludeFromCodeCoverage]
    public class PermitResult : ServiceResponseResult<Stream>
    {
        private PermitResult(Stream? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
        }

        public static new PermitResult Success(Stream value) => new(value, HttpStatusCode.OK);
        public static new PermitResult NoContent() => new(null, HttpStatusCode.NoContent);
        public static new PermitResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);
        public static PermitResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
