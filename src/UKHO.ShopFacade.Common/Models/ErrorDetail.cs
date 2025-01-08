using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models
{
    public class ErrorDetail
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
