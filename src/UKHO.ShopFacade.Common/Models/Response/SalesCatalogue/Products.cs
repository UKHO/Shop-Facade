using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.Response.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class Products
    {
        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("editionNumber")]
        public int? LatestEditionNumber { get; set; }

        [JsonPropertyName("updateNumber")]
        public int? LatestUpdateNumber { get; set; }

        [JsonPropertyName("status")]
        public ProductStatus? Status { get; set; }
    }
}
