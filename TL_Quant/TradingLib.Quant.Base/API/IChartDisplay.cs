using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant;


namespace TradingLib.Quant.Base
{
    public interface IChartDisplay //: IDocument
    {
        // Events
        //abstract event ChartObjectSelectionChanged ChartObjectSelectionChange;

        // Methods
        bool AddIndicator(IIndicatorPlugin plugin, IndicatorInfo info);
        void AddIndicatorPlugin(IIndicatorPlugin plugin);
        void AddTrade(Trade trade);
        //void ApplySavedChart(ChartSettings chartSettings);
        ChartObject CreateChart(string chartName);
        ChartObject CreateChart(int chartY, int size, string chartName);
        void DisplayPriceAndVolumeChart(IBarData bars);


        //int GetAggrFrequency();
        //int GetBarsPerPage();
        //int GetDataFrequency();
        //ChartFormIndicatorManager GetIndicatorManager();
        //bool GetQuickChart();
        //void NewBar(Bar bar, DateTime barEndTime);
        //void NewTick(IEnumerable<Tick ticks, Bar partialBar);
        void ScrollChart(DateTime dateTime, int offset);
        //void SetAggrFrequency(int freq);
        //void SetBarConstruction(BarConstructionType barConstruction);
        void SetBarsPerPage(int barsPerPage);
        void SetChartObjects(List<IChartObject> chartObjects);
        //void SetQuickChart(bool quickChart);
        void SetTradeData(List<Trade> trades);
        void SetPRData(List<PositionRound> prs);
        void SetUserChartCollection(List<ChartPane> chartPanes);//设定用户自定义的ChartCollection
        //void UpdateChartObject(IChartObject chartObject);
        //void ZoomChart(double factor);
    }


}
