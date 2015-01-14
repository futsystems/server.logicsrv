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
            CommissionTemplateItem item = account.GetCommissionTemplateItem(f.oSymbol);
            //计算对应的手续费率 可以是按金额，也可以是按手数
            decimal commissionrate = 0;
            if (item == null)
            {
                //包含手续费模板设置
                if (f.IsEntryPosition)
                {
                    commissionrate = f.oSymbol.EntryCommission + f.oSymbol.EntryCommission > 1 ? item.OpenByVolume : item.OpenByMoney;
                }
                //平仓
                else
                {
                    //检查手续费优惠项目
                    //if (!CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.Symbol), out commissionrate))
                    //{
                    //    commissionrate = f.oSymbol.ExitCommission;
                    //}
                    commissionrate = f.oSymbol.ExitCommission+f.oSymbol.ExitCommission>1?item.CloseByVolume:item.CloseByMoney;
                }
            }
            else//没有手续费模板设置 则使用默认
            {
                
                if (f.IsEntryPosition)
                {
                    commissionrate = f.oSymbol.EntryCommission;
                }
                //平仓
                else
                {
                    //检查手续费优惠项目
                    if (!CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.Symbol), out commissionrate))
                    {
                        commissionrate = f.oSymbol.ExitCommission;
                    }
                }
            }

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