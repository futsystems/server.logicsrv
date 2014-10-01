using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text;
//using TradeLink.Common;
using TradingLib.API;
using TradingLib.Common;
using System.ComponentModel;

namespace Strategy.ExitPosition
{

    class StopLossProperty :IProperty
    {
        public Response Response { get { return _r; } }
        public string Name { get { return _r.FullName; } }
        StopLoss _r;
        public StopLossProperty(StopLoss response)
        {
            _r = response;
        }
        [Description("固定点数止损值"), Category("运行参数")]
        public decimal 止损 { get { return _r.止损; } set { _r.止损 = value; } }
    }
    /// <summary>
    /// 固定点数止损,当价格反向达到我们设定的止损值时,我们平仓出局
    /// </summary>
    class StopLoss:PositionCheckTemplate
    {
        //该position在配置窗口中显示的标题名称
        public static string Title
        {
            get { return "止损"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "固定点数止损"; }
        }

        [Description("固定点数止损值"), Category("运行参数")]
        public decimal 止损 { get; set; }

        decimal Loss;
        GUI.fmStopLoss fm;
        public override void Reset()
        {

            Loss = 止损;
            base.Reset();
            if (fm == null)
            {
                fm = new GUI.fmStopLoss(myPosition.Symbol, Loss);
                fm.SetSecurity(this.Security);
                DisplayForm = fm;
            }
            
        }
        public override void GotTick(Tick k)
        {
            base.GotTick(k);
           
            if (k.isTrade)
                fm.updateForm(k.trade, myPosition.AvgPrice, myPosition.isLong);
        }

        bool trigerlock = false;
        public override void checkPosition(out string msg)
        {
            msg = "";
            if (myPosition.isFlat)
            {
                trigerlock = false;
                return;
            }
            //D(myPosition.ToString());
            if(myPosition.isLong && !trigerlock)
            {
                if ((myPosition.AvgPrice - myPosition.LastPrice) >fm.StopLoss)
                {
                    D("止损触发 平掉所有仓位");
                    FlatPosition();
                    trigerlock = true;
                }
            }
            if (myPosition.isShort && !trigerlock)
            {
                if ((myPosition.LastPrice - myPosition.AvgPrice) > fm.StopLoss)
                {
                    D("止损触发 平掉所有仓位");
                    FlatPosition();
                    trigerlock = true;
                }
            }
            //D("Send indicators");
            //D(myBarList[myBarList.Last].Bardate.ToString());
            //回报计算结果或者相关信息
            //I(new object[] { _bolling.Mid.LookBack(0), _bolling.UNBand.LookBack(0), _bolling.DNBand.LookBack(0) });

        }

        //定义如何保存该策略
        public override string ToText()
        {
            string s = string.Empty;
            string[] r = new string[] { DataDriver.ToString(),止损.ToString() };
            return string.Join(",", r);
        }

        //定义如何从保存的文本生成该策略
        public override IPositionCheck FromText(string str)
        {
            string[] rec = str.Split(',');
            //if (rec.Length < 5) throw new InvalidResponse();

            PositionDataDriver driver = (PositionDataDriver)Enum.Parse(typeof(PositionDataDriver), rec[0]);
            decimal loss = Convert.ToDecimal(rec[1]);
            

            DataDriver = driver;
            止损 = loss;
            return this;
        }

    }
}
