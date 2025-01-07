// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Configuration;
using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public class UpnDataProvider : IUpnDataProvider
    {
        public readonly IGraphClient _graphClient;
        private readonly SharePointSiteConfiguration _sharePointSiteConfiguration;
        public UpnDataProvider(IGraphClient graphClient, IOptions<SharePointSiteConfiguration> sharePointSiteConfiguration)
        {
            _graphClient = graphClient;
            _sharePointSiteConfiguration = sharePointSiteConfiguration.Value;
        }

        public async Task<S100UpnRecord> GetUpnDetailsByLicenseId(int licenceId)
        {
            var graphClient = _graphClient.GetGraphServiceClient();

            var filterCondition = $"fields/Title eq '{licenceId}'";
            try
            {
                var items = await graphClient.Sites[_sharePointSiteConfiguration.SiteId]
                   .Lists[_sharePointSiteConfiguration.ListId]
                   .Items
                   .GetAsync(requestConfiguration =>
                   {
                       requestConfiguration.QueryParameters.Expand = new string[] { "fields($select=Title,UPN1_Title,UPN_1,UPN2_Title,UPN_2,UPN3_Title,UPN_3,UPN4_Title,UPN_4,UPN5_Title,UPN_5)" };
                       requestConfiguration.QueryParameters.Filter = filterCondition;
                   });

                var result = items.Value.Select(item => new S100UpnRecord
                {
                    LicenceId = item.Fields.AdditionalData.TryGetValue("Title", out var title) ? title?.ToString() : string.Empty,
                    UPN1_Title = item.Fields.AdditionalData.TryGetValue("UPN1_Title", out var upn1Title) ? upn1Title?.ToString() : string.Empty,
                    UPN1 = item.Fields.AdditionalData.TryGetValue("UPN_1", out var upn1) ? upn1?.ToString() : string.Empty,
                    UPN2_Title = item.Fields.AdditionalData.TryGetValue("UPN2_Title", out var upn2Title) ? upn2Title?.ToString() : string.Empty,
                    UPN2 = item.Fields.AdditionalData.TryGetValue("UPN_2", out var upn2) ? upn2?.ToString() : string.Empty,
                    UPN3_Title = item.Fields.AdditionalData.TryGetValue("UPN3_Title", out var upn3Title) ? upn3Title?.ToString() : string.Empty,
                    UPN3 = item.Fields.AdditionalData.TryGetValue("UPN_3", out var upn3) ? upn3?.ToString() : string.Empty,
                    UPN4_Title = item.Fields.AdditionalData.TryGetValue("UPN4_Title", out var upn4Title) ? upn4Title?.ToString() : string.Empty,
                    UPN4 = item.Fields.AdditionalData.TryGetValue("UPN_4", out var upn4) ? upn4?.ToString() : string.Empty,
                    UPN5_Title = item.Fields.AdditionalData.TryGetValue("UPN5_Title", out var upn5Title) ? upn5Title?.ToString() : string.Empty,
                    UPN5 = item.Fields.AdditionalData.TryGetValue("UPN_5", out var upn5) ? upn5?.ToString() : string.Empty
                }).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching UPN details for licenceId: {licenceId}", ex);
            }
        }
    }
}
