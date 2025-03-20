using System.Net;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;

namespace UKHO.ShopFacade.API.Services
{
    public class S100PermitService(ILogger<S100PermitService> logger, IPermitServiceClient permitServiceClient) : IS100PermitService
    {
        private readonly ILogger<S100PermitService> _logger = logger;
        private readonly IPermitServiceClient _permitServiceClient = permitServiceClient;

        public async Task<S100PermitServiceResult> GetS100PermitZipFileAsync(PermitRequest permitRequest, string correlationId)
        {
            _logger.LogInformation(EventIds.GetS100PermitServiceRequestStarted.ToEventId(), ErrorDetails.GetS100PermitServiceRequestStartedMessage);
            var response = await _permitServiceClient.CallPermitServiceApiAsync(permitRequest, correlationId);
            var result = await CreatePermitServiceResponse(response, correlationId);
            return result;
        }

        private async Task<S100PermitServiceResult> CreatePermitServiceResponse(HttpResponseMessage httpResponse, string correlationId)
        {
            S100PermitServiceResult response;
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var body = await httpResponse.Content.ReadAsStreamAsync();
                response = S100PermitServiceResult.Success(body);
                _logger.LogInformation(EventIds.GetS100PermitServiceRequestCompleted.ToEventId(), ErrorDetails.GetS100PermitServiceRequestCompletedMessage);
            }
            else
            {
                response = httpResponse.StatusCode switch
                {
                    _ => S100PermitServiceResult.InternalServerError(S100PermitServiceResult.SetErrorResponse(correlationId, ErrorDetails.S100PermitServiceSource, ErrorDetails.S100PermitServiceInternalServerErrorMessage)),
                };

                _logger.LogError(EventIds.S100PermitServiceInternalServerError.ToEventId(), ErrorDetails.S100PermitServiceInternalServerErrorMessage, httpResponse.StatusCode, httpResponse.RequestMessage!.RequestUri);
            }
            return response;
        }
    }
}
