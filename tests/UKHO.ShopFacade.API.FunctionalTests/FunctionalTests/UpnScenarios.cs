using System.Net;
using Newtonsoft.Json;
using UKHO.ShopFacade.API.FunctionalTests.Auth;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class UpnScenarios : TestFixtureBase
    {
        private readonly UpnEndpoint _upnEndpoint;

        private readonly AuthTokenProvider _authTokenProvider;

        public UpnScenarios()
        {
            _upnEndpoint = new UpnEndpoint();
            _authTokenProvider = new AuthTokenProvider();
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenAndLicenceId_ThenUpnServiceReturns200OkResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content!);
            Assert.That(jsonResponse!.Count > 0);
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenWithNoRole_ThenUpnServiceReturns403ForbiddenResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(true), "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithoutToken_ThenUpnServiceReturns401UnauthorizedResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync("", "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithInvalidToken_ThenUpnServiceReturns401UnauthorizedResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync("Invalid Token", "1");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenAndNonExistingLicenceId_ThenUpnServiceReturns404NotFoundResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "3");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content!);
            Assert.That("Licence not found.", Is.EqualTo((string)jsonResponse!.errors[0].description.ToString()));
        }

        [Test]
        public async Task WhenSharePointListIsDown_ThenUpnServiceReturns500InternalServerErrorResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "2");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task WhenUpnServiceEndpointCalledWithValidTokenAndLicenceWithoutUpn_ThenUpnServiceReturns204NoContentResponse()
        {
            var response = await _upnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "4");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
