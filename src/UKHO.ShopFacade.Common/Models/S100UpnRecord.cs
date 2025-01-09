using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class S100UpnRecord
    {
        public required string LicenceId { get; set; }
        public required string UPN1_Title { get; set; }
        public required string ECDIS_UPN_1 { get; set; }
        public string? UPN2_Title { get; set; }
        public string? ECDIS_UPN_2 { get; set; }
        public string? UPN3_Title { get; set; }
        public string? ECDIS_UPN_3 { get; set; }
        public string? UPN4_Title { get; set; }
        public string? ECDIS_UPN_4 { get; set; }
        public string? UPN5_Title { get; set; }
        public string? ECDIS_UPN_5 { get; set; }
    }
}
