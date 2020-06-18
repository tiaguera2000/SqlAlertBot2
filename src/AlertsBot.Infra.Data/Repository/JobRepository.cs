using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Infra.Data.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> VerifyAmountJobsTbrInBne()
        {
            int amountcvsIndex = 0;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.BNE_IMP))
            {
                cmd.CommandText = "SELECT COUNT(*) as vagasIndexadas FROM BNE_Vaga where Idf_Origem = 2 and Dta_Cadastro >= CONVERT(date, getdate())";

                var result = await cmd.ExecuteReaderAsync();
                while (await result.ReadAsync())
                {
                    if (result["vagasIndexadas"] != DBNull.Value)
                    {
                        amountcvsIndex = Convert.ToInt32(result["vagasIndexadas"]);
                    }
                }

                result.Close();
            }
            _unitOfWork.Commit();
            return amountcvsIndex;

        }

        public async Task<bool> VerifyLastJobPosted(DateTime horaVerify)
        {
            bool retorno = false;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.SINE_PRD))
            {
                cmd.CommandText = "SELECT TOP(1) Dta_Cadastro FROM SIN_VAGA WHERE Idf_Origem_Importacao IS NULL ORDER BY Idf_Vaga DESC";
                var result = await cmd.ExecuteReaderAsync();
                while (await result.ReadAsync())
                {
                    var dataCadastroLastCompany = Convert.ToDateTime(result["Dta_Cadastro"]);
                    if (dataCadastroLastCompany >= horaVerify.AddHours(-2))
                    {
                        retorno = false;
                    }
                    else
                    {
                        retorno = true;
                    }
                }

                result.Close();
            }

            _unitOfWork.Commit();

            return retorno;
        }
    }
}
