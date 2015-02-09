﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_ExStrategy
    {
        /// <summary>
        /// 获得交易账户的交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static ExStrategyTemplate GetExStrategyTemplate(this IAccount account)
        {
            return BasicTracker.ExStrategyTemplateTracker[account.ExStrategy_ID];
        }

        /// <summary>
        /// 获得账户交易参数
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static ExStrategy GetExStrategy(this IAccount account)
        {
            ExStrategyTemplate t = account.GetExStrategyTemplate();
            if (t != null)
                return t.ExStrategy;
            return null;
        }


        
 
    
    }
}