using System.Net;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Extension;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Permit;

namespace UKHO.ShopFacade.API.Services
{
    public class PermitService(IUpnService upnService, ISalesCatalogueService salesCatalogueService, IS100PermitService s100PermitService, IOptions<PermitExpiryDaysConfiguration> permitExpiryDaysConfiguration) : IPermitService
    {
        private readonly IUpnService _upnService = upnService;
        private readonly ISalesCatalogueService _salesCatalogueService = salesCatalogueService;
        private readonly IS100PermitService _s100PermitService = s100PermitService;
        private readonly IOptions<PermitExpiryDaysConfiguration> _permitExpiryDaysConfiguration = permitExpiryDaysConfiguration;

        public async Task<PermitResult> GetPermitDetails(int licenceId, string correlationId)
        {
            var upnServiceResult = await _upnService.GetUpnDetails(licenceId, correlationId);
            var result = HandleServiceResult(upnServiceResult.StatusCode, upnServiceResult.ErrorResponse);
            if (result != null) return result;

            var salesCatalogueResult = await _salesCatalogueService.GetProductsCatalogueAsync(correlationId);
            result = HandleServiceResult(salesCatalogueResult.StatusCode, salesCatalogueResult.ErrorResponse);
            if (result != null) return result;

            var permitRequest = PermitRequestMapper.MapToPermitRequest(salesCatalogueResult.Value, upnServiceResult.Value, _permitExpiryDaysConfiguration.Value.PermitExpiryDays);
            var s100PermitServiceResult = await _s100PermitService.GetS100PermitZipFileAsync(permitRequest, correlationId);

            return HandleServiceResult(s100PermitServiceResult.StatusCode, s100PermitServiceResult.ErrorResponse) ?? PermitResult.Success(s100PermitServiceResult.Value);
        }

        private PermitResult? HandleServiceResult(HttpStatusCode statusCode, ErrorResponse? errorResponse)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => null,
                HttpStatusCode.NoContent => PermitResult.NoContent(),
                HttpStatusCode.NotFound => PermitResult.NotFound(errorResponse!),
                _ => PermitResult.InternalServerError()
            };
        }
    }
}
