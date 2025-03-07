using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.Response.S100Permit
{
    [ExcludeFromCodeCoverage]
    public class S100Product
    {
        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("editionNumber")]
        public int? EditionNumber { get; set; }

        [JsonPropertyName("permitExpiryDate")]
        public string? PermitExpiryDate { get; set; }
    }
}
