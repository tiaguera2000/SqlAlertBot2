using SharedKernel.DomainEvents.Core;
using SharedKernel.DomainEvents.CrossDomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Domain.EventHandlers
{
    public static class DomainEventsHandler
    {
        private static CrossDomainEventHandler _crossDomainEventHandler;
        public static CrossDomainEventHandler CrossDomainEventHandler => _crossDomainEventHandler ?? (_crossDomainEventHandler = new CrossDomainEventHandler());

        private static CrossDomainEventDelayedHandler _crossDomainEventDelayedHandler;
        public static CrossDomainEventDelayedHandler CrossDomainEventDelayedHandler => _crossDomainEventDelayedHandler ?? (_crossDomainEventDelayedHandler = new CrossDomainEventDelayedHandler());

        public static void Handle(ICrossDomainEvent args)
        {
            CrossDomainEventHandler.Handle(args);
        }
        public static void Handle(ICrossDomainEvent args, TimeSpan span)
        {
            CrossDomainEventDelayedHandler.Handle(args, span);
        }

        public static void Handle(ICrossDomainEvent args, INamesResolver namesResolver)
        {
            CrossDomainEventHandler.Handle(args, namesResolver);
        }
    }
}
