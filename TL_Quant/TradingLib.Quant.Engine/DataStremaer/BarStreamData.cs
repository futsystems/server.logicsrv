using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 封装了tick/bar数据
    /// </summary>
    public sealed class BarStreamData
    {
        // Methods

        public BarStreamData(NewBarEventArgs newBar)
        {
            this.NewBar = newBar;
        }

        public BarStreamData(TickItem tick)
        {
            //this.TickSymbol = symbol;
            this.Tick = tick;
        }

        // Properties
        public bool IsBarEvent
        {
            get
            {
                return (this.NewBar != null);
            }
        }

        public NewBarEventArgs NewBar { get; private set; }

        public TickItem Tick { get; private set; }

        //public Security TickSymbol { get; private set; }
    }


}
