using System.Net;
using System.Text;
using FakeItEasy;
using Microsoft.Extensions.Options;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.UnitTests.Services
{
    [TestFixture]
    public class PermitServiceTests
    {
        private IUpnService _fakeUpnService;
        private PermitService _permitService;
        private ISalesCatalogueService _fakeSalesCatalogueService;
        private IS100PermitService _fakeS100PermitService;
        private IOptions<PermitExpiryDaysConfiguration> _fakePermitExpiryDaysConfiguration;

        [SetUp]
        public void Setup()
        {
            _fakeUpnService = A.Fake<IUpnService>();
            _fakeSalesCatalogueService = A.Fake<ISalesCatalogueService>();
            _fakeS100PermitService = A.Fake<IS100PermitService>();
            _fakePermitExpiryDaysConfiguration = A.Fake<IOptions<PermitExpiryDaysConfiguration>>();
            _permitService = new PermitService(_fakeUpnService, _fakeSalesCatalogueService, _fakeS100PermitService, _fakePermitExpiryDaysConfiguration);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PermitService(null!, _fakeSalesCatalogueService, _fakeS100PermitService, _fakePermitExpiryDaysConfiguration), "upnService");
            Assert.Throws<ArgumentNullException>(() => new PermitService(_fakeUpnService, null!, _fakeS100PermitService, _fakePermitExpiryDaysConfiguration), "salesCatalogueService");
            Assert.Throws<ArgumentNullException>(() => new PermitService(_fakeUpnService, _fakeSalesCatalogueService, null!, _fakePermitExpiryDaysConfiguration), "s100PermitService");
            Assert.Throws<ArgumentNullException>(() => new PermitService(_fakeUpnService, _fakeSalesCatalogueService, _fakeS100PermitService, null!), "permitExpiryDaysConfiguration");
        }

        [Test]
        public async Task WhenAllServicesReturnSuccess_ThenGetPermitDetailsReturnsSuccess()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";
            var upns = new List<UserPermit>
            {
                new UserPermit { Title = "ECDIS_UPN1_Title", Upn = "ECDIS_UPN_1" },
                new UserPermit { Title = "ECDIS_UPN2_Title", Upn = "ECDIS_UPN_2" }
            };

            var products = new List<Products>
            {
                new Products
                {
                    ProductName = "101GB40304A",
                    LatestEditionNumber = 3,
                    LatestUpdateNumber = 1,
                    Status = new ProductStatus { StatusName = "newDataset", StatusDate = DateTime.Now }
                },
                new Products
                {
                    ProductName = "101GB40304B",
                    LatestEditionNumber = 2,
                    LatestUpdateNumber = 1,
                    Status = new ProductStatus { StatusName = "newDataset", StatusDate = DateTime.Now }
                }
            };

            var upnServiceResult = UpnServiceResult.Success(upns);
            var salesCatalogueResult = SalesCatalogueResult.Success(products);
            var s100PermitServiceResult = S100PermitServiceResult.Success(new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString())));

            A.CallTo(() => _fakeUpnService.GetUpnDetails(licenceId, correlationId)).Returns(upnServiceResult);
            A.CallTo(() => _fakeSalesCatalogueService.GetProductsCatalogueAsync()).Returns(salesCatalogueResult);
            A.CallTo(() => _fakeS100PermitService.GetS100PermitZipFileAsync(A<PermitRequest>.Ignored)).Returns(s100PermitServiceResult);
            A.CallTo(() => _fakePermitExpiryDaysConfiguration.Value).Returns(new PermitExpiryDaysConfiguration { PermitExpiryDays = 30 });

            var result = await _permitService.GetPermitDetails(licenceId, correlationId);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task WhenS100PermitServiceReturnsError_ThenGetPermitDetailsReturnsInternalServerError()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";
            var upnServiceResult = UpnServiceResult.Success(new List<UserPermit>());
            var salesCatalogueResult = SalesCatalogueResult.Success(new List<Products>());
            var s100PermitServiceResult = S100PermitServiceResult.InternalServerError();

            A.CallTo(() => _fakeUpnService.GetUpnDetails(licenceId, correlationId)).Returns(upnServiceResult);
            A.CallTo(() => _fakeSalesCatalogueService.GetProductsCatalogueAsync()).Returns(salesCatalogueResult);
            A.CallTo(() => _fakeS100PermitService.GetS100PermitZipFileAsync(A<PermitRequest>.Ignored)).Returns(s100PermitServiceResult);
            A.CallTo(() => _fakePermitExpiryDaysConfiguration.Value).Returns(new PermitExpiryDaysConfiguration { PermitExpiryDays = 30 });

            var result = await _permitService.GetPermitDetails(licenceId, correlationId);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task WhenUpnServiceReturnsError_ThenGetPermitDetailsReturnsInternalServerError()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";
            var upnServiceResult = UpnServiceResult.InternalServerError();
            var salesCatalogueResult = SalesCatalogueResult.Success(new List<Products>());
            var s100PermitServiceResult = S100PermitServiceResult.Success(new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString())));

            A.CallTo(() => _fakeUpnService.GetUpnDetails(licenceId, correlationId)).Returns(upnServiceResult);
            A.CallTo(() => _fakeSalesCatalogueService.GetProductsCatalogueAsync()).Returns(salesCatalogueResult);
            A.CallTo(() => _fakeS100PermitService.GetS100PermitZipFileAsync(A<PermitRequest>.Ignored)).Returns(s100PermitServiceResult);
            A.CallTo(() => _fakePermitExpiryDaysConfiguration.Value).Returns(new PermitExpiryDaysConfiguration { PermitExpiryDays = 30 });

            var result = await _permitService.GetPermitDetails(licenceId, correlationId);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task WhenSalesCatalogueServiceReturnsError_ThenGetPermitDetailsReturnsInternalServerError()
        {
            var licenceId = 123;
            var correlationId = "test-correlation-id";
            var upnServiceResult = UpnServiceResult.Success(new List<UserPermit>());
            var salesCatalogueResult = SalesCatalogueResult.InternalServerError(new ErrorResponse());
            var s100PermitServiceResult = S100PermitServiceResult.Success(new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString())));

            A.CallTo(() => _fakeUpnService.GetUpnDetails(licenceId, correlationId)).Returns(upnServiceResult);
            A.CallTo(() => _fakeSalesCatalogueService.GetProductsCatalogueAsync()).Returns(salesCatalogueResult);
            A.CallTo(() => _fakeS100PermitService.GetS100PermitZipFileAsync(A<PermitRequest>.Ignored)).Returns(s100PermitServiceResult);
            A.CallTo(() => _fakePermitExpiryDaysConfiguration.Value).Returns(new PermitExpiryDaysConfiguration { PermitExpiryDays = 30 });

            var result = await _permitService.GetPermitDetails(licenceId, correlationId);

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        private static string GetExpectedXmlString()
        {
            var sb = new StringBuilder();
            sb.Append("<?xmlversion=\"1.0\"encoding=\"UTF-8\"standalone=\"yes\"?><Permitxmlns:S100SE=\"http://www.iho.int/s100/se/5.2\"xmlns:ns2=\"http://standards.iso.org/iso/19115/-3/gco/1.0\"xmlns=\"http://www.iho.int/s100/se/5.2\"><S100SE:header>");
            sb.Append("<S100SE:issueDate>2024-09-02+01:00</S100SE:issueDate><S100SE:dataServerName>fakeDataServerName</S100SE:dataServerName><S100SE:dataServerIdentifier>fakeDataServerIdentifier</S100SE:dataServerIdentifier><S100SE:version>1</S100SE:version>");
            sb.Append("<S100SE:userpermit>fakeUserPermit</S100SE:userpermit></S100SE:header><S100SE:products><S100SE:productid=\"fakeID\"><S100SE:datasetPermit><S100SE:filename>fakefilename</S100SE:filename><S100SE:editionNumber>1</S100SE:editionNumber>");
            sb.Append("<S100SE:issueDate>2024-09-02+01:00</S100SE:issueDate><S100SE:expiry>2024-09-02</S100SE:expiry><S100SE:encryptedKey>fakeencryptedkey</S100SE:encryptedKey></S100SE:datasetPermit></S100SE:product></S100SE:products></Permit>");

            return sb.ToString();
        }
    }
}
