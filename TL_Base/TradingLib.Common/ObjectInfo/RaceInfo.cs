//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{

//    public class RaceInfo : IRaceInfo
//    {
//        public string AccountID { get; set; }
//        public DateTime EntryTime { get; set; }//加入比赛时间
//        public string RaceID { get; set; }//比赛标识
//        public decimal ObverseProfit { get; set; }//折算收益
//        public decimal StartEquity { get; set; }//起始权益
//        public decimal PromptEquity { get; set; }//晋级权益
//        public decimal ElimiateEquity { get; set; }//淘汰权益
//        public decimal NowEquity { get; set; }//当前权益
//        public QSEnumAccountRaceStatus RaceStatus { get; set; }//当前比赛状态

//        public RaceInfo()
//        {
//            AccountID = string.Empty;
//            EntryTime = new DateTime(1970, 1, 1);
//            RaceID = string.Empty;
//            ObverseProfit = 0;
//            StartEquity = 0;
//            PromptEquity = 0;
//            ElimiateEquity = 0;
//            NowEquity = 0;
//        }
//        /*
//        public static RaceInfo getRaceInfo(IAccount acc)
//        {
//            RaceInfo ri = new RaceInfo();
//            ri.AccountID = acc.ID;
//            ri.EntryTime = acc.RaceEntryTime;
//            ri.RaceID = acc.RaceID;
//            ri.RaceStatus = acc.RaceStatus;
//            ri.ObverseProfit = acc.ObverseProfit;
//            //ri.StartEquity = 

//            return ri;
//        }**/

//        public static RaceInfo Deserialize(string msg)
//        {
//            RaceInfo ri = new RaceInfo();
//            string[] p = msg.Split(',');
//            ri.AccountID = Convert.ToString(p[0]);
//            ri.EntryTime = Convert.ToDateTime(p[1]);
//            ri.RaceID = Convert.ToString(p[2]);
//            ri.RaceStatus = (QSEnumAccountRaceStatus)Enum.Parse(typeof(QSEnumAccountRaceStatus), Convert.ToString(p[3]));
//            ri.ObverseProfit = Convert.ToDecimal(p[4]);
//            ri.StartEquity = Convert.ToDecimal(p[5]);
//            ri.PromptEquity = Convert.ToDecimal(p[6]);
//            ri.ElimiateEquity = Convert.ToDecimal(p[7]);
//            ri.NowEquity = Convert.ToDecimal(p[8]);
//            return ri;

//        }
//        public static string Serialize(IRaceInfo ri)
//        {
//            string d = ",";
//            return ri.AccountID + d + ri.EntryTime.ToString() + d + ri.RaceID + d + ri.RaceStatus.ToString() + d + ri.ObverseProfit.ToString() + d + ri.StartEquity.ToString() + d + ri.PromptEquity.ToString() + d + ri.ElimiateEquity.ToString() + d + ri.NowEquity;
//        }
//    }
//}
