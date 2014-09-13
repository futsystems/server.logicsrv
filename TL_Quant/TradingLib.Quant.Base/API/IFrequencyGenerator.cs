using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// Bar发生器接口,用于处理Tick数据按一定规则生成Bar数据
    /// </summary>
    public interface IFrequencyGenerator
    {
        void SetBarStore(BarDataV Barstore);
        // Events
        event NewTickEventArgsDel SendNewTickEvent;
        event SingleBarEventArgsDel SendNewBarEvent;
        // Methods
        void Initialize(Security symbol, BarConstructionType barConstruction);
        //处理Bar
        void ProcessBar(SingleBarEventArgs args);
        //处理Tick
        void ProcessTick(Tick tick);

        // Properties
        DateTime NextTimeUpdateNeeded { get; }
    }

 

 

}
