using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using UKHO.ShopFacade.Common.Events;

namespace UKHO.ShopFacade.API.Controllers
{
    [ApiController]
    public class UpnController : BaseController<UpnController>
    {
        private readonly ILogger<UpnController> _logger;
        public UpnController(IHttpContextAccessor httpContextAccessor, ILogger<UpnController> logger) : base(httpContextAccessor)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all User Permit Numbers (UPNs) associated with the requested licence.
        /// </summary>
        [HttpGet]
        [Route("/licenses/{licenceId}/s100/userpermits")]
        public async Task<IActionResult> GetUPNs(int licenceId)
        {
            _logger.LogInformation(EventIds.GetUPNsStarted.ToEventId(), "GetUPNs API Call Started.");

            //Values from app registration
            var clientId = "";
            var tenantId = "";
            var clientSecret = "";

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var graphClient = new GraphServiceClient(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });

            var siteId = "f63fa270-6bf5-4db1-bc4d-47a19d05a36b";
            //var siteId = "f45aa5ad-ac44-4638-9ae1-a056d2396463"; // Access denied
            //var siteId = "8c57a24e-b24c-430f-95f8-94122a471cb5"; // Requested site could not be found
            var listId = "898176af-f10f-43c8-8af7-31aee3ae6d2f";
            var filterCondition = $"fields/Title eq '{licenceId}'";


            var items = await graphClient.Sites[siteId]
               .Lists[listId]
               .Items
               .GetAsync(requestConfiguration =>
               {
                   requestConfiguration.QueryParameters.Expand = new string[] { "fields($select=Title,UPNTitle_1,UPN_1)" };
                   requestConfiguration.QueryParameters.Filter = filterCondition;
               });


            var result = items.Value.Select(item => new SharePointListItem
            {
                LicenceId = item.Fields.AdditionalData["Title"]?.ToString(),
                UPN1_Title = item.Fields.AdditionalData["UPNTitle_1"]?.ToString(),
                UPN1 = item.Fields.AdditionalData["UPN_1"]?.ToString()
            }).ToList();

            return Ok(result);
        }
    }

    public class SharePointListItem
    {
        public string LicenceId { get; set; }
        public string UPN1_Title { get; set; }
        public string UPN1 { get; set; }
    }
}
