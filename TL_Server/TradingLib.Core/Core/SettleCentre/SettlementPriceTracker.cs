using System;
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
        Dictionary<string, SettlementPrice> settlementPriceMap = new Dictionary<string, SettlementPrice>();

        /// <summary>
        /// 从数据库加载某个结算日的计算机信息
        /// </summary>
        /// <param name="settleday"></param>
        public void LoadSettlementPrice(int settleday)
        { 
            foreach(var price in ORM.MSettlement.SelectSettlementPrice(settleday))
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

        public IEnumerable<SettlementPrice> SettlementPrices
        { 
            get
            {
                return settlementPriceMap.Values;
            }
        }
        /// <summary>
        /// 获得某个合约的结算价信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public SettlementPrice this[string symbol]
        {
            get
            {
                SettlementPrice target = null;
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
        public void UpdateSettlementPrice(SettlementPrice price)
        {
            SettlementPrice target = null;
            //结算价信息已经存在
            if (settlementPriceMap.TryGetValue(price.Symbol, out target))
            {
                target.Price = price.Price;
                ORM.MSettlement.UpdateSettlementPrice(target);//更新到数据库

            }
            else
            {
                target = new SettlementPrice();
                target.Price = price.Price;
                target.SettleDay = price.SettleDay;
                target.Symbol = price.Symbol;
                //插入数据库记录
                ORM.MSettlement.InsertSettlementPrice(target);
                //放到缓存
                settlementPriceMap.Add(target.Symbol, target);
            }
        }
    }
}
