using System.Net;
using Microsoft.AspNetCore.Mvc;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    public class UpnController : BaseController<UpnController>
    {
        private readonly ILogger<UpnController> _logger;
        public readonly IUpnService _upnService;

        public UpnController(IHttpContextAccessor httpContextAccessor, ILogger<UpnController> logger, IUpnService upnService) : base(httpContextAccessor)
        {
            _logger = logger;
            _upnService = upnService;
        }

        /// <summary>
        /// Get all User Permit Numbers (UPNs) associated with the requested licence.
        /// </summary>
        [HttpGet]
        [Route("/licenses/{licenceId}/s100/userpermits")]
        public async Task<IActionResult> GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            if (licenceId <= 0)
            {
                return BadRequest(UpnServiceResult.SetErrorResponse(GetCorrelationId(), "licenceId", "Bad request - could be missing or invalid licenceId, it must be an integer and greater than zero."));
            }

            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, GetCorrelationId());
            return upnServiceResult.StatusCode switch
            {
                HttpStatusCode.OK => Ok(upnServiceResult.Value),
                HttpStatusCode.NotFound => NotFound(upnServiceResult.ErrorResponse),
                HttpStatusCode.InternalServerError => StatusCode((int)upnServiceResult.StatusCode, upnServiceResult.ErrorResponse),
                _ => StatusCode((int)upnServiceResult.StatusCode)
            };
        }
    }
}
