using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Models.Authentication;

namespace UKHO.ShopFacade.Common.Authentication
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has AD interaction
    public class AuthTokenProvider : IAuthTokenProvider
    {
        private readonly IOptions<AzureAdConfiguration> _azureAdConfiguration;
        private readonly ICacheProvider _cacheProvider;

        public AuthTokenProvider(IOptions<AzureAdConfiguration> azureAdConfiguration, ICacheProvider cacheProvider)
        {
            _azureAdConfiguration = azureAdConfiguration;
            _cacheProvider = cacheProvider;
        }

        public async Task<string> GetManagedIdentityAuthAsync(string resource, string scope)
        {
            var accessToken = _cacheProvider.GetAuthTokenFromCache(resource);
            if (accessToken != null && accessToken.AccessToken != null && accessToken.ExpiresIn > DateTime.UtcNow)
            {
                return accessToken.AccessToken;
            }

            var newAccessToken = await GetNewAuthToken($"{resource}/{scope}");
            _cacheProvider.AddAuthTokenToCache(resource, newAccessToken, _azureAdConfiguration.Value.DeductTokenExpiryMinutes);

            return newAccessToken.AccessToken;
        }

        private async Task<AccessTokenItem> GetNewAuthToken(string scope)
        {
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = await tokenCredential.GetTokenAsync(new TokenRequestContext([scope]) { });

            return new AccessTokenItem
            {
                ExpiresIn = accessToken.ExpiresOn.UtcDateTime,
                AccessToken = accessToken.Token
            };
        }
    }
}
