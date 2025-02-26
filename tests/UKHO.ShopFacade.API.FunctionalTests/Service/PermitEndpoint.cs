using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestSharp;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;

namespace UKHO.ShopFacade.API.FunctionalTests.Service
{
    public class PermitEndpoint : TestFixtureBase
    {
        private readonly RestClient _client;
        private readonly RestClientOptions _options;
        private readonly ShopFacadeConfiguration _shoFacadeConfiguration;

        public PermitEndpoint()
        {
            var serviceProvider = GetServiceProvider();
            _shoFacadeConfiguration = serviceProvider!.GetRequiredService<IOptions<ShopFacadeConfiguration>>().Value;

            _options = new RestClientOptions(_shoFacadeConfiguration.BaseUrl!);
            _client = new RestClient(_options);
        }

        public async Task<RestResponse> GetUpnResponseAsync(string token, string licenceId)
        {
            var endPoint = _shoFacadeConfiguration.PermitEndpoint!.Replace("licenceId", licenceId);
            var request = new RestRequest(endPoint);

            request.AddHeader("Authorization", "Bearer " + token);

            return await _client.ExecuteAsync(request);
        }
    }
}
