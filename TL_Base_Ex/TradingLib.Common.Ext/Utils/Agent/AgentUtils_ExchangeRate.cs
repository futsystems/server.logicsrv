using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class AgentUtils_ExchangeRate
    {
        /// <summary>
        /// 获得某个币种转换成账户币种的汇率系数
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static decimal GetExchangeRate(this AgentImpl agent, int settleday, CurrencyType currency)
        {
            if (currency == agent.Currency) return 1;
            //帐户货币为主货币
            if (agent.Currency == GlobalConfig.BaseCurrency)
            {
                //获得品种货币对应的汇率 返回中间汇率
                ExchangeRate secRate = agent.Manager.Domain.GetExchangeRate(settleday, currency);
                if (secRate == null) return 1;//没有找到品种汇率 则默认返回1
                return secRate.IntermediateRate;
            }
            else
            {
                ExchangeRate secRate = agent.Manager.Domain.GetExchangeRate(settleday, currency);
                ExchangeRate accRate = agent.Manager.Domain.GetExchangeRate(settleday, agent.Currency);
                if (secRate == null || accRate == null) return 1;
                //将品种货币换算成系统基础货币然后再换算成帐户货币
                return secRate.IntermediateRate / accRate.IntermediateRate;
            }
        }
    }
}
