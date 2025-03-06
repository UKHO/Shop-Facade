
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Authentication;
using UKHO.ShopFacade.Common.Configuration;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    public class S100PermitServiceClient : IS100PermitServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<PermitServiceConfiguration> _permitServiceConfig;
        private readonly IAuthTokenProvider _tokenProvider;
        public S100PermitServiceClient(HttpClient httpClient, IOptions<PermitServiceConfiguration> permitServiceConfig, IAuthTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _permitServiceConfig = permitServiceConfig;
            _tokenProvider = tokenProvider;
        }
        public async Task<HttpResponseMessage> CallPermitServiceApiAsync(object requestBody)
        {
            var uri = $"/{_permitServiceConfig.Value.Version}/permits/s100";
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            var authToken = await _tokenProvider.GetManagedIdentityAuthAsync(_permitServiceConfig.Value.ResourceId!, _permitServiceConfig.Value.PublisherScope!);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            return await _httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
        }

    }
}
