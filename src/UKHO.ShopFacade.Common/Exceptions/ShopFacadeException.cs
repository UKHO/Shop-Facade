using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace UKHO.ShopFacade.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ShopFacadeException : Exception
    {
        public EventId EventId { get; set; }
        public object[] MessageArguments { get; }

        public ShopFacadeException(EventId eventId, string message, params object[] messageArguments) : base(message)
        {
            EventId = eventId;
            MessageArguments = messageArguments ?? [];
        }
    }
}
