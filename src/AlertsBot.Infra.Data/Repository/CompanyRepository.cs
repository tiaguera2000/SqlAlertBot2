using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Infra.Data.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> VerifyLastSignUpCompanyHour(DateTime horaVerify)
        {
            bool retorno = false;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.BNE_IMP))
            {
                cmd.CommandText = "SELECT TOP(1) Dta_Cadastro FROM TAB_Filial ORDER BY Idf_Filial DESC";
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
