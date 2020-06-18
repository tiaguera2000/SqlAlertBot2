using AlertsBot.Application.Interfaces;
using AlertsBot.Domain.Commands;
using AlertsBot.Domain.Core.Bus;
using AlertsBot.Domain.Interfaces;
using System.Threading.Tasks;

namespace AlertsBot.Application.Services
{
    public class TelegramAppService : ITelegramAppService
    {
        private readonly IMediatorHandler bus;

        public TelegramAppService(IMediatorHandler bus)
        {
            this.bus = bus;
        }

        public async Task<string> ExecuteCommandAsync(string command)
        {
            return (await bus.SendCommand<ExecuteTelegramCommand, ITelegramCommandResult>(new ExecuteTelegramCommand(command))).Result;
        }
    }
}
