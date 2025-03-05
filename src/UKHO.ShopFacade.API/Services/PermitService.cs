using System.Net;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService : IPermitService
    {
        private readonly IUpnService _upnService;

        public PermitService(IUpnService upnService)
        {
            _upnService = upnService ?? throw new ArgumentNullException(nameof(upnService));
        }

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {

            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);

            return upnServiceResult.StatusCode switch
            {
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(upnServiceResult.ErrorResponse),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
