using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using TradingLib.API;

using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;
using TradingLib.Quant.Engine;

namespace TradingLib.Quant.GUI
{
    public delegate void StrategySimulationDel(ctStrategySimulation control);
    public partial class ctStrategySimulation : UserControl
    {

        public event StrategySimulationDel SendFinishEvent;
        //private TradingModuleWrapper _tradingModule;
        private IStrategyResults _results;

        public IStrategyResults Result { get { return _results; } }
        BackgroundWorker bw = new BackgroundWorker();
        SharedSystemRunData _rundata;
        public string StrategySetupFriendlyName { get { return _rundata.InternalSettings.StrategySettingFriendlyName; } }
        public ctStrategySimulation(SharedSystemRunData rundata)
        {
            InitializeComponent();

            _rundata = rundata;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            InitAndRunStrategy();
        }

        public void InitAndRunStrategy()
        {
            try
            {
                showTradeInChart.Enabled = false;
                showBackTestReport.Enabled = false;
                debug("开始回测");
                string filename = null;
                BarFrequency frequency;
                TradingModuleWrapperArgs args;


                ServiceAppDomainFactory factory = new ServiceManagerAppDomainFactory(QuantGlobals.Access.GetServiceManager().GetServiceSetup(QuantGlobals.PaperBroker), QuantGlobals.UserAppDataPath, QuantGlobals.PluginDirectory);

                //交易模块wrapper参数(将所有回测需要的数据生成args传递)
                args = new TradingModuleWrapperArgs();
                //args.Acccess = QuantGlobals.Access;
                args.BrokerFactoryFactory = factory;//Broker
                args.DataStoreSettings = QuantGlobals.DataStoreSetting;//数据读写插件
                args.SystemClassName = _rundata.InternalSettings.SystemClassName;//策略类名
                args.SystemFilename = _rundata.InternalSettings.StrategyFile;//策略文件

                TradingModuleWrapper wrapper = new TradingModuleWrapper(args);//this._xe146d4dea8d8c728.CreateScriptObject(args);
                wrapper.SendDebugEvent += new API.DebugDelegate(debug);
                wrapper.UpdateProgressFunction = this.ProgressUpdate;

                //MessageBox.Show(args.SystemFilename);

                wrapper.RunSystem(_rundata);

                //运行完毕后保存运行结果文件
                debug("加载策略回测结果文件到内存....");
                filename = wrapper.GetRunResults(null).ResultsFile;

                //运行结束后获得回测报告文件
                //filename = wrapper.GetRunResults(null).ResultsFile;
                if (filename != null)
                {
                    this._results = TradingModuleWrapper.LoadResultsFromFile(filename);
                    
                }

                //清理wrapper.并回收内存
                if (_rundata.InternalSettings.ShutDownWhenDone)
                {
                    wrapper.Shutdown();
                    wrapper.Dispose();
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                debug("回测失败 请检查配置或相关程序....");
                debug(ex.ToString());
            }
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
            _progress.Value = 0;
            _progress.Invalidate();
            if (this._results != null)
            {
                showBackTestReport.Enabled = true;
                showTradeInChart.Enabled = true;
            }

            //fmConfirm.Show("回测完成");

            if (SendFinishEvent != null)
                SendFinishEvent(this);
        }



        /// <summary>
        /// 输出日志到日志显示区域
        /// </summary>
        /// <param name="msg"></param>
        void debug(string msg)
        {
            ctDebug1.GotDebug(msg);
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

        void ProgressUpdate(int currentItem, int totalItems, DateTime currentTime)
        {
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new StrategyProgressUpdate(ProgressUpdate), new object[] { currentItem, totalItems, currentTime });
            else
            {
                double percent = ((double)currentItem) / ((double)totalItems);
                int p = (int)(percent * 100);
                //debug(p.ToString() +"cu:" + currentItem.ToString() + " to:" + totalItems.ToString());
                if (p > 100) p = 100;
                if (p < 0) p = 0;
                _progress.Value = p;
                _progress.Invalidate();
                //ProgressMessage(currentTime.ToString() + "    " + currentItem.ToString() + "/" + totalItems.ToString());
            }
        }


        private void showBackTestReport_Click(object sender, EventArgs e)
        {
            if (_results == null)
            {

                fmConfirm.Show("没有回测结果数据");
                return;
            }
            BackTestReport fm = new BackTestReport();
            BackTestData data = new BackTestData(null, this._results);
            fm.ShowBackTestReport(data);

            fm.Show();
        }

        private void showTradeInChart_Click(object sender, EventArgs e)
        {
            if (_results == null)
            {

                fmConfirm.Show("没有回测结果数据");
                return;
            }

            Security sec = this._results.Data.Symbols[0];
            SecurityFreq sf = new SecurityFreq(sec, this._results.Data.BarFrequency.BarFrequency);
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

            barFrequency = this._results.Data.BarFrequency.BarFrequency;
            if (barFrequency.Type != API.BarInterval.CustomTime)
            {
                throw new Exception("只能显示时间序列的价格图");
            }

            IDataStore datastore = QuantGlobals.Access.GetBarDataStoragePlugin();

            DateTime start = this._results.Data.DataStartDate;
            DateTime end = this._results.Data.EndDate;
            //如果是Bar模拟 则直接加载Bar数据  否则需要从Tick数据生成
            chartBars = BarUtil.IBarDataFromLocalStorage(sf, start, end);


            if (_results.Data.UseTicksForSimulation)
            {
                //MessageBox.Show("Tick数据回测");
                //generator = _results.Data.BarFrequency.CreateFrequencyGenerator();
                //generator.SendNewBarEvent += new SingleBarEventArgsDel(generator_SendNewBarEvent);

            }
            //MessageBox.Show(this._results.Data.Bars.Count.ToString());
            //调用全局MainForm生成Chart图表
            document = QuantGlobals.Access.CreateChartInstance(sf, this._results.Data.Bars["IF"]);
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
            document.SetChartObjects(this._results.Data.ChartObjects[sec]);//加载模型中生成的ChartObjects
            //this.SymbolCharts[symbol] = document.InternalName;
            //document.ScrollChart(dateTime, 1);
            //x7d323be108bfd078.mainForm.SetActiveDocument(document);
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            ctDebug1.Clear();
        }
    }
}
