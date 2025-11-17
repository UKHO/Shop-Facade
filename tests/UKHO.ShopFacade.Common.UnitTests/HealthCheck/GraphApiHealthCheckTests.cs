using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.HealthCheck;

namespace UKHO.ShopFacade.Common.Tests.HealthCheck
{
    [TestFixture]
    public class GraphApiHealthCheckTests
    {
        private ILogger<GraphApiHealthCheck> _logger;
        private IGraphClient _graphClient;
        private GraphApiHealthCheck _healthCheck;

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<GraphApiHealthCheck>>();
            _graphClient = A.Fake<IGraphClient>();
            _healthCheck = new GraphApiHealthCheck(_logger, _graphClient);
        }

        [Test]
        public async Task CheckHealthAsync_ReturnsHealthy_WhenGraphApiIsHealthy()
        {
            // Arrange
            A.CallTo(() => _graphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(new ListItemCollectionResponse()));

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
            Assert.That(result.Description, Is.EqualTo("Graph api is healthy"));

            A.CallTo(_logger).Where(call => call.Method.Name == "Log"
                                               && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                               && call.GetArgument<EventId>(1) == EventIds.GraphApiIsHealthy.ToEventId()
                                               && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == "Graph Api is healthy").MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task CheckHealthAsync_ReturnsUnhealthy_WhenGraphApiThrowsException()
        {
            // Arrange
            A.CallTo(() => _graphClient.GetListItemCollectionResponse(A<string>.Ignored, A<string>.Ignored))
                .Throws<Exception>();

            // Act
            var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

            // Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(result.Description, Is.EqualTo("Graph api is unhealthy"));

            A.CallTo(_logger).Where(call => call.Method.Name == "Log"
                                             && call.GetArgument<LogLevel>(0) == LogLevel.Error
                                             && call.GetArgument<EventId>(1) == EventIds.GraphApiIsUnhealthy.ToEventId()
            && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == "Health check for the Graph Api threw an exception").MustHaveHappenedOnceExactly();
        }
    }
}
