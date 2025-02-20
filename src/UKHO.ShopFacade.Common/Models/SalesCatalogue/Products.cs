using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.SalesCatalogue
{
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
