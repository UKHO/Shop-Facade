using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class S100UpnScenarios : TestFixtureBase
    {
        private S100UpnEndpoint _s100UpnEndpoint;

        public S100UpnScenarios()
        {
            _s100UpnEndpoint = new S100UpnEndpoint();
        }

    }
}
