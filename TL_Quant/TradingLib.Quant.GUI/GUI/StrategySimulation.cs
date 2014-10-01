using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;
using TradingLib.Quant.Engine;


namespace TradingLib.Quant.GUI
{
    /// <summary>
    /// 策略回测窗口
    /// </summary>
    public partial class StrategySimulation : KryptonForm
    {

        SharedSystemRunData _rundata;
        StrategySetup _setup;
        private TradingModuleWrapper _x3a5f5a5151101c65;
        private IStrategyResults _results;

        BackgroundWorker bw = new BackgroundWorker();

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        public StrategySimulation(StrategySetup setup,SharedSystemRunData rundata)
        {
            InitializeComponent();
            _rundata = rundata;
            _setup = setup;
            strategyFriendlyName.Text = setup.FriendlyName;
            strategyClassName.Text = setup.StrategyClassName;

            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            InitAndRunStrategy();
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // status back to user
            //bool g = (bool)e.Result;
            //重置进度条与处理tick总数
            // reset progress bar
            //progress(0);
            // reset ticks processed
            //_ticksprocessed = 0;
        }

        SharedSystemRunData GetDemoRunData()
        {
            DateTime start = new DateTime(2011, 5, 1);
            DateTime end = new DateTime(2013, 10, 1);
            end = DateTime.MaxValue;
            List<SymbolSetup> sl = new List<SymbolSetup>();
            Security security = new SecurityImpl("IF");
            security.Margin = 0.1M;
            security.Multiple = 300;
            security.PriceTick = 0.2M;
            //security.EntryCommission = 100;
            //security.ExitCommission = 100;
            //security.Type = SecurityType.FUT;


            sl.Add(new SymbolSetup(security,new BarFrequency(BarInterval.CustomTime,240)));


            KeyValuePair<string, double> sp = new KeyValuePair<string, double>("demo", 2000);
            List<KeyValuePair<string, double>> splist = new List<KeyValuePair<string, double>>();
            splist.Add(sp);



            SharedSystemRunData data = new SharedSystemRunData();

            data.RunSettings = new StrategyRunSettings();
            data.RunSettings.AccountCurrency = CurrencyType.RMB;
            //data.RunSettings.AllocationPerPosition = 0.2;
            //data.RunSettings.AllocationType = PositionAllocationType.FixedSize;
            //data.RunSettings.ApplyForexInterest = false;
            //data.RunSettings.BarCountExit = 20;
            data.RunSettings.CreateTicksFromBars = false;
            data.RunSettings.DataStartDate = start;
            data.RunSettings.EndDate = end;
            //data.RunSettings.ForceRoundLots = true;
            //data.RunSettings.ForexRolloverTime = new TimeSpan(1, 1, 1);
            data.RunSettings.HighBeforeLowDuringSimulation = true;
            data.RunSettings.IgnoreSystemWarnings = false;
            data.RunSettings.LeadBars = 20;
            data.RunSettings.MaxOpenPositions = 10;
            data.RunSettings.MaxOpenPositionsPerSymbol = 2;
            //data.RunSettings.ProfitTarget = 1000;
            //data.RunSettings.ProfitTargetType = TargetPriceType.AbsolutePrice;
            data.RunSettings.RestrictOpenOrders = false;
            data.RunSettings.RunNumber = 10;
            data.RunSettings.SaveOptimizationResults = true;
            data.RunSettings.StartingCapital = 1000000;
            //data.RunSettings.StopLoss = 2000;
            data.RunSettings.Symbols = sl;
            data.RunSettings.SynchronizeBars = true;
            //data.RunSettings.SystemParameters = splist;
            data.RunSettings.TradeStartDate = start;
            data.RunSettings.UseTicksForSimulation = false;
            //data.RunSettings.BarFrequency


            data.InternalSettings = new InternalSystemRunSettings();
            data.InternalSettings.TradingSystemProjectPath = "D:\\test";
            data.InternalSettings.SystemClassName = "demostrategy";
            data.InternalSettings.ProjectDir = "D:\\test";
            data.InternalSettings.OutputDir = "D:\\output";
            data.InternalSettings.LiveMode = false;
            data.InternalSettings.ShutDownWhenDone = true;


            //创建timefrequency pluginsetting.传递个runingData
            //PluginSettings ts = PluginSettings.CreateTimeFrequencySettings(new BarFrequency(BarInterval.CustomTime,240));
            //data.SelectedSystemFrequency = ts;
            return data;
        }


        public void InitAndRunStrategy()
        {
            if (! (_rundata.RunSettings.Symbols.Count > 0))
            {
                fmMessageBox.Show("没有选择回测时所交易的合约");
                return;
            }

            try
            {
                string filename = null;
                BarFrequency frequency;
                TradingModuleWrapperArgs args;


                bool flag;
                DateTime now = DateTime.Now;
                //检测数据组件
                if (((IDataStore)QuantGlobals.PluginManager.CreatePlugin(QuantGlobals.DataStoreSetting, null).Plugin) == null)
                {
                    fmMessageBox.Show("没有有效数据组件");
                    return;
                }
                //矫正频率信息
                frequency = this._rundata.RunSettings.Symbols[0].Frequency;
                foreach (SymbolSetup setup in this._rundata.RunSettings.Symbols)
                {
                    if (setup.Frequency != frequency)
                    {
                        fmMessageBox.Show("所有选中的合约必须使用同样的频率进行回测");
                        return;
                    }
                }

                StrategySetup ss = new StrategySetup();
                ss.FriendlyName = "demostrategy_001";
                //ss.RunDataFile = "appdata001";
                ss.StrategyClassName = "DemoStrategy.MyStrategy";

                QuantGlobals.Access.GetStrategyManager().ClearStrategyProject();
                QuantGlobals.Access.GetStrategyManager().AddStrategyProject(ss);

                StrategyInfo sinfo = QuantGlobals.Access.GetStrategyManager().GetStrategyInfo(ss.FriendlyName);


                ServiceAppDomainFactory factory = new ServiceManagerAppDomainFactory(QuantGlobals.Access.GetServiceManager().GetServiceSetup(QuantGlobals.PaperBroker), QuantGlobals.UserAppDataPath,QuantGlobals.PluginDirectory);
                
                //交易模块wrapper参数(将所有回测需要的数据生成args传递)
                args = new TradingModuleWrapperArgs();
                //args.Acccess = QuantGlobals.Access;
                args.BrokerFactoryFactory = factory;
                args.DataStoreSettings = QuantGlobals.DataStoreSetting;
                args.SystemClassName = ss.StrategyClassName;
                args.SystemFilename = sinfo.FileName;
                //MessageBox.Show("Strategy Class Name:" + _rundata.InternalSettings.SystemClassName);
               //args.SystemFilename = QuantGlobals.Access.GetStrategyManager().GetStrategyInfo(_setup.FriendlyName).FileName;



                SharedSystemRunData data = this.GetDemoRunData();//获得模拟的rundata数据

                data.RunSettings.UseTicksForSimulation = true;
                data.RunSettings.DataStartDate = new DateTime(2011, 5, 1);
                data.RunSettings.EndDate = new DateTime(2011, 5, 20);


                //生成交易模块 并运行策略
                TradingModuleWrapper wrapper =  new TradingModuleWrapper(args);//this._xe146d4dea8d8c728.CreateScriptObject(args);
                wrapper.SendDebugEvent +=new API.DebugDelegate(debug);
                //wrapper.UpdateProgressFunction = this.ProgressUpdate;

                
                wrapper.RunSystem(data);
                
                //运行完毕后保存运行结果文件
                filename = wrapper.GetRunResults(null).ResultsFile;
                //debug("got backtest file " + filename);
                if (_rundata.InternalSettings.ShutDownWhenDone)
                {
                    wrapper.Shutdown();
                    wrapper.Dispose();
                    //GC.Collect();
                    
                }
                //while (!this._xb4ef597285b40b20.InternalSettings.ShutDownWhenDone)
                //{
                //    this._x3a5f5a5151101c65 = wrapper;
                //    goto Label_0070;
                //}
                //this.x3765436e7e76b5ed = DateTime.Now.Subtract(now);
                //this.x0a51d06e762d4568(string.Concat(new object[] { "Marshalling system results (Step ", 3, "/", 3, ")" }), "", 0, 1);
                now = DateTime.Now;

                //运行结束后获得回测报告文件
                //filename = wrapper.GetRunResults(null).ResultsFile;
                if (filename != null)
                {
                    
                    this._results = TradingModuleWrapper.LoadResultsFromFile(filename);
                }

                QuantGlobals.GDebug("回测结果:" + _results.Data.StratgyHistory.StrategyStatistics.BarStats.Count.ToString());

                MessageBox.Show(_results.Data.StratgyHistory.StrategyStatistics.ToString());





            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //delegate void pdouble(int currentitem, int totalitems,DateTime currenttime);

        void ProgressUpdate(int currentItem, int totalItems, DateTime currentTime)
        {
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new StrategyProgressUpdate(ProgressUpdate), new object[] { currentItem,totalItems,currentTime });
            else
            {
                double percent = ((double)currentItem)/((double)totalItems);
                int p = (int)(percent * 100);
                //debug(p.ToString() +"cu:" + currentItem.ToString() + " to:" + totalItems.ToString());
                if (p > 100) p = 100;
                if (p < 0) p = 0;
                _progress.Value = p;
                _progress.Invalidate();
                ProgressMessage(currentTime.ToString()  +"    "+currentItem.ToString() +"/" + totalItems.ToString());
            }
        }
        delegate void pstring(string message);
        void ProgressMessage(string message)
        {
            if (msg.InvokeRequired)
                msg.Invoke(new pstring(ProgressMessage), new object[] { message });
            else
            {
                msg.Text = message;
            }
        }

        private void startSimulation_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
            {
                debug("有回测任务正在运行,请等待...");
                return;
            }
            // start background thread to convert
            bw.RunWorkerAsync(null);
        }

        private void clearDebug_Click(object sender, EventArgs e)
        {
            debugControl1.Clear();
        }

        /// <summary>
        /// 在Chart图表中显示回测信息/价格K线/指标曲线/买卖信息等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowResults_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this._results.Data.Bars["IF"].Count.ToString());
            Security sec = this._results.Data.Symbols[0];
            SecurityFreq  sf = new SecurityFreq (sec,this._results.Data.BarFrequency.BarFrequency);
            //if (this._results != null)
            //    QuantGlobals.Access.CreateChartInstance(sf);

            IChartDisplay document = null;
            BarFrequency barFrequency;
            IFrequencyGenerator generator;
            bool flag;
            //Tick data;
            //List<Bar> list2;
            IBarData chartBars;

            //查询当前Chart图表中是否有对应合约与频率的Chart
            if (false)
            { 
            
            
            }
            
            barFrequency = this._results.Data.BarFrequency.BarFrequency ;
            if(barFrequency.Type!= API.BarInterval.CustomTime) 
            {
                throw new Exception("只能显示时间序列的价格图");
            }

            IDataStore  datastore = QuantGlobals.Access.GetBarDataStoragePlugin();

            DateTime start = this._results.Data.DataStartDate;
            DateTime end = this._results.Data.EndDate;
            //如果是Bar模拟 则直接加载Bar数据  否则需要从Tick数据生成
            chartBars = BarUtil.IBarDataFromLocalStorage(sf, start, end);


            if (_results.Data.UseTicksForSimulation)
            {
                MessageBox.Show("Tick数据回测");
                //generator = _results.Data.BarFrequency.CreateFrequencyGenerator();
                //generator.SendNewBarEvent += new SingleBarEventArgsDel(generator_SendNewBarEvent);

            }
            //MessageBox.Show(this._results.Data.Bars.Count.ToString());
            //调用全局MainForm生成Chart图表
            document = QuantGlobals.Access.CreateChartInstance(sf,this._results.Data.Bars["IF"]);
            // 绑定用户chart到图表
            //MessageBox.Show("it is run to here:" + this._results.Data.ChartPaneCollections.Count.ToString() +"chartbas num:"+chartBars.Count.ToString());
            document.SetUserChartCollection(this._results.Data.ChartPaneCollections[sec]);
            document.SetTradeData(this._results.Data.TradingInfoTracker.TradeManager);//设定交易数据
            document.SetPRData(this._results.Data.TradingInfoTracker.GetPositionRoundList().ToList());
            //while (this.x853fd1dd0c2b1c7d.ContainsKey(symbol))
            //{   //设定模型对应的交易数据
            //    document.SetTradeData(this.x853fd1dd0c2b1c7d[symbol]);
            //    break;
            //}
            //设定对应的ChartObjects
            //document.SetChartObjects(this.x10f21d244bfbeaab.SystemData.ChartObjects[symbol]);//加载模型中生成的ChartObjects
            //this.SymbolCharts[symbol] = document.InternalName;
            //document.ScrollChart(dateTime, 1);
            //x7d323be108bfd078.mainForm.SetActiveDocument(document);





        }

        void generator_SendNewBarEvent(SingleBarEventArgs barEventArgs)
        {
            throw new NotImplementedException();
        }

        private void showBackTestReport_Click(object sender, EventArgs e)
        {
            BackTestReport fm = new BackTestReport();
            BackTestData data = new BackTestData(null,this._results);
            fm.ShowBackTestReport(data);

            fm.Show();
        }
    }


}
