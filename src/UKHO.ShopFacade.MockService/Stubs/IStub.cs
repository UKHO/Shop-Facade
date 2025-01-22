using WireMock.Server;

namespace UKHO.ShopFacade.MockService.Stubs
{
    public interface IStub
    {
        void ConfigureStub(WireMockServer server);
    }
}
