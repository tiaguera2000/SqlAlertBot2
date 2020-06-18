using System.Collections.Generic;

namespace AlertsBot.Domain.Interfaces
{
    public interface ITelegramCommand
    {
        string CommandName { get; }

        ITelegramCommandResult Execute(IEnumerable<string> arguments);
    }

    public interface ITelegramCommandResult
    {
        bool Success { get; }

        string Result { get; }

        IEnumerable<string> Errors { get; }
    }
}
