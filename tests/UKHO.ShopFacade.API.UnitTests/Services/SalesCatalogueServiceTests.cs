using System.Net;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.API.Tests.Services
{
    [TestFixture]
    public class SalesCatalogueServiceTests
    {
        private ISalesCatalogueClient _fakeSalesCatalogueClient;
        private ILogger<SalesCatalogueService> _fakeLogger;
        private SalesCatalogueService _salesCatalogueService;

        [SetUp]
        public void Setup()
        {
            _fakeSalesCatalogueClient = A.Fake<ISalesCatalogueClient>();
            _fakeLogger = A.Fake<ILogger<SalesCatalogueService>>();
            _salesCatalogueService = new SalesCatalogueService(_fakeSalesCatalogueClient, _fakeLogger);
        }

        [Test]
        public async Task GetProductsFromSpecificDateAsync_ReturnsSalesCatalogueResponse_WhenResponseIsOk()
        {
            // Arrange
            var products = new List<Products>
            {
                new Products { ProductName = "Product1", LatestEditionNumber = 1 }
            };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(products))
            };
            httpResponse.Content.Headers.LastModified = DateTimeOffset.UtcNow;
            A.CallTo(() => _fakeSalesCatalogueClient.CallSalesCatalogueServiceApi()).Returns(Task.FromResult(httpResponse));

            // Act
            var result = await _salesCatalogueService.GetProductsCatalogueAsync();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Count, Is.EqualTo(1));
            Assert.That(result.Value[0].ProductName, Is.EqualTo("Product1"));
        }

        [Test]
        public async Task GetProductsFromSpecificDateAsync_ReturnsSalesCatalogueResponse_WhenResponseIsNotModified()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotModified);
            A.CallTo(() => _fakeSalesCatalogueClient.CallSalesCatalogueServiceApi()).Returns(Task.FromResult(httpResponse));

            // Act
            var result = await _salesCatalogueService.GetProductsCatalogueAsync();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task GetProductsFromSpecificDateAsync_ReturnsSalesCatalogueResponse_WhenResponseIsError()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://example.com"))
            };
            A.CallTo(() => _fakeSalesCatalogueClient.CallSalesCatalogueServiceApi()).Returns(Task.FromResult(httpResponse));

            // Act
            var result = await _salesCatalogueService.GetProductsCatalogueAsync();

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Value, Is.Null);
        }
    }
}
