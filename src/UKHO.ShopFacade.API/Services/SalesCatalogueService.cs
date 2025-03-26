using System.Net;
using Newtonsoft.Json;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.API.Services
{
    public class SalesCatalogueService(ISalesCatalogueClient salesCatalogueClient,
                                     ILogger<SalesCatalogueService> logger) : ISalesCatalogueService
    {
        private readonly ILogger<SalesCatalogueService> _logger = logger;
        private readonly ISalesCatalogueClient _salesCatalogueClient = salesCatalogueClient;

        public async Task<SalesCatalogueResult> GetProductsCatalogueAsync(string correlationId)
        {
            _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestStarted.ToEventId(), ErrorDetails.GetSalesCatalogueDataRequestStartedMessage, ErrorDetails.ScsSource);
            var httpResponse = await _salesCatalogueClient.CallSalesCatalogueServiceApi(correlationId);
            return await CreateSalesCatalogueServiceResponse(httpResponse, correlationId);
        }

        private async Task<SalesCatalogueResult> CreateSalesCatalogueServiceResponse(HttpResponseMessage httpResponse, string correlationId)
        {
            SalesCatalogueResult response;
            var responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.StatusCode != HttpStatusCode.OK && httpResponse.StatusCode != HttpStatusCode.NotModified)
            {
                response = httpResponse.StatusCode switch
                {
                    _ => SalesCatalogueResult.InternalServerError(SalesCatalogueResult.SetErrorResponse(correlationId, ErrorDetails.ScsSource, ErrorDetails.ScsInternalErrorMessage)),
                };

                _logger.LogError(EventIds.SalesCatalogueServiceNonOkResponse.ToEventId(), ErrorDetails.SalesCatalogueDataRequestInternalServerErrorMessage, ErrorDetails.ScsSource, httpResponse.StatusCode, httpResponse.RequestMessage!.RequestUri, responseBody);
            }
            else
            {
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = SalesCatalogueResult.Success(JsonConvert.DeserializeObject<List<Products>>(responseBody)!);
                }
                else
                {
                    response = SalesCatalogueResult.NotModified([]);
                }

                _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestCompleted.ToEventId(), ErrorDetails.GetSalesCatalogueDataRequestCompletedMessage, ErrorDetails.ScsSource);
            }

            return response;
        }
    }
}
