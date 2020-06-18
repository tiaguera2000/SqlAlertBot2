using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<bool> VerifyLastSignUpCompanyHour(DateTime horaVerify);
    }
}
