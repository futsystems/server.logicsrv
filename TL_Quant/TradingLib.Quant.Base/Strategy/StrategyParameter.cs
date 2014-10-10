using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用于对策略参数字段应用属性
    /// Display = "下单量", Description = "每次下单的手数", Category = "参数"
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StrategyParameter : Attribute
    {
        string display;
        string desp;


        public string Display { get { return display; } set { display = value; } }
        public string Description { get { return desp; } set { desp = value; } }


    }
}
