using System.Net;
using Microsoft.AspNetCore.Mvc;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    public class UpnController : BaseController<UpnController>
    {
        private readonly ILogger<UpnController> _logger;
        private readonly IUpnService _upnService;

        public UpnController(IHttpContextAccessor httpContextAccessor, ILogger<UpnController> logger, IUpnService upnService) : base(httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _upnService = upnService ?? throw new ArgumentNullException(nameof(upnService));
        }

        /// <summary>
        /// Get all User Permit Numbers (UPNs) associated with the requested licence.
        /// </summary>
        [HttpGet]
        [Route("/licences/{licenceId}/s100/userpermits")]
        public async Task<IActionResult> GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsCallStarted.ToEventId(), ErrorDetails.GetUPNsCallStartedMessage);

            if (licenceId <= 0)
            {
                _logger.LogInformation(EventIds.InvalidLicenceId.ToEventId(), ErrorDetails.InvalidLicenceIdMessage);
                return BadRequest(UpnServiceResult.SetErrorResponse(GetCorrelationId(), ErrorDetails.Source, ErrorDetails.InvalidLicenceIdMessage));
            }

            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, GetCorrelationId());

            switch (upnServiceResult.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation(EventIds.GetUPNsCallCompleted.ToEventId(), ErrorDetails.GetUPNsCallCompletedMessage);
                    return Ok(upnServiceResult.Value);
                case HttpStatusCode.NotFound:
                    _logger.LogInformation(EventIds.LicenceNotFound.ToEventId(), ErrorDetails.LicenceNotFoundMessage);
                    return NotFound(upnServiceResult.ErrorResponse);
                default:
                    _logger.LogInformation(EventIds.InternalError.ToEventId(), ErrorDetails.InternalErrorMessage);
                    return StatusCode((int)upnServiceResult.StatusCode);
            }
        }
    }
}
