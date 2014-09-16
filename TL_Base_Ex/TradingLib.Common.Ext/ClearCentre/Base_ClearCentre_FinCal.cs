using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {
        #region 【IFinanceCaculation】【多品种财务数据计算】

        /// <summary>
        /// 获得某个帐户的某种交易集合
        /// </summary>
        /// <param name="acc"></param>
        protected Trade[] FilterTrades(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetTradeBook(acc.ID).Where(t => t.SecurityType == type).ToArray();
            }
            catch (Exception ex)
            {
                debug("filtertrades error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Trade[] { };
            }
        }

        /// <summary>
        /// 获得某个帐户的所有证券类委托集合
        /// </summary>
        /// <param name="acc"></param>
        protected Order[] FilterOrder(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetOrderBook(acc.ID).ToArray().Where(o => o.SecurityType == type).ToArray();
            }
            catch (Exception ex)
            {
                debug("filterorder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Order[] { };
            }
        }

        /// <summary>
        /// 过滤某个帐户某个证券类别下的待成交委托,Placed submited ,Opened,PartFilled
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Order[] FilterPendingOrder(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetOrderBook(acc.ID).ToArray().Where(delegate(Order o) { return o.SecurityType == type && OrderTracker.IsPending(o); }).ToArray();
            }
            catch (Exception ex)
            {
                debug("filterpendingorder error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Order[] { };
            }
        }

        /// <summary>
        /// 获得某个帐户所有商品类持仓
        /// </summary>
        /// <param name="acc"></param>
        protected Position[] FilterPosition(IAccount acc, SecurityType type)
        {
            try
            {
                return acctk.GetPositionBook(acc.ID).ToArray().Where(p => p.oSymbol.SecurityType == type).ToArray();
            }
            catch (Exception ex)
            {
                debug("filterposition error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return new Position[] { };
            }

        }

        /// <summary>
        /// 计算某个帐户的期货保证金占用
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutMargin(IAccount acc)
        {
            try
            {
                return FilterPosition(acc, SecurityType.FUT).Sum(pos=>pos.CalcPositionMargin());
            }
            catch (Exception ex)
            {
                debug("callfutmargin error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
            return 0;
        }

        /// <summary>
        /// 计算期货冻结保证金
        /// 将PendingOrder计算资金占用
        /// 这里的资金占用将所有委托都算入了资金占用,如果存在方向相反的委托应该如何处理？
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutMarginFrozen(IAccount acc)
        {
            try
            {
                //期货冻结保证金 为计算开仓的冻结保证金
                return FilterPendingOrder(acc, SecurityType.FUT).Where(o=>o.IsEntryPosition).Sum(e => acc.CalOrderFundRequired(e, 0));
            }
            catch (Exception ex)
            {
                debug("callfutmarginfronzen error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return decimal.MaxValue;
            }
        }
        /// <summary>
        /// 计算某个帐户的期货浮动盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutUnRealizedPL(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.FUT).Sum(pos=>pos.CalcUnRealizedPL());
        }

        /// <summary>
        /// 计算某个帐户的期货盯市盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutSettleUnRealizedPL(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.FUT).Sum(pos=>pos.CalcSettleUnRealizedPL());
        }
        /// <summary>
        /// 计算某个帐户的期货平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutRealizedPL(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.FUT).Sum(pos=>pos.CalcRealizedPL());
        }

        /// <summary>
        /// 计算某个帐户的期货交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalFutCommission(IAccount acc)
        {
            return FilterTrades(acc, SecurityType.FUT).Sum(fill=>fill.GetCommission());
        }

        #region opt计算
        /// <summary>
        /// 计算某个帐户期权的持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptPositionCost(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.OPT).Sum(pos=>pos.CalcPositionCost());
        }

        /// <summary>
        /// 计算某个帐户期权的持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptPositionValue(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.OPT).Sum(pos=>pos.CalcPositionValue());
        }

        /// <summary>
        /// 计算期权结算时的结算市价值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptSettlePositionValue(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.OPT).Sum(pos=>pos.CalcSettlePositionValue());
        }

        /// <summary>
        /// 计算某个帐户期权平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptRealizedPL(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.OPT).Sum(pos=>pos.CalcRealizedPL());
        }

        /// <summary>
        /// 计算某个帐户的期权交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptCommission(IAccount acc)
        {
            return FilterTrades(acc, SecurityType.OPT).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 计算期权资金冻结
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalOptMoneyFrozen(IAccount acc)
        {
            try
            {
                return FilterPendingOrder(acc, SecurityType.OPT).Sum(e => acc.CalOrderFundRequired(e, 0));
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }
        #endregion

        #region INNOV

        /// <summary>
        /// 计算异化合约持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovPositionCost(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.INNOV).Sum(pos=>pos.CalcPositionCost());
        }

        /// <summary>
        /// 计算异化合约持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovPositionValue(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.INNOV).Sum(pos=>pos.CalcPositionValue());
        }

        /// <summary>
        /// 计算异化合约的持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovSettlePositionValue(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.INNOV).Sum(pos=>pos.CalcSettlePositionValue());
        }

        /// <summary>
        /// 计算异化合约收学费统计
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovCommission(IAccount acc)
        {
            return FilterTrades(acc, SecurityType.INNOV).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 计算异化合约的平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovRealizedPL(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.INNOV).Sum(pos=>pos.CalcRealizedPL());
        }

        /// <summary>
        /// 计算异化合约的保证金
        /// 在正常的期权或者股票交易过程中 资金占用就是实际的购入成本
        /// 但是由于异化合约是底层合约的封装,实际购买的是底层合约，但是和客户结算时是按异化合约的保证金数据进行计算
        /// 所以客户可以用低保证金来享受到底层合约的价格波动
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovMargin(IAccount acc)
        {
            return FilterPosition(acc, SecurityType.INNOV).Sum(pos=>pos.CalcPositionMargin());
        }


        /// <summary>
        /// 计算异化合约的保证金占用 比如挂单,防止避免保证金计算穿越
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public decimal CalInnovMarginFrozen(IAccount acc)
        {
            decimal m = 0;
            try
            {
                return FilterPendingOrder(acc, SecurityType.INNOV).Sum(e => acc.CalOrderFundRequired(e, 0));
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }
        #endregion


        /// <summary>
        /// 计算某个委托所需要占用的保证金
        /// 下单方向与原持仓位委托方向一致(开仓/增仓)某个委托当前所占用的保证金
        /// 价格在正常报价范围内,则用指定价格计算冻结保证金,如果在非正常范围则按照当前最新价格计算保证金
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public decimal CalOrderFundRequired(Order o, decimal mktMvalue = decimal.MaxValue)
        {
            Symbol symbol = o.oSymbol;
            decimal price = 0;
            price = GetAvabilePrice(symbol.TickSymbol);

            //期权委托资金占用计算
            if (symbol.SecurityType == SecurityType.OPT)
            {
                if (price < 0)
                    return mktMvalue;

                if (Math.Abs(o.price - price) / price > 0.1M)
                    return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                return Calc.CalFundRequired(symbol, o.stopp, o.UnsignedSize);//o.UnsignedSize * o.stopp * symbol.Margin * symbol.Multiple;
            }

            //期货资金占用计算
            if (symbol.SecurityType == SecurityType.FUT)
            {
                //市价委托用当前的市场价格来计算保证金占用
                if (symbol.Margin <= 1)
                {
                    //debug("Orderid:" + o.id.ToString() + " Margin:" + symbol.Margin.ToString() + " price:" + price.ToString() + " mktvalue:" + mktMvalue.ToString(), QSEnumDebugLevel.INFO);
                    if (price < 0)
                        return mktMvalue;
                    //debug(PROGRAME + ":"+sec.ToString()+" margin:"+sec.Margin.ToString(), QSEnumDebugLevel.DEBUG);
                    if (o.isMarket)
                    {
                        return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                    }
                    //限价委托用限定价格计算保证金占用
                    if (o.isLimit)
                    {

                        if (Math.Abs(o.price - price) / price > 0.1M)//如果价格偏差在10以外 则以当前的价格来计算保证金 10%以内则以 设定的委托价格来计算保证金
                            return Calc.CalFundRequired(symbol, price, o.UnsignedSize);//o.unsignedSize标识剩余委托数量来求保证金占用size为0的委托 保证金占用为0 这里不是按totalsize来进行的
                        return Calc.CalFundRequired(symbol, o.price, o.UnsignedSize);
                    }
                    //追价委托用追价价格计算保证金占用
                    if (o.isStop)
                    {
                        if (Math.Abs(o.stopp - price) / price > 0.1M)
                            return Calc.CalFundRequired(symbol, price, o.UnsignedSize);
                        return Calc.CalFundRequired(symbol, o.stopp, o.UnsignedSize);//o.UnsignedSize * o.stopp * symbol.Margin * symbol.Multiple;
                    }
                    else
                        //如果便利的委托类型未知 则发挥保证金为最大
                        return decimal.MaxValue;


                }
                else
                    return symbol.Margin * o.UnsignedSize;//固定金额保证金计算 手数×保证金额度 = 总保证金额度
            }

            //异化合约资金占用
            if (symbol.SecurityType == SecurityType.INNOV)
            {
                if (symbol.Margin > 0)
                {
                    return (symbol.Margin + (symbol.ExtraMargin > 0 ? symbol.ExtraMargin : 0)) * o.UnsignedSize;
                }
                else
                {
                    return decimal.MaxValue;
                }
            }
            return decimal.MaxValue;
        }

        #endregion

    }
}
