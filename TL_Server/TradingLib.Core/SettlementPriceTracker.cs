﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;


namespace TradingLib.Core
{
    /// <summary>
    /// 以结算日为一组
    /// </summary>
    public class SettlementPriceTracker
    {
        /// <summary>
        /// 按交易日 建立结算价map
        /// </summary>
        Dictionary<int, Dictionary<string, MarketData>> settlementPriceMap = new Dictionary<int, Dictionary<string, MarketData>>();

        Dictionary<string, Tick> lastticksnapshot = new Dictionary<string, Tick>();
        /// <summary>
        /// 从数据库加载某个结算日的计算机信息
        /// </summary>
        /// <param name="settleday"></param>
        public void LoadSettlementPrice(int settleday)
        {
            if (!settlementPriceMap.Keys.Contains(settleday))
            {
                settlementPriceMap.Add(settleday, new Dictionary<string, MarketData>());
            }
            foreach (var price in ORM.MSettlement.SelectMarketData(settleday))
            {

                if (!settlementPriceMap[settleday].Keys.Contains(price.Symbol))
                {
                    settlementPriceMap[settleday].Add(price.Symbol, price);
                }

                UpdateLastTickSnapshot(price);
            }
        }

        /// <summary>
        /// 清空结算价缓存
        /// </summary>
        public void Clear()
        {
            settlementPriceMap.Clear();
        }

        /// <summary>
        /// 清空某个交易日的结算数据
        /// </summary>
        /// <param name="tradingday"></param>
        public void Clear(int tradingday)
        {
            if (settlementPriceMap.Keys.Contains(tradingday))
            {
                settlementPriceMap[tradingday].Clear();
            }
        }

        public IEnumerable<MarketData> this[int settleday]
        {
            get
            {
                if (!settlementPriceMap.Keys.Contains(settleday))
                {
                    settlementPriceMap.Add(settleday, new Dictionary<string, MarketData>());
                }
                return settlementPriceMap[settleday].Values;
            }
        }

        public Tick GetLastTickSnapshot(string symbol)
        {
            Tick target = null;
            if (lastticksnapshot.TryGetValue(symbol, out target))
            {
                return target;
            }
            return null;
        }

        //public int Count
        //{
        //    get
        //    {
        //        return settlementPriceMap.Count;
        //    }
        //}
        /// <summary>
        /// 获得某个合约的结算价信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public MarketData this[int settleday,string symbol]
        {
            get
            {
                MarketData target = null;
                if (!settlementPriceMap.Keys.Contains(settleday))
                {
                    settlementPriceMap.Add(settleday, new Dictionary<string, MarketData>());
                }
                if (settlementPriceMap[settleday].TryGetValue(symbol, out target))
                {
                    return target;
                }
                return null;
            }
        }

        void UpdateLastTickSnapshot(MarketData price)
        {
            //更新最新行情快照
            if (lastticksnapshot.Keys.Contains(price.Symbol))
            {
                if (lastticksnapshot[price.Symbol].Date <= price.SettleDay)
                {
                    lastticksnapshot[price.Symbol] = price.ToTick();
                }
            }
            else
            {
                lastticksnapshot.Add(price.Symbol, price.ToTick());
            }
        }
        /// <summary>
        /// 更新结算价信息
        /// </summary>
        /// <param name="price"></param>
        public void UpdateSettlementPrice(MarketData price)
        {
            MarketData target = null;

            
            //结算价信息已经存在 更新结算价
            if (!settlementPriceMap.Keys.Contains(price.SettleDay))
            {
                settlementPriceMap.Add(price.SettleDay, new Dictionary<string, MarketData>());
            }
            if (settlementPriceMap[price.SettleDay].TryGetValue(price.Symbol, out target))
            {
                target.Settlement = price.Settlement;
                ORM.MSettlement.UpdateMarketData(target);//更新到数据库

            }
            else
            {
                target = price;
                //插入数据库记录
                ORM.MSettlement.InsertMarketData(target);
                //放到缓存
                settlementPriceMap[price.SettleDay].Add(target.Symbol, target);
            }
        }
    }
}