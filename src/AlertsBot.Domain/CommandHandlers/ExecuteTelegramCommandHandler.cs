using AlertsBot.Domain.Commands;
using AlertsBot.Domain.Core.Bus;
using AlertsBot.Domain.Core.Notifications;
using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AlertsBot.Domain.CommandHandlers
{
    public class ExecuteTelegramCommandHandler : CommandHandler,
        IRequestHandler<ExecuteTelegramCommand, ITelegramCommandResult>
    {
        private readonly IEnumerable<ITelegramCommand> telegramCommands;
        private readonly INotificationHandler<DomainNotification> notifications;
        private readonly Regex reCommand = new Regex("^/(?<command>[^ ]+)( +(?<param>[^ ]+))?", RegexOptions.Compiled);

        public ExecuteTelegramCommandHandler(
            IEnumerable<ITelegramCommand> telegramCommands,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        {
            this.telegramCommands = telegramCommands;
            this.notifications = notifications;
        }

        public async Task<ITelegramCommandResult> Handle(ExecuteTelegramCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return new TelegramCommandResult(request.ValidationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Getting commands and parameters
            var match = reCommand.Match(request.Command);
            if (!match.Success)
            {
                await notifications.Handle(new DomainNotification("INVALID_TELEGRAM_FORMT", $"The comand is not in format '/<command> <param1> ... <paramN>': '{request.Command}'"), cancellationToken);
                return new TelegramCommandResult(new List<string>() { $"The comand is not in format '/<command> <param1> ... <paramN>': '{request.Command}'" });
            }

            var commandName = match.Groups["command"].Value;

            var cmd = telegramCommands.FirstOrDefault(tc => tc.CommandName.ToLower().Equals(commandName.ToLower()));
            if (cmd == null && !"indexacao".Equals(commandName))
            {
                await notifications.Handle(new DomainNotification("INVALID_TELEGRAM_COMMAND", $"There's no telegram command for '{commandName}'"), cancellationToken);
                return new TelegramCommandResult(new List<string>() { $"There's no telegram command for '{commandName}'" });
            }

            IEnumerable<string> param;
            if (match.Groups.Any(g => g.Name.Equals("param")))
            {
                param = match.Groups["param"].Captures.Select(c => c.Value).ToList();
            }
            else
            {
                param = new List<string>();
            }

            return cmd.Execute(param);
        }
    }
}
