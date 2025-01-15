using Microsoft.Extensions.Options;
using UKHO.GraphApi.MockService.Configuration;
using UKHO.GraphApi.MockService.Stubs;

namespace UKHO.GraphApi.MockService.StubSetup
{
    public class StubFactory
    {
        private readonly GraphApiConfiguration _graphApiConfiguration;

        public StubFactory(IOptions<GraphApiConfiguration> graphApiConfiguration)
        {
            _graphApiConfiguration = graphApiConfiguration?.Value ?? throw new ArgumentNullException(nameof(graphApiConfiguration));
        }

        public IStub CreateSapStub()
        {
            return
                new GraphApiStub(_graphApiConfiguration);
        }
    }
}
