using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Infra.Data.Repository
{
    public class CurriculumRepository : ICurriculum
    {
        private readonly IUnitOfWork _unitOfWork;

        public CurriculumRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<int> VerifyAmountIndexCvs()
        {

            int amountcvsIndex = 0;

            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.BNE_IMP))
            {
                cmd.CommandText = "SELECT COUNT(*) as cvsIndexados FROM [BNE_IMP].[BNE].[BNE_Curriculo_Origem] WHERE Idf_Origem = 2 AND Dta_Cadastro >= CONVERT(date, getdate())";

                var result = await cmd.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    if (result["cvsIndexados"] != DBNull.Value)
                    {
                        amountcvsIndex = Convert.ToInt32(result["cvsIndexados"]);
                    }
                }

                result.Close();
            }
            _unitOfWork.Commit();
            return amountcvsIndex;

        }

        public async Task<bool> VerifyLastCvSignUpTBR(DateTime horaVerify)
        {


            bool retorno = false;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.SINE_PRD))
            {
                cmd.CommandText = "SELECT TOP(1) Dta_Cadastro FROM SIN_Curriculo order by Idf_Usuario DESC";

                var result = await cmd.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    var dataCadastroLastCurriculum = Convert.ToDateTime(result["Dta_Cadastro"]);
                    if (dataCadastroLastCurriculum >= horaVerify.AddHours(-2))
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
