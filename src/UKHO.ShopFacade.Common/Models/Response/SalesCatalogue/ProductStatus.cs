using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models.Response.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class ProductStatus
    {
        [JsonPropertyName("statusName")]
        public string? StatusName { get; set; }

        [JsonPropertyName("statusDate")]
        public DateTime StatusDate { get; set; }
    }
}
