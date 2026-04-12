using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.MSI
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            MockServices.AddServices();
            ServiceRegistry.AddDefinition(new ServiceDefinition("scsShopFacade", "Sales Catalog Service (ShopFacade)", []));
            ServiceRegistry.AddDefinition(new ServiceDefinition("pgsFacade", "Permit Generator Service (ShopFacade)", []));
            ServiceRegistry.AddDefinition(new ServiceDefinition("graphapiShopFacade", "Graph API (ShopFacade)", []));

            await MockServer.RunAsync(args);
        }
    }
}
