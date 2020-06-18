using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Domain.Interfaces
{
    public interface IJobRepository
    {
        Task<bool> VerifyLastJobPosted(DateTime verifyHour);
        Task<int> VerifyAmountJobsTbrInBne();
    }
}
