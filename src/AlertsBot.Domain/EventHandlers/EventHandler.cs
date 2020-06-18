using AlertsBot.Domain.Core.Events;
using SharedKernel.DomainEvents.CrossDomainEvents;

namespace AlertsBot.Domain.EventHandlers
{
    public class EventHandler
    {
        private readonly SharedKernel.DomainEvents.Core.IHandler<ICrossDomainEvent> _crossDomainEventHandler;

        public EventHandler(SharedKernel.DomainEvents.Core.IHandler<ICrossDomainEvent> crossDomainEventHandler)
        {
            _crossDomainEventHandler = crossDomainEventHandler;
        }

        protected void HandleCrossDomainEvents(Event evt)
        {
            if (evt is ICrossDomainEvent crossDomainEvent)
            {
                _crossDomainEventHandler.Handle(crossDomainEvent);
            }
        }
    }
}
