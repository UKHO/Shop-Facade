using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Override.Mocks.scssf
{
    public class GetSalesCatalogueEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/v2/catalogues/s100/basic", (HttpRequest request) =>
                {
                    var state = GetState(request);
                    
                    // Check for If-Modified-Since header to handle conditional requests
                    var ifModifiedSince = request.Headers["If-Modified-Since"].ToString();

                    switch (state)
                    {
                        case WellKnownState.Default:
                            return HandleIfModifiedSinceScenarios(ifModifiedSince);

                        default:
                            // Handle well-known states (e.g., 500 errors, timeouts)
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>(200)
                .Produces(304)
                .Produces(400)
                .Produces(500)
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Gets S-100 Basic Catalogue (Sales Catalogue Service)", 3));
                    d.Append(new MarkdownParagraph("This endpoint simulates the Sales Catalogue Service basic product catalogue endpoint."));
                    d.Append(new MarkdownParagraph("Supports conditional requests using the If-Modified-Since header:"));
                    d.Append(new MarkdownList(new[]
                    {
                        "No header or valid date: 200 OK with catalogue JSON",
                        "If-Modified-Since: 2020-10-27T00:00:00Z - 304 Not Modified",
                        "If-Modified-Since: 20221027 (invalid format) - 400 Bad Request",
                        "If-Modified-Since: 3000-01-01T00:00:00Z - 500 Internal Server Error"
                    }));
                });

        private IResult HandleIfModifiedSinceScenarios(string ifModifiedSince)
        {
            // Handle specific If-Modified-Since scenarios
            switch (ifModifiedSince)
            {
                case "2020-10-27T00:00:00Z":
                    return Results.StatusCode(304); // Not Modified

                case "20221027":
                    return Results.BadRequest("Bad request.");

                case "3000-01-01T00:00:00Z":
                    return Results.StatusCode(500); // Internal Server Error

                default:
                    // Return the basic catalogue from file
                    return GetBasicCatalogueFromFile();
            }
        }

        private IResult GetBasicCatalogueFromFile()
        {
            try
            {
                var fs = GetFileSystem();
                return Results.File(fs.OpenFile("/basic-catalogue.json", FileMode.Open, FileAccess.Read), MimeType.Application.Json);
            }
            catch (FileNotFoundException)
            {
                return Results.NotFound("Could not find basic-catalogue.json");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error retrieving basic catalogue: {ex.Message}", statusCode: 500);
            }
        }
    }
}
