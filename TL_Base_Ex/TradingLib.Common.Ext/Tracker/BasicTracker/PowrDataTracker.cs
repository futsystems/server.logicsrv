using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.ORM;

namespace TradingLib.Common
{
    /// <summary>
    /// 1.正常启动时 加载当天除权数据，一般情况没有除权数据
    /// 收盘时获得除权数据并保存到数据库，同时更新内存数据结构 执行除权操作
    /// 
    /// 2.手工结算时 重新加载某个交易日的除权数据 并支持编辑
    /// 
    /// </summary>
    public class PowerDataTracker
    {

        ConcurrentDictionary<string, PowerData> pdmap = new ConcurrentDictionary<string, PowerData>();

        public PowerDataTracker()
        {
            //获得当前交易日的所有除权数据放入内存中
            foreach (var pd in MPowerData.SelectPowerData(TLCtxHelper.ModuleSettleCentre.Tradingday))
            { 
                pdmap[pd.Symbol] = pd;
            }
        }

        /// <summary>
        /// 获取某个合约的除权数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public PowerData this[string symbol]
        {
            get
            {
                if (string.IsNullOrEmpty(symbol)) return null;
                PowerData target = null;
                if (pdmap.TryGetValue(symbol, out target))
                {
                    return target;
                }
                return null;

            }
        }

        /// <summary>
        /// 更新某个除权数据
        /// </summary>
        /// <param name="pd"></param>
        public void UpdatePowerData(PowerData pd)
        {
            PowerData target = null;
            if (pdmap.TryGetValue(pd.Symbol, out target))
            {
                target.Dividend = pd.Dividend;
                target.DonateShares = pd.DonateShares;
                target.RationeShares = pd.RationeShares;
                target.RationePrice = pd.RationePrice;
                ORM.MPowerData.UpdatePowerData(pd);
            }
            else
            {
                pdmap[pd.Symbol] = pd;
                ORM.MPowerData.InsertPowerData(pd);
                
            }
        }

        /// <summary>
        /// 删除某个除权数据
        /// </summary>
        /// <param name="pd"></param>
        public void DeletePowerData(PowerData pd)
        {
            PowerData target = null;
            pdmap.TryRemove(pd.Symbol, out target);
            ORM.MPowerData.DeletePowerData(pd);
        }

        /// <summary>
        /// 充值某个交易日的除权数据
        /// </summary>
        public void Clear(int settleday)
        {
            pdmap.Clear();
            ORM.MPowerData.DeletePowerData(settleday);
        }
    }
}
