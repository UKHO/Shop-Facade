# Copilot Instructions for Shop-Facade

## Project Overview

Shop-Facade is an ASP.NET Core 8 Web API built by UKHO (UK Hydrographic Office). It acts as a facade over multiple downstream services to expose:
- **UPN endpoint** – retrieves S-100 User Permit Numbers (UPNs) for a licence from a SharePoint list via Microsoft Graph API
- **Permit endpoint** – orchestrates UPN retrieval, Sales Catalogue data, and the S-100 Permit Service to return a zip of permit files

## Git Submodule – ADDS Mock

The repository includes a git submodule at `mock/UKHO.ADDS-Mock` (from `https://github.com/UKHO/UKHO.ADDS.Mocks.git`, branch `main`). This is a **WireMock-based HTTP mock server** used to stub downstream services during functional testing.

### Cloning with the submodule

```bash
git clone --recurse-submodules <repo-url>
# or, if already cloned without submodules:
git submodule update --init --recursive
```

### Running the mock server

The mock is a standalone .NET app (`mock/UKHO.ADDS-Mock/src/ADDSMock`). Run it in console mode (headless) or interactive (Avalonia GUI) mode:

```bash
cd mock/UKHO.ADDS-Mock/src/ADDSMock
dotnet run                        # console/headless mode
dotnet run -- --interactive       # GUI mode
```

Default port is **5678** (configured in `mock-configuration.json`). The `OverrideConfigurationPath` in that file must point to the repo's `mock/UKHO.ADDS-Mock/override-configuration` directory.

### How mock responses are structured

Each downstream service has two layers of WireMock mappings, loaded by the mock server at startup:

| Directory | Purpose |
|---|---|
| `service-configuration/<service>/api-responses.cs` | Default stub registrations for a service (compiled and executed at runtime by the mock) |
| `service-configuration/<service>/files/` | Static response body files referenced by `WithBodyFromFile(...)` |
| `override-configuration/<service>/` | Overrides that take precedence over service-configuration stubs |

Services stubbed for Shop-Facade:

- **`graphapi`** – stubs Microsoft Graph API (`fields/Title eq '<licenceId>'` filter). Each `licenceId` maps to a specific scenario (see table below).
- **`scs`** – stubs the Sales Catalogue Service (`GET /v2/catalogues/s100/basic`), returning catalogue JSON from `service-configuration/scs/files/`.
- **`pgs`** – stubs the S-100 Permit Generator Service (`POST /v1/permits/s100`). Responses are driven by **magic keyword strings** embedded in the UPN value of the request body.

### Licence ID → scenario mapping (Graph API mock)

| `licenceId` | Graph API mock response | Shop-Facade result |
|---|---|---|
| `1` | 200 with 5 UPNs | 200 OK |
| `2` | 500 Internal Server Error | 500 |
| `3` | 200 with empty `value[]` | 404 Not Found |
| `4` | 200 with metadata but no UPN fields | 204 No Content |
| `5` | 401 Unauthorized | 500 |
| `6` | 403 Forbidden | 500 |
| `7` | 200 with UPN containing `400BadRequestResponse` | 400 from PGS |
| `8` | 200 with UPN containing `500InternalServerErrorResponse` | 500 from PGS |
| `9` | 200 with UPN containing `401UnauthorizedResponse` | 401 from PGS |
| `10` | 200 with UPN containing `403ForbiddenResponse` | 403 from PGS |
| `11` | 200 with UPN containing `404NotFoundResponse` | 404 from PGS |

### Adding new mock scenarios

1. To add a new licence ID scenario, add a new `server.Given(...)` block to `mock/UKHO.ADDS-Mock/service-configuration/graphapi/api-responses.cs` using the next unused licence ID.
2. To trigger a specific PGS response, embed the corresponding keyword string (`400BadRequestResponse`, `500InternalServerErrorResponse`, etc.) into a UPN value in the Graph API stub.
3. Override stubs (in `override-configuration/`) always register **before** the default stubs so they take priority in WireMock's matching order.

## Build & Test Commands

```bash
# Build the solution
dotnet build ShopFacade.sln

# Run all unit tests
dotnet test ShopFacade.sln --filter "FullyQualifiedName~UnitTests"

# Run a single test class
dotnet test tests/UKHO.ShopFacade.API.UnitTests/UKHO.ShopFacade.API.UnitTests.csproj --filter "FullyQualifiedName~UpnControllerTests"

# Run a single test method
dotnet test tests/UKHO.ShopFacade.API.UnitTests/UKHO.ShopFacade.API.UnitTests.csproj --filter "Name=WhenLicenceIdIsValid_ThenReturn200OkResponseWithUpns"

# Run mutation tests (Stryker) – run from the test project directory
cd tests/UKHO.ShopFacade.API.UnitTests
dotnet stryker

# Run code coverage report
./CodeCoverageReport.ps1
```

Local overrides go in `src/UKHO.ShopFacade.API/appsettings.local.overrides.json` (not committed). Azure Key Vault is loaded when `KeyVaultSettings:ServiceUri` is set.

## Architecture

```
src/
  UKHO.ShopFacade.API/        # ASP.NET Core host – controllers, middleware, DI wiring
  UKHO.ShopFacade.Common/     # Shared library – models, services, clients, config, events

tests/
  UKHO.ShopFacade.API.UnitTests/        # NUnit unit tests (FakeItEasy mocks)
  UKHO.ShopFacade.Common.UnitTests/     # NUnit unit tests for Common
  UKHO.ShopFacade.API.FunctionalTests/  # Integration/functional tests against live env
  UKHO.ShopFacade.API.PerformanceTests/ # Performance tests
```

### Request flow

1. Request arrives → `CorrelationIdMiddleware` (generates/propagates `X-Correlation-ID`) → `ExceptionHandlingMiddleware`
2. Azure AD JWT bearer auth (`AzureAdScheme`); two authorization policies: `UpnReader` and `PermitReader` (role-based)
3. Controller validates input, delegates to a service
4. **UPN flow**: `UpnService` → `UpnDataProvider` → `GraphClient` (MS Graph API) → SharePoint list
5. **Permit flow**: `PermitService` orchestrates `UpnService` + `SalesCatalogueService` + `S100PermitService` in sequence, short-circuiting on non-OK results
6. Downstream HTTP clients (`SalesCatalogueClient`, `PermitServiceClient`) use Polly retry policies

### Key external dependencies

| Dependency | Purpose |
|---|---|
| Microsoft Graph API | Reads UPN data from a SharePoint list |
| Sales Catalogue Service | Provides product catalogue for permit generation |
| S-100 Permit Service | Generates the permit zip file |
| Azure Key Vault | Runtime secrets |
| Azure Event Hub | Structured logging in production |

## Key Conventions

### Result pattern
Every service and data provider returns a typed `Result<T>` subclass (e.g., `UpnServiceResult`, `PermitResult`). Results carry a `StatusCode`, `Value`, and `ErrorResponse`. Controllers switch on `StatusCode` to produce the appropriate `IActionResult`. Use the static factory methods (`Success`, `NoContent`, `NotFound`, `InternalServerError`) instead of constructing results directly.

### EventIds
All log calls must use a strongly-typed `EventIds` enum value (range `950001–950025`). Convert with `.ToEventId()`. Add new IDs to `UKHO.ShopFacade.Common.Events.EventIds` with the next sequential number and an XML doc comment.

### Error responses
Error details (messages, source) live in `UKHO.ShopFacade.Common.Constants.ErrorDetails` as constants. Controllers never hardcode error strings; always reference constants from `ErrorDetails`.

### Authorization policies
Two named policies are defined: `ShopFacadeConstants.ShopFacadeUpnPolicy` (`"UpnReader"`) and `ShopFacadeConstants.ShopFacadePermitPolicy` (`"PermitReader"`). Each controller endpoint must be decorated with `[Authorize(Policy = ...)]`.

### Correlation ID
Every controller action reads `GetCorrelationId()` (from `BaseController`) and threads it through service/data-provider calls. All log messages and error responses include the correlation ID.

### Test naming
Test method names follow the pattern `When<Condition>_Then<ExpectedOutcome>` (e.g., `WhenLicenceIdIsInvalid_ThenReturn400BadRequestResponse`).

### Mocking
Unit tests use **FakeItEasy** (`A.Fake<T>()`, `A.CallTo()`). Logger calls are verified using the `A.CallTo(_fakeLogger).Where(...)` pattern with explicit `EventId` and message checks.

### `[ExcludeFromCodeCoverage]`
Applied to `Program`, `BaseController`, pure configuration/constant classes, and middleware extensions. The CI pipeline enforces ≥90% coverage.

### Configuration binding
Configuration sections are bound via `builder.Services.Configure<TOptions>(configuration.GetSection("SectionName"))`. Section names match the class name (e.g., `SalesCatalogueConfiguration` ↔ `"SalesCatalogue"`) — except `AzureAdConfiguration` which is bound from `"AzureAdConfiguration"`.

### Stryker mutation testing
Thresholds are configured in `tests/UKHO.ShopFacade.API.UnitTests/stryker-config.json` (break at 55, low at 60, high at 75). The CI pipeline fails if mutations fall below the break threshold.
