using System.Net;
using FakeItEasy;
using FluentAssertions;
using UKHO.ShopFacade.API.Services;
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
            Action nullupnDataProvider = () => new UpnService(null);
            nullupnDataProvider.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("upnDataProvider");
        }

        [Test]
        public async Task WhenUpnDetailsAreValid_ThenReturns200SuccessResponse()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";

            A.CallTo(() => _fakeUpnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId))
                .Returns(GetUpnDataProviderResult(HttpStatusCode.OK));

            var result = await _upnService.GetUpnDetails(licenceId, correlationId);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            result.Value.LicenceId.Should().Be(licenceId);
            result.Value.UserPermits.Count.Should().Be(5);
        }

        [Test]
        public async Task WhenLicenceIsNotFound_ThenReturns404NotFoundResponse()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";

            A.CallTo(() => _fakeUpnDataProvider.GetUpnDetailsByLicenseId(licenceId, correlationId))
                .Returns(GetUpnDataProviderResult(HttpStatusCode.NotFound));

            var result = await _upnService.GetUpnDetails(licenceId, correlationId);

            result.ErrorResponse.Errors.Should().BeEquivalentTo(new List<ErrorDetail>
            {
                new() { Source = "licenceId" ,Description = "Licence not found"}
            });

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private UpnDataProviderResult GetUpnDataProviderResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.NotFound => UpnDataProviderResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = new List<ErrorDetail> { new ErrorDetail() { Source = "licenceId", Description = "Licence not found" } } }),
                _ => UpnDataProviderResult.Success(new S100UpnRecord { LicenceId = "123", ECDIS_UPN1_Title = "Title1", ECDIS_UPN_1 = "UPN1", ECDIS_UPN2_Title = "Title2", ECDIS_UPN_2 = "UPN2", ECDIS_UPN3_Title = "Title3", ECDIS_UPN_3 = "UPN3", ECDIS_UPN4_Title = "Title4", ECDIS_UPN_4 = "UPN4", ECDIS_UPN5_Title = "Title5", ECDIS_UPN_5 = "UPN5" }),
            };
        }
    }
}
