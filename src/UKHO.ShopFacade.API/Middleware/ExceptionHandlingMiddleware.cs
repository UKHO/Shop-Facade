using System.Net;
using Microsoft.AspNetCore.Mvc;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Exceptions;

namespace UKHO.ShopFacade.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ShopFacadeException shopFacadeException)
            {
                await HandleExceptionAsync(httpContext, shopFacadeException, shopFacadeException.EventId, shopFacadeException.Message, shopFacadeException.MessageArguments);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception, EventIds.UnhandledException.ToEventId(), exception.Message);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception, EventId eventId, string message, params object[] messageArgs)
        {
            httpContext.Response.ContentType = ApiHeaderKeys.ContentType;
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(eventId, exception, message, messageArgs);

            var correlationId = httpContext.Request.Headers[ApiHeaderKeys.XCorrelationIdHeaderKey].FirstOrDefault()!;
            var problemDetails = new ProblemDetails
            {
                Extensions =
                {
                    ["correlationId"] = correlationId
                }
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
