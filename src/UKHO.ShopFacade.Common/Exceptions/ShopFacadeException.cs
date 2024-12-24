// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
