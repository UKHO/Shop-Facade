using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Authentication;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    [ExcludeFromCodeCoverage] //Excluded from code coverage as it has actual http calls 
    public class SalesCatalogueClient : ISalesCatalogueClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<SalesCatalogueConfiguration> _salesCatalogueConfig;
        private readonly IAuthTokenProvider _tokenProvider;

        public SalesCatalogueClient(HttpClient httpClient, IOptions<SalesCatalogueConfiguration> salesCatalogueConfig, IAuthTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _salesCatalogueConfig = salesCatalogueConfig;
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage> CallSalesCatalogueServiceApi(string correlationId)
        {
            var uri = $"/{_salesCatalogueConfig.Value.Version}/catalogues/{_salesCatalogueConfig.Value.ProductType}/basic";
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            var authToken = _tokenProvider.GetManagedIdentityAuthAsync(_salesCatalogueConfig.Value.ResourceId!, _salesCatalogueConfig.Value.PublisherScope!).Result;
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            httpRequestMessage.Headers.Add(ApiHeaderKeys.XCorrelationIdHeaderKey, correlationId);

            return await _httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
        }
    }
}
