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
    public class PermitControllerTests
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<PermitController> _fakeLogger;
        private IPermitService _fakePermitService;
        private PermitController _permitController;
        private const string _fakeProductType = "S100";

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<PermitController>>();
            _fakePermitService = A.Fake<IPermitService>();
            _permitController = new PermitController(_fakeHttpContextAccessor, _fakeLogger, _fakePermitService);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            var nullLogger = Assert.Throws<ArgumentNullException>(() => new PermitController(_fakeHttpContextAccessor, null!, null!));
            Assert.That(nullLogger!.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public async Task WhenLicenceIdIsValid_ThenReturn200OKResponse()
        {
            A.CallTo(() => _fakePermitService.GetPermitDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.OK));

            var result = (StatusCodeResult)await _permitController.GetPermits(_fakeProductType, 1);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task WhenLicenceIdIsInValid_ThenReturn400BadRequestResponse(int invalidLicenceId)
        {
            var result = (BadRequestObjectResult)await _permitController.GetPermits(_fakeProductType, invalidLicenceId);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((dynamic)result.Value!).Errors.Count, Is.EqualTo(1));
            Assert.That(((dynamic)result.Value).Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(((dynamic)result.Value).Errors[0].Description, Is.EqualTo(ErrorDetails.InvalidLicenceIdMessage));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.InvalidLicenceId.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InvalidLicenceIdMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenLicenceIdIsNotFound_ThenReturn404NotFoundResponse()
        {
            A.CallTo(() => _fakePermitService.GetPermitDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.NotFound));

            var result = (NotFoundObjectResult)await _permitController.GetPermits(_fakeProductType, 6);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((dynamic)result.Value!).Errors[0].Source, Is.EqualTo(ErrorDetails.Source));
            Assert.That(((dynamic)result.Value).Errors[0].Description, Is.EqualTo(ErrorDetails.LicenceNotFoundMessage));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.LicenceNotFound.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.LicenceNotFoundMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenPermitControllerFailed_ThenReturn500InternalServerErrorResponse()
        {
            A.CallTo(() => _fakePermitService.GetPermitDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.InternalServerError));

            var result = (StatusCodeResult)await _permitController.GetPermits(_fakeProductType, 1);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Error
                                                && call.GetArgument<EventId>(1) == EventIds.InternalError.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.InternalErrorMessage).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task WhenS100PermitsNotAvailableForLicence_ThenReturn204NoContentResponse()
        {
            A.CallTo(() => _fakePermitService.GetPermitDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.NoContent));

            var result = (NoContentResult)await _permitController.GetPermits(_fakeProductType, 7);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.NoContentFound.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.PermitNoContentMessage).MustHaveHappenedOnceExactly();
        }

        private static PermitServiceResult GetPermitServiceResult(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.OK => PermitServiceResult.Success(),
                HttpStatusCode.NoContent => PermitServiceResult.NoContent(),
                HttpStatusCode.NotFound => PermitServiceResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage }] }),
                _ => PermitServiceResult.InternalServerError()
            };
        }
    }
}
