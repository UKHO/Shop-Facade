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

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RetryPolicyProvider>>();
            _fakeRetryPolicyProvider = new RetryPolicyProvider(_fakeLogger);
        }

        [Test]
        public async Task WhenValidInputsProvided_ThenSCSRetrypolicySuccessfullyExecuted()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var policy = _fakeRetryPolicyProvider.GetRetryPolicy("SalesCatalogueService", EventIds.RetryAttemptForSalesCatalogueService, 3, 5);
            await policy.ExecuteAsync(() => Task.FromResult(httpResponseMessage));

            Assert.That(httpResponseMessage, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task WhenValidInputsProvided_ThenPermitServiceRetrypolicySuccessfullyExecuted()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var policy = _fakeRetryPolicyProvider.GetRetryPolicy("PermitService", EventIds.RetryAttemptForPermitService, 3, 5);
            await policy.ExecuteAsync(() => Task.FromResult(httpResponseMessage));

            Assert.That(httpResponseMessage, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }
    }
}
