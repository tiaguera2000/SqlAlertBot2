
using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models.Enum;
using log4net;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace AlertsBot.Infra.SqlServer.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly List<IDbConnection> Connections;
        private IDbConnection connection;
        private NpgsqlConnection _connectionPostgre;
        private readonly ILog log;
        private IDbTransaction _transaction;

        public UnitOfWork(List<IDbConnection> Connections, NpgsqlConnection connectionPostgre,  ILog log)
        {
            this.Connections = Connections;
            this.log = log;
            _connectionPostgre = connectionPostgre;
        }

        public virtual IDbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
            private set
            {
                _transaction = value;
            }
        }

        public bool Commit()
        {
            if (Transaction == null)
            {
                log.Debug("The transactions was already committed or there's nothing to commit.");
                return false;
            }

            try
            {
                Transaction.Commit();
                Transaction = null;
                log.Debug("Transaction committed successfully.");
            }
            catch (Exception ex)
            {
                log.Error("An error ocurred commiting the transaction.", ex);
                return false;
            }

            return true;
        }

        public DbCommand CreateCommand(ConnectionBase connectionBase)
        {
            // if ternario; se a conexaoBse for igual BNE_IMP pega o primeiro caso senao, o segundo;
            if (connectionBase == ConnectionBase.BNE_IMP)
            {
                connection = Connections.Where(x => x.ConnectionString.Contains("BNE_IMP")).FirstOrDefault();
            }else if (connectionBase == ConnectionBase.SINE_PRD)
            {
                connection = Connections.Where(x => x.ConnectionString.Contains("SINE_PRD")).FirstOrDefault();
            }

            //connection = connectionBase == ConnectionBase.BNE_IMP ?
            //Connections.Where(x => x.ConnectionString.Contains("BNE_IMP")).FirstOrDefault() :
            //Connections.Where(x => x.ConnectionString.Contains("SINE_PRD")).FirstOrDefault();

            if (log.IsDebugEnabled)
                log.Debug("Start Create Command");

            if (connection.State == ConnectionState.Closed)
            {
                if (log.IsDebugEnabled) { log.Debug("Start Create Command - State connection Closed."); }
                connection.Open();
            }

            if (Transaction == null)
            {
                if (log.IsDebugEnabled) { log.Debug("Start Create Command - Transaction == null."); }
                Transaction = connection.BeginTransaction();
            }

            if (log.IsDebugEnabled)
                log.Debug("Start Create Command - connection.CreateCommand() as DbCommand.");

            var command = connection.CreateCommand() as DbCommand;

            if (log.IsDebugEnabled)
                log.Debug("Start Create Command - Transaction as DbTransaction");

            command.Transaction = Transaction as DbTransaction;

            if (log.IsDebugEnabled)
                log.Debug($"Start Create Command - return command - {command}");

            return command;
        }

        public NpgsqlCommand CreateCommandPostgres()
        {
            if (log.IsDebugEnabled)
                log.Debug("Start Create Command");

            if (_connectionPostgre.State == ConnectionState.Closed)
            {
                _connectionPostgre.Open();
            }

            if (Transaction == null)
            {
                if (log.IsDebugEnabled) { log.Debug("Start Create Command - Transaction == null."); }
                Transaction = _connectionPostgre.BeginTransaction();
            }

            if (log.IsDebugEnabled)
                log.Debug("Start Create Command - connection.CreateCommand() as NpgsqlCommand.");

            var command = _connectionPostgre.CreateCommand() as NpgsqlCommand;

            if (log.IsDebugEnabled)
                log.Debug("Start Create Command - Transaction as NpgsqlTransaction");

            command.Transaction = Transaction as NpgsqlTransaction;

            if (log.IsDebugEnabled)
                log.Debug($"Start Create Command - return command - {command}");

            return command;
        }

        public string ChangeDatabase(ConnectionBase connectionBase)
        {
            if (connectionBase == ConnectionBase.BNE_IMP)
            {
                connection = Connections.Where(x => x.ConnectionString.Contains("BNE_IMP")).FirstOrDefault();
            }
            else if (connectionBase == ConnectionBase.SINE_PRD)
            {
                connection = Connections.Where(x => x.ConnectionString.Contains("SINE_PRD")).FirstOrDefault();
            }
            return connection.ConnectionString;
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }

            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();

                connection.Dispose();
            }
        }


    }
}
