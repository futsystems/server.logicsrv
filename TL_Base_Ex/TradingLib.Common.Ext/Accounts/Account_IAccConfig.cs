using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        /// <summary>
        /// 获得某个交易帐户某个合约的手续费设置
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            //初始合约手续费设置
            CommissionConfig cfg = symbol.GetCommissionConfig();
            CommissionTemplateItem item = this.GetCommissionTemplateItem(symbol);
            //如果手续费模板项目不为空则需要按照模板调整收费率
            if (item != null)
            {
                if (item.ChargeType == QSEnumChargeType.Absolute)
                {
                    cfg.OpenRatioByMoney = item.OpenByMoney;
                    cfg.OpenRatioByVolume = item.OpenByVolume;
                    cfg.CloseRatioByMoney = item.CloseByMoney;
                    cfg.CloseRatioByVolume = item.CloseByVolume;
                    cfg.CloseTodayRatioByMoney = item.CloseTodayByMoney;
                    cfg.CloseTodayRatioByVolume = item.CloseTodayByVolume;
                }
                else if (item.ChargeType == QSEnumChargeType.Relative)
                {
                    cfg.OpenRatioByMoney = cfg.OpenRatioByMoney == 0 ? 0 : cfg.OpenRatioByMoney + item.OpenByMoney;
                    cfg.OpenRatioByVolume = cfg.OpenRatioByVolume == 0 ? 0 : cfg.OpenRatioByVolume + item.OpenByVolume;
                    cfg.CloseRatioByMoney = cfg.CloseRatioByMoney == 0 ? 0 : cfg.CloseRatioByMoney + item.CloseByMoney;
                    cfg.CloseRatioByVolume = cfg.CloseRatioByVolume == 0 ? 0 : cfg.CloseRatioByVolume + item.CloseByVolume;
                    cfg.CloseTodayRatioByMoney = cfg.CloseTodayRatioByMoney == 0 ? 0 : cfg.CloseTodayRatioByMoney + item.CloseTodayByMoney;
                    cfg.CloseTodayRatioByVolume = cfg.CloseTodayRatioByVolume == 0 ? 0 : cfg.CloseTodayRatioByVolume + item.CloseTodayByVolume;
                }
                else if(item.ChargeType == QSEnumChargeType.Percent)
                {
                    cfg.OpenRatioByMoney = cfg.OpenRatioByMoney*(1 + item.Percent);
                    cfg.OpenRatioByVolume = cfg.OpenRatioByVolume * (1 + item.Percent);
                    cfg.CloseRatioByMoney = cfg.CloseRatioByMoney * (1 + item.Percent);
                    cfg.CloseRatioByVolume = cfg.CloseRatioByVolume * (1 + item.Percent);
                    cfg.CloseTodayRatioByMoney = cfg.CloseTodayRatioByMoney * (1 + item.Percent);
                    cfg.CloseTodayRatioByVolume = cfg.CloseTodayRatioByVolume * (1 + item.Percent);
                }
            }
            cfg.Account = this.ID;
            return cfg;
            
        }
    }
}
