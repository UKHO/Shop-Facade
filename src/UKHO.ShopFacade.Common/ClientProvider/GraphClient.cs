// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Azure.Identity;
using Microsoft.Graph;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    public class GraphClient : IGraphClient
    {
        public GraphServiceClient GetGraphServiceClient()
        {
            var graphClient = new GraphServiceClient(new DefaultAzureCredential(), new[] { "https://graph.microsoft.com/.default" });
            return graphClient;
        }
    }
}
