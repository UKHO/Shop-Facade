using UKHO.GraphApi.MockService.Configuration;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UKHO.GraphApi.MockService.Stubs
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

            // Mock endpoint for Graph API
            var endpoint = $"/sites/{_graphApiConfiguration.SiteId}/lists/{_graphApiConfiguration.ListId}/items";

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdFor200OkResponse}'")
                    .WithParam("$expand", "fields")
                    .UsingGet()
            )
            .RespondWith(
        Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"
                    {{
                        {{
                          ""licenceId"": {licenceIdFor200OkResponse},
                          ""userPermits"":
                            [
                                {{
                                  ""title"": ""Master"",
                                  ""upn"": ""C23DAD797C966EC9F6A55B66ED98281599B3A231859868""
                                }},
                                {{
                                  ""title"": ""Backup 1"",
                                  ""upn"": ""BA2DAD797C966EC9F6A55B66ED98281599B3C7B1859868""
                                }},
                                {{
                                  ""title"": ""Backup 2"",
                                  ""upn"": ""A39E2BD79F867CA1B52A44C16FD98281599FA2C31595878E""
                                }},
                                {{
                                  ""title"": ""Backup 3"",
                                  ""upn"": ""D43BFA197C562EB8F73A23D45ED98270599B1C54784968D""
                                }},
                                {{
                                  ""title"": ""Backup 4"",
                                  ""upn"": ""E18ACD947B726FC3A85C66B19AD98231589D3A42765987A""
                                }}
                            ]
                        }}
                    }}")
            );

            server.Given(
                Request.Create()
                    .WithPath(new WildcardMatcher(endpoint))
                    .WithParam("$filter", $"fields/Title eq '{licenceIdFor500InternalServerErrorResponse}'")
                    .WithParam("$expand", "fields")
                    .UsingGet()
                )
            .RespondWith(
        Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("Content-Type", "application/json")
            );
        }
    }
}
