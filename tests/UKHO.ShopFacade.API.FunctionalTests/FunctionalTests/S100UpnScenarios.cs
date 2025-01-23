using FluentAssertions;
using UKHO.ShopFacade.API.FunctionalTests.Auth;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class S100UpnScenarios : TestFixtureBase
    {
        private readonly S100UpnEndpoint _s100UpnEndpoint;

        private readonly AuthTokenProvider _authTokenProvider;

        public S100UpnScenarios()
        {
            _s100UpnEndpoint = new S100UpnEndpoint();
            _authTokenProvider = new AuthTokenProvider();
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenAndLicenceId_ThenUpnServiceReturns200OkResponse()
        {
            var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "1");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenWithNoRole_ThenUpnServiceReturns403ForbiddenResponse()
        {
            var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(true), "1");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithoutToken_ThenUpnServiceReturns401UnauthorizedResponse()
        {
            var response = await _s100UpnEndpoint.GetUpnResponseAsync("", "1");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithInvalidToken_ThenUpnServiceReturns401UnauthorizedResponse()
        {
            var response = await _s100UpnEndpoint.GetUpnResponseAsync("Invalid Token", "1");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

    }
}
