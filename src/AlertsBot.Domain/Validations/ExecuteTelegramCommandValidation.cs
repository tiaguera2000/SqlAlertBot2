using AlertsBot.Domain.Commands;
using FluentValidation;

namespace AlertsBot.Domain.Validations
{
    public class ExecuteTelegramCommandValidation : AbstractValidator<ExecuteTelegramCommand>
    {
        public ExecuteTelegramCommandValidation()
        {
            RuleFor(c => c.Command)
                .NotEmpty().WithMessage("Command name is required");
        }
    }
}
