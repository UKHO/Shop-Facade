using System.Net;

namespace UKHO.ShopFacade.Common.Models.SalesCatalogue
{
    public class SalesCatalogueResponse
    {
        public List<Products>? ResponseBody { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
