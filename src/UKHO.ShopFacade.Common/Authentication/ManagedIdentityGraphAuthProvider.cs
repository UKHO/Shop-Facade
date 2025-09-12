using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using UKHO.ShopFacade.Common.Configuration;

namespace UKHO.ShopFacade.Common.Authentication
{
    [ExcludeFromCodeCoverage] ////Excluded from code coverage as it has AD interaction
    public class ManagedIdentityGraphAuthProvider : IAuthenticationProvider
    {
        private readonly IOptions<GraphApiConfiguration> _graphApiConfiguration;
        TokenCredential _credential;

        public ManagedIdentityGraphAuthProvider(IOptions<GraphApiConfiguration> graphApiConfiguration, TokenCredential credential)
        {
            _graphApiConfiguration = graphApiConfiguration ?? throw new ArgumentNullException(
                                                     nameof(graphApiConfiguration));
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }
        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var scopes = new[] { _graphApiConfiguration.Value.GraphApiScope! };
            var accessToken = await _credential.GetTokenAsync(new TokenRequestContext(scopes), cancellationToken);
            request.Headers.Add("Authorization", $"Bearer {accessToken.Token}");
        }
    }
}
