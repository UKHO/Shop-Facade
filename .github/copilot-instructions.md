# Copilot Instructions for Shop-Facade

## Build & Test Commands

```bash
# Build the solution
dotnet build ShopFacade.sln

# Run all unit tests
dotnet test --settings test.runsettings

# Run tests for a specific project
dotnet test tests/UKHO.ShopFacade.API.UnitTests/UKHO.ShopFacade.API.UnitTests.csproj
dotnet test tests/UKHO.ShopFacade.Common.UnitTests/UKHO.ShopFacade.Common.UnitTests.csproj

# Run a single test by name
dotnet test tests/UKHO.ShopFacade.API.UnitTests/UKHO.ShopFacade.API.UnitTests.csproj --filter "FullyQualifiedName~WhenLicenceIdIsInvalid"

# Run Stryker mutation tests (from within the test project directory)
dotnet stryker --test-project tests/UKHO.ShopFacade.API.UnitTests/UKHO.ShopFacade.API.UnitTests.csproj
```

Local configuration overrides go in `src/UKHO.ShopFacade.API/appsettings.local.overrides.json` (loaded in `DEBUG` builds only, gitignored).

---

## Architecture

Shop-Facade is an ASP.NET Core 9.0 API that acts as a façade over multiple downstream services, exposing two endpoints:

- **GET `v1/licences/{licenceId}/s100/userPermits`** — retrieves S-100 User Permit Numbers (UPNs) for a licence by querying a **Microsoft Graph / SharePoint list**.
- **GET `/v1/licences/{licenceId}/{productType}/permits`** — assembles and returns a zip of S-100 permit files by orchestrating three downstream calls in sequence: UPN lookup → Sales Catalogue Service → S100 Permit Service. `{productType}` is currently always `s100` in practice.

### Project layout

| Project | Role |
|---|---|
| `src/UKHO.ShopFacade.API` | Web API host — controllers, services, middleware, filters, DI wiring |
| `src/UKHO.ShopFacade.Common` | Shared library — models, config classes, authentication, data providers, HTTP clients, Polly policies |
| `src/UKHO.ADDS.Mocks.ShopFacade` | Mock service used in ADDS integration/functional test environments |
| `tests/UKHO.ShopFacade.API.UnitTests` | NUnit unit tests for the API project |
| `tests/UKHO.ShopFacade.Common.UnitTests` | NUnit unit tests for the Common project |
| `tests/UKHO.ShopFacade.API.FunctionalTests` | End-to-end functional tests run against deployed environments |

### Project dependency diagram

```mermaid
graph TD
    API["UKHO.ShopFacade.API<br/>ASP.NET Core host<br/>Controllers / Services / Middleware"]
    Common["UKHO.ShopFacade.Common<br/>Class library<br/>Models / Config / Auth / DataProvider<br/>ClientProvider / Policies / Events"]
    APITests["UKHO.ShopFacade.API.UnitTests"]
    CommonTests["UKHO.ShopFacade.Common.UnitTests"]
    FuncTests["UKHO.ShopFacade.API.FunctionalTests<br/>hits deployed API via HTTP"]
    Mock["UKHO.ADDS.Mocks.ShopFacade<br/>Standalone mock web app"]

    API -->|ProjectReference| Common
    APITests -->|ProjectReference| API
    CommonTests -->|ProjectReference| Common
    FuncTests -.->|"HTTP (no project ref)"| API

    classDef srcProject fill:#1565C0,stroke:#0D47A1,color:#FFFFFF,font-weight:bold
    classDef testProject fill:#6A1B9A,stroke:#4A148C,color:#FFFFFF
    classDef mockProject fill:#424242,stroke:#212121,color:#FFFFFF

    class API,Common srcProject
    class APITests,CommonTests,FuncTests testProject
    class Mock mockProject
```

### Request flow — UPN endpoint

```mermaid
sequenceDiagram
    participant C as 🌐 Client
    participant UC as UpnController
    participant US as UpnService
    participant DP as UpnDataProvider
    participant GC as GraphClient
    participant Graph as 📋 Microsoft Graph API (SharePoint List)

    C->>+UC: GET v1/licences/{licenceId}/s100/userPermits (Bearer JWT, X-Correlation-ID)

    rect rgb(21, 101, 192)
        note over UC,US: 🔵 API Layer — UKHO.ShopFacade.API
        UC->>UC: validate licenceId > 0
        UC->>+US: GetUpnDetails(licenceId, correlationId)
    end

    rect rgb(106, 27, 154)
        note over DP,GC: 🟣 Common Layer — UKHO.ShopFacade.Common
        US->>+DP: GetUpnDetailsByLicenseId(licenceId, correlationId)
        DP->>+GC: GetListItemCollectionResponse(expandFields, filter)
        GC->>Graph: OData query — fields/Title eq licenceId
        Graph-->>GC: ListItemCollectionResponse
        GC-->>-DP: ListItemCollectionResponse
        DP-->>-US: UpnDataProviderResult
    end

    rect rgb(21, 101, 192)
        note over UC,US: 🔵 API Layer — response propagation
        US-->>-UC: UpnServiceResult
    end

    UC-->>-C: 200 OK / 204 No Content / 404 Not Found / 500
```

### Request flow — Permit endpoint

```mermaid
sequenceDiagram
    participant C as 🌐 Client
    participant PC as PermitController
    participant PS as PermitService
    participant US as UpnService
    participant SCS as SalesCatalogueService
    participant SCC as SalesCatalogueClient
    participant S100 as S100PermitService
    participant PSC as PermitServiceClient
    participant Ext as ☁️ External Services

    C->>+PC: GET /v1/licences/{licenceId}/{productType}/permits (Bearer JWT, X-Correlation-ID)

    rect rgb(21, 101, 192)
        note over PC,S100: 🔵 API Layer — UKHO.ShopFacade.API (short-circuits on any non-OK result)
        PC->>+PS: GetPermitDetails(licenceId, correlationId)
        PS->>+US: GetUpnDetails(licenceId, correlationId)
        US-->>-PS: UpnServiceResult — short-circuits if non-OK
        PS->>+SCS: GetProductsCatalogueAsync(correlationId)
    end

    rect rgb(106, 27, 154)
        note over SCC,PSC: 🟣 Common Layer — UKHO.ShopFacade.Common (HTTP clients + Polly retry)
        SCS->>+SCC: CallSalesCatalogueServiceApi(correlationId)
        SCC->>Ext: HTTP GET Sales Catalogue Service
        Ext-->>SCC: List of Products
        SCC-->>-SCS: HttpResponseMessage
    end

    rect rgb(21, 101, 192)
        note over PS,S100: 🔵 API Layer — response propagation + permit orchestration
        SCS-->>-PS: SalesCatalogueResult — short-circuits if non-OK
        PS->>PS: PermitRequestMapper.MapToPermitRequest(products, upns, expiryDays)
        PS->>+S100: GetS100PermitZipFileAsync(permitRequest, correlationId)
    end

    rect rgb(106, 27, 154)
        note over SCC,PSC: 🟣 Common Layer — UKHO.ShopFacade.Common (HTTP clients + Polly retry)
        S100->>+PSC: CallPermitServiceApiAsync(permitRequest, correlationId)
        PSC->>Ext: HTTP POST S100 Permit Service
        Ext-->>PSC: zip stream
        PSC-->>-S100: HttpResponseMessage
    end

    rect rgb(21, 101, 192)
        note over PS,PC: 🔵 API Layer — final response
        S100-->>-PS: S100PermitServiceResult
        PS-->>-PC: PermitResult
    end

    PC-->>-C: 200 OK (application/zip) / 204 / 404 / 500
```

### Runtime dependency diagram

```mermaid
graph TD
    API["🚀 Shop Facade API<br/>Azure App Service"]

    subgraph Data ["📦 Data Sources"]
        Graph["📋 Microsoft Graph API<br/>SharePoint List — UPN data"]
    end

    subgraph External ["🔗 External HTTP Services (Polly retry)"]
        SCS["Sales Catalogue Service"]
        S100["S100 Permit Service"]
    end

    subgraph Azure ["☁️ Azure Supporting Services"]
        KV["🔑 Key Vault<br/>secrets"]
        AI["📊 Application Insights<br/>telemetry"]
        EH["📨 Event Hub<br/>structured logging"]
    end

    API --> Graph
    API --> SCS
    API --> S100
    API --> KV
    API --> AI
    API --> EH

    classDef apiNode fill:#1565C0,stroke:#0D47A1,color:#FFFFFF,font-weight:bold,font-size:14px
    classDef dataNode fill:#1B5E20,stroke:#145214,color:#FFFFFF
    classDef extNode fill:#E65100,stroke:#BF360C,color:#FFFFFF
    classDef azureNode fill:#4A148C,stroke:#311B92,color:#FFFFFF

    class API apiNode
    class Graph dataNode
    class SCS,S100 extNode
    class KV,AI,EH azureNode
```

---

## SharePoint UPN Schema

UPN data is stored in a SharePoint list queried via Microsoft Graph. The list is identified by `GraphApiConfiguration:SiteId` and `GraphApiConfiguration:ListId`. The `Title` field holds the licence ID (used as the OData filter key).

Each list item can hold up to **5 UPN slots**. The fields selected are:

| Field name           | Meaning                     |
|----------------------|-----------------------------|
| `ECDIS_UPN1_Title`   | Display title for UPN slot 1 |
| `ECDIS_UPN_1`        | UPN value for slot 1         |
| `ECDIS_UPN2_Title`   | Display title for UPN slot 2 |
| `ECDIS_UPN_2`        | UPN value for slot 2         |
| `ECDIS_UPN3_Title`   | Display title for UPN slot 3 |
| `ECDIS_UPN_3`        | UPN value for slot 3         |
| `ECDIS_UPN4_Title`   | Display title for UPN slot 4 |
| `ECDIS_UPN_4`        | UPN value for slot 4         |
| `ECDIS_UPN5_Title`   | Display title for UPN slot 5 |
| `ECDIS_UPN_5`        | UPN value for slot 5         |

Rules applied by `UpnDataProvider`:
- If `Value.Count == 0` → `NotFound` (licence not in SharePoint).
- If items exist but **no field key contains `"UPN"`** → `NoContent` (licence found, but no UPNs assigned).
- UPN slots 2–5 are only included in the API response when **both** `Title` and `Upn` are non-empty.
- Field values are read from `ListItem.Fields.AdditionalData` (Microsoft Graph SDK `AdditionalData` dictionary).

The OData expand string (defined in `UpnDataProviderConstants.ExpandFields`) explicitly selects only those 10 fields plus the filter uses `fields/Title eq '{licenceId}'`.

---

## Functional Test Setup

Functional tests (`tests/UKHO.ShopFacade.API.FunctionalTests`) run against a **live deployed environment** — they are not in-process and do not mock anything. They require real Azure AD credentials.

### Configuration

Fill in `tests/UKHO.ShopFacade.API.FunctionalTests/appsettings.json` before running locally:

```json
{
  "ShopFacadeConfiguration": {
    "BaseUrl": "<deployed API base URL>",
    "UpnEndpoint": "v1/licences/licenceId/s100/userPermits",
    "PermitEndpoint": "v1/licences/licenceId/s100/permits"
  },
  "AzureADConfiguration": {
    "MicrosoftOnlineLoginUrl": "https://login.microsoftonline.com/",
    "TenantId": "<tenant GUID>",
    "ClientId": "<API app registration client ID>",
    "AutoTestClientId": "<test client with role>",
    "ClientSecret": "<test client secret with role>",
    "AutoTestClientIdNoRole": "<test client without role>",
    "ClientSecretNoRole": "<test client secret without role>",
    "IsRunningOnLocalMachine": true
  }
}
```

- Set `IsRunningOnLocalMachine: true` to trigger **interactive browser auth** (`AcquireTokenInteractive`) instead of client-credentials flow.
- In CI, `IsRunningOnLocalMachine` is `false` and credentials are injected via Azure DevOps variable groups using `FileTransform` on `appsettings.json`.

### Auth flow

```
AuthTokenProvider.GetAzureADTokenAsync(noRole: bool)
  │
  ├─ noRole=false → ConfidentialClientApplication with AutoTestClientId + ClientSecret
  │                  scope: {ClientId}/.default
  │
  └─ noRole=true  → ConfidentialClientApplication with AutoTestClientIdNoRole + ClientSecretNoRole
                     (this client has no app roles → tests 403 Forbidden behaviour)
```

### Test fixture base class

All test classes extend `TestFixtureBase`, which builds a `ServiceProvider` from `appsettings.json` via `TestServiceConfiguration.ConfigureServices()`. Endpoint helper classes (`UpnEndpoint`, `PermitEndpoint`) use **RestSharp** to call the API and are also `TestFixtureBase` subclasses.

### Well-known test licence IDs

The functional tests use fixed licence IDs that map to specific SharePoint/mock data states:

| licenceId | Expected behaviour |
|---|---|
| `1` | Valid licence with UPNs → 200 OK |
| `2` | Triggers 500 Internal Server Error (SharePoint list down / error state) |
| `3` | Licence does not exist → 404 Not Found |
| `4` | Licence exists but has no UPNs → 204 No Content |

---

## Deployment Pipeline

The pipeline is defined in `azure-pipelines.yml` and uses templates under `Deployment/templates/`.

### Pipeline stages

```mermaid
graph TD
    SM["🧬 Stryker_Mutator<br/>mutation testing"]
    BTP["🔨 BuildTestPublish<br/>unit tests · build API<br/>publish ADDS mock<br/>publish functional test artifact"]
    Dev["🚀 Devdeploy<br/>all branches"]
    vIAT["🧪 vNextIatdeploy<br/>develop branch only<br/>ADDS tests trigger"]
    vE2E["✅ vNextE2Edeploy<br/>develop branch only"]
    IAT["🧪 IATdeploy<br/>main / release/* only<br/>ADDS tests trigger"]
    PP["🔒 PreProddeploy<br/>main / release/* only"]
    Live["🌍 Livedeploy<br/>main / release/* only"]

    BTP --> Dev
    Dev --> vIAT
    Dev --> IAT
    vIAT --> vE2E
    IAT --> PP
    PP --> Live
    SM -.->|runs in parallel| BTP

    classDef buildStage  fill:#1565C0,stroke:#0D47A1,color:#FFFFFF,font-weight:bold
    classDef devStage    fill:#2E7D32,stroke:#1B5E20,color:#FFFFFF
    classDef testStage   fill:#6A1B9A,stroke:#4A148C,color:#FFFFFF
    classDef prodStage   fill:#B71C1C,stroke:#7F0000,color:#FFFFFF
    classDef mutStage    fill:#E65100,stroke:#BF360C,color:#FFFFFF

    class BTP buildStage
    class Dev devStage
    class vIAT,vE2E,IAT testStage
    class PP,Live prodStage
    class SM mutStage
```

### Per-environment deploy jobs (inside each deploy stage)

```mermaid
graph TD
    TF["🏗️ DeployTerraform<br/>provisions Azure infra<br/>outputs app name, resource group, slot names"]
    App["📦 DeployApp<br/>deploys zip to staging slot<br/>exports AD config as output vars"]
    HC["❤️ HealthCheck<br/>hits /health on staging slot<br/>swaps slot to production on success"]
    FT["🧪 FunctionalTests<br/>Dev environment only<br/>uses output vars from DeployApp"]
    ADDS["🔁 ADDS auto-test trigger<br/>vNextIAT and IAT only"]

    TF --> App
    App --> HC
    HC --> FT
    App --> ADDS

    classDef infraJob  fill:#1565C0,stroke:#0D47A1,color:#FFFFFF,font-weight:bold
    classDef deployJob fill:#2E7D32,stroke:#1B5E20,color:#FFFFFF
    classDef healthJob fill:#E65100,stroke:#BF360C,color:#FFFFFF
    classDef testJob   fill:#6A1B9A,stroke:#4A148C,color:#FFFFFF

    class TF infraJob
    class App deployJob
    class HC healthJob
    class FT,ADDS testJob
```

### Key pipeline details

- **Terraform state & secrets** are sourced from Azure DevOps variable groups named `Shop-Facade-<Env>`, `Shop-Facade-<Env>-TF`, and (non-PreProd/Live) `Shop-Facade-<Env>-KV`.
- **Assembly version** is stamped by `Apply-AssemblyVersionAndDefaults.ps1` before build using the build number (`1.0.<date>.<counter>`).
- The **ADDS mock service** (`UKHO.ADDS.Mocks.ShopFacade`) is built and deployed separately alongside the main API in each environment.
- NuGet restore uses `BuildNuget.config` (custom feed configuration).
- Target SDK: **.NET 9.0**.

---

## Key Conventions

### Result pattern
All service/data-provider return values extend `Result<T>` via a typed subclass (e.g., `UpnServiceResult`, `PermitResult`, `ServiceResponseResult<T>`). Each subclass has static factory methods:
```csharp
UpnServiceResult.Success(value)
UpnServiceResult.NoContent()
UpnServiceResult.NotFound(errorResponse)
UpnServiceResult.InternalServerError()
```
Controllers switch on `.StatusCode` (an `HttpStatusCode`) and return the appropriate `IActionResult`. Services propagate `ErrorResponse` objects from lower layers.

### EventIds & logging
Every log statement must use a named `EventIds` enum value (range starts at `950001`) converted via `.ToEventId()`:
```csharp
_logger.LogInformation(EventIds.GetUPNsCallStarted.ToEventId(), ErrorDetails.GetUPNsCallStartedMessage);
```
When adding new log calls, add a new entry to `src/UKHO.ShopFacade.Common/Events/EventIds.cs` with the next available integer and an XML doc comment. Unit tests assert specific event IDs, log levels, and message strings.

### Correlation ID
All service methods accept a `string correlationId` parameter passed from `BaseController.GetCorrelationId()`, which reads the `X-Correlation-ID` request header. Always thread it through every layer.

### Authorization
Two role-based policies defined in `ShopFacadeConstants`:
- `ShopFacadeUpnPolicy` (`"UpnReader"`) — for the UPN endpoint
- `ShopFacadePermitPolicy` (`"PermitReader"`) — for the Permit endpoint

Both require Azure AD JWT Bearer auth (`AzureAdScheme`).

### Testing conventions
- **Framework**: NUnit 4 + FakeItEasy
- Fakes are created with `A.Fake<T>()` and verified with `A.CallTo(_fakeLogger).Where(...)`.
- Logger calls are verified by inspecting `LogLevel`, `EventId`, and the `{OriginalFormat}` key in the structured log arguments — see existing controller tests for the exact pattern.
- Test methods follow the naming pattern: `When<Condition>_Then<ExpectedOutcome>`.
- `[TestFixture]` / `[SetUp]` / `[Test]` / `[TestCase(...)]` attributes are used throughout.
- Code coverage must stay at or above **90%** (enforced in CI).

### Configuration
All configuration classes live in `src/UKHO.ShopFacade.Common/Configuration/`. Secrets are sourced from **Azure Key Vault** (URI configured via `KeyVaultSettings:ServiceUri`); runtime config uses `IOptions<T>` binding.

### HTTP clients & retry
External HTTP clients (`SalesCatalogueClient`, `PermitServiceClient`) are registered with `AddHttpClient<>` and decorated with a **Polly retry policy** via `RetryPolicyProvider`. Retry count and duration come from the `RetryPolicyConfiguration` config section.
