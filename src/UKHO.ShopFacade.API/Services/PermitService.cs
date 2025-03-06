using System.Net;
using UKHO.ShopFacade.Common.Extension;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService : IPermitService
    {
        private readonly IUpnService _upnService;
        private readonly ISalesCatalogueService _salesCatalogueService;
        private readonly IS100PermitService _s100PermitService;

        public PermitService(IUpnService upnService, ISalesCatalogueService salesCatalogueService, IS100PermitService s100PermitService)
        {
            _upnService = upnService ?? throw new ArgumentNullException(nameof(upnService));
            _salesCatalogueService = salesCatalogueService ?? throw new ArgumentNullException(nameof(salesCatalogueService));
            _s100PermitService = s100PermitService ?? throw new ArgumentNullException(nameof(s100PermitService));
        }

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);
            var products = await _salesCatalogueService.GetProductsCatalogueAsync();
            var productModel = products.Value.SelectMany(p => p.MapToProductModel(30)).ToList();
            var permitRequest = PermitRequestMapper.MapToPermitRequest(productModel, upnServiceResult.Value);
            var response = _s100PermitService.GetS100PermitZipFileAsync(permitRequest);

            return upnServiceResult.StatusCode switch
            {
                HttpStatusCode.OK => PermitServiceResult.Success(response),
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(upnServiceResult.ErrorResponse),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
