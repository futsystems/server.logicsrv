using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 强平时间与市场交易时间 值对
    /// 在某个时间点进行强平 并制定该市场时间对象是哪个，在强平时需要把所有绑定该市场交易时间 品种 进行强平
    /// </summary>
    public class FlatTimeMarketTimePair
    {
        public FlatTimeMarketTimePair(int flattime, MarketTime mt)
        {
            this.FlatTime = flattime;
            this.MarketTime = mt;
        }
        /// <summary>
        /// 对应的强平时间
        /// </summary>
        public int FlatTime { get; set; }

        /// <summary>
        /// 对应的市场交易时间
        /// </summary>
        public MarketTime MarketTime { get; set; }
    }
    /// <summary>
    /// 交易时间段维护器, 用于从数据库加载交易时间段的定义
    /// 从而在加载品种列表时为每个品种指定对应的交易时间
    /// 只有在每次开机时才加载市场交易时间盘中没有修改交易时间对象的必要
    /// </summary>
    public class DBMarketTimeTracker
    {
        Dictionary<int, MarketTime> idxsessionmap = new Dictionary<int, MarketTime>();

        public DBMarketTimeTracker()
        {
            foreach (MarketTime mt in ORM.MBasicInfo.SelectSession())
            {
                idxsessionmap.Add(mt.ID, mt);
            }
        }

        public MarketTime this[int idx]
        {
            get
            {
                MarketTime session = null;
                if (idxsessionmap.TryGetValue(idx, out session))
                {
                    return session;
                }
                return null;
            }
        }

        public MarketTime[] MarketTimes
        {
            get
            {
                return idxsessionmap.Values.ToArray();
            }
        }

        /// <summary>
        /// 获得强平时间点与MarketTime 值对
        /// </summary>
        /// <returns></returns>
        public FlatTimeMarketTimePair[] GetFlatTimeMarketTimePairs()
        {
            List<FlatTimeMarketTimePair> list = new List<FlatTimeMarketTimePair>();
            //遍历所有市场交易时间对象
            foreach (MarketTime mt in idxsessionmap.Values)
            {
                //每个市场交易时间对象下的时间片段进行遍历 找出有强平标识的片段
                foreach (MktTimeEntry me in mt.MktTimeEntries)
                { 
                    if(me.NeedFlat)//如果需要强平 则记录对应的交易时间对象
                    {
                        list.Add(new FlatTimeMarketTimePair(me.FlatStartTime,mt));                
                    }
                }
            }
            return list.ToArray();
        }

        
    }
}
