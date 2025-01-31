using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.ShopFacade.Common.Models.Response;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class UpnServiceResult : ServiceResponseResult<List<UserPermit>>
    {
        public new ErrorResponse ErrorResponse { get; }
        public new HttpStatusCode StatusCode { get; }

        private UpnServiceResult(List<UserPermit>? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse!;
        }

        public static new UpnServiceResult Success(List<UserPermit> value) => new(value, HttpStatusCode.OK);

        public static new UpnServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);

        public static UpnServiceResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
