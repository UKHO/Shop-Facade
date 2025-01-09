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
        /// 950003 - GetUPN API call started.
        /// </summary>
        GetUPNCallStarted = 950003,

        /// <summary>
        /// 950004 - GetUPN API call completed.
        /// </summary>
        GetUPNCallCompleted = 950004,

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
        GraphClientCallCompleted = 950009

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
