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
        public const string UpnServiceCallStartedMessage = "UpnService call started.";
        public const string UpnServiceCallCompletedMessage = "UpnService call completed successfully.";

        //Error messages for graph client service
        public const string GraphClientCallStartedMessage = "Graph service client call started.";
        public const string GraphClientCallCompletedMessage = "Graph service client call completed.";

        //Error messages for permit service
        public const string GetPermitsCallStartedMessage = "Retrieval process of the user permits data from sharepoint is started.";
        public const string PermitNoContentMessage = "There are no S100 permits for the licence.";
        public const string GetPermitsCallCompletedMessage = "Retrieval process of the user permits data from sharepoint is completed.";
        public const string PermitLicenceNotFoundMessage = "Licence not found.";
        public const string PermitInternalErrorMessage = "Error occurred while processing request.";

        //Error messages for sales catalogue service
        public const string ScsSource = "Sales catalogue service";
        public const string ScsInternalErrorMessage = "Error occurred while processing request from {ScsSource}.";
        public const string GetSalesCatalogueDataRequestStartedMessage = "Retrieval process of the latest S-100 basic catalogue data from {ScsSource} is started.";
        public const string GetSalesCatalogueDataRequestCompletedMessage = "Retrieval process of the latest S-100 basic catalogue data from {ScsSource} is completed.";
        public const string SalesCatalogueDataRequestInternalServerErrorMessage = "Error occurred while processing {ScsSource} request | StatusCode: {StatusCode} | Uri: {RequestUri} | Error:{errorResponse}.";

        //Error messages for S100 permit service
        public const string S100PermitServiceSource = "S100 permit service";
        public const string GetS100PermitServiceRequestStartedMessage = "Retrieval process of the permit data from {S100PermitServiceSource} is started.";
        public const string GetS100PermitServiceRequestCompletedMessage = "Retrieval process of the permit data from {S100PermitServiceSource} is completed.";
        public const string S100PermitServiceInternalServerErrorMessage = "Error occurred while processing {S100PermitServiceSource} request | StatusCode: {StatusCode} | Uri: {RequestUri} | Error:{errorResponse}.";
    }
}
