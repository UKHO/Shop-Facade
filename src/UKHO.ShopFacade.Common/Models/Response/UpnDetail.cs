// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Models.Response
{
    [ExcludeFromCodeCoverage]
    public class UserPermit
    {
        public string Title { get; set; }
        public string Upn { get; set; }
    }
}
