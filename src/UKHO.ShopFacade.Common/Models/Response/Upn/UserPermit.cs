using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Models.Response.Upn
{
    [ExcludeFromCodeCoverage]
    public class UserPermit
    {
        public string? Title { get; set; }
        public string? Upn { get; set; }
    }
}
