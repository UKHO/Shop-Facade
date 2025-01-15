using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            return Ok();
        }
    }
}
