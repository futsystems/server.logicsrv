using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    
    public class TradeCache
    {

        public TradeCache(Symbol symbol)
        {
            this.Symbol = symbol;
            this.TradeList = new List<Tick>();
            this.PriceVolList = new SortedDictionary<decimal, PriceVol>();
        }

        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay { get; set; }

        /// <summary>
        /// 成交列表
        /// </summary>
        public List<Tick> TradeList { get; set; }

        /// <summary>
        /// 量价分布列表
        /// </summary>
        public SortedDictionary<decimal, PriceVol> PriceVolList { get; set; }

        object _object = new object();
        /// <summary>
        /// 添加一个新的成交数据
        /// </summary>
        /// <param name="k"></param>
        public void NewTrade(Tick k)
        {
            lock (_object)
            {
                TradeList.Add(k);

                PriceVol pv = null;
                if (!this.PriceVolList.TryGetValue(k.Trade, out pv))
                {
                    pv = new PriceVol(k.Trade);
                    this.PriceVolList.Add(k.Trade,pv);
                }
                pv.Vol += k.Size;
            }
        }

        /// <summary>
        /// 清空所有缓存数据
        /// </summary>
        public void Clear()
        {
            lock (_object)
            {
                this.TradeList.Clear();
                this.PriceVolList.Clear();
            }
        }

         /// <summary>
         /// 查询成交数据
         /// </summary>
         /// <param name="startIndex"></param>
         /// <param name="count"></param>
         /// <returns></returns>
        public List<Tick> QryTrade(int startIndex, int maxcount)
        {
            lock (_object)
            {
                IEnumerable<Tick> records = null;
                if (maxcount <= 0)
                {
                    records = this.TradeList.Take(Math.Max(0, TradeList.Count() - startIndex));
                }
                else //设定最大数量 返回数据要求 按时间先后排列
                {
                    //startIndex 首先从数据序列开头截取对应数量的数据
                    //maxcount 然后从数据序列末尾截取最大数量的数据
                    records = this.TradeList.Take(Math.Max(0, TradeList.Count() - startIndex)).Skip(Math.Max(0, (TradeList.Count() - startIndex) - maxcount));//返回序列后段元素
                }
                return records.ToList() ;
            }
        }

        /// <summary>
        /// 查询价格成交量分布
        /// </summary>
        /// <returns></returns>
        public List<PriceVol> QryPriceVol()
        {
            return this.PriceVolList.Values.ToList();
        }
        
    }
}
