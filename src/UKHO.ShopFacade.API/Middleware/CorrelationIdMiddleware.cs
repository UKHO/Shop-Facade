using System.Diagnostics.CodeAnalysis;
using UKHO.ShopFacade.Common.Constants;

namespace UKHO.ShopFacade.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var correlationId = httpContext.Request.Headers[ApiHeaderKeys.XCorrelationIdHeaderKey].FirstOrDefault();

            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Append(ApiHeaderKeys.XCorrelationIdHeaderKey, correlationId);
            }

            httpContext.Response.Headers.Append(ApiHeaderKeys.XCorrelationIdHeaderKey, correlationId);

            var state = new Dictionary<string, object>
            {
                [ApiHeaderKeys.XCorrelationIdHeaderKey] = correlationId!,
            };

            using (_logger.BeginScope(state))
            {
                await _next(httpContext);
            }
        }
    }
}
