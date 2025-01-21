using System.Net;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.DataProvider;
using UKHO.ShopFacade.Common.Events;

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
            var nullLogger = Assert.Throws<ArgumentNullException>(() => new UpnDataProvider(null, _fakeGraphClient));
            Assert.That(nullLogger!.ParamName, Is.EqualTo("logger"));

            var nulGraphClient = Assert.Throws<ArgumentNullException>(() => new UpnDataProvider(_fakeLogger, null));
            Assert.That(nulGraphClient!.ParamName, Is.EqualTo("graphClient"));
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

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value.LicenceId, Is.EqualTo("123"));
            Assert.That(result.Value.ECDIS_UPN1_Title, Is.EqualTo("UPN1"));
            Assert.That(result.Value.ECDIS_UPN_1, Is.EqualTo("ECDIS1"));

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

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.ErrorResponse.Errors.Count, Is.EqualTo(1));
            Assert.That(result.ErrorResponse.Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(result.ErrorResponse.Errors[0].Description, Is.EqualTo(ErrorDetails.LicenceNotFoundMessage));

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

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

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
