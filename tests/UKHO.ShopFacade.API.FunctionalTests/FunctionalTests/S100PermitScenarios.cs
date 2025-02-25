using System.Net;
using UKHO.ShopFacade.API.FunctionalTests.Auth;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class S100PermitScenarios : TestFixtureBase
    {
        private readonly S100PermitEndpoint _s100PermitEndpoint;

        private readonly AuthTokenProvider _authTokenProvider;

        public S100PermitScenarios()
        {
            _s100PermitEndpoint = new S100PermitEndpoint();
            _authTokenProvider = new AuthTokenProvider();
        }

        [Test]
        public async Task WhenPermitServiceEndpointCalledWithValidTokenAndLicenceId_ThenPermitServiceReturns200OkResponse()
        {
            var response = await _s100PermitEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenPermitServiceEndpointCalledWithValidTokenWithNoRole_ThenPermitServiceReturns403ForbiddenResponse()
        {
            var response = await _s100PermitEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(true), "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task WhenPermitServiceEndpointCalledWithoutToken_ThenPermitServiceReturns401UnauthorizedResponse()
        {
            var response = await _s100PermitEndpoint.GetUpnResponseAsync("", "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task WhenPermitServiceEndpointCalledWithInvalidToken_ThenPermitServiceReturns401UnauthorizedResponse()
        {
            var response = await _s100PermitEndpoint.GetUpnResponseAsync("Invalid Token", "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

    }
}
