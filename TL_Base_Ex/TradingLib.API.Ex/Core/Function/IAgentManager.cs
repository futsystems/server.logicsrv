using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAgentManager
    {
        void CashOperation(CashTransaction txn);

        void AssignTxnID(CashTransaction txn);

        IEnumerable<IAgent> Agents { get; }
    }
}
