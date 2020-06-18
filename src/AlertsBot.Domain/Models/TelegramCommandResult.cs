using AlertsBot.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AlertsBot.Domain.Models
{
    public class TelegramCommandResult : ITelegramCommandResult
    {
        public TelegramCommandResult(string result)
        {
            Success = true;
            Result = result;
        }

        public TelegramCommandResult(IEnumerable<string> errors)
        {
            Success = false;
            Errors = errors;
        }

        public bool Success { get; }
        public string Result { get; }
        public IEnumerable<string> Errors { get; } = new List<string>();
    }
}
