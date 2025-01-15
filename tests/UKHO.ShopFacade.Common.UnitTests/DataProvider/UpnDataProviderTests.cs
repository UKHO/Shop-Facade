using System.Net;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
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
        private UpnDataProvider _upnDataProvider;
        private ILogger<UpnDataProvider> _fakeLogger;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<UpnDataProvider>>();
            _fakeGraphClient = A.Fake<IGraphClient>();
            _upnDataProvider = new UpnDataProvider(_fakeLogger, _fakeGraphClient);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            Action nullLogger = () => new UpnDataProvider(null, _fakeGraphClient);
            nullLogger.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("logger");

            Action nullGraphClient = () => new UpnDataProvider(_fakeLogger, null);
            nullGraphClient.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("graphClient");
        }

        [Test]
        public async Task WhenLicenceExists_ThenReturnUpnDetailsByLicenseId()
        {
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

            A.CallTo(() => _fakeGraphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored)).Returns(fakeListItemCollectionResponse);

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
        public async Task WhenLicenceIsNotFound_ThenReturns404NotFoundResponse()
        {
            var fakeListItemCollectionResponse = new ListItemCollectionResponse
            {
                Value = new List<ListItem>()
            };

            A.CallTo(() => _fakeGraphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored)).Returns(fakeListItemCollectionResponse);

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

        [Test]
        public async Task WhenLicenceExistsWithNullFieldsValues_ThenReturnUpnDetailsByLicenseId()
        {
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
                                        { UpnSchema.Title, null },
                                        { UpnSchema.ECDIS_UPN1_Title, null },
                                        { UpnSchema.ECDIS_UPN_1, null}
                                    }
                                }
                            }
                        }
            };

            A.CallTo(() => _fakeGraphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored)).Returns(fakeListItemCollectionResponse);

            var result = await _upnDataProvider.GetUpnDetailsByLicenseId(123, "correlationId");

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
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
        public async Task WhenLicenceExistsWithEmptyFieldsValues_ThenReturnUpnDetailsByLicenseId()
        {
            var fakeListItemCollectionResponse = new ListItemCollectionResponse
            {
                Value = new List<ListItem>
                        {
                            new ListItem
                            {
                                Fields = new FieldValueSet{}
                            }
                        }
            };

            A.CallTo(() => _fakeGraphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored)).Returns(fakeListItemCollectionResponse);

            var result = await _upnDataProvider.GetUpnDetailsByLicenseId(123, "correlationId");

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
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
