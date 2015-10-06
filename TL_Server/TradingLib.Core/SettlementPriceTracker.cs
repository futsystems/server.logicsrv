﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;



namespace TradingLib.Core
{
    public class SettlementPriceTracker
    {
        Dictionary<string, MarketData> settlementPriceMap = new Dictionary<string, MarketData>();

        /// <summary>
        /// 从数据库加载某个结算日的计算机信息
        /// </summary>
        /// <param name="settleday"></param>
        public void LoadSettlementPrice(int settleday)
        {
            foreach (var price in ORM.MSettlement.SelectMarketData(settleday))
            {
                settlementPriceMap.Add(price.Symbol, price);
            }
        }

        /// <summary>
        /// 清空结算价缓存
        /// </summary>
        public void Clear()
        {
            settlementPriceMap.Clear();
        }

        public IEnumerable<MarketData> SettlementPrices
        {
            get
            {
                return settlementPriceMap.Values;
            }
        }

        public int Count
        {
            get
            {
                return settlementPriceMap.Count;
            }
        }
        /// <summary>
        /// 获得某个合约的结算价信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public MarketData this[string symbol]
        {
            get
            {
                MarketData target = null;
                if (settlementPriceMap.TryGetValue(symbol, out target))
                {
                    return target;
                }
                return null;
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
            if (settlementPriceMap.TryGetValue(price.Symbol, out target))
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
                settlementPriceMap.Add(target.Symbol, target);
            }
        }
    }
}