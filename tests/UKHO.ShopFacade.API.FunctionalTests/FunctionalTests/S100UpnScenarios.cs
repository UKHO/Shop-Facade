    using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UKHO.ShopFacade.API.FunctionalTests.Auth;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class S100UpnScenarios : TestFixtureBase
    {
        //private readonly S100UpnEndpoint _s100UpnEndpoint;

        //private readonly AuthTokenProvider _authTokenProvider;

        //public S100UpnScenarios()
        //{
        //    _s100UpnEndpoint = new S100UpnEndpoint();
        //    _authTokenProvider = new AuthTokenProvider();
        //}

        //[Test]
        //public async Task WhenUpnServiceEndpointCalledWithValidTokenAndLicenceId_ThenUpnServiceReturns200OkResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "1");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content!);
        //    Assert.That(jsonResponse!.Count > 0);
        //}

        //[Test]
        //public async Task WhenUpnServiceEndpointCalledWithValidTokenWithNoRole_ThenUpnServiceReturns403ForbiddenResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(true), "1");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        //}

        //[Test]
        //public async Task WhenUpnServiceEndpointCalledWithoutToken_ThenUpnServiceReturns401UnauthorizedResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync("", "1");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        //}

        //[Test]
        //public async Task WhenUpnServiceEndpointCalledWithInvalidToken_ThenUpnServiceReturns401UnauthorizedResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync("Invalid Token", "1");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        //}

        //[Test]
        //public async Task WhenUpnServiceEndpointCalledWithValidTokenAndNonExistingLicenceId_ThenUpnServiceReturns404NotFoundResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "3");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        //    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content!);
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        //    Assert.That("Licence not found.", Is.EqualTo((string)jsonResponse!.errors[0].description.ToString()));
        //}

        //[Test]
        //public async Task WhenSharePointListIsDown_ThenUpnServiceReturns500InternalServerErrorResponse()
        //{
        //    var response = await _s100UpnEndpoint.GetUpnResponseAsync(await _authTokenProvider.GetAzureADTokenAsync(false), "2");
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        //}

        [Test]
        public async Task CommonMockTest()
        {

            var _options = new RestClientOptions("https://localhost:5678/");
            var _client = new RestClient(_options);

            var request = new RestRequest("demo/health");


            var response = await _client.ExecuteAsync(request);

            // Log request and response details
            Console.WriteLine($"Request URL: {_client.BuildUri(request)}");
            Console.WriteLine($"Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Content: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

    }
}
