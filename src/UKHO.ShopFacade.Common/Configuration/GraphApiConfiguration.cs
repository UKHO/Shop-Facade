using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class GraphApiConfiguration
    {
        public string? SiteId { get; set; }

        public string? ListId { get; set; }

        public string? GraphApiScope { get; set; }

        public string? GraphApiBaseUrl { get; set; }
    }
}
