using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.Permit
{
    [ExcludeFromCodeCoverage]
    public class PermitServiceResult : ServiceResponseResult<Stream>
    {
        private PermitServiceResult(Stream? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
        }

        public static new PermitServiceResult Success(Stream value) => new(value, HttpStatusCode.OK);
        public static new PermitServiceResult NoContent() => new(null, HttpStatusCode.NoContent);
        public static new PermitServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);
        public static PermitServiceResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
