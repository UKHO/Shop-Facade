using Microsoft.AspNetCore.Mvc;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    public class UpnController : BaseController<UpnController>
    {
        public UpnController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        /// <summary>
        /// Get all User Permit Numbers (UPNs) associated with the requested licence.
        /// </summary>
        [HttpGet]
        [Route("/licenses/{licenceId}/s100/userpermits")]
        public IActionResult GetUPNs(int licenceId)
        {
            return Ok();
        }
    }
}
