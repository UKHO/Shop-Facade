using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class Result<T>
    {
        public T Value { get; }

        public HttpStatusCode StatusCode { get; }

        public ErrorResponse ErrorResponse { get; }

        public string Origin { get; set; }

        protected Result(T value, HttpStatusCode statusCode, ErrorResponse? errorResponse = null, string origin = null)
        {
            Value = value;
            StatusCode = statusCode;
            ErrorResponse = errorResponse!;
            Origin = origin;
        }

        public bool IsSuccess => StatusCode == HttpStatusCode.OK;
    }
}
