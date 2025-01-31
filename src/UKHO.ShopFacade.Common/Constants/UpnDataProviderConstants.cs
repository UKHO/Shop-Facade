using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Constants
{
    [ExcludeFromCodeCoverage]
    public static class UpnDataProviderConstants
    {
        // The expandFields is used to select the fields from the Sharepoint list.
        public const string ExpandFields = "fields($select=ECDIS_UPN1_Title,ECDIS_UPN_1,ECDIS_UPN2_Title,ECDIS_UPN_2,ECDIS_UPN3_Title,ECDIS_UPN_3,ECDIS_UPN4_Title,ECDIS_UPN_4,ECDIS_UPN5_Title,ECDIS_UPN_5)";
    }
}
