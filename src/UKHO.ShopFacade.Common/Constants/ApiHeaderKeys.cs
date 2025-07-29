using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Constants
{
    [ExcludeFromCodeCoverage]
    public static class ApiHeaderKeys
    {
        public const string XCorrelationIdHeaderKey = "X-Correlation-ID";
        public const string ContentType = "application/json";
    }
}
