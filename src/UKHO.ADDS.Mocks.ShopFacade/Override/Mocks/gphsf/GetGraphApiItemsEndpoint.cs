using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Override.Mocks.gphsf
{
    public class GetGraphApiItemsEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/sites/{siteId}/lists/{listId}/items", (HttpRequest request) =>
                {
                    var state = GetState(request);
                    
                    // Extract the filter parameter to determine which licence ID is being requested
                    var filterParam = request.Query["$filter"].ToString();
                    var licenceId = ExtractLicenceIdFromFilter(filterParam);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            return HandleLicenceIdResponse(licenceId);

                        default:
                            // Handle well-known states (e.g., 500 errors, timeouts)
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>(200)
                .Produces(401)
                .Produces(403)
                .Produces(500)
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Gets SharePoint List Items (Graph API)", 3));
                    d.Append(new MarkdownParagraph("This endpoint simulates Microsoft Graph API SharePoint list item queries."));
                    d.Append(new MarkdownParagraph("It supports different licence IDs (1-11) with various response scenarios:"));
                    d.Append(new MarkdownList(new[]
                    {
                        "Licence 1: 200 OK with 5 UPNs",
                        "Licence 2: 500 Internal Server Error",
                        "Licence 3: 200 OK with empty array (404 in Shop Facade)",
                        "Licence 4: 200 OK with no UPNs (204 in Shop Facade)",
                        "Licence 5: 401 Unauthorized",
                        "Licence 6: 403 Forbidden",
                        "Licence 7-11: 200 OK with special UPN keywords for PGS error scenarios"
                    }));
                });

        private string ExtractLicenceIdFromFilter(string filter)
        {
            // Extract licence ID from filter like "fields/Title eq '1'"
            if (string.IsNullOrEmpty(filter))
                return "1"; // default

            var match = System.Text.RegularExpressions.Regex.Match(filter, @"fields/Title eq '(\d+)'");
            return match.Success ? match.Groups[1].Value : "1";
        }

        private IResult HandleLicenceIdResponse(string licenceId)
        {
            try
            {
                var fs = GetFileSystem();
                var fileName = $"/licence-{licenceId}.json";
                
                // Handle specific error status codes
                return licenceId switch
                {
                    "2" => Results.StatusCode(500), // Internal server error
                    "5" => Results.StatusCode(401), // Unauthorized
                    "6" => Results.StatusCode(403), // Forbidden
                    _ => Results.File(fs.OpenFile(fileName, FileMode.Open, FileAccess.Read), MimeType.Application.Json)
                };
            }
            catch (FileNotFoundException)
            {
                // If specific licence file not found, return default response with empty value array
                return Results.Json(new { value = Array.Empty<object>() });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error retrieving licence data: {ex.Message}", statusCode: 500);
            }
        }
    }
}
