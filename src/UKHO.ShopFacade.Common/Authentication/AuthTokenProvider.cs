using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
using Azure.Core.Diagnostics;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Models.Authentication;
using Microsoft.Extensions.Logging;

namespace UKHO.ShopFacade.Common.Authentication
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has AD interaction
    public class AuthTokenProvider(IOptions<AzureAdConfiguration> azureAdConfiguration, ICacheProvider cacheProvider, TokenCredential credential, ILogger<AuthTokenProvider> logger) : IAuthTokenProvider
    {
        static AuthTokenProvider()
        {
            AzureEventSourceListener.CreateConsoleLogger();
        }

        public async Task<string> GetManagedIdentityAuthAsync(string resource, string scope, string correlationId = null)
        {
            var accessToken = cacheProvider.GetAuthTokenFromCache(resource);
            if (accessToken != null && accessToken.AccessToken != null && accessToken.ExpiresIn > DateTime.UtcNow)
            {
                logger.LogDebug("Access Token retrieved from cache");
                return accessToken.AccessToken;
            }

            var newAccessToken = await GetNewAuthToken($"{resource}/{scope}");
            cacheProvider.AddAuthTokenToCache(resource, newAccessToken, azureAdConfiguration.Value.DeductTokenExpiryMinutes);

            // TEMPORARY LOGGING - REMOVE AFTER DEBUGGING
            logger.LogInformation($"Access Token for resource {resource}/{scope}: {newAccessToken.AccessToken}");
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
