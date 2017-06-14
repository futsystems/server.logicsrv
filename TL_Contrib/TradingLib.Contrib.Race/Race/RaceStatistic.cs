using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Contrib
{

    public class RaceStatistic:IRaceStatistic
    {

        public RaceStatistic()
        { 
            
        }
        
        public string RaceID { get; set; }
        public QSEnumRaceType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime BeginSingUpTime { get; set; }
        public DateTime EndSingUpTime { get; set; }
        public decimal StartEquity { get; set; }
        public int EntryNum { get; set; }
        public int EliminateNum { get; set; }
        public int PromotNum { get; set; }
        public int ContestantsNum { get; set; }

        public override string ToString()
        {
            string d = ",";
            return RaceID + d + this.Type.ToString() + d + StartTime.ToString() + d + BeginSingUpTime.ToString() + d + EndSingUpTime.ToString() + d + StartEquity.ToString() + d + EntryNum.ToString() + d + EliminateNum.ToString() + d + PromotNum.ToString() + d + ContestantsNum.ToString();

        }

        public static RaceStatistic FromString(string msg)
        {
            RaceStatistic r = new RaceStatistic();
            string[] p = msg.Split(',');
            r.RaceID = p[0];
            r.Type = (QSEnumRaceType)Enum.Parse(typeof(QSEnumRaceType), p[1]);
            r.StartTime = Convert.ToDateTime(p[2]);
            r.BeginSingUpTime = Convert.ToDateTime(p[3]);
            r.EndSingUpTime = Convert.ToDateTime(p[4]);
            r.StartEquity = Convert.ToDecimal(p[5]);
            r.EntryNum = Convert.ToInt32(p[6]);
            r.EliminateNum = Convert.ToInt32(p[7]);
            r.PromotNum = Convert.ToInt32(p[8]);
            r.ContestantsNum = Convert.ToInt32(p[9]);

            return r;
        }
    }
}
