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
    public class PermitController : BaseController<PermitController>
    {
        private readonly ILogger<PermitController> _logger;

        public PermitController(IHttpContextAccessor httpContextAccessor, ILogger<PermitController> logger) : base(httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a zip file containing all the S-100 permit file(s) of the requested licence..
        /// </summary>
        /// <param name="licenceId" example="12345678">Requested licence id.</param>
        /// <response code="200">OK - Returns a zip containing permit files</response>
        /// <response code="401">Unauthorised - either you have not provided valid token, or your token is not recognised.</response>
        /// <response code="403">Forbidden - you have no permission to use this API.</response>
        [HttpGet]
        [Route("/v1/licences/{licenceId}/s100/permits")]
        [Authorize(Policy = ShopFacadeConstants.ShopFacadePermitPolicy)]
        [SwaggerOperation(Tags = new[] { "Licensing" }, Description = "<p>Returns a zip file containing all the S-100 permit file(s) of the requested licence.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(string), description: "<p>OK - Returns a zip containing permit files</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Unauthorized, description: "<p>Unauthorised - either you have not provided valid token, or your token is not recognised.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Forbidden, description: "<p>Forbidden - you have no permission to use this API.</p>")]
        public IActionResult GetPermits([SwaggerParameter(Description = "Licence Id. It must be an integer value and greater than zero.", Required = true)] int licenceId)
        {
            _logger.LogInformation(EventIds.GetPermitsCallStarted.ToEventId(), "GetPermits API Call Started.");

            return Ok();
        }
    }
}
