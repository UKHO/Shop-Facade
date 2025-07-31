using System.Net;
using System.Text;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.API.Controllers;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models;
using UKHO.ShopFacade.Common.Models.Response.Permit;

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
        }

        [Test]
        public async Task WhenPermitControllerFailed_ThenReturn500InternalServerErrorResponse()
        {
            A.CallTo(() => _fakePermitService.GetPermitDetails(A<int>.Ignored, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.InternalServerError));

            var result = (ObjectResult)await _permitController.GetPermits(_fakeProductType, 1);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Error
                                                && call.GetArgument<EventId>(1) == EventIds.InternalError.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.PermitInternalServerErrorMessage).MustHaveHappenedOnceExactly();
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
        }

        [Test]
        public async Task WhenPermitServiceReturnsOk_ThenGetPermitsReturnsFileResult()
        {
            int validLicenceId = 12345678;
            var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString()));

            A.CallTo(() => _fakePermitService.GetPermitDetails(validLicenceId, A<string>.Ignored)).Returns(GetPermitServiceResult(HttpStatusCode.OK));

            var result = await _permitController.GetPermits(_fakeProductType, validLicenceId);

            var fileResult = result as FileStreamResult;
            Assert.That(fileResult, Is.Not.Null);
            Assert.That(fileResult!.FileDownloadName, Is.EqualTo(PermitServiceConstants.PermitZipFileName));
            Assert.That(fileResult.ContentType, Is.EqualTo(PermitServiceConstants.ZipContentType));
            Assert.That(fileResult.FileStream.Length, Is.EqualTo(expectedStream.Length));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                              && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                              && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                              && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallStartedMessage).MustHaveHappenedOnceExactly();

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallCompleted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitsCallCompletedMessage).MustHaveHappenedOnceExactly();


        }

        private static PermitResult GetPermitServiceResult(HttpStatusCode httpStatusCode)
        {
            var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString()));

            return httpStatusCode switch
            {
                HttpStatusCode.OK => PermitResult.Success(expectedStream),
                HttpStatusCode.NoContent => PermitResult.NoContent(),
                HttpStatusCode.NotFound => PermitResult.NotFound(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.LicenceNotFoundMessage }] }),
                _ => PermitResult.InternalServerError(new ErrorResponse() { CorrelationId = Guid.NewGuid().ToString(), Errors = [new ErrorDetail() { Source = ErrorDetails.Source, Description = ErrorDetails.PermitInternalServerErrorMessage }] })
            };
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
