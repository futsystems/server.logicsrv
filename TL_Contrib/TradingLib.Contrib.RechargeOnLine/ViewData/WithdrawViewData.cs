using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using DotLiquid;
using System.Security;
using System.Security.Cryptography;


namespace TradingLib.Contrib.RechargeOnLine
{
    public class WithdrawViewData:Drop
    {
        JsonWrapperCashOperation _cashop = null;
        public WithdrawViewData(JsonWrapperCashOperation cashop)
        {
            _cashop = cashop;

        }

        //交易帐号
        public string Account { get { return _cashop.Account; } }
        //原始资金数额
        public string Amount { get { return Util.FormatDecimal(_cashop.Amount,"{0:F2}"); } }
        //出入金操作
        public string CashOperation { get { return Util.GetEnumDescription(_cashop.Operation); } }
        //编号
        public string Ref { get { return _cashop.Ref; } }
    }
}
