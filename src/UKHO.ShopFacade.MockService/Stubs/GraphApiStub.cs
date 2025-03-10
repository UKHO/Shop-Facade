using UKHO.ShopFacade.MockService.Configuration;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UKHO.ShopFacade.MockService.Stubs
{
    public class GraphApiStub : IStub
    {
        private readonly GraphApiConfiguration _graphApiConfiguration;

        public GraphApiStub(GraphApiConfiguration graphApiConfiguration)
        {
            _graphApiConfiguration = graphApiConfiguration ?? throw new ArgumentNullException(nameof(graphApiConfiguration));
        }

        public void ConfigureStub(WireMockServer server)
        {
            var licenceIdFor200OkResponse = "1";
            var licenceIdFor500InternalServerErrorResponse = "2";
            var licenceIdForEmptyResponse = "3";
            var licenceIdFor204Response = "4";

            // Mock endpoint for Graph API
            var endpoint = $"/sites/{_graphApiConfiguration.SiteId}/lists/{_graphApiConfiguration.ListId}/items";

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdFor200OkResponse}'")
                    .WithParam("$expand", "fields($select=ECDIS_UPN1_Title,ECDIS_UPN_1,ECDIS_UPN2_Title,ECDIS_UPN_2,ECDIS_UPN3_Title,ECDIS_UPN_3,ECDIS_UPN4_Title,ECDIS_UPN_4,ECDIS_UPN5_Title,ECDIS_UPN_5)")
                    .UsingGet()
            )
            .RespondWith(
        Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"
                       {{
                        ""value"": [
                            {{
                                ""fields"": {{
                                    ""Title"": ""{licenceIdFor200OkResponse}"",
                                    ""ECDIS_UPN1_Title"": ""Master"",
                                    ""ECDIS_UPN_1"": ""C23DAD797C966EC9F6A55B66ED98281599B3A231859868"",
                                    ""ECDIS_UPN2_Title"": ""Backup 1"",
                                    ""ECDIS_UPN_2"": ""BA2DAD797C966EC9F6A55B66ED98281599B3C7B1859868"",
                                    ""ECDIS_UPN3_Title"": ""Backup 2"",
                                    ""ECDIS_UPN_3"": ""A39E2BD79F867CA1B52A44C16FD98281599FA2C31595878E"",
                                    ""ECDIS_UPN4_Title"": ""Backup 3"",
                                    ""ECDIS_UPN_4"": ""D43BFA197C562EB8F73A23D45ED98270599B1C54784968D"",
                                    ""ECDIS_UPN5_Title"": ""Backup 4"",
                                    ""ECDIS_UPN_5"": ""E18ACD947B726FC3A85C66B19AD98231589D3A42765987A""
                                }}
                            }}
                            ]
                        }}")
            );

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdFor500InternalServerErrorResponse}'")
                    .UsingGet()
                )
            .RespondWith(
        Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("Content-Type", "application/json")
            );

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdForEmptyResponse}'")
                    .UsingGet()
                )
            .RespondWith(
        Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"
                   {{
                    ""value"": []
                    }}")
            );

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdFor204Response}'")    //204 OK response with 0 UPNs
                    .UsingGet())
            .RespondWith(
        Response.Create()
                    .WithStatusCode(204)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"
                    {{
                        ""value"": [
                            ""Fields"":{{
                                ""AdditionalData"":{{
                                    ""@odata.etag"":""975454d2-ead9-482e-8472-7c620847fae8""
                                }}
                            }}
                        ]
                    }}")
            );
        }
    }
}
