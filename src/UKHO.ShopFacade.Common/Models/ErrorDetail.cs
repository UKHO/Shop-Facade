using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class ErrorDetail
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
