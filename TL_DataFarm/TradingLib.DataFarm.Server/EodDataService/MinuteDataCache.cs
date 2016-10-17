using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{

    /// <summary>
    /// 分时数据缓存
    /// </summary>
    public class MinuteDataCache
    {

        public MinuteDataCache(Symbol symbol,MarketDay marketday)
        {
            this.Symbol = symbol;
            this.MinuteDataMap = new Dictionary<long, MinuteData>();
            this.MarketDay = marketday;

            int i=0;
            //初始化所有分时数据
            foreach (var session in this.MarketDay.MarketSessions)
            {
                for (DateTime d = session.Start.AddMinutes(1); d <= session.End; d = d.AddMinutes(1))
                {
                    long key = d.ToTLDateTime();
                    this.MinuteDataMap.Add(key, new MinuteData(d.ToTLDate(), d.ToTLTime(),-1, 0, 0));
                    locationMap.Add(key, i);
                    i++;
                }
            }
        }


        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// MarketDay
        /// </summary>
        public MarketDay MarketDay { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay { get; set; }

        /// <summary>
        /// 量价分布列表
        /// </summary>
        public Dictionary<long, MinuteData> MinuteDataMap { get; set; }

        Dictionary<long, int> locationMap = new Dictionary<long, int>();

        bool restored = true;
        object _object = new object();


        long _latestBarKey = 0;
        public void On1MinBarClosed(Bar bar)
        {
            this.GotBar(bar);
        }

        public void On1MinPartialBarUpdate(Bar bar)
        {
            this.GotBar(bar);
        }

        //List<Bar> tmpList = new List<Tick>();
        /// <summary>
        /// 恢复历史成交数据
        /// </summary>
        /// <param name="k"></param>
        public void RestoreMinuteData(List<BarImpl> barList)
        {
            //lock (_object)
            //{
                foreach (var bar in barList)
                {
                    GotBar(bar);
                }
                //Tick lastTick = TradeList.LastOrDefault();
                ////没有恢复到任何历史Tick 则将临时Tick列表中的所有Tick数据导入
                //if (lastTick == null)
                //{
                //    foreach (var k in tmpList)
                //    {
                //        GotTick(k);
                //    }
                //}
                //else
                //{
                //    foreach (var k in tmpList)
                //    {
                //        if (k.Vol > lastTick.Vol)//利用总成交量进行拼接数据 总成交量大于 恢复Tick的最后一个Tick的成交量 的所有Tick合并到列表中
                //        {
                //            GotTick(k);
                //        }
                //    }
                //}
                //restored = true;
            //}
        }

        void GotBar(Bar bar)
        {
            MinuteData target = null;
            //找到对应的分时数据更新数值
            long key = bar.EndTime.ToTLDateTime();
            
            if (this.MinuteDataMap.TryGetValue(bar.EndTime.ToTLDateTime(), out target))
            {
                target.Close = bar.Close;
                target.Vol = bar.Volume;
                target.AvgPrice = 0;
                long dt = bar.EndTime.ToTLDateTime();
                if (dt > _latestBarKey)
                {
                    _latestBarKey = dt;
                }

                int idx = locationMap[key];
                for(int i=idx-1;i>=0;i--)
                {
                    MinuteData md = this.MinuteDataMap.ElementAt(i).Value;
                    if (md.Vol == 0)
                    {
                        md.Close = target.Close;
                    }
                    else
                    {
                        break;
                    }
                }
                
                foreach (var cache in this.MinuteDataMap.Where(p => p.Key < _latestBarKey))
                { 
                
                }
            }
        }

        /// <summary>
         /// 查询成交数据
         /// </summary>
         /// <param name="startIndex"></param>
         /// <param name="count"></param>
         /// <returns></returns>
        public List<MinuteData> QryMinuteDate()
        {
            return this.MinuteDataMap.Where(d => d.Key <= _latestBarKey).Select(d => d.Value).ToList();
        }

    }
}
