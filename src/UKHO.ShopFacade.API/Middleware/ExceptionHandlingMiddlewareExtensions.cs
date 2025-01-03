using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
