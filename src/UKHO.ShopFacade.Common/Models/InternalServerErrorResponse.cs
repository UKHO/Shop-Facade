using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models
{
    public class InternalServerErrorResponse
    {
        [JsonPropertyName("correlationId")]
        public string? CorrelationId { get; set; }
    }
}
