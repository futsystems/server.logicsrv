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
            Util.Debug("get month xxxxxxxxxxxxxxxxxx:" + symbol.GetMonth(), QSEnumDebugLevel.INFO);
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
            decimal commissionrate = 0;
            if (f.IsEntryPosition)
            {
                commissionrate = f.oSymbol.EntryCommission;
            }
            //平仓
            else
            {
                //进行特殊手续费判定并设定对应的手续费费率
                //如果对应的合约是单边计费的或者有特殊计费方式的合约，则我们单独计算该部分费用,注这里还需要加入一个日内交易的判断,暂时不做(当前交易均为日内)
                //获得平仓手续费特例
                if (CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.Symbol), out commissionrate))
                {
                    //debug("合约:" + SymbolHelper.genSecurityCode(f.symbol) + "日内手续费费差异", QSEnumDebugLevel.MUST);
                }
                else//没有特殊费率参数,则为标准的出场费率
                {
                    commissionrate = f.oSymbol.ExitCommission;
                }
            }

            //计算标准手续费
            decimal basecommission = 0;
            if (commissionrate < 1)//百分比计算费率
                basecommission = commissionrate * f.xPrice * f.UnsignedSize * f.oSymbol.Multiple;
            else
                basecommission = commissionrate * f.UnsignedSize;

            //获得该帐户某个合约的手续费模板项
            CommissionTemplateItem item = account.GetCommissionTemplateItem(f.oSymbol);

            //没有设置手续费模板
            if (item == null)
            {
                return basecommission;
            }
            else
            {
                decimal templatecommission = item.CalCommission(f, f.OffsetFlag);
                if (item.ChargeType == QSEnumChargeType.Absolute)
                {
                    return templatecommission;
                }
                else
                {
                    return basecommission + templatecommission;
                }
                Util.Debug("basecommission:" + basecommission.ToString() + " templatecommission:" + templatecommission.ToString() + " commissionrate:" + commissionrate.ToString(),QSEnumDebugLevel.INFO);
            }
        }

        
    
    }
}