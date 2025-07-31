using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.S100Permit
{
    [ExcludeFromCodeCoverage]
    public class S100PermitServiceResult : ServiceResponseResult<Stream>
    {
        private S100PermitServiceResult(Stream? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
        }

        public static new S100PermitServiceResult Success(Stream value) => new(value, HttpStatusCode.OK);
        public static new S100PermitServiceResult InternalServerError(ErrorResponse errorResponse) => new(default!, HttpStatusCode.InternalServerError, errorResponse);
    }
}
