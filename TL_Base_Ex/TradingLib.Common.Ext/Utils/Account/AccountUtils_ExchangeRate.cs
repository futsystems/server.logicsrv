using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_ExchangeRate
    {
        /// <summary>
        /// 某个帐户计算某个品种的汇率换算值
        /// 将Sec对应的货币转换成Account对应的货币汇率换算系数
        /// 默认需要制定交易日,交易所分部结算时制定交易日
        /// 国内交易所结算后 美国交易所仍然在交易，但是当时使用的汇率是同一个，这里不区分美国结算日的汇率和中国结算日的汇率
        /// 统一按照当前结算日使用的汇率
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal GetExchangeRate(this IAccount account, SecurityFamily sec)
        {
            return account.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday,sec);
        }

        public static decimal GetExchangeRate(this IAccount account, CurrencyType currency)
        {
            return account.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, currency);
        }

        /// <summary>
        /// 获得某个币种转换成账户币种的汇率系数
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static decimal GetExchangeRate(this IAccount account, int settleday, CurrencyType currency)
        {
            if (currency == account.Currency) return 1;
            //帐户货币为主货币
            if (account.Currency == GlobalConfig.BaseCurrency)
            {
                //获得品种货币对应的汇率 返回中间汇率
                ExchangeRate secRate = account.Domain.GetExchangeRate(settleday, currency);
                if (secRate == null) return 1;//没有找到品种汇率 则默认返回1
                return secRate.IntermediateRate;
            }
            else
            {
                ExchangeRate secRate = account.Domain.GetExchangeRate(settleday, currency);
                ExchangeRate accRate = account.Domain.GetExchangeRate(settleday, account.Currency);
                if (secRate == null || accRate == null) return 1;
                //将品种货币换算成系统基础货币然后再换算成帐户货币
                return secRate.IntermediateRate / accRate.IntermediateRate;
            }
        }
        /// <summary>
        /// 获得某个交易日某个品种的本币汇率系数
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sec"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static decimal GetExchangeRate(this IAccount account,int settleday,SecurityFamily sec)
        { 
            //品种货币与帐户货币一直则返回1
            if (sec.Currency == account.Currency) return 1;


            //帐户货币为主货币
            if (account.Currency == GlobalConfig.BaseCurrency)
            {
                //获得品种货币对应的汇率 返回中间汇率
                ExchangeRate secRate = account.Domain.GetExchangeRate(settleday, sec.Currency);
                if (secRate == null) return 1;//没有找到品种汇率 则默认返回1
                return secRate.IntermediateRate;
            }
            else
            {
                ExchangeRate secRate = account.Domain.GetExchangeRate(settleday, sec.Currency);
                ExchangeRate accRate = account.Domain.GetExchangeRate(settleday, account.Currency);
                if (secRate == null || accRate == null) return 1;
                //将品种货币换算成系统基础货币然后再换算成帐户货币
                return secRate.IntermediateRate / accRate.IntermediateRate;
            }
        
        }
    }
}