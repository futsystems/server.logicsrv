using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;


namespace TradingLib.Quant.Base
{
    public enum QSEnumPostionChangeType
    {
        ADD,//加仓
        DEL,//减仓
    }
    //以StrategySetup为参数的委托
    public delegate void StrategySetupDel(StrategySetup setup);
    //策略回测过程中 进度委托
    public delegate void StrategyProgressUpdate(int currentItem, int totalItems, DateTime currentTime);

    //显示Chart图委托
    public delegate void ShowChartDelegate(Security symbol, DateTime dateTime);


    //SingleBarEventArgs Event
    public delegate void SingleBarEventArgsDel(SingleBarEventArgs barEventArgs);

    public delegate void NewTickEventArgsDel(NewTickEventArgs tickEventArgs);

    public delegate void PositionChangeDel(Position position,QSEnumPostionChangeType type);

    public delegate void PositionEventDel(Trade fill,PositionDataPair data);

    //ChartObject变动委托
    //public delegate void ChartObjectChangedDelegate(IChartObject chartObject);



    

 

}
