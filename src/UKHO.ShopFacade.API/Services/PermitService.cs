using System.Net;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService : IPermitService
    {
        private readonly IUpnService _upnService;
        private readonly ISalesCatalogueService _salesCatalogueService;

        public PermitService(IUpnService upnService, ISalesCatalogueService salesCatalogueService)
        {
            _upnService = upnService ?? throw new ArgumentNullException(nameof(upnService));
            _salesCatalogueService = salesCatalogueService ?? throw new ArgumentNullException(nameof(salesCatalogueService));
        }

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);
            var products = await _salesCatalogueService.GetProductsCatalogueAsync();

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
