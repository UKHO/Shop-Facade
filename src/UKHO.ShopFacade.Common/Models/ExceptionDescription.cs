using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class ExceptionDescription
    {
        [JsonPropertyName("correlationId")]
        public string? CorrelationId { get; set; }
    }
}
