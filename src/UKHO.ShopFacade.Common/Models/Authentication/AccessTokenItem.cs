using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class AccessTokenItem
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
    }
}
