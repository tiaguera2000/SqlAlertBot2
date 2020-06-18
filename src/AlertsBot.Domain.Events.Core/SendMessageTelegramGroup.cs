using AlertsBot.Domain.Core.Events;
using SharedKernel.DomainEvents.CrossDomainEvents;

namespace AlertsBot.Domain.Events.Core
{
    public class SendMessageTelegramGroup : Event, ICrossDomainEvent
    {
        public long ChatId { get; set; }
        public string Message { get; set; }

        public SendMessageTelegramGroup(long chatId, string message)
        {
            this.ChatId = chatId;
            this.Message = message;
        }

    }
}
