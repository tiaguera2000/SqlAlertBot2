using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlertsBot.Domain.Interfaces
{
    public interface IPayment
    {
        Task<bool> VerifyLastBillet(DateTime hourVerify);
        Task<bool> VerifyLastCreditCard(DateTime hourVerify);

    }
}
