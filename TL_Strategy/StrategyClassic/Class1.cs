using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradeLink.API;

namespace StrategyClassic
{
    public class DemoStrategy:TradingLib.Core.StrategyTemplate
    {
        public static string Title
        {
            get { return "测试策略"; }
        }
        //positioncheck的中文解释
        public static string Description
        {
            get { return "测试策略"; }
        }

        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            D(k.ToString());
        }
        public override string ToText()
        {
            //throw new NotImplementedException();
            return "";
        }

        public override IStrategy FromText(string cfg)
        {
            //throw new NotImplementedException();
            return this;
        }
    }
}
