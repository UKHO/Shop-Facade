// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using UKHO.ShopFacade.Common.Models;

namespace UKHO.ShopFacade.Common.DataProvider
{
    public interface IUpnDataProvider
    {
        Task<S100UpnRecord> GetUpnDetailsByLicenseId(int licenceId);
    }
}
