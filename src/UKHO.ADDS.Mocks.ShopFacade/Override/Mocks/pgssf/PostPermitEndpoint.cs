using System.Text;
using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Override.Mocks.pgssf
{
    public class PostPermitEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapPost("/v1/permits/s100", async (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            // Read the request body to check for special keywords
                            string requestBody = await ReadRequestBodyAsync(request);
                            return HandlePermitRequestScenarios(requestBody);

                        default:
                            // Handle well-known states (e.g., 500 errors, timeouts)
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<byte[]>(200, "application/zip")
                .Produces(400)
                .Produces(401)
                .Produces(403)
                .Produces(404)
                .Produces(500)
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Generate S-100 Permit (Permit Generation Service)", 3));
                    d.Append(new MarkdownParagraph("This endpoint simulates the S-100 Permit Service permit generation endpoint."));
                    d.Append(new MarkdownParagraph("Returns different responses based on UPN keywords in the request body:"));
                    d.Append(new MarkdownList(new[]
                    {
                        "Default/No keyword: 200 OK with permit zip file",
                        "Body contains '400BadRequestResponse': 400 Bad Request",
                        "Body contains '500InternalServerErrorResponse': 500 Internal Server Error",
                        "Body contains '401UnauthorizedResponse': 401 Unauthorized",
                        "Body contains '403ForbiddenResponse': 403 Forbidden",
                        "Body contains '404NotFoundResponse': 404 Not Found"
                    }));
                    d.Append(new MarkdownParagraph("The keywords are typically embedded in UPN values to trigger specific test scenarios."));
                });

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return body;
            }
            catch
            {
                return string.Empty;
            }
        }

        private IResult HandlePermitRequestScenarios(string requestBody)
        {
            // Check for specific keywords in the request body
            if (requestBody.Contains("400BadRequestResponse", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Invalid Product or UPN.");
            }

            if (requestBody.Contains("500InternalServerErrorResponse", StringComparison.OrdinalIgnoreCase))
            {
                return Results.StatusCode(500);
            }

            if (requestBody.Contains("401UnauthorizedResponse", StringComparison.OrdinalIgnoreCase))
            {
                return Results.Unauthorized();
            }

            if (requestBody.Contains("403ForbiddenResponse", StringComparison.OrdinalIgnoreCase))
            {
                return Results.StatusCode(403);
            }

            if (requestBody.Contains("404NotFoundResponse", StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound("Not found.");
            }

            // Default: return the permit zip file
            return GetPermitZipFromFile();
        }

        private IResult GetPermitZipFromFile()
        {
            try
            {
                var fs = GetFileSystem();
                return Results.File(fs.OpenFile("/Permits.zip", FileMode.Open, FileAccess.Read), "application/zip", "s100-permit.zip");
            }
            catch (FileNotFoundException)
            {
                // If zip file not found, return a minimal mock zip
                return CreateMockZipResponse();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error generating permit: {ex.Message}", statusCode: 500);
            }
        }

        private IResult CreateMockZipResponse()
        {
            // Create a minimal valid zip file in memory
            using var memoryStream = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                // Add a sample permit XML file
                var entry = archive.CreateEntry("permit.xml");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<S100_Permit>
    <permitId>TEST-PERMIT-001</permitId>
    <licenceId>1</licenceId>
    <expiryDate>2025-12-31T23:59:59Z</expiryDate>
    <upn>C23DAD797C966EC9F6A55B66ED98281599B3A231859868A</upn>
    <products>
        <product>S-101</product>
        <product>S-102</product>
        <product>S-104</product>
    </products>
</S100_Permit>");
            }
            
            memoryStream.Position = 0;
            var bytes = memoryStream.ToArray();
            return Results.File(bytes, "application/zip", "s100-permit.zip");
        }
    }
}
