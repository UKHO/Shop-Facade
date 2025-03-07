using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.Models.Response.S100Permit
{
    [ExcludeFromCodeCoverage]
    public class PermitRequest
    {
        [JsonPropertyName("products")]
        public IEnumerable<S100Product>? Products { get; set; }

        [JsonPropertyName("userPermits")]
        public IEnumerable<UserPermit>? UserPermits { get; set; }
    }
}
