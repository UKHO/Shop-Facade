using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class SalesCatalogueConfiguration
    {
        public string? BaseUrl { get; set; }
        public string? Version { get; set; }
        public string? ResourceId { get; set; }
        public string? PublisherScope { get; set; }
        public string? ProductType { get; set; }
    }
}
