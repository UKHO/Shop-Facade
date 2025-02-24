using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UKHO.ShopFacade.Common.Models.SalesCatalogue
{
    [ExcludeFromCodeCoverage]
    public class SalesCatalogueResponse
    {
        public List<Products>? ResponseBody { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
