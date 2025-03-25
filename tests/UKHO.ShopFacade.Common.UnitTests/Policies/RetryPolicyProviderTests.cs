using System.Net;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Policies;

namespace UKHO.ShopFacade.Common.UnitTests.Policies
{
    public class RetryPolicyProviderTests
    {
        private ILogger<RetryPolicyProvider> _fakeLogger;
        private RetryPolicyProvider _fakeRetryPolicyProvider;
        private const string _serviceName = "PermitService";

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RetryPolicyProvider>>();
            _fakeRetryPolicyProvider = new RetryPolicyProvider(_fakeLogger);
        }


        [Test]
        public async Task WhenInternalServerErrorOccured_ThenRetryPolicyGetsApplied()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var policy = _fakeRetryPolicyProvider.GetRetryPolicy(_serviceName, EventIds.RetryAttemptForPermitService, 3, 5);
            await policy.ExecuteAsync(() => Task.FromResult(httpResponseMessage));

            Assert.That(httpResponseMessage, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.RetryAttemptForPermitService.ToEventId()).MustHaveHappened();
        }


        [Test]
        public async Task WhenOkResponseOccured_ThenRetrypolicyNotApplied()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            var policy = _fakeRetryPolicyProvider.GetRetryPolicy(_serviceName, EventIds.RetryAttemptForPermitService, 3, 5);
            await policy.ExecuteAsync(() => Task.FromResult(httpResponseMessage));

            Assert.That(httpResponseMessage, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.RetryAttemptForPermitService.ToEventId()).MustNotHaveHappened();
        }

        [Test]
        public async Task WhenBadRequestResponseOccured_ThenRetrypolicyNotApplied()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var policy = _fakeRetryPolicyProvider.GetRetryPolicy(_serviceName, EventIds.RetryAttemptForPermitService, 3, 5);
            await policy.ExecuteAsync(() => Task.FromResult(httpResponseMessage));

            Assert.That(httpResponseMessage, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Warning
                                                && call.GetArgument<EventId>(1) == EventIds.RetryAttemptForPermitService.ToEventId()).MustNotHaveHappened();
        }
    }
}
