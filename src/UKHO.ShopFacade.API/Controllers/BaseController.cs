using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using UKHO.ShopFacade.Common.Constants;

namespace UKHO.ShopFacade.API.Controllers
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseController<T> : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get Correlation Id.
        /// </summary>
        /// <remarks>
        /// Correlation Id is Guid based id to track request.
        /// Correlation Id can be found in request headers.
        /// </remarks>
        /// <returns>Correlation Id</returns>
        protected string GetCorrelationId()
        {
            return _httpContextAccessor.HttpContext!.Request.Headers[ApiHeaderKeys.XCorrelationIdHeaderKey].FirstOrDefault()!;
        }
    }
}
