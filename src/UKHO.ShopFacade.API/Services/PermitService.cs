using System.Net;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService(IUpnService upnService, ISalesCatalogueService salesCatalogueService) : IPermitService
    {
        private readonly IUpnService _upnService = upnService;
        private readonly ISalesCatalogueService _salesCatalogueService = salesCatalogueService;

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);
            var products = await _salesCatalogueService.GetProductsCatalogueAsync(correlationId);

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
