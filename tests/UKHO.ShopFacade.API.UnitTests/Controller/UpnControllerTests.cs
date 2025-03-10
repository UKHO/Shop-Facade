using System.Net;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.API.Controllers;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Upn;

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
            var nullLogger = Assert.Throws<ArgumentNullException>(() => new UpnController(_fakeHttpContextAccessor, null!, _fakeUpnService));
            Assert.That(nullLogger!.ParamName, Is.EqualTo("logger"));

            var nullUpnService = Assert.Throws<ArgumentNullException>(() => new UpnController(_fakeHttpContextAccessor, _fakeLogger, null!));
            Assert.That(nullUpnService!.ParamName, Is.EqualTo("upnService"));
        }

        [Test]
        public async Task WhenLicenceIdIsInvalid_ThenReturn400BadRequestResponse()
        {
            int invalidLicenceId = 0;

            var result = (BadRequestObjectResult)await _upnController.GetUPNs(invalidLicenceId);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((dynamic)result.Value!).Errors.Count, Is.EqualTo(1));
            Assert.That(((dynamic)result.Value).Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(((dynamic)result.Value).Errors[0].Description, Is.EqualTo(ErrorDetails.InvalidLicenceIdMessage));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                 && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                 && call.GetArgument<EventId>(1) == EventIds.InvalidLicenceId.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InvalidLicenceIdMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenLicenceIdIsValid_ThenReturn200OkResponseWithUpns()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.OK));

            var result = (OkObjectResult)await _upnController.GetUPNs(1);

            var userPermits = result.Value as List<UserPermit>;

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(userPermits![0].Title, Is.EqualTo("upn1"));
            Assert.That(userPermits[0].Upn, Is.EqualTo("1A1DAD797C"));

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
        public async Task WhenUPNsNotAvailableForLicence_ThenReturn204NoContentResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.NoContent));

            var result = (NoContentResult)await _upnController.GetUPNs(7);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                 && call.GetArgument<EventId>(1) == EventIds.NoContentFound.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.NoContentMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenLicenceIdIsNotFound_ThenReturn404NotFoundResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.NotFound));

            var result = (NotFoundObjectResult)await _upnController.GetUPNs(6);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((dynamic)result.Value!).Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(((dynamic)result.Value).Errors[0].Description, Is.EqualTo(ErrorDetails.LicenceNotFoundMessage));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                 && call.GetArgument<EventId>(1) == EventIds.LicenceNotFound.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.LicenceNotFoundMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenUpnServiceFailed_ThenReturn500InternalServerErrorResponse()
        {
            A.CallTo(() => _fakeUpnService.GetUpnDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetUpnServiceResult(HttpStatusCode.InternalServerError));

            var result = (StatusCodeResult)await _upnController.GetUPNs(1);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                  && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                  && call.GetArgument<EventId>(1) == EventIds.GetUPNsCallStarted.ToEventId()
                                                  && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetUPNsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                 && call.GetArgument<LogLevel>(0) == LogLevel.Error
                                                 && call.GetArgument<EventId>(1) == EventIds.InternalError.ToEventId()
                                                 && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InternalErrorMessage).MustHaveHappenedOnceExactly();
        }

        private static UpnServiceResult GetUpnServiceResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.OK => UpnServiceResult.Success(new List<UserPermit>() { new() { Title = "upn1", Upn = "1A1DAD797C" } }),
                HttpStatusCode.NoContent => UpnServiceResult.NoContent(),
                HttpStatusCode.NotFound => UpnServiceResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage }] }),
                _ => UpnServiceResult.InternalServerError()
            };
        }
    }
}
