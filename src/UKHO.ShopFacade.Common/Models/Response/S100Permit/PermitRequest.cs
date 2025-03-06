using System.Text.Json.Serialization;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.Common.Models.Response.S100Permit
{
    public class PermitRequest
    {
        [JsonPropertyName("products")]
        public IEnumerable<ProductModel>? Products { get; set; }

        [JsonPropertyName("userPermits")]
        public IEnumerable<UserPermit>? UserPermits { get; set; }
    }
}
