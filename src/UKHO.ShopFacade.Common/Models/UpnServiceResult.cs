using System.Net;

namespace UKHO.ShopFacade.Common.Models
{
    public class UpnServiceResult : ServiceResponseResult<S100UpnRecord>
    {
        public new S100UpnRecord Value { get; }
        public new ErrorResponse ErrorResponse { get; }
        public new HttpStatusCode StatusCode { get; }

        private UpnServiceResult(S100UpnRecord value, HttpStatusCode statusCode, ErrorResponse errorResponse = null)
            : base(value, statusCode, errorResponse)
        {
            Value = value;
            StatusCode = statusCode;
            ErrorResponse = errorResponse;
        }

        public new static UpnServiceResult Success(S100UpnRecord value) => new(value, HttpStatusCode.OK);

        public new static UpnServiceResult NotFound(ErrorResponse errorResponse) => new(null, HttpStatusCode.NotFound, errorResponse);

        public new static UpnServiceResult BadRequest(ErrorResponse errorResponse) => new(null, HttpStatusCode.BadRequest, errorResponse);

        public static UpnServiceResult InternalServerError(ErrorResponse errorResponse) => new(null, HttpStatusCode.InternalServerError, errorResponse);

        public static ErrorResponse SetErrorResponse(string correlationId, string source, string description)
        {
            return new ErrorResponse
            {
                CorrelationId = correlationId,
                Errors =
                   [
                       new ErrorDetail
                       {
                           Description = description,
                           Source = source
                       }
                   ]
            };
        }
    }
}
