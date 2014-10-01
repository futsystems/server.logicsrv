using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;
//using TradingLib.Core;
using TradingLib.API;

using System.ComponentModel;

namespace Strategy.ExitPosition
{
    class TwoStepPointProperty :IProperty
    {
        public Response Response { get { return _r; } }
        public string Name { get { return _r.FullName; } }
        TwoStepPoint _r;
        public TwoStepPointProperty(TwoStepPoint response)
        {
            _r = response;
        }

        [Description("止损值,亏损止损点位后平仓"), Category("运行参数")]
        public decimal 止损 { get { return _r.止损; } set { _r.止损 = value; } }
        [Description("保本点位,当获利保本点位后回吐一半,则保本平仓"), Category("运行参数")]
        public decimal 保本 { get { return _r.保本; } set { _r.保本 = value; } }
        [Description("一级止盈起步值"), Category("运行参数")]
        public decimal 止盈起步1 { get { return _r.止盈起步1; } set { _r.止盈起步1 = value; } }
        [Description("一级止盈值"), Category("运行参数")]
        public decimal 回吐1 { get { return _r.回吐1; } set { _r.回吐1 = value; } }
        [Description("二级止盈起步值"), Category("运行参数")]
        public decimal 止盈起步2 { get { return _r.止盈起步2; } set { _r.止盈起步2 = value; } }
        [Description("二级止盈值"), Category("运行参数")]
        public decimal 回吐2 { get { return _r.回吐2; } set { _r.回吐2 = value; } }

    }
    /// <summary>
    /// 止损,BreakEven,两级回调止盈
    /// 当价格触及止损价格时,止损平仓
    /// 当价格触及BreakEven触发价时,将止损价提高到BreakEven价格
    /// 当价格触及一段止盈价格时,将止损价格提高到一级止盈价格
    /// 当价格触及二段止盈价格时,将止损价格提高到二级止盈价格
    /// </summary>
    public class TwoStepPoint :PositionCheckTemplate
    {
        //该position在配置窗口中显示的标题名称
        public static string Title
        {
            get { return "跟踪止盈"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "两阶段跟踪止盈"; }
        }


        [Description("止损值,亏损止损点位后平仓"), Category("运行参数")]
        public decimal 止损 { get; set; }
        [Description("保本点位,当获利保本点位后回吐一半,则保本平仓"), Category("运行参数")]
        public decimal 保本 { get; set; }
        [Description("一级止盈起步值"),Category("运行参数")]
        public decimal 止盈起步1 { get; set; }
        [Description("一级止盈值"), Category("运行参数")]
        public decimal 回吐1 { get; set; }
        [Description("二级止盈起步值"), Category("运行参数")]
        public decimal 止盈起步2 { get; set; }
        [Description("二级止盈值"), Category("运行参数")]
        public decimal 回吐2 { get; set; }
              
        //策略内部调用参数
        private decimal stoplossprice = 0;
        private decimal breakeventriger = 0;
        private decimal profittakeprice1 = 0;
        private decimal profittakeprice2 = 0;
        private GUI.fmStopTrailing fm;

        //按照给定的参数 我们预先判断逻辑状态
        //private bool stopEnable = false;
        //private bool breakevenEnable = false;
       // private bool step1Enable = false;
       // private bool step2Enable = false;
        //出场策略触发所锁 如果已经触发就不要重复发单 防止由于保单的延迟 造成误发单
        private bool trigerlock = false;
        //激活策略
        decimal StopLoss;
        decimal BreakEven;
        decimal Start1;
        decimal Loss1;
        decimal Start2;
        decimal Loss2;

        public override void Reset()
        {
            StopLoss = 止损;
            BreakEven = 保本;
            Start1 = 止盈起步1;
            Loss1 = 回吐1;
            Start2 = 止盈起步2;
            Loss2 = 回吐2;

            base.Reset();
            //D("打开交易提示窗口");
            if (fm == null)
            {
                fm = new GUI.fmStopTrailing(StopLoss,BreakEven,Start1,Loss1,Start2,Loss2);
                fm.SetSecurity(this.Security);
                //显示策略标题
                //fm.Text = myPosition.symbol + ":" + Title + " " + ToText();
                //绑定输出窗体的数据源
                //fm.bindDataSource(DataManager, myPosition.symbol);
                //绑定事件
                fm.SendFlatPosition += new VoidDelegate(fm_SendFlatPosition);
                fm.SendBuyAction += new VoidDelegate(fm_SendBuyAction);
                fm.SendSellAction += new VoidDelegate(fm_SendSellAction);
                DisplayForm = fm;
                //fm.Show();
            }
            fm.Visible = true;
              
        }
        //界面操作函数
        void fm_SendSellAction()
        {
            SellMarket(fm.OrdSize);
            fm.message("手动卖出");
        }

        void fm_SendBuyAction()
        {
            BuyMarket(fm.OrdSize);
            fm.message("手动买入");
        }

        void fm_SendFlatPosition()
        {
            if (!trigerlock)
            {
                FlatPosition();
                fm.message("手动干预平仓");
                trigerlock = true;
            }
        }
        //关闭策略
        public override void Shutdown()
        {
            base.Shutdown();
            fm.Visible = false;
        }


        //策略的GotTick调用 当有新的tick进来时 我们进行的调用
        public  override void GotTick(Tick k)
        {
            base.GotTick(k);
            //根据tick更新界面数据
            try
            {
                if(fm!=null)
                    //只有在valid情况下 positioncheckcentre 才通过switchrepsonse激活了某个策略，这个时候信息输出窗口也必然已经实例化了。
                    fm.updateForm(k, myPosition, stoplossprice,breakeventriger, profittakeprice1, profittakeprice2);
            }
            catch (Exception ex)
            {
                D(ex.ToString());
            }
        }
        /// <summary>
        /// 改进:注意平仓后的重复平仓发单问题,这个问题最好由positionchecktemplate统一解决
        /// positiontemplate 记录本地发单。比较本地发单和当前仓位 避免oversell
        /// 关注oversell tracker 组件 看看是否有构件可以很好的解决这个问题。
        /// </summary>
        /// <param name="msg"></param>
        public override void checkPosition(out string msg)
        {
            msg = "";
            //空仓情况下我们将止损价格与保本价格等相关价格止空
            if(myPosition.isFlat)
            {
                resetPriceTriger();
                trigerlock = false;
                return;
            }
            //计算止损价
            //多头
            if (myPosition.isLong && !trigerlock)
            {
                //二级止盈
                if (fm.Trailing2Enable && (myPosition.Highest - myPosition.AvgPrice) >= fm.Start2)
                {
                    profittakeprice2 = myPosition.Highest - fm.Loss2;
                    if ((myPosition.Highest - myPosition.LastPrice) >= fm.Loss2)
                    {
                        //平掉所有仓位
                        FlatPosition();
                        //SellMarket(myPosition.Size / 2);
                        fm.message("多头触发二级止盈 平仓");
                        trigerlock = true;
                        
                    }
                }
                //一级止盈
                else if (fm.Trailing1Enable && (myPosition.Highest - myPosition.AvgPrice) >= fm.Start1)
                {
                    profittakeprice1 = myPosition.Highest - fm.Loss1;
                    if ((myPosition.Highest - myPosition.LastPrice) >= fm.Loss1)
                    {
                        FlatPosition();
                        fm.message("多头触发一级止盈 平仓");
                        trigerlock = true;
                        
                    }
                }
                //是否运行保本
                if (fm.BreakEvenEnable)
                {
                    //最高价>成本+BreakEven触发点位
                    if (myPosition.Highest - myPosition.AvgPrice >= fm.BreakEven)
                    {   //breakeventriger为保本触发价
                        breakeventriger = myPosition.AvgPrice + fm.BreakEven/2;
                        if (myPosition.LastPrice <= breakeventriger)
                        {
                            FlatPosition();
                            fm.message("多头触发保本 平仓");
                            trigerlock = true;
                        }
                    }
                    //D("breakeven price:"+breakeventriger.ToString());
                }
                //是否运行止损
                if (fm.StopLossEnable)
                {
                    stoplossprice = myPosition.AvgPrice - fm.StopLoss;
                    if (myPosition.LastPrice <= stoplossprice)
                    {
                        FlatPosition();
                        fm.message("多头触发止损 平仓");
                        trigerlock = true;
                        
                    }
                }
            }
            //空头
            if (myPosition.isShort && !trigerlock)
            {
                //二级止盈
                if (fm.Trailing2Enable && (myPosition.AvgPrice - myPosition.Lowest) >= fm.Start2)
                {
                    profittakeprice2 = myPosition.Lowest +fm.Loss2;
                    if ((myPosition.LastPrice - myPosition.Lowest) >= fm.Loss2)
                    {
                        FlatPosition();
                        fm.message("空头触发二级止盈 平仓");
                        trigerlock = true;
                        
                    }
                }
                //一级止盈
                else if (fm.Trailing1Enable && (myPosition.AvgPrice - myPosition.Lowest) >= fm.Start1)
                {
                    profittakeprice1 = myPosition.Lowest + fm.Loss1;
                    if ((myPosition.LastPrice - myPosition.Lowest) >= fm.Loss1)
                    {
                        FlatPosition();
                        fm.message("空头触发一级止盈 平仓");
                        trigerlock = true;
                        
                    }
                }
                if (fm.BreakEvenEnable)
                {
                    if (myPosition.AvgPrice -myPosition.Lowest >=fm.BreakEven)
                    {
                        breakeventriger = myPosition.AvgPrice -fm.BreakEven/2;
                        if (myPosition.LastPrice >= breakeventriger)
                        {
                            FlatPosition();
                            fm.message("空头触发保本 平仓");
                            trigerlock = true;
                            
                        }
                    }
                
                }
                if (fm.StopLossEnable)
                {
                    stoplossprice = myPosition.AvgPrice + fm.StopLoss;
                    if (myPosition.LastPrice >= stoplossprice)
                    {
                        FlatPosition();
                        fm.message("空头触发止损 平仓");
                        trigerlock = true;
                        
                    }
                }
            }

        }
        //仓位归0后 重置内部参数
        private void resetPriceTriger()
        {
            stoplossprice = 0;
            breakeventriger = 0;
            profittakeprice1 = 0;
            profittakeprice2 = 0;
        }
        

        //定义如何保存该策略
        public override string ToText()
        { 
            string s= string.Empty;
            string[] r = new string[] {DataDriver.ToString(), 止盈起步1.ToString(),回吐1.ToString(),止盈起步2.ToString(),回吐2.ToString(),止损.ToString(),保本.ToString()};
            return string.Join(",", r);
        }
        
        //定义如何从保存的文本生成该策略
        public override IPositionCheck FromText(string str)
        {
            string[] rec = str.Split(',');
            //if (rec.Length < 5) throw new InvalidResponse();
            
            PositionDataDriver driver = (PositionDataDriver)Enum.Parse(typeof(PositionDataDriver), rec[0]);
            decimal start1 = Convert.ToDecimal(rec[1]);
            decimal loss1 = Convert.ToDecimal(rec[2]);
            decimal start2 = Convert.ToDecimal(rec[3]);
            decimal loss2 = Convert.ToDecimal(rec[4]);
            decimal stoploss = Convert.ToDecimal(rec[5]);
            decimal breakeven = Convert.ToDecimal(rec[6]);

            DataDriver = driver;
            止盈起步1 = start1;
            回吐1 = loss1;
            止盈起步2 = start2;
            回吐2 = loss2;
            止损 = stoploss;
            保本 = breakeven;
            return this;
        }
    }

}
