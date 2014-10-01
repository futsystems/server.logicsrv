//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using DotLiquid;


//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 结算信息
//    /// </summary>
//    public class SettlementInfo : Drop
//    {
//        public string account { get; set; }
//        public string settleday { get; set; }
//        public string lastequity { get; set; }
//        public string nowequity { get; set; }
//        public string realizedpl { get; set; }
//        public string unrealziedpl { get; set; }
//        public string commission { get; set; }
//        public string netcash { get; set; }

//        public List<SettleTrades> trades { get; set; }
//        public SettlementInfo(Settlement settle)
//        {
//            account = settle.Account;
//            lastequity = settle.LastEqutiy.ToString();
//            settleday = settle.SettleDay.ToString();
//            nowequity = settle.NowEquity.ToString();
//            realizedpl = settle.RealizedPL.ToString();
//            unrealziedpl = settle.UnRealizedPL.ToString();
//            commission = settle.Commission.ToString();
//            netcash = (settle.CashIn - settle.CashOut).ToString();

//            trades = new List<SettleTrades>();
//            IList<Trade> list = ORM.MTradingInfo.SelectTrades(settle.SettleDay, settle.SettleDay);
//            foreach (Trade f in list)
//            {
//                f.oSymbol = BasicTracker.SymbolTracker[f.symbol];
//                trades.Add(new SettleTrades(f));
//            }
            
//        }



//    }

//    public class SettleTrades : Drop
//    {
//        public string xdate { get; set; }
//        public string exchange { get; set; }
//        public string security { get; set; }
//        public string expiremonth { get; set; }
//        public string side { get; set; }
//        public string hedgeflag { get; set; }
//        public string xprice { get; set; }
//        public string xsize { get; set; }
//        public string amount { get; set; }
//        public string posflag { get; set; }
//        public string commission { get; set; }
//        public string profit { get; set; }
//        public string seq { get; set; }

//        public SettleTrades(Trade f)
//        {
//            xdate = f.xdate.ToString();
//            exchange = f.oSymbol.SecurityFamily.Exchange.Name;
//            security = f.oSymbol.SecurityFamily.Name;
//            expiremonth = "";// f.oSymbol.ExpireMonth.ToString();
//            side = f.side ? "买" : "卖";
//            hedgeflag = "投机";
//            xprice = f.xprice.ToString();
//            amount = Math.Abs(f.xprice * f.xsize * f.oSymbol.Multiple).ToString();
//            posflag = f.IsEntryPosition? "开仓" : "平仓";
//            commission = f.Commission.ToString();
//            profit = "";
//            seq = "";
//        }

//    }


//}
