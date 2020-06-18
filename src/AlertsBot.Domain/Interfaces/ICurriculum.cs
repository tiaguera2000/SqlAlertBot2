using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Domain.Interfaces
{
    public interface ICurriculum
    {
        Task<int> VerifyAmountIndexCvs();
        Task<bool> VerifyLastCvSignUpTBR(DateTime verifyHour);
    }
}
