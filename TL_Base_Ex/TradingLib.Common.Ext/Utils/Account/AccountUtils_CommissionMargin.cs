﻿using System;
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

        public static MarginTemplate GetMarginTemplate(this IAccount account)
        {
            return BasicTracker.MarginTemplateTracker[account.Margin_ID];
        }

        public static CommissionTemplateItem GetCommissionTemplateItem(this IAccount account,Symbol symbol)
        {
            CommissionTemplate tmp = account.GetCommissionTemplate();
            if (tmp == null)
                return null;
            return tmp[symbol.SecurityFamily.Code, symbol.GetMonth()];
        }

        public static MarginTemplateItem GetMarginTemplateItem(this IAccount account, Symbol symbol)
        {
            MarginTemplate tmp = account.GetMarginTemplate();
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
                switch (item.ChargeType)
                {
                    case QSEnumChargeType.Absolute:
                        return item.CalCommission(f, f.OffsetFlag);
                    case QSEnumChargeType.Relative:
                        return basecommission + item.CalCommission(f, f.OffsetFlag);
                    case QSEnumChargeType.Percent:
                        return basecommission * (1 + item.Percent);
                    default:
                        return basecommission;
                }
            }
        }

        /// <summary>
        /// 计算持仓保证金
        /// </summary>
        /// <param name="account"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalPositionMargin(this IAccount account, Position p)
        {
            //计算基准保证金
            decimal basemargin = 0;
            //异化合约按照固定金额来计算
            if (p.oSymbol.SecurityType == SecurityType.INNOV)
            {
                basemargin =  p.UnsignedSize * (p.oSymbol.Margin + (p.oSymbol.ExtraMargin > 0 ? p.oSymbol.ExtraMargin : 0));//通过固定保证金来计算持仓保证金占用
            }

            //其余品种保证金按照最新价格计算
            if (p.oSymbol.Margin <= 1)
            {
                //需要判断价格的有效性
                basemargin = p.UnsignedSize * p.LastPrice * p.oSymbol.Multiple * p.oSymbol.Margin;
            }
            else
                basemargin = p.oSymbol.Margin * p.UnsignedSize;

            //获得保证金模板
            MarginTemplateItem item = account.GetMarginTemplateItem(p.oSymbol);
            if (item == null) return basemargin;

            switch (item.ChargeType)
            {
                case QSEnumChargeType.Absolute:
                    return item.CalMargin(p,p.LastPrice);
                case QSEnumChargeType.Relative:
                    return basemargin + item.CalMargin(p, p.LastPrice);
                case QSEnumChargeType.Percent:
                    return basemargin * (1 + item.Percent);
                default:
                    return basemargin;
            }
        }
    
    }
}