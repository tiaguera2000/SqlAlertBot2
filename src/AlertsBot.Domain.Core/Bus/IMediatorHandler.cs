using System.Threading.Tasks;
using AlertsBot.Domain.Core.Commands;
using AlertsBot.Domain.Core.Events;

namespace AlertsBot.Domain.Core.Bus
{
    public interface IMediatorHandler
    {
        Task<TResponse> SendCommand<T, TResponse>(T command) where T : Command<TResponse>;
        Task SendCommand<T>(T command) where T : Command;
        Task RaiseEvent<T>(T @event) where T : Event;
    }
}
