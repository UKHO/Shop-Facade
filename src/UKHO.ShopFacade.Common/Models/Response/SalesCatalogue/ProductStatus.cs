using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.Response.SalesCatalogue
{
    public class ProductStatus
    {
        [JsonPropertyName("statusName")]
        public string? StatusName { get; set; }

        [JsonPropertyName("statusDate")]
        public DateTime StatusDate { get; set; }
    }
}
