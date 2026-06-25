using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        public ManagedIdentityGraphAuthProvider(IOptions<GraphApiConfiguration> graphApiConfiguration)
        {
            _graphApiConfiguration = graphApiConfiguration ?? throw new ArgumentNullException(
                                                     nameof(graphApiConfiguration));
        }
        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var credential = new DefaultAzureCredential();
            var scopes = new[] { _graphApiConfiguration.Value.GraphApiScope! };
            //var accessToken = await credential.GetTokenAsync(new TokenRequestContext(scopes), cancellationToken);
            var accessToken="abc";
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            //request.Headers.Add("Authorization", $"Bearer {accessToken.Token}");
        }
    }
}
