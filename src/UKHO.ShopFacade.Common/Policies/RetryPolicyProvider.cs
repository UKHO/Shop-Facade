using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.Common.Policies
{
    public class RetryPolicyProvider
    {
        private readonly ILogger<RetryPolicyProvider> _logger;

        public RetryPolicyProvider(ILogger<RetryPolicyProvider> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(string service, EventIds eventId, int retryCount, double sleepDuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(sleepDuration),
                    onRetry: (response, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(eventId.ToEventId(), "Failed to connect {service} | StatusCode: {statusCode} | Retry attempted: {retryAttempt}.", service, response.Result.StatusCode.ToString(), retryAttempt);
                    });
        }
    }
}
