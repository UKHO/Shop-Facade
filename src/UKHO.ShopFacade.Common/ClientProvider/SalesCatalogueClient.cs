using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    [ExcludeFromCodeCoverage] //Excluded from code coverage as it has actual http calls 
    public class SalesCatalogueClient : ISalesCatalogueClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<SalesCatalogueConfiguration> _salesCatalogueConfig;

        public SalesCatalogueClient(HttpClient httpClient, IOptions<SalesCatalogueConfiguration> salesCatalogueConfig)
        {
            _httpClient = httpClient;
            _salesCatalogueConfig = salesCatalogueConfig;
        }

        public async Task<HttpResponseMessage> CallSalesCatalogueServiceApi()
        {
            var uri = $"/{_salesCatalogueConfig.Value.Version}/catalogues/{_salesCatalogueConfig.Value.ProductType}/basic";

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var res = await _httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
            return res;
        }
    }
}
