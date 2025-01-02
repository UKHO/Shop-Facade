using UKHO.ShopFacade.API.FunctionalTests.Auth;
using UKHO.ShopFacade.API.FunctionalTests.Configuration;
using UKHO.ShopFacade.API.FunctionalTests.Service;

namespace UKHO.ShopFacade.API.FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class S100UpnScenarios : TestFixtureBase
    {
        private AuthTokenProvider _authTokenProvider;
        private S100UpnEndpoint _s100UpnEndpoint;

        public S100UpnScenarios()
        {
            _authTokenProvider = new AuthTokenProvider();
            _s100UpnEndpoint = new S100UpnEndpoint();
        }

    }
}
