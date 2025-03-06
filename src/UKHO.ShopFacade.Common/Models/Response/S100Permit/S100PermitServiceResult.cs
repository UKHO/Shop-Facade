using System.Net;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.Models.Response.S100Permit
{
    public class S100PermitServiceResult : ServiceResponseResult<Stream>
    {
        public new ErrorResponse ErrorResponse { get; }
        public new HttpStatusCode StatusCode { get; }

        private S100PermitServiceResult(Stream? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse!;
        }

        public static new S100PermitServiceResult Success(Stream value) => new(value, HttpStatusCode.OK);

        public static new S100PermitServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);

        public static S100PermitServiceResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
