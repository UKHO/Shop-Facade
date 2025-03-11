using System.Net;
using FakeItEasy;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.UnitTests.Services
{
    [TestFixture]
    public class PermitServiceTests
    {
        private IUpnService _fakeUpnService;
        private ISalesCatalogueService _salesCatalogueService;
        private PermitService _permitService;

        [SetUp]
        public void Setup()
        {
            _fakeUpnService = A.Fake<IUpnService>();
            _salesCatalogueService = A.Fake<ISalesCatalogueService>();
            _permitService = new PermitService(_fakeUpnService, _salesCatalogueService);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            var nullUpnDataProvider = Assert.Throws<ArgumentNullException>(() => new PermitService(null!, _salesCatalogueService));
            Assert.That(nullUpnDataProvider!.ParamName, Is.EqualTo("upnService"));
        }

        [Test]
        public async Task WhenLicenceIdIsValid_ThenReturn200OkResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.OK));

            var result = await _permitService.GetPermitDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenLicenceIsNotFound_ThenReturn404NotFoundResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.NotFound));

            var result = await _permitService.GetPermitDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.ErrorResponse.Errors!.Count, Is.EqualTo(1));
            Assert.That(result.ErrorResponse.Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(result.ErrorResponse.Errors[0].Description, Is.EqualTo(ErrorDetails.LicenceNotFoundMessage));
        }

        [Test]
        public async Task WhenPermitServiceFailed_ThenReturn500InternalServerErrorResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.InternalServerError));

            var result = await _permitService.GetPermitDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task WhenUPNsNotAvailableForLicence_ThenReturn204NoContentResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.NoContent));

            var result = await _permitService.GetPermitDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        private static UpnServiceResult GetUpnServiceResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.OK => UpnServiceResult.Success(new List<UserPermit>()),
                HttpStatusCode.NoContent => UpnServiceResult.NoContent(),
                HttpStatusCode.NotFound => UpnServiceResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage }] }),
            _ => UpnServiceResult.InternalServerError()
            };
        }
    }
}
