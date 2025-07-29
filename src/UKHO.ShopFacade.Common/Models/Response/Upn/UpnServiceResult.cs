using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.Response.Upn
{
    [ExcludeFromCodeCoverage]
    public class UpnServiceResult : ServiceResponseResult<List<UserPermit>>
    {
        private UpnServiceResult(List<UserPermit>? value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null)
            : base(value, statusCode, errorResponse!)
        {
        }

        public static new UpnServiceResult Success(List<UserPermit> value) => new(value, HttpStatusCode.OK);
        public static new UpnServiceResult NoContent() => new(null, HttpStatusCode.NoContent);
        public static new UpnServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);
        public static UpnServiceResult InternalServerError() => new(null, HttpStatusCode.InternalServerError);
    }
}
