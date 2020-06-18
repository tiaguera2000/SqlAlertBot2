using AlertsBot.Domain.Models.Enum;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace AlertsBot.Domain.Interfaces
{
    public interface IUnitOfWork 
    {
        IDbTransaction Transaction { get; }

        DbCommand CreateCommand(ConnectionBase connectionBase);

        NpgsqlCommand CreateCommandPostgres();

        string ChangeDatabase(ConnectionBase connectionBase);

        bool Commit();
    }
}
