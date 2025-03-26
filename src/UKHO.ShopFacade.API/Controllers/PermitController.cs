using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Permit;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    [Authorize]
    public class PermitController : BaseController<PermitController>
    {
        private readonly ILogger<PermitController> _logger;
        private readonly IPermitService _permitService;

        public PermitController(IHttpContextAccessor httpContextAccessor, ILogger<PermitController> logger, IPermitService permitService) : base(httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _permitService = permitService ?? throw new ArgumentNullException(nameof(permitService));
        }

        /// <summary>
        /// Get a zip file containing all the S-100 permit file(s) of the requested licence.
        /// </summary>
        /// <param name="licenceId" example="12345678">Requested licence id.</param>
        /// <param name="productType" example="S100">Requested product type.</param>
        /// <response code="200">OK - Returns a zip containing permit files</response>
        /// <response code="401">Unauthorized - either you have not provided valid token, or your token is not recognized.</response>
        /// <response code="403">Forbidden - you have no permission to use this API.</response>
        [HttpGet]
        [Route("/v1/licences/{licenceId}/{productType}/permits")]
        [Authorize(Policy = ShopFacadeConstants.ShopFacadePermitPolicy)]
        [SwaggerOperation(Tags = new[] { "Licensing" }, Description = "<p>Returns a zip file containing all the S-100 permit file(s) of the requested licence.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(string), description: "<p>OK - Returns a zip containing permit files</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Unauthorized, description: "<p>Unauthorized - either you have not provided valid token, or your token is not recognized.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Forbidden, description: "<p>Forbidden - you have no permission to use this API.</p>")]
        public async Task<IActionResult> GetPermits([FromRoute] string productType, [SwaggerParameter(Required = true)] int licenceId)
        {
            _logger.LogInformation(EventIds.GetPermitsCallStarted.ToEventId(), ErrorDetails.GetPermitsCallStartedMessage);

            if (licenceId <= 0)
            {
                _logger.LogWarning(EventIds.InvalidLicenceId.ToEventId(), ErrorDetails.InvalidLicenceIdMessage);
                return BadRequest(PermitResult.SetErrorResponse(GetCorrelationId(), ErrorDetails.Source, ErrorDetails.InvalidLicenceIdMessage));
            }

            var permitResult = await _permitService.GetPermitDetails(licenceId, GetCorrelationId());

            switch (permitResult.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation(EventIds.GetPermitsCallCompleted.ToEventId(), ErrorDetails.GetPermitsCallCompletedMessage);
                    return File(permitResult.Value, PermitServiceConstants.ZipContentType, PermitServiceConstants.PermitZipFileName);
                case HttpStatusCode.NoContent:
                    return NoContent();
                case HttpStatusCode.NotFound:
                    return NotFound(permitResult.ErrorResponse);
                default:
                    _logger.LogError(EventIds.InternalError.ToEventId(), ErrorDetails.PermitInternalServerErrorMessage);
                    return StatusCode((int)permitResult.StatusCode, new InternalServerErrorResponse { CorrelationId = permitResult.ErrorResponse.CorrelationId });
            }
        }
    }
}
