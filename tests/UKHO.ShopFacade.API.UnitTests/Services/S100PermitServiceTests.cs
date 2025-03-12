using System.Net;
using System.Text;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using UKHO.ShopFacade.API.Services;
using UKHO.ShopFacade.Common.ClientProvider;
using UKHO.ShopFacade.Common.Constants;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Response.S100Permit;
using UKHO.ShopFacade.Common.Models.Response.Upn;

namespace UKHO.ShopFacade.API.UnitTests;

public class S100PermitServiceTests
{
    private ILogger<S100PermitService> _fakeLogger;
    private IPermitServiceClient _fakePermitServiceClient;
    private S100PermitService _s100PermitService;
    private readonly string _correlationId = Guid.NewGuid().ToString();

    [SetUp]
    public void Setup()
    {
        _fakeLogger = A.Fake<ILogger<S100PermitService>>();
        _fakePermitServiceClient = A.Fake<IPermitServiceClient>();
        _s100PermitService = new S100PermitService(_fakeLogger, _fakePermitServiceClient);
    }

    [Test]
    public void WhenParameterIsNull_ThenConstructorThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new S100PermitService(null!, _fakePermitServiceClient), "logger");
        Assert.Throws<ArgumentNullException>(() => new S100PermitService(_fakeLogger, null!), "permitServiceClient");
    }

    [Test]
    public async Task WhenGetS100PermitZipFileAsyncApiCallIsSuccessful_ReturnsSuccessResult()
    {
        var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(GetExpectedXmlString()));
        var permitRequest = new PermitRequest
        {
            Products = new List<S100Product>
                {
                    new() {
                        ProductName = "Product1",
                        EditionNumber = 1,
                        PermitExpiryDate = DateTime.UtcNow.AddDays(1).ToString("YYYY-MM-DD")
                    },
                    new() {
                        ProductName = "Product2",
                        EditionNumber = 2,
                        PermitExpiryDate = DateTime.UtcNow.AddDays(1).ToString("YYYY-MM-DD")
                    }
                },
            UserPermits = new List<UserPermit>
                {
                    new() {
                        Title = "IHO Test System",
                        Upn = "869D4E0E902FA2E1B934A3685E5D0E85C1FDEC8BD4E5F6"
                    },
                    new() {
                        Title = "OeM Test 1",
                        Upn = "7B5CED73389DECDB110E6E803F957253F0DE13D1G7H8I9"
                    }
                }
        };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(expectedStream)
        };

        A.CallTo(() => _fakePermitServiceClient.CallPermitServiceApiAsync(A<PermitRequest>.Ignored, A<string>.Ignored))
            .Returns(httpResponse);

        var result = await _s100PermitService.GetS100PermitZipFileAsync(permitRequest, _correlationId);
        var memoryStreamResult = result.Value as MemoryStream;
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Length, Is.GreaterThan(0));

        A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitServiceRequestStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitServiceRequestStartedMessage).MustHaveHappenedOnceExactly();
        A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                               && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                               && call.GetArgument<EventId>(1) == EventIds.GetPermitServiceRequestCompleted.ToEventId()
                                               && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitServiceRequestCompletedMessage).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task WhenGetS100PermitZipFileAsyncApiCallFailed_ThenReturnsInternalServerError()
    {
        var permitRequest = new PermitRequest();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        A.CallTo(() => _fakePermitServiceClient.CallPermitServiceApiAsync(A<PermitRequest>.Ignored, A<string>.Ignored))
            .Returns(httpResponse);

        var result = await _s100PermitService.GetS100PermitZipFileAsync(permitRequest, _correlationId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

        A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Information
                                                && call.GetArgument<EventId>(1) == EventIds.GetPermitServiceRequestStarted.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.GetPermitServiceRequestStartedMessage).MustHaveHappenedOnceExactly();
        A.CallTo(_fakeLogger).Where(call => call.Method.Name == "Log"
                                                && call.GetArgument<LogLevel>(0) == LogLevel.Error
                                                && call.GetArgument<EventId>(1) == EventIds.PermitServiceInternalError.ToEventId()
                                                && call.GetArgument<IEnumerable<KeyValuePair<string, object>>>(2)!.ToDictionary(c => c.Key, c => c.Value)["{OriginalFormat}"].ToString() == ErrorDetails.PermitServiceInternalErrorMessage).MustHaveHappenedOnceExactly();

    }

    private static string GetExpectedXmlString()
    {
        var sb = new StringBuilder();
        sb.Append("<?xmlversion=\"1.0\"encoding=\"UTF-8\"standalone=\"yes\"?><Permitxmlns:S100SE=\"http://www.iho.int/s100/se/5.2\"xmlns:ns2=\"http://standards.iso.org/iso/19115/-3/gco/1.0\"xmlns=\"http://www.iho.int/s100/se/5.2\"><S100SE:header>");
        sb.Append("<S100SE:issueDate>2024-09-02+01:00</S100SE:issueDate><S100SE:dataServerName>fakeDataServerName</S100SE:dataServerName><S100SE:dataServerIdentifier>fakeDataServerIdentifier</S100SE:dataServerIdentifier><S100SE:version>1</S100SE:version>");
        sb.Append("<S100SE:userpermit>fakeUserPermit</S100SE:userpermit></S100SE:header><S100SE:products><S100SE:productid=\"fakeID\"><S100SE:datasetPermit><S100SE:filename>fakefilename</S100SE:filename><S100SE:editionNumber>1</S100SE:editionNumber>");
        sb.Append("<S100SE:issueDate>2024-09-02+01:00</S100SE:issueDate><S100SE:expiry>2024-09-02</S100SE:expiry><S100SE:encryptedKey>fakeencryptedkey</S100SE:encryptedKey></S100SE:datasetPermit></S100SE:product></S100SE:products></Permit>");

        return sb.ToString();
    }
}
