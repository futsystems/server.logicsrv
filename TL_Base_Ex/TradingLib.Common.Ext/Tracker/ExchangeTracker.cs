using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 交易所信息维护
    /// 用于维护系统全局交易所信息
    /// </summary>
    public  class DBExchangeTracker
    {
        Dictionary<string, Exchange> exchagneIndexMap = new Dictionary<string, Exchange>();
        Dictionary<int, Exchange> exchangeIdMap = new Dictionary<int, Exchange>();

        public DBExchangeTracker()
        { 
            //从数据库加载交易所信息 将其缓存到内存
            foreach (Exchange ex in ORM.MBasicInfo.SelectExchange())
            {
                exchagneIndexMap.Add(ex.Index, ex);
                exchangeIdMap.Add(ex.ID, ex);
            }
        }

        public string GetExchangeTitle(string index)
        {
            Exchange ex = null;
            IExchange ee = null;
            
            if (exchagneIndexMap.TryGetValue(index, out ex))
            {
                return ex.Title;
            }
            return "未知";
        }
        /// <summary>
        /// 通过数据库ID获得交易所对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IExchange this[int id]
        {
            get {
                Exchange ex = null;
                if (exchangeIdMap.TryGetValue(id, out ex))
                {
                    return ex as IExchange;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 通过交易所编号获得交易所对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IExchange this[string index]
        {
            get
            {
                Exchange ex = null;
                if (exchagneIndexMap.TryGetValue(index, out ex))
                {
                    return ex as IExchange;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 返回所有交易所列表
        /// </summary>
        public IExchange[] Exchanges
        {
            get
            {
                return exchagneIndexMap.Values.ToArray();
            }
        }


    }
}
