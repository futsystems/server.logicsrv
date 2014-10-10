using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet;
using Krs.Ats.IBNet.Contracts;
using TradeLink.API;
using TradeLink.Common;
using TradingLib.Data;
using TradingLib;
namespace DataFeed.IB
{
    public class IBHelper
    {
        public IBHelper()
        {

            UpdateSecurity();
        }

        public void UpdateSecurity()
        {
            updateDefaultExchange();
            updateDefaultMasterSecurity();
            updateSymbolSecurityMap();
            updateSymbolInstance();
        }
        //品种全名--品种映射
        private Dictionary<string, Security> defaultMasterSecurity = new Dictionary<string, Security>();
        public void updateDefaultMasterSecurity()
        {
            List<Security> l = SecurityTracker.getMastSecurities();
            lock (defaultMasterSecurity)
            {
                defaultMasterSecurity.Clear();
                foreach (Security s in l)
                {
                    defaultMasterSecurity.Add(s.FullName, s);
                }
            }
        }
        //合约代码-- 主合约映射
        Dictionary<string, string> symbolMasterSecurityMap = new Dictionary<string, string>();
        public void updateSymbolSecurityMap()
        {
            lock (symbolMasterSecurityMap)
            {
                symbolMasterSecurityMap = BasketTracker.getSymbolSecurityMap();
            }
        }

        //交易所列表
        private Dictionary<string, Exchange> defaultExchange = new Dictionary<string, Exchange>();
        public void updateDefaultExchange()
        {
            List<Exchange> l = ExchangeTracker.getExchList();
            lock (defaultExchange)
            {
                defaultExchange.Clear();
                foreach (Exchange ex in l)
                {
                    defaultExchange.Add(ex.Index, ex);
                }
            }
        }
        //
        Dictionary<string, Security> securityInstanceMap = new Dictionary<string, Security>();
        void updateSymbolInstance()
        {
            securityInstanceMap = BasketTracker.getSymbolInstanceMap();
        }

        Security getSymbolInstance(string symbol)
        {
            Security sec = null;
            if (securityInstanceMap.TryGetValue(symbol, out sec))
                return sec;
            return sec;
        }
        //通过交易所全局编号得到交易所对象
        Exchange Indx2Exchange(string exindex)
        {
            if(defaultExchange.ContainsKey(exindex))
                return defaultExchange[exindex];
            return null;
        }
        //
        public Security getMasterSecurity(string symbol)
        {
            string mastsecfn = null;
            if(symbolMasterSecurityMap.TryGetValue(symbol,out mastsecfn))
                return defaultMasterSecurity[mastsecfn];//通过securityfn找到对应的默认security 然后就可以找到对应的exchange index
            return null;
        }



        public  Contract Symbol2IBContract(string symbol)
        {
            try
            {
                debug(symbol +" 转换成IBContract");
                //通过symbol获得其主合约
                Security mastersec = getMasterSecurity(symbol);
                //通过symbol获得其合约细节信息
                //debug("1");
                Security symbolinstance = this.getSymbolInstance(symbol);
                //debug("2");
                switch (mastersec.Type)
                {
                    case TradeLink.API.SecurityType.STK:
                        return new Equity(symbol);
                    case TradeLink.API.SecurityType.FUT:
                        string mastsymbol = mastersec.Symbol;
                        string excode = Indx2Exchange(mastersec.DestEx).EXCode;
                        debug(symbolinstance.Date.ToString());
                        DateTime date = Util.ToDateTime(symbolinstance.Date*100+1, 0);
                        debug("symbol:" + mastsymbol + "  Exchange:" + excode + "  Exprie:" + date.ToString("yyyyMM"));
                        return new Future(mastsymbol, excode, date.ToString("yyyyMM"));
                    case TradeLink.API.SecurityType.CASH:
                        string [] p = symbol.Split('/');
                        return new Forex(p[0], p[1]);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                return null;
            }
        }

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
    }
}
