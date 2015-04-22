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
        /// 这里需要按照账户设置的保证金计算规则进行计算
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

            decimal tprice = p.LastPrice;
            QSEnumMarginPrice s = account.GetParamMarginPriceType();
            switch (s)
            {
                case QSEnumMarginPrice.TradePrice:
                    tprice = p.LastPrice;
                    break;
                case QSEnumMarginPrice.OpenPrice:
                    tprice = p.AvgPrice;
                    break;
                default:
                    tprice = p.LastPrice;
                    break;
            }

            //其余品种保证金按照最新价格计算
            if (p.oSymbol.Margin <= 1)
            {
                //需要判断价格的有效性
                basemargin = p.UnsignedSize * tprice * p.oSymbol.Multiple * p.oSymbol.Margin;
            }
            else
                basemargin = p.oSymbol.Margin * p.UnsignedSize;

            //获得保证金模板
            MarginTemplateItem item = account.GetMarginTemplateItem(p.oSymbol);
            if (item == null) return basemargin;

            switch (item.ChargeType)
            {
                case QSEnumChargeType.Absolute:
                    return item.CalMargin(p, tprice);
                case QSEnumChargeType.Relative:
                    return basemargin + item.CalMargin(p, tprice);
                case QSEnumChargeType.Percent:
                    return basemargin * (1 + item.Percent);
                default:
                    return basemargin;
            }
        }

        /// <summary>
        /// 按合约 手数来计算保证金占用
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static decimal CalOrderMarginFrozen(this IAccount account, Symbol symbol, int size, QSEnumOffsetFlag offset = QSEnumOffsetFlag.OPEN)
        {
            if (offset != QSEnumOffsetFlag.OPEN) return 0;//开仓意外的委托不占用保证金，释放保证金
            size = Math.Abs(size);
            decimal basemarginfrozen = 0;
            decimal price = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Symbol);//获得某合约当前价格

            if (symbol.SecurityType == SecurityType.FUT)//期货资金需求计算
            {
                if (symbol.Margin < 1)
                {
                    //如果是按市值比例来确认保证金,而给出的当前市场价格<=0 则返回可开手数量为0
                    if (price <= 0)
                        basemarginfrozen = decimal.MaxValue;
                    basemarginfrozen = symbol.Margin * symbol.Multiple * price * size;
                }
                else
                {
                    basemarginfrozen = symbol.Margin * size;
                }
            }
            else if (symbol.SecurityType == SecurityType.OPT)//期权资金需求计算
            {
                basemarginfrozen = price * symbol.Multiple * size;
            }
            else if (symbol.SecurityType == SecurityType.INNOV)//异化资金需求计算
            {
                if (symbol.Margin > 0)
                {
                    basemarginfrozen = symbol.Margin + (symbol.ExtraMargin > 0 ? symbol.ExtraMargin : 0);
                }
                else
                {
                    basemarginfrozen = decimal.MaxValue;
                }
            }
            else
                basemarginfrozen = decimal.MaxValue;

            //获得保证金模板
            MarginTemplateItem item = account.GetMarginTemplateItem(symbol);
            if (item == null) return basemarginfrozen;

            switch (item.ChargeType)
            {
                case QSEnumChargeType.Absolute:
                    return item.CalMarginFrozen(symbol, size, price);
                case QSEnumChargeType.Relative:
                    return basemarginfrozen + item.CalMarginFrozen(symbol, size, price);
                case QSEnumChargeType.Percent:
                    return basemarginfrozen * (1 + item.Percent);
                default:
                    return basemarginfrozen;
            }

        }
        /// <summary>
        /// 计算委托的保证金冻结
        /// </summary>
        /// <param name="account"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static decimal CalOrderMarginFrozen(this IAccount account, Order o)
        {

            if (!o.IsEntryPosition) return 0;//平仓委托不冻结保证金

            decimal basemarginfrozen = 0;
            Symbol symbol = o.oSymbol;
            int size = o.UnsignedSize;
            decimal currentPrice = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Symbol);//获得某合约当前价格
            //取价算法 市委托 取得合约当前价格，限价或追价 取设定价格
            decimal price = o.isMarket ? currentPrice : (o.isLimit ? o.LimitPrice : o.StopPrice);

            if (symbol.SecurityType == SecurityType.FUT)//期货资金需求计算
            {
                if (symbol.Margin <= 1)
                {
                    //如果是按市值比例来确认保证金,而给出的当前市场价格<=0 则返回可开手数量为0
                    if (price <= 0)
                        basemarginfrozen= decimal.MaxValue;
                    basemarginfrozen = symbol.Margin * symbol.Multiple * price * size;
                }
                else
                {
                    basemarginfrozen = symbol.Margin * size;
                }
            }
            else if (symbol.SecurityType == SecurityType.OPT)//期权资金需求计算
            {
                basemarginfrozen =  price * symbol.Multiple * size;
            }
            else if (symbol.SecurityType == SecurityType.INNOV)//异化资金需求计算
            {
                if (symbol.Margin > 0)
                {
                    basemarginfrozen =  symbol.Margin + (symbol.ExtraMargin > 0 ? symbol.ExtraMargin : 0);
                }
                else
                {
                    basemarginfrozen =  decimal.MaxValue;
                }
            }
            else
                basemarginfrozen = decimal.MaxValue;


            //获得保证金模板
            MarginTemplateItem item = account.GetMarginTemplateItem(o.oSymbol);
            if (item == null) return basemarginfrozen;

            switch (item.ChargeType)
            {
                case QSEnumChargeType.Absolute:
                    return item.CalMarginFrozen(o, currentPrice);
                case QSEnumChargeType.Relative:
                    return basemarginfrozen + item.CalMarginFrozen(o, currentPrice);
                case QSEnumChargeType.Percent:
                    return basemarginfrozen * (1 + item.Percent);
                default:
                    return basemarginfrozen;
            }
        }

        /// <summary>
        /// 获得某个交易帐户 某个合约的保证金设置
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static MarginConfig GetMarginConfig(this IAccount account, Symbol symbol)
        {
            //获得合约的基础保证金设置
            MarginConfig cfg = symbol.GetMarginConfig();
            //获得帐户该合约的保证金模板项
            MarginTemplateItem item = account.GetMarginTemplateItem(symbol);
            if (item != null)
            {
                if (item.ChargeType == QSEnumChargeType.Absolute)
                {
                    cfg.LongMarginRatioByMoney = item.MarginByMoney;
                    cfg.LongMarginRatioByVolume = item.MarginByVolume;
                    cfg.ShortMarginRatioByMoney = item.MarginByMoney;
                    cfg.ShortMarginRatioByVoume = item.MarginByVolume;
                }
                else if (item.ChargeType == QSEnumChargeType.Relative)
                {
                    cfg.LongMarginRatioByMoney = cfg.LongMarginRatioByMoney == 0 ? 0 : cfg.LongMarginRatioByMoney + item.MarginByMoney;
                    cfg.LongMarginRatioByVolume = cfg.LongMarginRatioByVolume == 0 ? 0 : cfg.LongMarginRatioByVolume + item.MarginByVolume;
                    cfg.ShortMarginRatioByMoney = cfg.ShortMarginRatioByMoney == 0 ? 0 : cfg.ShortMarginRatioByMoney + item.MarginByMoney;
                    cfg.ShortMarginRatioByVoume = cfg.ShortMarginRatioByVoume == 0 ? 0 : cfg.ShortMarginRatioByVoume + item.MarginByVolume;
                }
                else if (item.ChargeType == QSEnumChargeType.Percent)
                {
                    cfg.LongMarginRatioByMoney = cfg.LongMarginRatioByMoney * (1 + item.Percent);
                    cfg.LongMarginRatioByVolume = cfg.LongMarginRatioByVolume * (1 + item.Percent);
                    cfg.ShortMarginRatioByMoney = cfg.ShortMarginRatioByMoney * (1 + item.Percent);
                    cfg.ShortMarginRatioByVoume = cfg.ShortMarginRatioByVoume * (1 + item.Percent);
                }
            }
            cfg.Account = account.ID;
            return cfg;
        }
    
    
    }
}