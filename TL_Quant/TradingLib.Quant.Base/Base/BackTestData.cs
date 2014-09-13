using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 封装了回测数据 用于形成BackTestReport/被支持IBackTestReport插件使用
    /// </summary>
    public class BackTestData
    { 
         // Fields
            private IStrategyResults strategyresults;
            private ShowChartDelegate showchartfunc;
            //public List<PositionInfo> PositionList;
            //private Dictionary<Security, SecurityFreq> inputsecurity;

            // Methods
            public BackTestData(ShowChartDelegate showChartDelegate, IStrategyResults strategyResults)
            {
                this.showchartfunc = showChartDelegate;
                this.strategyresults = strategyResults;
                this.InputSymbols = new Dictionary<Security, SecurityFreq>();
                /*
                foreach (SymbolSetup setup in this.strategyresults.RunSettings.Symbols)
                {
                    this.InputSymbols[setup.Security] = setup;
                }**/
            }

            public void ShowChart(Security symbol, DateTime dateTime)
            {
                ShowChartDelegate delegate2 = this.showchartfunc;
                if (delegate2 == null)
                {
                    //throw new RightEdgeError("ShowChart delegate was null");
                }
                //delegate2(symbol, dateTime);
            }

            // Properties
            public Dictionary<Security, SecurityFreq> InputSymbols { get; set; }

            /*
            public List<RiskAssessmentResults> RiskResults
            {
                get
                {
                    return this.strategyresults.RiskResults;
                }
            }
            **/
            public TimeSpan RunLength
            {
                get
                {
                    return this.strategyresults.RunLength;
                }
                set
                {
                    this.strategyresults.RunLength = value;
                }
            }

            public IStrategyData StrategyData
            {
                get
                {
                    return this.strategyresults.Data;
                }
            }

            public IStrategyResults StrategyResults
            {
                get
                {
                    return this.strategyresults;
                }
            }

            public StrategyRunSettings SystemRunSettings
            {
                get
                {
                    return this.strategyresults.RunSettings;
                }
            }

    // Nested Types
    public delegate void ShowChartDelegate(Security symbol, DateTime dateTime);

    }
    /*
    public void ShowChart(Symbol symbol, DateTime dateTime)
        {
            IChartDisplay document = null;
            TimeFrequency barFrequency;
            IFrequencyGenerator generator;
            bool flag;
            TickData data;
            List<BarData> list2;
            if ((((uint) flag) - ((uint) flag)) <= uint.MaxValue)
            {
                goto Label_032D;
            }
            goto Label_00E7;
        Label_0022:
            if (chartBars.Count == 0)
            {
                throw new RightEdgeError("No bars to display for symbol " + symbol);
            }
            SymbolFreq symbolFreq = new SymbolFreq(symbol, (int) barFrequency.BarLength.TotalMinutes);
            document = x7d323be108bfd078.mainForm.CreateChartInstance(symbolFreq, chartBars);
            document.SetUserChartCollection(this.x10f21d244bfbeaab.SystemData.ChartPaneCollections[symbol]);
            while (this.x853fd1dd0c2b1c7d.ContainsKey(symbol))
            {
                document.SetTradeData(this.x853fd1dd0c2b1c7d[symbol]);
                break;
            }
            document.SetChartObjects(this.x10f21d244bfbeaab.SystemData.ChartObjects[symbol]);
            this.SymbolCharts[symbol] = document.InternalName;
        Label_00CD:
            document.ScrollChart(dateTime, 1);
            x7d323be108bfd078.mainForm.SetActiveDocument(document);
            if (0xff != 0)
            {
                return;
            }
        Label_00E7:
            generator.ProcessTick(data);
            goto Label_0022;
        Label_0134:
            BarUtils.ProcessRListInBarGenerator(generator, symbol, new RList<BarData>(list2), DateTime.MinValue);
            goto Label_0022;
        Label_032D:
            while (this.SymbolCharts.ContainsKey(symbol))
            {
                document = RightEdge.DocumentManager.DocumentManager.Instance.Find(this.SymbolCharts[symbol]) as IChartDisplay;
                break;
            }
            if (document != null)
            {
                goto Label_00CD;
            }
            do
            {
                barFrequency = this.x10f21d244bfbeaab.SystemData.BarFrequency as TimeFrequency;
            }
            while ((((uint) flag) + ((uint) flag)) > uint.MaxValue);
            while (barFrequency == null)
            {
                throw new RightEdgeError("Currently can only show charts for time based frequencies.");
            }
            IDataStore barDataStoragePlugin = x7d323be108bfd078.mainForm.GetBarDataStoragePlugin();
            RList<BarData> chartBars = new RList<BarData>();
            generator = barFrequency.CreateFrequencyGenerator();
            generator.Initialize(symbol, this.x10f21d244bfbeaab.SystemData.GetBarConstruction(symbol));
            if (0 == 0)
            {
                if (((uint) flag) <= uint.MaxValue)
                {
                    generator.NewBar += delegate (object sender, SingleBarEventArgs e) {
                        chartBars.Add(e.Bar);
                    };
                    do
                    {
                        flag = false;
                        if (this.x10f21d244bfbeaab.SystemRunSettings.UseTicksForSimulation)
                        {
                            using (IDataAccessor<TickData> accessor = barDataStoragePlugin.GetTickStorage(symbol))
                            {
                                if (accessor.GetCount(DateTime.MinValue, DateTime.MaxValue) > 0)
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (!flag)
                        {
                            SymbolFreq freq = this.x10f21d244bfbeaab.InputSymbols[symbol];
                            list2 = barDataStoragePlugin.LoadBars(freq, this.x10f21d244bfbeaab.SystemData.DataStartDate, this.x10f21d244bfbeaab.SystemData.EndDate, -1, true);
                            goto Label_0134;
                        }
                        TickDataStreamer streamer = new TickDataStreamer(barDataStoragePlugin, new List<Symbol> { symbol }, this.x10f21d244bfbeaab.SystemData.DataStartDate, this.x10f21d244bfbeaab.SystemData.EndDate);
                        do
                        {
                            while (streamer.NextTick != null)
                            {
                                generator.ProcessTick(streamer.NextTick.Tick);
                                streamer.ConsumeTick();
                            }
                            data = new TickData {
                                time = DateTime.MaxValue.Subtract(TimeSpan.FromDays(365000.0)),
                                tickType = TickType.CurrentTime
                            };
                        }
                        while (-2 == 0);
                    }
                    while (0 != 0);
                    if (0 != 0)
                    {
                        return;
                    }
                    goto Label_00E7;
                }
                goto Label_0134;
            }
            goto Label_032D;
        }**/
}
