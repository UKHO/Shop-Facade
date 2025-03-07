using System.Net;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Extension;
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
            var salesCatalogueResult = await _salesCatalogueService.GetProductsCatalogueAsync();
            var permitRequest = PermitRequestMapper.MapToPermitRequest(salesCatalogueResult.Value, upnServiceResult.Value, _permitExpiryDaysConfiguration.Value.PermitExpiryDays);
            var s100PermitServiceResult = await _s100PermitService.GetS100PermitZipFileAsync(permitRequest);

            return s100PermitServiceResult.StatusCode switch
            {
                HttpStatusCode.OK => PermitServiceResult.Success(s100PermitServiceResult.Value),
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(s100PermitServiceResult.ErrorResponse),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
