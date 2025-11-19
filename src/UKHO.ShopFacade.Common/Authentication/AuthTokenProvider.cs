using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Models.Authentication;

namespace UKHO.ShopFacade.Common.Authentication
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has AD interaction
    public class AuthTokenProvider(IOptions<AzureAdConfiguration> azureAdConfiguration, ICacheProvider cacheProvider, TokenCredential credential) : IAuthTokenProvider
    {
        public async Task<string> GetManagedIdentityAuthAsync(string resource, string scope)
        {
            var accessToken = cacheProvider.GetAuthTokenFromCache(resource);
            if (accessToken != null && accessToken.AccessToken != null && accessToken.ExpiresIn > DateTime.UtcNow)
            {
                return accessToken.AccessToken;
            }

            var newAccessToken = await GetNewAuthToken($"{resource}/{scope}");
            cacheProvider.AddAuthTokenToCache(resource, newAccessToken, azureAdConfiguration.Value.DeductTokenExpiryMinutes);

            return newAccessToken.AccessToken;
        }

        private async Task<AccessTokenItem> GetNewAuthToken(string scope)
        {
            var accessToken = await credential.GetTokenAsync(new TokenRequestContext([scope]) { }, new CancellationToken());

            return new AccessTokenItem
            {
                ExpiresIn = accessToken.ExpiresOn.UtcDateTime,
                AccessToken = accessToken.Token
            };
        }
    }
}
