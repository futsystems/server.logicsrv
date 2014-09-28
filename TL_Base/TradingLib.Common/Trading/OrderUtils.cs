using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class OrderUtils
    {
        /// <summary>
        /// 判断委托是否处于pending状态
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsPending(this Order o)
        {
            if (o.Status == QSEnumOrderStatus.Opened || o.Status == QSEnumOrderStatus.PartFilled || o.Status == QSEnumOrderStatus.Submited || o.Status == QSEnumOrderStatus.Placed)
                return true;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o">委托</param>
        /// <param name="price">当前市场价格</param>
        /// <param name="defaultfundrequired">默认返回的资金需求</param>
        /// <returns></returns>
        public static decimal CalFundRequired(this Order o,decimal price,decimal defaultfundrequired=0)
        {
            Symbol symbol = o.oSymbol;
            //期权委托资金占用计算
            if (symbol.SecurityType == SecurityType.OPT)
            {
                if (price < 0)
                    return defaultfundrequired;

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
                        return defaultfundrequired;
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



        public static string GetOrderInfo(this Order o)
        { 
            //123342 953 [INFO] BrokerRouter:Reply Order To MessageExch |BUY 1 IF1409 @Mkt [9280007] 635474179608593751 Filled:0 Status:Submited PostFlag:OPEN OrderRef: OrderSeq:1011 HedgeFlag: OrderExchID:
            StringBuilder sb = new StringBuilder();
            sb.Append(o.side?"Buy":"Sell");
            sb.Append(" "+o.OffsetFlag.ToString());
            sb.Append(" "+Math.Abs(o.TotalSize).ToString());
            sb.Append(" "+o.symbol);
            sb.Append(" @" + (o.isMarket ? "Mkt" : (o.isLimit? Util.FormatDecimal(o.price): Util.FormatDecimal(o.price)+ "stp")));
            sb.Append(" ["+o.Account+"]");
            sb.Append(" ID:" + o.id.ToString());
            sb.Append(" T:"+Math.Abs(o.TotalSize).ToString()+" F:"+o.Filled.ToString()+" R:"+o.UnsignedSize.ToString());
            sb.Append(" Ref:" + o.OrderRef + " Seq:" + o.OrderSeq.ToString() + " ExchID:" + o.OrderExchID);
            sb.Append(" Status:" + o.Status.ToString());

            return sb.ToString();
        }

        public static string GetOrderStatus(this Order o)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ID" + o.id.ToString());
            sb.Append(" T:" + Math.Abs(o.TotalSize).ToString() + " F:" + o.Filled.ToString() + " R:" + o.UnsignedSize.ToString());
            sb.Append(" Ref:" + o.OrderRef + " Seq:" + o.OrderSeq.ToString() + " ExchID:" + o.OrderExchID);
            sb.Append(" Status:" + o.Status.ToString());
            return sb.ToString();
        }
    }
}
