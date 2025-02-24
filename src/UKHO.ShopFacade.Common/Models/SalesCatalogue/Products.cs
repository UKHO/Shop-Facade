using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class Products
    {
        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("editionNumber")]
        public int? LatestEditionNumber { get; set; }

        [JsonPropertyName("permitExpiryDate")]
        public DateTime? PermitExpiryDate { get; set; }
    }
}
