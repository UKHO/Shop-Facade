using System.Net;

namespace UKHO.ShopFacade.Common.Models
{
    public class ServiceResponseResult<T> : Result<T>
    {
        public new T Value { get; }

        public new ErrorResponse ErrorResponse { get; }

        protected ServiceResponseResult(T value, HttpStatusCode statusCode, ErrorResponse errorResponse = null)
            : base(value, statusCode, errorResponse)
        {
            Value = value;
            ErrorResponse = errorResponse;
        }

        public static ServiceResponseResult<T> Success(T value) => new(value, HttpStatusCode.OK);

        public static ServiceResponseResult<T> NoContent() => new(default, HttpStatusCode.NoContent);

        public static ServiceResponseResult<T> NotFound(ErrorResponse errorResponse) => new(default, HttpStatusCode.NotFound, errorResponse);

        public static ServiceResponseResult<T> BadRequest(ErrorResponse errorResponse) => new(default, HttpStatusCode.BadRequest, errorResponse);
    }
}
