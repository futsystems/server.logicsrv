using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 手续费与保证金扩展函数类
    /// </summary>
    public static class AccountUtils_CommissionMargin
    {
        public static CommissionTemplate GetCommissionTemplate(this IAccount account)
        {
            return BasicTracker.CommissionTemplateTracker[account.Commission_ID];
        }

        public static CommissionTemplateItem GetCommissionTemplateItem(this IAccount account,Symbol symbol)
        {
            CommissionTemplate tmp = account.GetCommissionTemplate();
            if (tmp == null)
                return null;
            return tmp[symbol.SecurityFamily.Code, symbol.GetMonth()];
        }


        /// <summary>
        /// 计算某个成交的手续费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalCommission(this IAccount account, Trade f)
        {
            //获得该帐户某个合约的手续费模板项
            CommissionTemplateItem item = account.GetCommissionTemplateItem(f.oSymbol);

            //调用合约的扩展函数 计算获得对应的手续费率
            decimal commissionrate = f.oSymbol.GetCommissionRate(item, f.OffsetFlag);

            //计算标准手续费
            decimal c = 0;
            if (commissionrate < 1)//百分比计算费率
                c = commissionrate * f.xPrice * f.UnsignedSize * f.oSymbol.Multiple;
            else
                c = commissionrate * f.UnsignedSize;
            return c;
        }

        
    
    }
}