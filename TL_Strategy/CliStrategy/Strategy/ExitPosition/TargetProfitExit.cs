using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using System.ComponentModel;
using TradingLib.Common;

namespace Strategy.ExitPosition
{
    class TargetProfitExitProperty :IProperty
    {
        public Response Response { get { return _r; } }
        public string Name { get { return _r.FullName; } }
        TargetProfitExit _r;
        public TargetProfitExitProperty(TargetProfitExit response)
        {
            _r = response;
        }

        [Description("止损值"), Category("运行参数")]
        public decimal 目标盈利 { get { return _r.目标盈利; } set { _r.目标盈利 = value; } }
        [Description("平仓数量"), Category("运行参数")]
        public int 平仓数量 { get { return _r.平仓数量; } set { _r.平仓数量 = value; } }
    }
    /// <summary>
    /// 当价格达到我们的目标位置时平仓
    /// </summary>
    class TargetProfitExit:PositionCheckTemplate
    {
        //该position在配置窗口中显示的标题名称
        public static string Title
        {
            get { return "目标盈利"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "目标盈利止盈"; }
        }

        [Description("止损值"), Category("运行参数")]
        public decimal 目标盈利 { get; set; }
        [Description("平仓数量"), Category("运行参数")]
        public int 平仓数量 { get; set; }

        bool trigerlock = false;
        GUI.fmTargetProfit fm;
        decimal TargetProfit;
        int CloseSize;
        public override void Reset()
        {
            TargetProfit = 目标盈利;
            CloseSize = 平仓数量;

            base.Reset();

            if (fm == null)
            {
                fm = new GUI.fmTargetProfit(myPosition.Symbol,TargetProfit,CloseSize);
                fm.SetSecurity(this.Security);
                DisplayForm = fm;
                //fm.Show(); 
            }
            fm.Visible = true;

        }
        public override void Shutdown()
        {
            base.Shutdown();
            fm.Visible = false;
        }

        public  override void GotTick(Tick k)
        {
            base.GotTick(k);
            //D("XXXXXXXXXXX got tick");
            if (k.isTrade)
                fm.updateForm(k.trade, myPosition.AvgPrice, myPosition.isLong);
        }

        public override void checkPosition(out string msg)
        {
            msg = "";

            if (myPosition.isFlat)
            {
                trigerlock = false;
                return;
            }

            //当价格达到我们预期利润时 平仓
            if (myPosition.isLong && !trigerlock)
            {
                if (myPosition.LastPrice >= myPosition.AvgPrice + fm.TargetProfit)
                {
                    if (CloseSize == 0)
                    {
                        FlatPosition();

                    }
                    else
                    {

                        SellMarket(CloseSize);
                    }
                    trigerlock = true;

                }

            }

            if (myPosition.isShort && !trigerlock)
            {
                if (myPosition.LastPrice <= myPosition.AvgPrice - fm.TargetProfit)
                {
                    if (CloseSize == 0)
                    {
                        FlatPosition();
                    }
                    else
                    {
                        BuyMarket(CloseSize);
                    }
                    trigerlock = true;
                }
            }


        }

        public override string ToText()
        {
            string s = string.Empty;
            string[] r = new string[] { DataDriver.ToString(),目标盈利.ToString(),平仓数量.ToString()};
            return string.Join(",", r);
        }

        public override IPositionCheck FromText(string msg)
        {
            string[] rec = msg.Split(',');
            //if (rec.Length < 5) throw new InvalidResponse();

            PositionDataDriver driver = (PositionDataDriver)Enum.Parse(typeof(PositionDataDriver), rec[0]);
            decimal target = Convert.ToDecimal(rec[1]);
            int size = Convert.ToInt16(rec[2]);

            目标盈利 = target;
            平仓数量 = size;

            return this;
        }
    }
}
