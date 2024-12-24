// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using UKHO.ShopFacade.Common.Constants;

namespace UKHO.ShopFacade.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
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

            var logger = httpContext.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>();
            using (logger.BeginScope(state))
            {
                await _next(httpContext);
            }
        }
    }
}
