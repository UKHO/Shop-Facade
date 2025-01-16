using WireMock.Server;

namespace UKHO.GraphApi.MockService.Stubs
{
    public interface IStub
    {
        void ConfigureStub(WireMockServer server);
    }
}
