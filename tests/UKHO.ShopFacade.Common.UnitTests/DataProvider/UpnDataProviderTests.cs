using System.Net;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Sites.Item.Lists.Item.Items;
using Microsoft.Kiota.Abstractions;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;

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
            _fakeSharePointSiteConfiguration = A.Fake<IOptions<SharePointSiteConfiguration>>();
            _upnDataProvider = new UpnDataProvider(_fakeLogger, _fakeGraphClient, _fakeSharePointSiteConfiguration);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            Action nullLogger = () => new UpnDataProvider(null, _fakeGraphClient, _fakeSharePointSiteConfiguration);
            nullLogger.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("logger");

            Action nullGraphClient = () => new UpnDataProvider(_fakeLogger, null, _fakeSharePointSiteConfiguration);
            nullGraphClient.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("graphClient");

            Action nullSharePointSiteConfiguration = () => new UpnDataProvider(_fakeLogger, _fakeGraphClient, null);
            nullSharePointSiteConfiguration.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("sharePointSiteConfiguration");
        }

        [Test]
        public async Task WhenLicenceExists_ThenReturnUpnDetailsByLicenseId()
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
                                        { UpnSchema.ECDIS_UPN1_Title, "UPN1" },
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

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            result.Value.LicenceId.Should().Be("123");

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GraphClientCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GraphClientCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.GraphClientCallCompleted.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GraphClientCallCompletedMessage).MustHaveHappenedOnceExactly();
        }


        [Test]
        public async Task WhenLicenceIsNotFound_ThenGetUpnDetailsByLicenseIdReturns404NotFoundResponse()
        {
            var fakeGraphServiceClient = A.Fake<GraphServiceClient>();
            var fakeListItemCollectionResponse = new ListItemCollectionResponse
            {
                Value = new List<ListItem>()
            };

            A.CallTo(() => _fakeGraphClient.GetGraphServiceClient()).Returns(fakeGraphServiceClient);
            A.CallTo(() => fakeGraphServiceClient.Sites[A<string>.Ignored].Lists[A<string>.Ignored].Items.GetAsync(A<Action<RequestConfiguration<ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters>>>.Ignored, A<CancellationToken>.Ignored))
                   .Returns(Task.FromResult<ListItemCollectionResponse?>(fakeListItemCollectionResponse));

            var result = await _upnDataProvider.GetUpnDetailsByLicenseId(123, "correlationId");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);

            result.ErrorResponse.Errors.Should().BeEquivalentTo(new List<ErrorDetail>
            {
                new() { Source = ErrorDetails.Source ,Description = ErrorDetails.LicenceNotFoundMessage}
            });

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                               && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                               && call.GetArgument<EventId>(1) == EventIds.GraphClientCallStarted.ToEventId()
                                               && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GraphClientCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.GraphClientCallCompleted.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GraphClientCallCompletedMessage).MustHaveHappenedOnceExactly();
        }
    }
}
