using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant;
using TradingLib.API;
using System.Threading;
using System.Windows.Forms;
using System.IO;

using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;


namespace TradingLib.Quant.Engine
{
    public class OptimizationRunner : IOptimizationServices
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private TradingModuleWrapperArgs _args;
        //private RightEdgeCompiler _compiler;
        private PluginSettings _optimizationPluginSettings;
        private string _optimizationSettingsFile;
        //private EventHandler<SystemOutputEventArgs> _outputDelegate;
        private List<StrategyParameterInfo> _parameters;
        private OptimizationPlugin _plugin;
        private Form _progressDialog;
        private List<OptimizationResult> _results;
        private SharedSystemRunData _runData;
        private int _runNumber;
        private Thread _thread;

        // Methods
        /// <summary>
        /// 初始化优化运行期,优化运行器需要提供runData,用于生成tradingmodule的args,以及对应的优化插件
        /// </summary>
        /// <param name="runData"></param>
        /// <param name="args"></param>
        /// <param name="optimizationPluginSettings"></param>
        /// <param name="parameters"></param>
        /// <param name="optimizationSettingsFile"></param>
        public OptimizationRunner(SharedSystemRunData runData, TradingModuleWrapperArgs args, PluginSettings optimizationPluginSettings, List<StrategyParameterInfo> parameters, string optimizationSettingsFile)//, EventHandler<SystemOutputEventArgs> outputDelegate)
        {
            this._runData = runData;
            this._args = args;
            //this._compiler = compiler;
            this._optimizationPluginSettings = optimizationPluginSettings;
            this._parameters = (from p in parameters select p.Clone()).ToList<StrategyParameterInfo>();
            foreach (StrategyParameterInfo info in this._parameters)
            {
                //校对策略参数数据
                if (info.NumSteps <= 0)
                {
                    info.NumSteps = 1;
                    info.Low = info.Value;
                    info.High = info.Value;
                }
            }
            //优化设置文件
            this._optimizationSettingsFile = optimizationSettingsFile;
            //this._outputDelegate = outputDelegate;
        }

        public void AbortOptimization()
        {
            this._thread.Abort();
            this._thread.Join();
            this._progressDialog.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 生成默认的进度窗口
        /// </summary>
        /// <param name="cancelCallback"></param>
        /// <returns></returns>
        public Form CreateDefaultProgressWindow(Action cancelCallback)
        {
            OptimizationProgressDialog dialog = new OptimizationProgressDialog();
            _dialog = dialog;
            dialog.CancelOptimEvent +=new VoidDelegate(ProgressDialog_CancelClicked);//传递取消事件
            return dialog;
        }
        OptimizationProgressDialog _dialog;

        public void SetTotalRunNumber(int totalrun)
        { 
            _dialog.SetTotalRunNumber(totalrun);
        }
        //生成优化算法插件,用于执行优化计算
        private void CreateOptimizationPlugin()
        {
            debug("加载优化插件");
            /*
            if (this._optimizationPluginSettings == null)
            {
                throw new Exception("Optimization Plugin not selected");
            }
            this._plugin = (OptimizationPlugin)QuantGlobals.PluginManager.CreatePlugin(this._optimizationPluginSettings);
            this._plugin.Initialize(this);**/

            //this._plugin = (OptimizationPlugin)new DefaultOptimizationPlugin();
            //(this._plugin as DefaultOptimizationPlugin).SendDebugEvent +=new DebugDelegate(debug);
            //(this._plugin as DefaultOptimizationPlugin).ThreadsToUse = 4;
            //this._plugin.Initialize(this);


        }

        //进度窗口点击取消,则我们通知优化插件取消优化计算
        private void ProgressDialog_CancelClicked()
        {
            this._plugin.CancelOptimization();
        }

        //进度窗口加载
        private void ProgressDialog_Loaded()
        {
            debug("进度窗口加载 并生成优化线程");
            this._thread = new Thread(startOptim);
            this._thread.Name = "Optimization thread";
            this._thread.Start();
        }

        void startOptim()
        {
            Action method = null;
            try
            {
                debug("开始运行优化过程.....");
                this._results = this._plugin.RunOptimization(this._runData.RunSettings);
                if (method == null)
                {
                    method = delegate
                    {
                        this._progressDialog.DialogResult = DialogResult.OK;
                    };
                }
                this._progressDialog.BeginInvoke(method);
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                //this._progressDialog.BeginInvoke(showError);
            }
            
        }

        void showError()
        { 
            //if (!(ex is ThreadAbortException) && !(ex is OperationCanceledException))
            //        {
            //            //SharedGlobals.ShowExceptionDialog(ex);
            //        }
                    this._progressDialog.DialogResult = DialogResult.Cancel;
        }

        //执行策略回测,并将策略回测结果组成对应的策略优化结果进行返回
        OptimizationResult IOptimizationServices.RunSystem(StrategyRunSettings runSettings, StrategyProgressUpdate progressCallback)
        {
            debug("defalutoptim call runner to runsystem");
            try
            {
                //this._compiler.GenerateDebugInformation = true;
                TradingModuleWrapper wrapper = new TradingModuleWrapper(this._args);
                wrapper.SendDebugEvent +=new DebugDelegate(debug);

                wrapper.UpdateProgressFunction = progressCallback;
                //wrapper.OutputDelegate = this._outputDelegate;
                SharedSystemRunData systemRunData = this._runData.ShallowClone();//复制统一的runData
                systemRunData.RunSettings = runSettings;//将优化插件传递过来的runSetting绑定到运行时的策略参数组合
                wrapper.RunSystem(systemRunData);

                OptimizationResult result = new OptimizationResult
                {
                    RunNumber = Interlocked.Increment(ref this._runNumber)
                };
                Directory.CreateDirectory(systemRunData.InternalSettings.OutputDir);
                result.ResultsFile = Path.Combine(systemRunData.InternalSettings.OutputDir, "results" + result.RunNumber + ".sysresults");
                SingleRunResults runResults = wrapper.GetRunResults(result.ResultsFile);
                result.FinalStatistic = runResults.FinalStatistic;
                //result.RiskResults = runResults.RiskResults;
                result.ParameterValues = runSettings.StrategyParameters;
                return result;
                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                return null;
            }
        }

        //显示默认的优化参数设置 然后将设定的数据绑定到本地参数
        bool IOptimizationServices.ShowDefaultOptimizationSettingsDialog(IWin32Window owner)
        {
            using (OptimizationDialog dialog = new OptimizationDialog(this._parameters,_runData.InternalSettings.StrategySettingFriendlyName))
            {
                if (dialog.ShowDialog(owner) != DialogResult.OK)
                {
                    return false;
                }
                this._parameters.Clear();
                this._parameters.AddRange(from p in dialog.Parameters select p.GetUpdatedSystemParameter());
                this._plugin.ThreadsToUse = dialog.ThreadToOpen;
                return true;
            }
        }
        /// <summary>
        /// 更新进度信息
        /// </summary>
        /// <param name="progressItems"></param>
        void IOptimizationServices.UpdateProgress(List<OptimizationPlugin.ProgressItem> progressItems,int currentitem)
        {
            if (this._progressDialog == null)
            {
                throw new InvalidOperationException("Cannot call UpdateProgress before the optimization progress window has been created.");
            }
            IOptimizationProgressUpdate progressUpdate = this._progressDialog as IOptimizationProgressUpdate;
            if (progressUpdate == null)
            {
                throw new InvalidOperationException("The optimization progress dialog must implement the " + typeof(IOptimizationProgressUpdate).FullName + " interface in order for you to call UpdateProgress.");
            }
            //利用委托,从其他线程调用UI线程的界面更新
            this._progressDialog.BeginInvoke(new beginInvokeDelegate(updateprogressitem),new object[]{progressUpdate,progressItems,currentitem});
        }
        private delegate void beginInvokeDelegate(IOptimizationProgressUpdate progressUpdate, List<OptimizationPlugin.ProgressItem> progressItems,int currentitem);
        void updateprogressitem(IOptimizationProgressUpdate progressUpdate, List<OptimizationPlugin.ProgressItem> progressItems,int currentitem)
        {
            try
            {
                progressUpdate.UpdateProgress(progressItems,currentitem);
            }
            catch (Exception ex)
            {
                this._plugin.CancelOptimization();
                debug(ex.ToString());
                //SharedGlobals.ShowExceptionDialog(exception);
            }
        }

        /// <summary>
        /// 运行优化运算过程
        /// </summary>
        /// <returns></returns>
        public List<OptimizationResult> RunOptimization()
        {
            debug("运行优化过程...");
            this.CreateOptimizationPlugin();//生成优化器插件
            //如果有配置文件 则插件加载配置文件运行优化
            if (!string.IsNullOrEmpty(this._optimizationSettingsFile))
            {
                this._plugin.LoadOptimizationSettingsFromFile(this._runData.RunSettings, this._optimizationSettingsFile);
            }
            //如果没有配置文件则显示默认的优化参数设置窗口进行设置
            else if (!this._plugin.ShowOptimizationSettings(this._runData.RunSettings, this._args.MainForm))
            {
                return null;
            }

            //生成优化进度窗口
            this._progressDialog = this._plugin.CreateProgressWindow(new Action(this.ProgressDialog_CancelClicked));
            this._progressDialog.Load += delegate
            {
                this.ProgressDialog_Loaded();
            };

            //如果优化进度窗口返回OK则我们返回对应的结果
            if (this._progressDialog.ShowDialog(this._args.MainForm) == DialogResult.OK)
            {
                return this._results;
            }
            return null;
        }

        // Properties
        List<StrategyParameterInfo> IOptimizationServices.OptimizationParameters
        {
            get
            {
                return this._parameters;
            }
        }

        public List<StrategyParameterInfo> SystemParameters
        {
            get
            {
                return this._parameters;
            }
        }
    }


}
