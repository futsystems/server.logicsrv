﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AgentUtil
    {
        /// <summary>
        /// 获得帐户设定的手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        static CommissionTemplate GetCommissionTemplate(this IAgent agent)
        {
            return BasicTracker.CommissionTemplateTracker[agent.Commission_ID];
        }

        /// <summary>
        /// 获得帐户设定的保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        static MarginTemplate GetMarginTemplate(this IAgent agent)
        {
            return BasicTracker.MarginTemplateTracker[agent.Margin_ID];
        }


        /// <summary>
        /// 获得某个合约的手续费项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        static CommissionTemplateItem GetCommissionTemplateItem(this IAgent agent, Symbol symbol)
        {
            CommissionTemplate tmp = agent.GetCommissionTemplate();
            if (tmp == null)
                return null;
            return tmp.GetCommissionItem(symbol);
        }



        static MarginTemplateItem GetMarginTemplateItem(this IAgent agent, Symbol symbol)
        {
            MarginTemplate tmp = agent.GetMarginTemplate();
            if (tmp == null)
                return null;
            return tmp[symbol.SecurityFamily.Code, symbol.GetMonth()];

        }


        #region 计算手续费 保证金等数据
        /// <summary>
        /// 计算某个成交的手续费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalCommission(this IAgent agent, Trade f)
        {
            //股票通过交易手续费计算
            if (f.oSymbol.SecurityFamily.Type == SecurityType.STK)
            {
                CommissionTemplate template = agent.GetCommissionTemplate();
                return f.GetAmount() * (template == null ? GlobalConfig.STKCommissionRate : template.STKCommissioinRate / 10000);
            }


            //计算标准手续费
            decimal basecommission = f.oSymbol.CalcBaseCommission(f);

            //获得该帐户某个合约对应的手续费计算策略
            CommissionTemplateItem item = agent.GetCommissionTemplateItem(f.oSymbol);

            //没有设置手续费模板
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
        /// 计算印花税
        /// </summary>
        /// <param name="account"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalcStampTax(this IAgent agent, Trade f)
        {
            //股票计算印花税
            if (f.oSymbol.SecurityFamily.Type == SecurityType.STK)
            {
                CommissionTemplate template = agent.GetCommissionTemplate();
                //平仓才收取印花税
                if (!f.IsEntryPosition)
                {
                    return f.GetAmount() * (template == null ? GlobalConfig.STKStampTaxRate : template.STKStampTaxRate / 10000);
                }
            }
            return 0;
        }

        /// <summary>
        /// 计算过户费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal CalcTransferFee(this IAgent agent, Trade f)
        {
            if (f.oSymbol.SecurityFamily.Type == SecurityType.STK)
            {
                CommissionTemplate template = agent.GetCommissionTemplate();
                //每1000股收取1元,不足1元按1元收取
                int t = (f.UnsignedSize / 1000) + f.UnsignedSize % 1000 > 0 ? 1 : 0;
                return t * (template == null ? GlobalConfig.STKTransferFee : template.STKTransferFee);
            }
            return 0;
        }

        ///// <summary>
        ///// 计算持仓保证金
        ///// 这里需要按照账户设置的保证金计算规则进行计算
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="f"></param>
        ///// <returns></returns>
        //public static decimal CalPositionMargin(this IAccount account, Position p)
        //{
        //    //计算基准保证金
        //    decimal basemargin = 0;

        //    decimal tprice = p.LastPrice;
        //    QSEnumMarginPrice s = account.GetParamMarginPriceType();
        //    switch (s)
        //    {
        //        case QSEnumMarginPrice.TradePrice:
        //            tprice = p.LastPrice;
        //            break;
        //        case QSEnumMarginPrice.OpenPrice:
        //            tprice = p.AvgPrice;
        //            break;
        //        default:
        //            tprice = p.LastPrice;
        //            break;
        //    }

        //    //其余品种保证金按照最新价格计算
        //    if (p.oSymbol.Margin <= 1)
        //    {
        //        //需要判断价格的有效性
        //        basemargin = p.UnsignedSize * tprice * p.oSymbol.Multiple * p.oSymbol.Margin;
        //    }
        //    else
        //        basemargin = p.oSymbol.Margin * p.UnsignedSize;

        //    //获得保证金模板
        //    MarginTemplateItem item = account.GetMarginTemplateItem(p.oSymbol);
        //    if (item == null) return basemargin;

        //    switch (item.ChargeType)
        //    {
        //        case QSEnumChargeType.Absolute:
        //            return item.CalMargin(p, tprice);
        //        case QSEnumChargeType.Relative:
        //            return basemargin + item.CalMargin(p, tprice);
        //        case QSEnumChargeType.Percent:
        //            return basemargin * (1 + item.Percent);
        //        default:
        //            return basemargin;
        //    }
        //}

        ///// <summary>
        ///// 按合约 手数来计算保证金占用
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="symbol"></param>
        ///// <param name="size"></param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //public static decimal CalOrderMarginFrozen(this IAccount account, Symbol symbol, int size, QSEnumOffsetFlag offset = QSEnumOffsetFlag.OPEN)
        //{
        //    if (offset != QSEnumOffsetFlag.OPEN) return 0;//开仓意外的委托不占用保证金，释放保证金
        //    size = Math.Abs(size);
        //    decimal price = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Exchange, symbol.Symbol);//获得某合约当前价格
        //    switch (symbol.SecurityType)
        //    {
        //        case SecurityType.FUT:
        //            {
        //                decimal basemarginfrozen = 0;

        //                if (symbol.Margin < 1)
        //                {
        //                    //如果是按市值比例来确认保证金,而给出的当前市场价格<=0 则返回可开手数量为0
        //                    if (price <= 0)
        //                        basemarginfrozen = decimal.MaxValue;
        //                    basemarginfrozen = symbol.Margin * symbol.Multiple * price * size;
        //                }
        //                else
        //                {
        //                    basemarginfrozen = symbol.Margin * size;
        //                }
        //                //获得保证金模板
        //                MarginTemplateItem item = account.GetMarginTemplateItem(symbol);
        //                if (item == null) return basemarginfrozen;

        //                switch (item.ChargeType)
        //                {
        //                    case QSEnumChargeType.Absolute:
        //                        return item.CalMarginFrozen(symbol, size, price);
        //                    case QSEnumChargeType.Relative:
        //                        return basemarginfrozen + item.CalMarginFrozen(symbol, size, price);
        //                    case QSEnumChargeType.Percent:
        //                        return basemarginfrozen * (1 + item.Percent);
        //                    default:
        //                        return basemarginfrozen;
        //                }
        //            }
        //        case SecurityType.STK:
        //            if (price <= 0) return decimal.MaxValue;
        //            return size * price;
        //        default:
        //            return decimal.MaxValue;

        //    }
        //}

        ///// <summary>
        ///// 计算委托的保证金冻结
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="o"></param>
        ///// <returns></returns>
        //public static decimal CalOrderMarginFrozen(this IAccount account, Order o)
        //{

        //    if (!o.IsEntryPosition) return 0;//平仓委托不冻结保证金

        //    decimal basemarginfrozen = 0;
        //    Symbol symbol = o.oSymbol;
        //    int size = o.UnsignedSize;
        //    decimal currentPrice = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Exchange, symbol.Symbol);//获得某合约当前价格
        //    //取价算法 市委托 取得合约当前价格，限价或追价 取设定价格
        //    decimal price = o.isMarket ? currentPrice : (o.isLimit ? o.LimitPrice : o.StopPrice);

        //    if (symbol.SecurityType == SecurityType.FUT)//期货资金需求计算
        //    {
        //        if (symbol.Margin <= 1)
        //        {
        //            //如果是按市值比例来确认保证金,而给出的当前市场价格<=0 则返回可开手数量为0
        //            if (price <= 0)
        //                basemarginfrozen = decimal.MaxValue;
        //            basemarginfrozen = symbol.Margin * symbol.Multiple * price * size;
        //        }
        //        else
        //        {
        //            basemarginfrozen = symbol.Margin * size;
        //        }
        //    }
        //    else if (symbol.SecurityType == SecurityType.OPT)//期权资金需求计算
        //    {
        //        basemarginfrozen = price * symbol.Multiple * size;
        //    }
        //    else
        //        basemarginfrozen = decimal.MaxValue;


        //    //获得保证金模板
        //    MarginTemplateItem item = account.GetMarginTemplateItem(o.oSymbol);
        //    if (item == null) return basemarginfrozen;

        //    switch (item.ChargeType)
        //    {
        //        case QSEnumChargeType.Absolute:
        //            return item.CalMarginFrozen(o, currentPrice);
        //        case QSEnumChargeType.Relative:
        //            return basemarginfrozen + item.CalMarginFrozen(o, currentPrice);
        //        case QSEnumChargeType.Percent:
        //            return basemarginfrozen * (1 + item.Percent);
        //        default:
        //            return basemarginfrozen;
        //    }
        //}

        ///// <summary>
        ///// 计算某个委托所要占用的资金
        ///// 这里的计算与单纯计算某个委托需要占用的保证金有所不同，这里需要按照
        ///// 保证金计算算法 试算该委托下达后所增加的保证金占用 包含单向大边的处理
        ///// </summary>
        ///// <param name="o"></param>
        ///// <returns></returns>
        //public static decimal CalOrderFundRequired(this IAccount account, Order o, decimal defaultvalue = 0)
        //{
        //    switch (o.oSymbol.SecurityType)
        //    {
        //        case SecurityType.FUT:
        //            {
        //                //需要判断是否启用单向大边
        //                if (!account.GetParamSideMargin())
        //                {
        //                    return account.CalOrderMarginFrozen(o) * account.GetExchangeRate(o.oSymbol.SecurityFamily);
        //                }
        //                else
        //                {
        //                    decimal marginfrozennow = account.CalFutMarginSet().Sum(ms => ms.MarginFrozen * account.GetExchangeRate(ms.Security));
        //                    //将当前委托纳入待成交委托集，然后按单向大边规则计算冻结保证金
        //                    decimal marginfrozenwill = account.CalFutMarginSet(o).Sum(ms => ms.MarginFrozen * account.GetExchangeRate(ms.Security));
        //                    return marginfrozenwill - marginfrozennow;//纳入开仓委托的单向大边冻结保证金 - 当前冻结保证金 为该委托所需冻结保证金
        //                }
        //            }
        //        //股票占用资金为报单价格*数量
        //        case SecurityType.STK:
        //            {
        //                return o.LimitPrice * o.UnsignedSize;
        //            }
        //        default:
        //            return decimal.MaxValue;
        //    }
        //}

        #endregion

    }
}
