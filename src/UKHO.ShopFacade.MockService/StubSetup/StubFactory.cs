using Microsoft.Extensions.Options;
using UKHO.ShopFacade.MockService.Configuration;
using UKHO.ShopFacade.MockService.Stubs;

namespace UKHO.ShopFacade.MockService.StubSetup
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
            return new GraphApiStub(_graphApiConfiguration);
        }
    }
}
