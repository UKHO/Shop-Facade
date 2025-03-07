using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace UKHO.ShopFacade.Common.Events
{
    /// <summary>
    /// Event Ids
    /// </summary>
    public enum EventIds
    {
        /// <summary>
        /// 950001 - An unhandled exception occurred while processing the request.
        /// </summary>
        UnhandledException = 950001,

        /// <summary>
        /// 950002 - Shop facade exception.
        /// </summary>
        ShopFacadeException = 950002,

        /// <summary>
        /// 950003 - GetUPNs API call started.
        /// </summary>
        GetUPNsCallStarted = 950003,

        /// <summary>
        /// 950004 - GetUPNs API call completed.
        /// </summary>
        GetUPNsCallCompleted = 950004,

        /// <summary>
        /// 950005 - Invalid licence id.
        /// </summary>
        InvalidLicenceId = 950005,

        /// <summary>
        /// 950006 - No data found against license.
        /// </summary>
        LicenceNotFound = 950006,

        /// <summary>
        /// 950007 - Internal error occurred.
        /// </summary>
        InternalError = 950007,

        /// <summary>
        /// 950008 - Graph client call started.
        /// </summary>
        GraphClientCallStarted = 950008,

        /// <summary>
        /// 950009 - Graph client call completed.
        /// </summary>
        GraphClientCallCompleted = 950009,

        /// <summary>
        /// 950010 - Graph Api is healthy.
        /// </summary>
        GraphApiIsHealthy = 950010,

        /// <summary>
        /// 950011 - Graph Api is unhealthy.
        /// </summary>
        GraphApiIsUnhealthy = 950011,

        /// <summary>
        /// 950012 - GetPermits API call started.
        /// </summary>
        GetPermitsCallStarted = 950012,

        /// <summary>
        /// 950013 - Request for sales catalogue service endpoint is failed due to non ok response.
        /// </summary>
        SalesCatalogueServiceNonOkResponse = 950013,

        /// <summary>
        /// 950014 - Request for sales catalogue service catalogue endpoint is started.
        /// </summary>
        GetSalesCatalogueDataRequestStarted = 950014,

        /// <summary>
        /// 950015 - Request for sales catalogue service catalogue endpoint is completed.
        /// </summary>
        GetSalesCatalogueDataRequestCompleted = 950015,

        /// <summary>
        /// 950016 - GetPermits API call completed.
        /// </summary>
        GetPermitsCallCompleted = 950016,

        /// <summary>
        /// 950017 - New access token is added to cache for external end point resource.
        /// </summary>
        CachingExternalEndPointToken = 950017,

        /// <summary>
        /// 950018 - Request for Permit Service endpoint is started.
        /// </summary>
        GetPermitServiceRequestStartedMessage = 950018,

        /// <summary>
        /// 950019 - Request for Permit Service endpoint is completed.
        /// </summary>
        GetPermitServiceRequestCompletedMessage = 950019,

        /// <summary>
        /// 950020 - Request for Permit Service Internal Server Error.
        /// </summary>
        PermitServiceInternalErrorMessage = 950020


    }

    /// <summary>
    /// EventId Extensions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class EventIdExtensions
    {
        /// <summary>
        /// Event Id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
