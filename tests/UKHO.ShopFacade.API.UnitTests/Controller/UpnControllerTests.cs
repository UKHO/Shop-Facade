using System.Net;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.API.Controllers;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response;

namespace UKHO.ShopFacade.API.UnitTests.Controller
{
    [TestFixture]
    public class UpnControllerTests
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<UpnController> _fakeLogger;
        private IUpnService _fakeUpnService;
        private UpnController _upnController;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<UpnController>>();
            _fakeUpnService = A.Fake<IUpnService>();
            _upnController = new UpnController(_fakeHttpContextAccessor, _fakeLogger, _fakeUpnService);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            Action nullLogger = () => new UpnController(_fakeHttpContextAccessor, null, _fakeUpnService);
            nullLogger.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("logger");

            Action nullUpnService = () => new UpnController(_fakeHttpContextAccessor, _fakeLogger, null);
            nullUpnService.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("upnService");
        }

        [Test]
        public async Task WhenLicenceIdIsInvalid_ThenReturn400BadRequestResponse()
        {
            int invalidLicenceId = 0;

            var result = await _upnController.GetUPNs(invalidLicenceId);

            var badRequestResult = result as BadRequestObjectResult;

            badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.InvalidLicenceId.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InvalidLicenceIdMessage).MustHaveHappenedOnceExactly();

        }

        [Test]
        public async Task WhenLicenceIdIsValid_ThenReturn200OkResponseWithUpns()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored))
                .Returns(GetUpnServiceResult(HttpStatusCode.OK));

            var result = await _upnController.GetUPNs(1);
            var okResult = result as OkObjectResult;

            var upnRecord = okResult!.Value as UpnDetail;

            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            upnRecord!.LicenceId.Should().Be(1);
            upnRecord.UserPermits[0].Title.Should().Be("upn1");
            upnRecord.UserPermits[0].Upn.Should().Be("1A1DAD797C");

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallCompleted.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallCompletedMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenLicenceIdIsNotFound_ThenReturn404NotFoundResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored))
                .Returns(GetUpnServiceResult(HttpStatusCode.NotFound));

            var result = await _upnController.GetUPNs(1);

            var notFoundObjectResult = result as NotFoundObjectResult;

            notFoundObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            notFoundObjectResult.Value.Should().BeEquivalentTo(new
            {
                Errors = new List<ErrorDetail>
                {
                    new() { Source = "licenceId" ,Description = "Licence not found"}
                }
            });

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.LicenceNotFound.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.LicenceNotFoundMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenUpnServiceFailed_ThenReturn500InternalServerErrorResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored))
                .Returns(GetUpnServiceResult(HttpStatusCode.InternalServerError));

            var result = await _upnController.GetUPNs(1);

            var statusCodeResult = result as StatusCodeResult;

            statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.InternalError.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InternalErrorMessage).MustHaveHappenedOnceExactly();
        }

        private static UpnServiceResult GetUpnServiceResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.OK => UpnServiceResult.Success(new UpnDetail() { LicenceId = 1, UserPermits = [new() { Title = "upn1", Upn = "1A1DAD797C" }] }),
                HttpStatusCode.NotFound => UpnServiceResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage }] }),
                _ => UpnServiceResult.InternalServerError()
            };
        }

    }
}
