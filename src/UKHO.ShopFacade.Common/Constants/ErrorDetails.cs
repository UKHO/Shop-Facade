using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Constants
{
    [ExcludeFromCodeCoverage]
    public static class ErrorDetails
    {
        //Error messages for upn service
        public const string Source = "licenceId";
        public const string GetUPNCallStartedMessage = "GetUPNs API call started.";
        public const string GetUPNCallCompletedMessage = "GetUPNs API call completed successfully.";
        public const string InvalidLicenceIdMessage = "Bad request - could be missing or invalid licenceId, it must be an integer and greater than zero.";
        public const string LicenceNotFoundMessage = "Licence not found.";
        public const string InternalErrorMessage = "Error occurred while processing request.";
        public const string GraphClientCallStartedMessage = "Graph service client call started.";
        public const string GraphClientCallCompletedMessage = "Graph service client call completed.";
    }
}
