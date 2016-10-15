using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public enum EnumMarketSessionType
    { 
        /// <summary>
        /// 集合竞价时间段
        /// </summary>
        CallAuction,
        /// <summary>
        /// 连续竞价时间段
        /// </summary>
        Continuous,
    }

    /// <summary>
    /// 交易所时间段
    /// </summary>
    public class MarketSession
    {

        public MarketSession(DateTime start, DateTime end, EnumMarketSessionType sessionType = EnumMarketSessionType.Continuous)
        {
            this.Start = start;
            this.End = end;
            this.SessionType = sessionType;
            this._timeKey = this.Start.ToTLDateTime();
        }

        long _timeKey = 0;
        public long TimeKey
        {
            get
            {
                return _timeKey;
            }
        }
        /// <summary>
        /// 交易时间段类别
        /// </summary>
        public EnumMarketSessionType SessionType { get; private set; }

        /// <summary>
        /// 交易时间段开始
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// 交易时间段结束
        /// </summary>
        public DateTime End { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}-{2}", this.Start.ToString("MM/dd HHmm"), this.End.ToString("MM/dd HHmm"));
        }
    }


    /// <summary>
    /// 交易日对象
    /// 用于定义一个交易 并界定对应的交易小节
    /// 
    /// 系统运行时 对应交易所开盘时间点执行定时任务 判定当前是否交易如果不交易则不执行初始化操作，如果是交易则执行初始化操作
    /// 并加载当前交易信息将TradingDay执行初始化
    /// </summary>
    public class MarketDay
    {

        public MarketDay()
        {
            this.TradingDay = 0;
        }

        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay { get; set; }

        /// <summary>
        /// 所有交易时间段
        /// </summary>
        public IEnumerable<MarketSession> MarketSessions { get { return marketSessionMap.Values; } }


        SortedDictionary<long, MarketSession> marketSessionMap = new SortedDictionary<long, MarketSession>();



        public void Init(int tradingday, IEnumerable<MarketSession> sessionList)
        {
            marketSessionMap.Clear();

            this.TradingDay = tradingday;
            foreach (var session in sessionList)
            {
                marketSessionMap.Add(session.TimeKey, session);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.TradingDay, string.Join(",", this.MarketSessions.Select(s => s.ToString()).ToArray()));
        }
    }
}
