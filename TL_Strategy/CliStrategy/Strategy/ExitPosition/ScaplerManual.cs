using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text;
//using TradeLink.Common;
using TradingLib.API;
using TradingLib.Common;
using System.ComponentModel;
using Strategy.GUI;


namespace Strategy.ExitPosition
{
    //通过继承invalidresponse可以使得strategyhelper可以动态的找到他
    class ScalperManualProperty :IProperty
    {
        public Response Response { get { return _r; } }
        public string Name { get { return _r.FullName; } }
        ScalperManual _r;
        public ScalperManualProperty(ScalperManual response)
        {
            _r = response;
        }
        [Description("损值点数"), Category("运行参数")]
        public decimal 止损 { get { return _r.止损; } set { _r.止损 = value; } }
        [Description("目标盈利"), Category("运行参数")]
        public decimal 止盈 { get { return _r.止盈; } set { _r.止盈 = value; } }
        [Description("建仓后是否立即发送平仓委托"), Category("运行参数")]
        public bool 建仓后发送委托 { get { return _r.建仓后发送委托; } set { _r.建仓后发送委托 = value; } }

    }
    /// <summary>
    /// 固定点数止损,当价格反向达到我们设定的止损值时,我们平仓出局
    /// </summary>
    class ScalperManual :PositionCheckTemplate
    {
        //该position在配置窗口中显示的标题名称
        public static string Title
        {
            get { return "炒单"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "手工选择入场点,当仓位形成时自动下达限价出场单,止损时市价止损"; }
        }

        [Description("损值点数"), Category("运行参数")]
        public decimal 止损 { get; set; }
        [Description("目标盈利"), Category("运行参数")]
        public decimal 止盈 { get; set; }
        [Description("建仓后是否立即发送平仓委托"), Category("运行参数")]
        public bool 建仓后发送委托 { get; set; }


        private decimal Loss;
        private decimal ProfitTarget;
        private bool isProfitTakeOrderParked;
        fmScalperManual fm;
        public override void Reset()
        {
            base.Reset();
            //从模型配置中得到初始化参数
            Loss = 止损;
            ProfitTarget = 止盈;
            isProfitTakeOrderParked = 建仓后发送委托;

            //如果首次启动则初始化监控窗口
            if (fm == null)
            {
                fm = new fmScalperManual(myPosition.Symbol, ProfitTarget, Loss, isProfitTakeOrderParked);
                fm.SetSecurity(this.Security);
                fm.SendFlatPosition += new VoidDelegate(fm_SendFlatPosition);
                fm.SendBuyAction += new VoidDelegate(fm_SendBuyAction);
                fm.SendSellAction += new VoidDelegate(fm_SendSellAction);
                //if (defaultSetting()) fm.setDefaultConfig(ProfitTarget, Loss, isProfitTakeOrderParked);//检查是否加载过配置,如果没有则配置默认参数
                //fm.Show();
                DisplayForm = fm;
            }
            fm.Visible = true;//显示监视窗口

        }
        bool profitTakeOrderSent = false;
        bool lossTakeOrderSent = false;

        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            //if (k.isTrade)
            fm.updateForm(k, myPosition);
        }

        public override void checkPosition(out string msg)
        {
            msg = "";
            #region 当价格达到我们止损时 平仓
            if (!lossTakeOrderSent)
            {
                if (myPosition.isLong && Quote.hasBid)
                {
                    if ((myPosition.AvgPrice - Quote.bid) >= fm.Loss)
                    {
                        D("止损触发 平掉所有仓位");
                        FlatPosition();
                        lossTakeOrderSent = true;
                    }
                }

                if (myPosition.isShort && Quote.hasAsk)
                {
                    if ((Quote.ask - myPosition.AvgPrice) >= fm.Loss)
                    {
                        D("止损触发 平掉所有仓位");
                        FlatPosition();
                        lossTakeOrderSent = true;
                    }
                }
            }
            #endregion

            #region 当价格达到我们预期利润时 平仓
            //当我们不发送平仓委托同时 获利平仓单并没有发出时,我们需要检查仓位
            if (!fm.IsProfitTakeOrderPark && !profitTakeOrderSent)
            {
                if (myPosition.isLong && Quote.hasBid)
                {
                    if (Quote.bid >= myPosition.AvgPrice + fm.ProfitTake)
                    {
                        FlatPosition();
                        profitTakeOrderSent = true;

                    }

                }

                if (myPosition.isShort && Quote.hasAsk)
                {
                    if (myPosition.LastPrice <= myPosition.AvgPrice - fm.ProfitTake)
                    {
                        FlatPosition();
                        profitTakeOrderSent = true;
                    }
                }
            }
            #endregion
        }

        public override void GotFill(Trade f)
        {
            base.GotFill(f);
            if (fm != null)
                fm.updateForm(myPosition);
        }
        //当仓位建立时 根据设定直接发送限价委托平仓
        protected override void  onPositionEntry(Trade f)
        {
            D("建仓事件触发");
            if (fm.IsProfitTakeOrderPark)
            {
                if (myPosition.isLong)
                    SellLimit(f.xsize, f.xprice + fm.ProfitTake);
                if (myPosition.isShort)
                    BuyLimit(f.xsize, f.xprice - fm.ProfitTake);
            }
        }
        //当退出仓位时,将止损单委托发送与止盈单委托发送开关重置
        protected override void onPositionExit(Trade f)
        {
            D("平仓事件触发");
            profitTakeOrderSent = false;
            lossTakeOrderSent = false;
            resetFlag();
        }
        protected override void onPositionAdd(Trade f)
        {
            D("加仓事件触发");
            if (fm.IsProfitTakeOrderPark)
            {
                if (myPosition.isLong)
                    SellLimit(f.xsize, f.xprice + fm.ProfitTake);
                if (myPosition.isShort)
                    BuyLimit(f.xsize, f.xprice - fm.ProfitTake);
            }
        }


        protected override void onPositionCut(Trade f)
        {
            D("减仓事件触发");
        }

        public override void Shutdown()
        {
            base.Shutdown();
            fm.Visible = false;
        }

        void resetFlag()
        {
            FlatTriger = false;
            profitTakeOrderSent = false;
            lossTakeOrderSent = false;
        }

        //界面操作函数
        void fm_SendSellAction()
        {
            D("手动卖出");
            if (fm.IsLimitEntryOrder)
            {
                decimal price = 0;
                if (fm.IsOffset)
                    price = fm.LastPrice + fm.Offset;
                if (fm.IsLimitPrice)
                    price = fm.LimitPirce;

                if(fm.TIF ==0)
                    SellLimit(fm.OrdSize, price);
                else
                    SellLimit(fm.OrdSize, price, fm.TIF.ToString());
            }
            else
            {
                SellMarket(fm.OrdSize);
            }
        }

        void fm_SendBuyAction()
        {
            D("手动买入");
            
            if (fm.IsLimitEntryOrder)//是否是限定价格
            {
                decimal price = 0;
                if (fm.IsOffset)
                    price = fm.LastPrice + fm.Offset;
                if (fm.IsLimitPrice)
                    price = fm.LimitPirce;
                if(fm.TIF ==0)
                    BuyLimit(fm.OrdSize, price);
                else
                    BuyLimit(fm.OrdSize, price,fm.TIF.ToString());
            }
            else
            {
                BuyMarket(fm.OrdSize);
            }
            
        }
        bool FlatTriger = false;
        //平仓部分由错误，需要仔细排查 同时多策略同时运行也有冲突需要整理清楚当中的逻辑过程
        void fm_SendFlatPosition()
        {
            D("手动全平:"+profitTakeOrderSent.ToString() +" "+ lossTakeOrderSent.ToString() +" "+FlatTriger.ToString());
            if (!profitTakeOrderSent && !lossTakeOrderSent &&!FlatTriger)
            {
               
                    if(FlatPosition())
                        FlatTriger = true;
            }
        }


        //定义如何保存该策略
        public override string ToText()
        {
            string s = string.Empty;
            string[] r = new string[] { DataDriver.ToString(), 止损.ToString(), 止盈.ToString(), 建仓后发送委托.ToString()};
            return string.Join(",", r);
        }


        bool defaultSetting()
        {
            if (!_isconfiged)
            {
                止损 = 5 * this.Security.PriceTick;
                止盈 = 10 * this.Security.PriceTick;
                建仓后发送委托 = false;
                _isconfiged = true;
                return true;
            }
            return false;
        }
        //标识 是否设定过参数,如果没有则设定默认参数
        bool _isconfiged = false;
        //定义如何从保存的文本生成该策略
        public override IPositionCheck FromText(string str)
        {
            string[] rec = str.Split(',');
            //if (rec.Length < 5) throw new InvalidResponse();
            _isconfiged = true;
            PositionDataDriver driver = (PositionDataDriver)Enum.Parse(typeof(PositionDataDriver), rec[0]);
            decimal loss = Convert.ToDecimal(rec[1]);
            decimal profit = Convert.ToDecimal(rec[2]);
            bool park = Convert.ToBoolean(rec[3]);

            DataDriver = driver;
            止损 = loss;
            止盈 = profit;
            建仓后发送委托 = park;
            return this;

        }
    }
}
