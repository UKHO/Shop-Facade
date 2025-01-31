using System.Net;
using FakeItEasy;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.API.UnitTests.Services
{
    [TestFixture]
    public class UpnServiceTests
    {
        private IUpnDataProvider _fakeUpnDataProvider;
        private UpnService _upnService;

        [SetUp]
        public void Setup()
        {
            _fakeUpnDataProvider = A.Fake<IUpnDataProvider>();
            _upnService = new UpnService(_fakeUpnDataProvider);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            var nullUpnDataProvider = Assert.Throws<ArgumentNullException>(() => new UpnService(null!));
            Assert.That(nullUpnDataProvider!.ParamName, Is.EqualTo("upnDataProvider"));
        }

        [Test]
        public async Task WhenUpnDetailsAreValid_ThenReturn200SuccessResponse()
        {
            A.CallTo(() => _fakeUpnDataProvider.GetUpnDetailsByLicenseId(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnDataProviderResult(HttpStatusCode.OK));

            var result = await _upnService.GetUpnDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task WhenLicenceIsNotFound_ThenReturn404NotFoundResponse()
        {
            A.CallTo(() => _fakeUpnDataProvider.GetUpnDetailsByLicenseId(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnDataProviderResult(HttpStatusCode.NotFound));

            var result = await _upnService.GetUpnDetails(123, "correlationId");

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.ErrorResponse.Errors!.Count, Is.EqualTo(1));
            Assert.That(result.ErrorResponse.Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(result.ErrorResponse.Errors[0].Description, Is.EqualTo(ErrorDetails.LicenceNotFoundMessage));
        }

        private static UpnDataProviderResult GetUpnDataProviderResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.NotFound => UpnDataProviderResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = new List<ErrorDetail> { new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage } } }),
                _ => UpnDataProviderResult.Success(new S100UpnRecord { ECDIS_UPN1_Title = "Title1", ECDIS_UPN_1 = "UPN1", ECDIS_UPN2_Title = "Title2", ECDIS_UPN_2 = "UPN2", ECDIS_UPN3_Title = "Title3", ECDIS_UPN_3 = "UPN3", ECDIS_UPN4_Title = "Title4", ECDIS_UPN_4 = "UPN4", ECDIS_UPN5_Title = "Title5", ECDIS_UPN_5 = "UPN5" }),
            };
        }
    }
}
