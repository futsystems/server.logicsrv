using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradeLink.API;
using TradingLib.API;

namespace StrategyClassic.Scalper
{
    //随机开仓 固定点数止损 固定点数平仓
    public class ScalperRandmo : TradingLib.Core.StrategyTemplate
    {
        public static string Title
        {
            get { return "随即入场高频"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "随机入场,固定点数止损,固定点数止盈"; }
        }

        #region 策略运行参数

        [Description("手数"), Category("运行参数")]
        public int Lots { get; set; }
        [Description("止损值"), Category("运行参数")]
        public decimal StopLoss { get; set; }
        [Description("止盈值"), Category("运行参数")]
        public decimal ProfitTarget { get; set; }
        [Description("是否预埋"), Category("运行参数")]
        public bool isProfitTakeOrderParked { get; set; }
        
        #endregion 

        private bool EnableStopLoss = false;
        //策略初始化函数
        public override void Reset()
        {
            base.Reset();

            if (StopLoss > 0)
                EnableStopLoss = true;

            EntryOrderSent = false;


        }
        private bool EntryOrderSent = false;
        private bool ExitOrderSent = false;

        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            #region 入场
            //空仓
            //我们发送Order委托完毕,可能委托没有回报 就会有一个新的Tick从而导致
            //状态不正确 导致重复发单,因此需要做好本地记录
            if (Position.isFlat && !EntryOrderSent)
            {
                D("#条件满足入场");
                BuyMarket(Lots);
                EntryOrderSent = true;
                
                

            }
            #endregion 
            

            #region 出场
            //止损
            
            if (EnableStopLoss && !ExitOrderSent)
            {
                if (Position.isLong)
                {
                    //如果亏损达到我们的止损额,市价平掉所有仓位
                    if (Position.AvgPrice - k.bid >= StopLoss)
                    {
                        D("多头止损:EntryPrice:" + Position.AvgPrice.ToString() + "ExitPrice:" + Position.LastPrice.ToString());
                        FlatPosition();
                        ExitOrderSent = true;
                    }
                }
                else if (Position.isShort)
                {

                    if (k.ask - Position.AvgPrice >= StopLoss)
                    {
                        D("空头止损:EntryPrice:" + Position.AvgPrice.ToString() + "ExitPrice:" + Position.LastPrice.ToString());
                        FlatPosition();
                        ExitOrderSent = true;
                    }
                }
            
            }
            //多头平仓 如果不是预埋方式提交出场单,则进行价格比较进行触发
            if (!isProfitTakeOrderParked && !ExitOrderSent)
            {
                if (Position.isLong)
                {
                    //多头达到预期盈利 平仓
                    if (k.bid - Position.AvgPrice > ProfitTarget)
                    {
                        D("多头止盈");
                        FlatPosition();
                        ExitOrderSent = true;
                    }
                }
                else if (Position.isShort)
                {
                    //空头达到预期盈利 平仓
                    if (Position.AvgPrice - k.ask > ProfitTarget)
                    {
                        D("空头止盈");
                        FlatPosition();
                        ExitOrderSent = true;
                    }
                }
            }
            #endregion 


        }

        //得到委托回报
        public override void GotOrder(Order o)
        {
            D("Strategy got order:"+o.ToString());
            base.GotOrder(o);
            
            

        }

        //当我们需要检测空仓
        /// <summary>
        ///111738: #条件满足入场                                                                                  条件满足发送委托
        ///111739: Strategy got order: BUY1 IF1212@Mkt [admin] 634890066250625015                                得到市价回报   
        ///111740: Strategy got filled:20121120,111430,IF1212,BUY,1,2177.00,admin,634890066250625015             得到刚才委托成交回报
        ///111740: orderFilled position:IF1212 1@2177.00 [admin]submit limit order for exiting position          委托成交后我们持有了仓位,提交限价出场委托
        ///111741: Strategy got order: SELL1 IF1212@2,177.40 [admin] 634890066250625016                          得到出场委托回报
        ///111853: 多头止损:EntryPrice:2177ExitPrice:2174.800000                                                  触发了止损
        ///111853: Strategy got order: SELL1 IF1212@2,177.40 [admin] 634890066250625016                           服务器撤销了限价出场委托，并立即市价平仓
        ///111854: Strategy got order: SELL1 IF1212@Mkt [admin] 634890066250625017                                得到市价平仓委托回报
        ///111854: Strategy got filled:20121120,111544,IF1212,SELL,1,2174.80,admin,634890066250625017             市价平仓委托成交
        ///111854: orderFilled position:IF1212 0@0.00 [admin]submit limit order for exiting position              

        /// </summary>
        /// <param name="f"></param>
        public override void GotFill(Trade f)
        {
            D("Strategy got filled:"+f.ToString());
            base.GotFill(f);
            //获得成交后就可以检查本地持仓,若仓位为0 则可以表明上次入场已经完毕,可以将入场开关打开，准备下次入场
            if (Position.isFlat)
            {
                EntryOrderSent = false;
                ExitOrderSent = false;
            }
            else //当我们得到成交回报时(并且此时有仓位表明这个fill我们入场),我们就挂一个limitOrder用于出场
            {
                if (isProfitTakeOrderParked)
                {
                    D("orderFilled position:" + Position.ToString() + "submit limit order for exiting position");
                    //islimited = true;
                    if (Position.isLong)
                        SellLimit(f.xsize, f.xprice + ProfitTarget);
                    if (Position.isShort)
                        BuyLimit(f.xsize, f.xprice - ProfitTarget);
                }
            }
            
        }

        public override void GotOrderCancel(long id)
        {
            //D("Strategy got filled:" + f.ToString());
            base.GotOrderCancel(id);
        }
       



        #region 参数保存以及载入
        public override string ToText()
        {
            //throw new NotImplementedException();
            return Lots.ToString()+","+StopLoss.ToString()+","+ProfitTarget.ToString()+","+(isProfitTakeOrderParked ?"1":"0");
        }

        public override IStrategy FromText(string cfg)
        {
            //throw new NotImplementedException();
            string[] res = cfg.Split(',');
            Lots = int.Parse(res[0]);
            StopLoss = decimal.Parse(res[1]);
            ProfitTarget = decimal.Parse(res[2]);
            isProfitTakeOrderParked = int.Parse(res[3]) == 1 ? true : false;
            return this;
        }
        #endregion

    }
}
