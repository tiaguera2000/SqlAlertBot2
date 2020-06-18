using AlertsBot.Domain.Interfaces;
using AlertsBot.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AlertsBot.Infra.Data.Repository
{
    public class PaymentRepository : IPayment
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> VerifyLastBillet(DateTime horaVerify)
        {
            bool retorno = false;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.BNE_IMP))
            {
                cmd.CommandText = "SELECT TOP(1) Dta_Cadastro FROM BNE_Pagamento where Idf_Tipo_Pagamento = 2 order by Dta_Cadastro DESC";

                var result = await cmd.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    var dataCadastroLastCompany = Convert.ToDateTime(result["Dta_Cadastro"]);
                    retorno = (dataCadastroLastCompany >= horaVerify.AddHours(-2) ? false : true);
                }

                result.Close();
            }

            _unitOfWork.Commit();
            return retorno;
        }

        public async Task<bool> VerifyLastCreditCard(DateTime horaVerify)
        {
            bool retorno = false;
            using (var cmd = _unitOfWork.CreateCommand(ConnectionBase.BNE_IMP))
            {
                cmd.CommandText = "SELECT TOP(1) Dta_Cadastro FROM BNE_Pagamento where Idf_Tipo_Pagamento = 1 order by Dta_Cadastro DESC";

                var result = await cmd.ExecuteReaderAsync();
                while (await result.ReadAsync())
                {
                    var dataCadastroLastCompany = Convert.ToDateTime(result["Dta_Cadastro"]);
                    retorno = (dataCadastroLastCompany >= horaVerify.AddHours(-2) ? false : true);
                }

                result.Close();
            }
            _unitOfWork.Commit();
            return retorno;
        }
    }
}
