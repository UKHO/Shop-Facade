using System.Net;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;

namespace UKHO.ShopFacade.API.Services
{
    public class S100PermitService : IS100PermitService
    {
        private readonly ILogger<S100PermitService> _logger;
        private readonly IPermitServiceClient _permitServiceClient;
        public S100PermitService(ILogger<S100PermitService> logger, IPermitServiceClient permitServiceClient)
        {
            _logger = logger;
            _permitServiceClient = permitServiceClient;
        }

        public async Task<S100PermitServiceResult> GetS100PermitZipFileAsync(PermitRequest permitRequest)
        {
            _logger.LogInformation(EventIds.GetPermitServiceRequestStartedMessage.ToEventId(), ErrorDetails.GetPermitServiceRequestStartedMessage);
            var response = await _permitServiceClient.CallPermitServiceApiAsync(permitRequest);
            var result = await CreatePermitServiceResponse(response);
            return result;
        }

        private async Task<S100PermitServiceResult> CreatePermitServiceResponse(HttpResponseMessage httpResponse)
        {
            S100PermitServiceResult response;
            var body = await httpResponse.Content.ReadAsStreamAsync();

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response = S100PermitServiceResult.Success(body);
                _logger.LogInformation(EventIds.GetPermitServiceRequestCompletedMessage.ToEventId(), ErrorDetails.GetPermitServiceRequestCompletedMessage);
            }
            else
            {
                response = S100PermitServiceResult.InternalServerError();
                _logger.LogInformation(EventIds.PermitServiceInternalErrorMessage.ToEventId(), ErrorDetails.PermitServiceInternalErrorMessage, httpResponse.StatusCode);
            }
            return response;
        }
    }
}
