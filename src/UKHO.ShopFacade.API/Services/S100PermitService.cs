using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.SalesCatalogue;

namespace UKHO.ShopFacade.API.Services
{
    public class S100PermitService : IS100PermitService
    {
        private readonly IS100PermitServiceClient _s100PermitServiceClient;
        public S100PermitService(IS100PermitServiceClient _s100PermitServiceClient)
        {
            _s100PermitServiceClient = _s100PermitServiceClient;
        }

        public async Task<S100PermitServiceResult> GetS100PermitZipFileAsync(PermitRequest permitRequest)
        {
            var response = await _s100PermitServiceClient.CallPermitServiceApiAsync(permitRequest);
            var result = await CreatePermitServiceResponse(response);
            return result;
        }

        private async Task<S100PermitServiceResult> CreatePermitServiceResponse(HttpResponseMessage httpResponse)
        {
            S100PermitServiceResult response;
            var body = await httpResponse.Content.ReadAsStreamAsync();

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response = S100PermitServiceResult.Success(body);
            }
            else
            {
                response = S100PermitServiceResult.InternalServerError();
            }
            return response;
        }
    }
}
