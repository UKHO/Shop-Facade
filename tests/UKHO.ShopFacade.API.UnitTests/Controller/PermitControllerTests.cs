using System.Net;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.API.Controllers;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.API.UnitTests.Controller
{
    [TestFixture]
    public class PermitControllerTests
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<PermitController> _fakeLogger;
        private PermitController _permitController;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<PermitController>>();
            _permitController = new PermitController(_fakeHttpContextAccessor, _fakeLogger);
        }

        [Test]
        public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
        {
            var nullLogger = Assert.Throws<ArgumentNullException>(() => new PermitController(_fakeHttpContextAccessor, null!));
            Assert.That(nullLogger!.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void WhenLicenceIdIsValid_ThenReturn200OKResponse()
        {
            var result = (OkResult) _permitController.GetPermits(1);

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitsCallStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == "GetPermits API Call Started.").MustHaveHappenedOnceExactly();
        }
    }
}
