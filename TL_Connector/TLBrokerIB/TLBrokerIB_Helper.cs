using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace Broker.Live
{
    public partial class TLBrokerIB
    {

        Krs.Ats.IBNet.Contract Symbol2Contract(Symbol sym)
        {

            Krs.Ats.IBNet.Contract c = new Krs.Ats.IBNet.Contract();
            if (sym.SecurityFamily.Type == SecurityType.FUT)
            { 
                return new Krs.Ats.IBNet.Contract(sym.SecurityFamily.Code,GetIBExchange(sym), Krs.Ats.IBNet.SecurityType.Future, "USD",GetExpireMonth(sym));
            }
            return c;
        }

        //交易所转换
        /* 
         * LocalExchange  IBExchange
         * NYMEX_GBX      NYMEX
         * 
         * 
         * 
         * */

        string GetExpireMonth(Symbol sym)
        {
            return Util.ToDateTime(sym.ExpireDate, 0).ToString("yyyyMM");
        }
        string GetIBExchange(Symbol sym)
        { 
            string localex = sym.SecurityFamily.Exchange.EXCode;
            
            return localex;
        }
    }
}
