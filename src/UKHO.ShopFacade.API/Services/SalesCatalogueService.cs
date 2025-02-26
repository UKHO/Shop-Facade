using System.Net;
using Newtonsoft.Json;
using UKHO.ShopFacade.Common.ClientProvider;
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

        public async Task<SalesCatalogueResponse> GetProductsCatalogueAsync()
        {
            _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestStarted.ToEventId(), "Retrieval process of the latest S-100 basic catalogue data from Sales catalogue service is started.");
            var httpResponse = await _salesCatalogueClient.CallSalesCatalogueServiceApi();
            return await CreateSalesCatalogueServiceResponse(httpResponse);
        }

        private async Task<SalesCatalogueResponse> CreateSalesCatalogueServiceResponse(HttpResponseMessage httpResponse)
        {
            var response = new SalesCatalogueResponse();
            var body = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.StatusCode != HttpStatusCode.OK && httpResponse.StatusCode != HttpStatusCode.NotModified)
            {
                response.ResponseCode = httpResponse.StatusCode;
                response.ResponseBody = null;
                _logger.LogInformation(EventIds.SalesCatalogueServiceNonOkResponse.ToEventId(), "Error in sales catalogue service with uri:{RequestUri} and responded with {StatusCode}.", httpResponse.RequestMessage!.RequestUri, httpResponse.StatusCode);
            }
            else
            {
                response.ResponseCode = httpResponse.StatusCode;
                var lastModified = httpResponse.Content.Headers.LastModified;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response.ResponseBody = JsonConvert.DeserializeObject<List<Products>>(body)!;
                }
                if (lastModified != null)
                {
                    response.LastModified = ((DateTimeOffset)lastModified).UtcDateTime;
                }
                _logger.LogInformation(EventIds.GetSalesCatalogueDataRequestCompleted.ToEventId(), "Retrieval process of the latest S-100 basic catalogue data from Sales catalogue service is completed.");
            }

            return response;
        }
    }
}
