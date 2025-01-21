using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class S100UpnRecord
    {
        public string LicenceId { get; set; }
        public string UPN1_Title { get; set; }
        public string UPN1 { get; set; }
        public string UPN2_Title { get; set; }
        public string UPN2 { get; set; }
        public string UPN3_Title { get; set; }
        public string UPN3 { get; set; }
        public string UPN4_Title { get; set; }
        public string UPN4 { get; set; }
        public string UPN5_Title { get; set; }
        public string UPN5 { get; set; }
    }
}
