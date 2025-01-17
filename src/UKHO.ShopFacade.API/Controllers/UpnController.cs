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
        /// <param name="licenceId" example="12345678">Requested licence id.</param>
        /// <response code="200">OK - Returns UPNs for the licence.</response>
        /// <response code="204">No Content - There are no UPNs for the licence.</response>
        /// <response code="400">Bad request - could be missing or invalid licenceId, it must be an integer and greater than zero.</response>
        /// <response code="401">Unauthorised - either you have not provided valid token, or your token is not recognised.</response>
        /// <response code="403">Forbidden - you have no permission to use this API.</response>
        /// <response code="404">Licence not found.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpGet]
        [Route("/licences/{licenceId}/s100/userPermits")]
        [Authorize(Policy = ShopFacadeConstants.ShopFacadePolicy)]
        [SwaggerOperation(Tags = new[] { "Licensing" }, Description = "<p>Returns all S-100 User Permit Numbers (UPNs) associated with the requested licence. There can be one or more S-100 UPNs for a licence.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, type: typeof(string), description: "<p>OK - Returns UPNs for the licence.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.NoContent, description: "<p>No Content - There are no UPNs for the licence.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(IDictionary<string, string>), description: "<p>Bad request - could be missing or invalid licenceId, it must be an integer and greater than zero.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Unauthorized, description: "<p>Unauthorised - either you have not provided valid token, or your token is not recognised.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.Forbidden, description: "<p>Forbidden - you have no permission to use this API.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.NotFound, type: typeof(IDictionary<string, string>), description: "<p>Licence not found.</p>")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.InternalServerError, type: typeof(IDictionary<string, string>), description: "<p>Internal Server Error.</p>")]
        public IActionResult GetUPNs([SwaggerParameter(Description = "Licence Id. It must be an integer value and greater than zero.", Required = true)] int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            return Ok();
        }
    }
}
