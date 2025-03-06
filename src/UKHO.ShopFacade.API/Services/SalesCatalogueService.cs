using System.Net;
using Newtonsoft.Json;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.API.Services
{
    public class SalesCatalogueService : ISalesCatalogueService
    {
        private readonly ILogger<SalesCatalogueService> _logger;
        private readonly ISalesCatalogueClient _salesCatalogueClient;

        public SalesCatalogueService(ISalesCatalogueClient salesCatalogueClient,
                                     ILogger<SalesCatalogueService> logger)
        {
            _logger = logger;
            _salesCatalogueClient = salesCatalogueClient;
        }

        public async Task<SalesCatalogueResult> GetProductsCatalogueAsync()
        {
            _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestStarted.ToEventId(), ErrorDetails.GetSalesCatalogueDataRequestStartedMessage);
            var httpResponse = await _salesCatalogueClient.CallSalesCatalogueServiceApi();
            return await CreateSalesCatalogueServiceResponse(httpResponse);
        }

        private async Task<SalesCatalogueResult> CreateSalesCatalogueServiceResponse(HttpResponseMessage httpResponse)
        {
            SalesCatalogueResult response;

            if (httpResponse.StatusCode != HttpStatusCode.OK && httpResponse.StatusCode != HttpStatusCode.NotModified)
            {
                response = httpResponse.StatusCode switch
                {
                    _ => SalesCatalogueResult.InternalServerError(SalesCatalogueResult.SetErrorResponse(httpResponse.Content.ReadAsStringAsync().Result, ErrorDetails.ScsSource, ErrorDetails.ScsInternalErrorMessage)),
                };

                _logger.LogInformation(EventIds.SalesCatalogueServiceNonOkResponse.ToEventId(), ErrorDetails.SalesCatalogueDataRequestInternalServerErrorMessage, httpResponse.RequestMessage!.RequestUri, httpResponse.StatusCode);
            }
            else
            {
                var body = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = SalesCatalogueResult.Success(JsonConvert.DeserializeObject<List<Products>>(body)!);
                }
                else
                {
                    response = SalesCatalogueResult.NotModified([]);
                }

                _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestCompleted.ToEventId(), ErrorDetails.GetSalesCatalogueDataRequestCompletedMessage);
            }

            return response;
        }
    }
}
