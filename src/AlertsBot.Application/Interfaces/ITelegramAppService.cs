using System.Threading.Tasks;

namespace AlertsBot.Application.Interfaces
{
    public interface ITelegramAppService
    {
        Task<string> ExecuteCommandAsync(string command);
    }
}
