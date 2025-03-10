using System.Diagnostics.CodeAnalysis;

namespace UKHO.ShopFacade.Common.Constants
{
    [ExcludeFromCodeCoverage]
    public static class ErrorDetails
    {
        //Error messages for upn service
        public const string Source = "licenceId";
        public const string GetUPNsCallStartedMessage = "GetUPNs API call started.";
        public const string GetUPNsCallCompletedMessage = "GetUPNs API call completed successfully.";
        public const string InvalidLicenceIdMessage = "Bad request - could be missing or invalid licenceId, it must be an integer and greater than zero.";
        public const string LicenceNotFoundMessage = "Licence not found.";
        public const string NoContentMessage = "No Content - There are no UPNs for the licence.";
        public const string InternalErrorMessage = "Error occurred while processing request.";

        //Error messages for graph client service
        public const string GraphClientCallStartedMessage = "Graph service client call started.";
        public const string GraphClientCallCompletedMessage = "Graph service client call completed.";

        //Error messages for permit service
        public const string GetPermitsCallStartedMessage = "GetPermits API call started.";
        public const string GetPermitsCallCompletedMessage = "GetPermits API call completed successfully.";
    }
}
