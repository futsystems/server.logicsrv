using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 某个比赛信息接口
    /// </summary>
    public interface IRaceStatistic
    {
        string RaceID{get;set;}
        QSEnumRaceType Type { get; set; }
        DateTime StartTime { get; set; }
        DateTime BeginSingUpTime { get; set; }
        DateTime EndSingUpTime { get; set; }
        decimal StartEquity { get; set; }
        int EntryNum { get; set; }
        int EliminateNum { get; set; }
        int PromotNum { get; set; }
        int ContestantsNum { get; set; }


    }
}
