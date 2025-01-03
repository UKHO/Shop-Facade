using Microsoft.AspNetCore.Mvc;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
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
        [Route("/licenses/{licenceId}/s100/userpermits")]
        public IActionResult GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            return Ok();
        }
    }
}
