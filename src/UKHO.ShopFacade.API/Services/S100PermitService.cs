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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _permitServiceClient = permitServiceClient ?? throw new ArgumentNullException(nameof(permitServiceClient));
        }

        public async Task<S100PermitServiceResult> GetS100PermitZipFileAsync(PermitRequest permitRequest)
        {
            _logger.LogInformation(EventIds.GetPermitServiceRequestStarted.ToEventId(), ErrorDetails.GetPermitServiceRequestStartedMessage);
            var response = await _permitServiceClient.CallPermitServiceApiAsync(permitRequest);
            var result = await CreatePermitServiceResponse(response);
            return result;
        }

        private async Task<S100PermitServiceResult> CreatePermitServiceResponse(HttpResponseMessage httpResponse)
        {
            S100PermitServiceResult response;
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var body = await httpResponse.Content.ReadAsStreamAsync();
                response = S100PermitServiceResult.Success(body);
                _logger.LogInformation(EventIds.GetPermitServiceRequestCompleted.ToEventId(), ErrorDetails.GetPermitServiceRequestCompletedMessage);
            }
            else
            {
                response = S100PermitServiceResult.InternalServerError();
                _logger.LogError(EventIds.PermitServiceInternalError.ToEventId(), ErrorDetails.PermitServiceInternalErrorMessage);
            }
            return response;
        }
    }
}
