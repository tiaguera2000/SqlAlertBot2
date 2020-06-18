using AlertsBot.Domain.Core.Commands;
using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Validations;

namespace AlertsBot.Domain.Commands
{
    public class ExecuteTelegramCommand : Command<ITelegramCommandResult>
    {
        public ExecuteTelegramCommand(string command)
        {
            Command = command;
        }
        public string Command { get; }

        public override bool IsValid()
        {
            ValidationResult = new ExecuteTelegramCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
