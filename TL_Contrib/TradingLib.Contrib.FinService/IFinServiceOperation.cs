using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IFinServiceOperation
    {

        void ActiveFinService(string account);
        void InActiveFinService(string account);
        void UpdateFinService(string account, decimal amount, string type, decimal discount, string agentcode);
        

    }
}
