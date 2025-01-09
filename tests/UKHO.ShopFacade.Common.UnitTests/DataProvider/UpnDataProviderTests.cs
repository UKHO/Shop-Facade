using System.Net;
using FakeItEasy;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using Microsoft.Kiota.Abstractions;
using Microsoft.Graph.Sites.Item.Lists.Item.Items;
using Microsoft.Extensions.Logging;

namespace UKHO.ShopFacade.Common.UnitTests.DataProvider
{
    [TestFixture]
    public class UpnDataProviderTests
    {
        private IGraphClient _fakeGraphClient;
        private IOptions<SharePointSiteConfiguration> _fakeSharePointSiteConfiguration;
        private UpnDataProvider _upnDataProvider;
        private ILogger<UpnDataProvider> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<UpnDataProvider>>();
            _fakeGraphClient = A.Fake<IGraphClient>();
            _fakeSharePointSiteConfiguration = Options.Create(new SharePointSiteConfiguration
            {
                SiteId = "fakeSiteId",
                ListId = "fakeListId"
            });

            _upnDataProvider = new UpnDataProvider(_fakeLogger, _fakeGraphClient, _fakeSharePointSiteConfiguration);
        }

        [Test]
        public async Task WhenLicenceIdExists_ThenGetUpnDetailsByLicenseId_ReturnsSuccessResponse()
        {
            var fakeGraphServiceClient = A.Fake<GraphServiceClient>();
            var fakeListItemCollectionResponse = new ListItemCollectionResponse
            {
                Value = new List<ListItem>
                        {
                            new ListItem
                            {
                                Fields = new FieldValueSet
                                {
                                    AdditionalData = new Dictionary<string, object>
                                    {
                                        { UpnSchema.Title, "123" },
                                        { UpnSchema.UPN1_Title, "UPN1" },
                                        { UpnSchema.ECDIS_UPN_1, "ECDIS1" }
                                    }
                                }
                            }
                        }
            };

            A.CallTo(() => _fakeGraphClient.GetGraphServiceClient()).Returns(fakeGraphServiceClient);
            A.CallTo(() => fakeGraphServiceClient.Sites[A<string>.Ignored].Lists[A<string>.Ignored].Items.GetAsync(A<Action<RequestConfiguration<ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters>>>.Ignored, A<CancellationToken>.Ignored))
                   .Returns(Task.FromResult<ListItemCollectionResponse?>(fakeListItemCollectionResponse));

            var result = await _upnDataProvider.GetUpnDetailsByLicenseId(123, "correlationId");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.LicenceId, Is.EqualTo("123"));
        }
    }
}

