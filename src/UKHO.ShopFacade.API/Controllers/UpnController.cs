using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    [Authorize]
    public class UpnController : BaseController<UpnController>
    {
        private readonly ILogger<UpnController> _logger;
        public UpnController(IHttpContextAccessor httpContextAccessor, ILogger<UpnController> logger) : base(httpContextAccessor)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all User Permit Numbers (UPNs) associated with the requested licence.
        /// </summary>
        [HttpGet]
        [Route("/licences/{licenceId}/s100/userPermits")]
        [Authorize(Policy = ShopFacadeConstants.ShopFacadePolicy)]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Unauthorized, description: "<p>Unauthorized - either you have not provided valid token, or your token is not recognized.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Forbidden, description: "<p>Forbidden - you have no permission to use this API.</p>")]
        public IActionResult GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            return Ok();
        }
    }
}
