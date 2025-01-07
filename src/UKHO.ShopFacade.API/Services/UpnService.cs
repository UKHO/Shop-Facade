// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using UKHO.ShopFacade.Common.DataProvider;

namespace UKHO.ShopFacade.API.Services
{
    public class UpnService : IUpnService
    {
        public readonly IUpnDataProvider _upnDataProvider;
        public UpnService(IUpnDataProvider upnDataProvider)
        {
            _upnDataProvider = upnDataProvider;
        }

        public async Task GetUpnDetails(int licenceId)
        {
            var upnList = await _upnDataProvider.GetUpnDetailsByLicenseId(licenceId);
        }
    }
}
