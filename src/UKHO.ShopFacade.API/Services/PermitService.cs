using System.Net;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService(IUpnService upnService) : IPermitService
    {
        private readonly IUpnService _upnService = upnService;

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);

            return upnServiceResult.StatusCode switch
            {
                HttpStatusCode.OK => PermitServiceResult.Success(),
                HttpStatusCode.NoContent => PermitServiceResult.NoContent(),
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(upnServiceResult.ErrorResponse),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
