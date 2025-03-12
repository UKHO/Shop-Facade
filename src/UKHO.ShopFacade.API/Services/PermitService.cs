using System.Net;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Extension;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService : IPermitService
    {
        private readonly IUpnService _upnService;
        private readonly ISalesCatalogueService _salesCatalogueService;
        private readonly IS100PermitService _s100PermitService;
        private readonly IOptions<PermitExpiryDaysConfiguration> _permitExpiryDaysConfiguration;

        public PermitService(IUpnService upnService, ISalesCatalogueService salesCatalogueService, IS100PermitService s100PermitService, IOptions<PermitExpiryDaysConfiguration> permitExpiryDaysConfiguration)
        {
            _upnService = upnService ?? throw new ArgumentNullException(nameof(upnService));
            _salesCatalogueService = salesCatalogueService ?? throw new ArgumentNullException(nameof(salesCatalogueService));
            _s100PermitService = s100PermitService ?? throw new ArgumentNullException(nameof(s100PermitService));
            _permitExpiryDaysConfiguration = permitExpiryDaysConfiguration ?? throw new ArgumentNullException(nameof(permitExpiryDaysConfiguration));
        }

        public async Task<PermitServiceResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);
            var result = HandleServiceResult(upnServiceResult.StatusCode, upnServiceResult.ErrorResponse);
            if (result != null) return result;

            var salesCatalogueResult = await _salesCatalogueService.GetProductsCatalogueAsync(correlationId);
            result = HandleServiceResult(salesCatalogueResult.StatusCode, salesCatalogueResult.ErrorResponse);
            if (result != null) return result;

            var permitRequest = PermitRequestMapper.MapToPermitRequest(salesCatalogueResult.Value, upnServiceResult.Value, _permitExpiryDaysConfiguration.Value.PermitExpiryDays);
            var s100PermitServiceResult = await _s100PermitService.GetS100PermitZipFileAsync(permitRequest, correlationId);

            return HandleServiceResult(s100PermitServiceResult.StatusCode, s100PermitServiceResult.ErrorResponse) ?? PermitServiceResult.Success(s100PermitServiceResult.Value);
        }

        private PermitServiceResult? HandleServiceResult(HttpStatusCode statusCode, ErrorResponse? errorResponse)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => null,
                HttpStatusCode.NoContent => PermitServiceResult.NoContent(),
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(errorResponse),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
