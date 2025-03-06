using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AzureAdConfiguration
    {
        public string? MicrosoftOnlineLoginUrl { get; set; }
        public string? TenantId { get; set; }
        public string? ClientId { get; set; }
        public int DeductTokenExpiryMinutes { get; set; }
    }
}
