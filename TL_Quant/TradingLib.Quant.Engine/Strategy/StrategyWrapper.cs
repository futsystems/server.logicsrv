using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using System.Xml;
using System.Reflection;
using System.Threading;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 策略wrapper用于加载新的策略,回测,以及切换到实盘模式等
    /// 托管了策略的运行/停止等
    /// </summary>
    public class StrategyWrapper : MarshalByRefObject, IDisposable
    {
        const string PROGRAME = "StrategyWrapper";
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (this.Callbacks != null)
                Callbacks.Debug(msg);
                
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        //StrategyData _systemdata;
        StrategyRunner _runner;
        Assembly systemAssembly = null;
        StrategyData _strategydata;
        BarDataStreamer _barDataStreamer;
        private PluginFinder _finder;
        private ConstructorInfo _systemConstructor;
        private MarshalledSynchronizationContext _synchContext;
        private StrategyResults _results;
        private IQService _brokerService;
        IStrategy _system;
        private ICallbacks Callbacks { get; set; }


        Dictionary<Security, QList<Bar>> AccountInfoSymbols { get; set; }
        List<StrategyRunner> _runerlist = new List<StrategyRunner>();

        Dictionary<StrategyRunner, BarDataStreamer> runDataStreamerMap = new Dictionary<StrategyRunner, BarDataStreamer>();
        //保存了我们在内存中生成的Strategy实例,可以用于进行实盘或者模拟
        Dictionary<string, StrategyRunner> _strategyMap = new Dictionary<string, StrategyRunner>();


        public StrategyWrapper()
        {
            //MessageBox.Show("初始化StrategyWrapper..");
            //this._orderInformation = new OrderInformation();
            //this._subscribedSymbols = new Dictionary<FrequencyManager.FreqKey, int>();
            //this._subscribedSymbolUpdates = new Dictionary<FrequencyManager.FreqKey, SystemSymbolUpdates>();
            //LifetimeServices.LeaseTime = TimeSpan.FromDays(36500.0);
        } 

        /// <summary>
        /// 从本地数据源加载security对应的bar数据信息
        /// </summary>
        /// <param name="runData"></param>
        /// <param name="dataStore"></param>
        private void LoadAccountInfoSymbols(SharedSystemRunData runData, IDataStore dataStore)
        {
            this.AccountInfoSymbols = new Dictionary<Security, QList<Bar>>();
            //获得对应的Security系想你
            runData.RunSettings.Symbols.ToDictionary<SymbolSetup,Security>(s => s.Security);
            //遍历设置中的所有合约-频率 组合 并加载数据
            foreach (SecurityFreq freq in new List<SecurityFreq>(runData.InternalSettings.AccountInfoSymbols.Keys))
            {
                //从数据库加载Bars数据
                //QList<Bar> list2 = new QList<Bar>(dataStore.GetBarStorage.LoadBars(freq, DateTime.MinValue, runData.RunSettings.TradeStartDate, 1, true));
                //this.AccountInfoSymbols[freq.Security] = list2;
                //runData.InternalSettings.AccountInfoSymbols[freq] = list2;
            }
        }

        private void FindAccountInfoSymbols(SharedSystemRunData runData)
        {
            Dictionary<CurrencyType, AccountInfoSymbol> dictionary = new Dictionary<CurrencyType, AccountInfoSymbol>();
            Dictionary<CurrencyType, AccountInfoSymbol> dictionary2 = new Dictionary<CurrencyType, AccountInfoSymbol>();
            foreach (SecurityFreq freq in runData.RunSettings.Symbols)
            {
                Security symbol = freq.Security;
                dictionary[symbol.Currency] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.Interest, symbol.Currency);
                /*
                if (symbol.AssetClass == AssetClass.Forex)
                {
                    dictionary[symbol.BaseCurrency] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.Interest, symbol.BaseCurrency);
                    dictionary[symbol.CurrencyType] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.Interest, symbol.CurrencyType);
                    if (symbol.BaseCurrency != runData.RunSettings.AccountCurrency)
                    {
                        dictionary2[symbol.BaseCurrency] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.ExchangeRate, symbol.BaseCurrency);
                    }
                    if (symbol.CurrencyType != runData.RunSettings.AccountCurrency)
                    {
                        dictionary2[symbol.CurrencyType] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.ExchangeRate, symbol.CurrencyType);
                    }
                    continue;
                }
                if (symbol.CurrencyType != runData.RunSettings.AccountCurrency)
                {
                    dictionary2[symbol.CurrencyType] = new AccountInfoSymbol(runData, AccountInfoSymbol.SymbolType.ExchangeRate, symbol.CurrencyType);
                }**/
            }
            /*
            bool flag = true;
            if (flag && ((dictionary.Count > 0) || (dictionary2.Count > 0)))
            {
            Label_020F:
                foreach (SymbolSetup setup in runData.InternalSettings.AllSymbols)
                {
                    if (setup.Symbol.AssetClass == AssetClass.InterestRate)
                    {
                        CurrencyType currencyType = setup.Symbol.CurrencyType;
                        if (dictionary.ContainsKey(currencyType))
                        {
                            dictionary[currencyType].ProcessSymbol(setup);
                        }
                        goto Label_020F;
                    }
                    if (setup.Symbol.AssetClass != AssetClass.Forex)
                    {
                        goto Label_020F;
                    }
                    CurrencyType none = CurrencyType.None;
                    if (setup.Symbol.BaseCurrency == runData.RunSettings.AccountCurrency)
                    {
                        none = setup.Symbol.CurrencyType;
                    }
                    else if (setup.Symbol.CurrencyType == runData.RunSettings.AccountCurrency)
                    {
                        none = setup.Symbol.BaseCurrency;
                    }
                    if ((none != CurrencyType.None) && dictionary2.ContainsKey(none))
                    {
                        dictionary2[none].ProcessSymbol(setup);
                    }
                }
                List<AccountInfoSymbol> list = new List<AccountInfoSymbol>(dictionary2.Values);
                if (runData.RunSettings.ApplyForexInterest)
                {
                    list.AddRange(dictionary.Values);
                }
                foreach (AccountInfoSymbol symbol2 in list)
                {
                    if (runData.RunSettings.Symbols.Contains(symbol2.Symbol))
                    {
                        symbol2.AlreadyIncluded = true;
                    }
                }
                foreach (AccountInfoSymbol symbol3 in list)
                {
                    if (symbol3.Symbol != null)
                    {
                        runData.InternalSettings.AccountInfoSymbols[symbol3.Symbol] = null;
                    }
                }
            }**/
        }









        #region 加载交易系统/初始化交易系统
        /// <summary>
        /// 初始化交易系统
        /// </summary>
        /// <param name="syncControl"></param>
        /// <param name="mainDomain"></param>
        /// <param name="callbacks"></param>
        /// <param name="systemFileName"></param>
        /// <param name="systemClassName"></param>
        /// <param name="userAppDataPath"></param>
        public void Initialize(AppDomain mainDomain,ICallbacks callbacks, string systemFileName, string systemClassName, string userAppDataPath)
        {
            
            //XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(userAppDataPath, "LogSettings.xml")));
            //GlobalContext.Properties["AppDomain"] = "System";
            //log.Info("Initializing");
            PluginManager.LogLoadedAssemblies();
            //this.CreateSynchronizationContext(access.GetMainForm(), mainDomain);
            this._finder = new PluginFinder(false);
            this._finder.SetSearchPath(PluginGlobals.PluginDirectory);
            this.Callbacks = callbacks;
            this.LoadFrom(systemFileName, systemClassName);
            debug(PROGRAME+":StrategyWrapper加载成功");
        }
        
        private void LoadFrom(string AssemblyFilename, string systemClassName)
        {
            string friendlyname = string.Empty;
            //不同的应用程序域之间无法以这样的方式实现数据共享,这里必须自己重新加载
            //StrategyInfo s = QuantGlobals.Access.GetStrategyManager().GetStrategyInfo(friendlyname);
            //_systemConstructor = s.Constructor;
            if (string.IsNullOrEmpty(AssemblyFilename))
            {
                //this._systemConstructor = typeof(StrategyBase).GetConstructor(Type.EmptyTypes);
            }
            else
            {
                Assembly assembly = Assembly.LoadFrom(AssemblyFilename);
                this.systemAssembly = assembly;
                Dictionary<Type, ConstructorInfo> dictionary = new Dictionary<Type, ConstructorInfo>();
                Type type = typeof(IStrategy);
                foreach (Type type2 in assembly.GetExportedTypes())
                {
                    if (type.IsAssignableFrom(type2) && !type2.IsAbstract)
                    {
                        this._systemConstructor = type2.GetConstructor(Type.EmptyTypes);
                        if (this._systemConstructor != null)
                        {
                            dictionary[type2] = this._systemConstructor;
                        }
                    }
                }
                foreach (KeyValuePair<Type, ConstructorInfo> pair in dictionary)
                {
                    if (pair.Key.FullName == systemClassName)
                    {
                        this._systemConstructor = pair.Value;
                    }
                }
            }
            if (this._systemConstructor == null)
            {
                throw new QSQuantError("Could not find non-abstract class implementing ISystem with a constructor that takes no arguments in the assembly " + AssemblyFilename);
            }

        }
        #endregion


        #region 获得/加载运行结果文件
        private BarStatistic GetFinalStatistic()
        {
            StrategyResults results = this.GetResults();
            return results.Data.StratgyHistory.GetFinalStatistics(results.Data.StrategyStatistic);
            //return null;// results.Data.SystemHistory.GetFinalStatistics(results.Data.SystemStatistics);
        }

 
        /// <summary>
        /// 获得单一的系统运行报告
        /// </summary>
        /// <param name="resultsFilename"></param>
        /// <returns></returns>
        public SingleRunResults GetRunResults(string resultsFilename)
        {
            if (resultsFilename == null)
            {
                resultsFilename = Path.GetTempFileName();
            }
            this.SerializeResultsToFile(resultsFilename);

            StrategyResults results = this.GetResults();
            return new SingleRunResults { ResultsFile = resultsFilename, FinalStatistic = this.GetFinalStatistic()};//, RiskResults = results.RiskResults };
        }

        /// <summary>
        /// 将回测结果文件保存
        /// </summary>
        /// <param name="filename"></param>
        private void SerializeResultsToFile(string filename)
        {
            //debug(PROGRAME + ":保存回测结果到文件 " + filename);
            StrategyResults.Save(this.GetResults(), filename);
        }
        
        
        /// <summary>
        /// 得到回测运行生成的回测文件
        /// </summary>
        /// <returns></returns>
        public StrategyResults GetResults()
        {
            return this._results;
        }

        #endregion


        #region 关闭以及销毁交易系统
        public void Shutdown()
        {
            if (this._system != null)
            {
                this._system.Stop();
            }
            this._system = null;
            this.DisposeBrokerService();
            //Console.SetOut(this._oldConsoleOut);
            //Console.SetError(this._oldConsoleError);
        }

        /// <summary>
        /// 销毁策略
        /// </summary>
        public void Dispose()
        {
            if ((this._system != null) && (this._system is IDisposable))
            {
                (this._system as IDisposable).Dispose();
                this._system = null;
            }
        }
        /// <summary>
        /// 销毁broker service
        /// </summary>
        public void DisposeBrokerService()
        {
            if (this._brokerService != null)
            {
                this._brokerService.Disconnect();
                this._brokerService.Dispose();
                this._brokerService = null;
            }
        }
        #endregion





        /*
        private void CreateSynchronizationContext(Control control, AppDomain mainDomain)
        {
            this._synchContext = new MarshalledSynchronizationContext(control, mainDomain, delegate(object sender, ExceptionEventArgs args)
            {
                this.Callbacks.SendException(null, args);
            });
            SynchronizationContext.SetSynchronizationContext(this._synchContext);
        }
        **/



        #region 加载回测数据/运行回测系统

        /// <summary>
        /// 生成Broker服务
        /// </summary>
        /// <param name="brokerFactory"></param>
        private void CreateBroker(IServiceFactory brokerFactory)
        {
            //debug(PROGRAME + ":创建Strategy所对应的Broker...");
            if (SynchronizationContext.Current == null)
            {
                //SynchronizationContext.SetSynchronizationContext(this._synchContext);
            }
            if (this._brokerService != null)
            {
                this._brokerService.Dispose();
                this._brokerService = null;
            }
            this._brokerService = brokerFactory.CreateService();
            //this._brokerService.SendDebugEvent +=new DebugDelegate(debug);
            //debug("测试broker serverice debug infomation");
            //this._brokerService.GetBrokerInterface().SendOrder(new OrderImpl());
        }
        /// <summary>
        /// 从文件运行某个策略
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="brokerFactory"></param>
        /// <param name="dataStoreSettings"></param>
        public void RunSystem(string filename, IServiceFactory brokerFactory, PluginSettings dataStoreSettings)
        {
            Profiler.Instance.Reset();
            DateTime now = DateTime.Now;

            debug(PROGRAME + ":从临时文件恢复策略运行配置信息RunData");
            SharedSystemRunData data = null;// new SharedSystemRunData();
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (SerializationReader reader = new SerializationReader(stream))
                {
                    data.DeserializeOwnedData(reader, new StreamingContext());
                }
            }
            StrategyRunSettings runSettings = data.RunSettings;

            this.Callbacks.UpdateSimulateProgress(0, 1, DateTime.MinValue);
            now = DateTime.Now;
            IDataStore dataStore = (IDataStore)PluginGlobals.PluginManager.CreatePlugin(dataStoreSettings);
            
            this.CreatedNewStrategy(data, dataStore);
            
            this.RunStrateggy(this._strategydata, data, brokerFactory);
            DateTime.Now.Subtract(now);
            //Trace.WriteLine(Profiler.Instance.GetStatsString());
            //debug("系统运行时间分类统计\r\n" + Profiler.Instance.GetStatsString());
        }


        /// <summary>
        /// 策略初始化并进行启动/在这里建立策略指标的相关运算顺序
        /// </summary>
        /// <param name="systemData"></param>
        /// <param name="runData"></param>
        /// <param name="brokerFactory"></param>
        private void InitializeModule(StrategyData systemData, SharedSystemRunData runData, IServiceFactory brokerFactory)
        {
            debug(PROGRAME +":InitializeModule初始化相关模块 并绑定Broker");

            using (new Profile("InitializeModule"))
            {
                this.CreateBroker(brokerFactory);//生成对应的broker服务

                if (this._system != null)
                {
                    this._strategydata = systemData;
                    
                }

                try
                {
                    IndicatorDependencyProcessor processor = new IndicatorDependencyProcessor(true);
                    List<IndicatorInfo> infolist = new List<IndicatorInfo>(runData.InternalSettings.Indicators.Count + systemData.StrategyParameters.Keys.Count);
                    Dictionary<string, bool> dictionary = new Dictionary<string, bool>(runData.InternalSettings.Indicators.Count);
                    
                    foreach (IndicatorInfo info in runData.InternalSettings.Indicators)
                    {
                        dictionary[info.SeriesName] = true;
                        infolist.Add(info);
                    }
                    //检查strategyparameters中的名称与indicator是否有重复的地方
                    foreach (string str3 in systemData.StrategyParameters.Keys)
                    {
                        if (dictionary.ContainsKey(str3))
                        {
                            throw new QSQuantError("An indicator and a system parameter both used the name \"" + str3 + "\".These names must be unique.");
                        }
                    }
                    string seriesName = null;
                    //按照计算的指标更新顺序 进行初始化
                    foreach (IndicatorInfo info2 in processor.CalculateUpdateOrder(infolist))
                    {
                        seriesName = info2.SeriesName;
                        IIndicatorPlugin plugin = this._finder.LoadIndicatorPlugin(info2.IndicatorId);//查找对应的indicator
                        List<ConstructorArgument> args = this.ProcessConstructorArgs(info2.ConstructorArguments, systemData);
                        
                        ISeries indicator = this._finder.ConstructIndicator(plugin.GetIndicatorClassName(), args, false);
                        
                        //调用indicatorcollection 生成指标
                        systemData.Indicators[info2.SeriesName].CreateIndicator(indicator);
                            
                        object[] inputs = new object[info2.SeriesInputs.Count];
                        for (int i = 0; i < info2.SeriesInputs.Count; i++)
                        {
                            inputs[i] = info2.SeriesInputs[i].Value;
                        }
                        //给指标绑定输入数据
                        if (inputs.Length > 0)
                        {
                            systemData.Indicators[info2.SeriesName].SetInputs(inputs);
                        }
                        //是否在图表进行显示
                        if (info2.ShowInChart)
                        {
                            systemData.Indicators[info2.SeriesName].SeriesColor = info2.LineColor;
                            systemData.Indicators[info2.SeriesName].ChartPaneName = info2.ChartName;
                            systemData.Indicators[info2.SeriesName].LineSize = info2.LineSize;
                            systemData.Indicators[info2.SeriesName].LineType = info2.LineType;
                            systemData.Indicators[info2.SeriesName].AddToCharts();
                        }
                        else
                        {
                            systemData.Indicators[info2.SeriesName].RemoveFromCharts();
                        }
                    }


                
                }
                catch(Exception ex)
                {
                
                }




            }
            //绑定Broker的事件处理器 这里将positionManager中的事件绑定到外部(StrategyWrapper本地的处理器进行处理委托回报,成交信息等)
            //systemData.PositionManager.OrderFilled += new EventHandler<OrderFilledEventArgs>(this.PositionManager_OrderFilled);
            //systemData.PositionManager.OrderUpdated += new EventHandler<OrderUpdatedEventArgs>(this.PositionManager_OrderUpdated);
            //systemData.PositionManager.OrderSubmitted += new EventHandler<OrderUpdatedEventArgs>(this.PositionManager_OrderSubmitted);
            
            //设置交易模型所使用的交易接口
            this.SetStrategyBroker(new BrokerAccountState());
            //启动StrategyRunner
            this._runner.Start();
        }
        /// <summary>
        /// 设定策略的broker
        /// </summary>
        /// <param name="accountState"></param>
        private void SetStrategyBroker(BrokerAccountState accountState)
        {

            
            if (this._brokerService == null)
            {
                this._strategydata.Broker = null;
            }
            else
            {
                this._strategydata.Broker = (ISimBroker)this._brokerService.GetBrokerInterface();
                //this._strategydata.ExuctionBroker = this._brokerService.GetBrokerInterface();
                
                //this._brokerService.GetBrokerInterface().SetAccountState(accountState);
                //连接Broker
                if (!this._brokerService.Connect(ServiceConnectOptions.Broker))
                {
                    throw new QSQuantError("Error in broker service connect.\r\n" + this._brokerService.GetError());
                }
                //同步账户信息
                //this._brokerService.GetBrokerInterface().SyncAccountState();
            }
            //调用runner将策略接口与broker进行对接
            this._runner.SetBroker(this._brokerService.GetBrokerInterface());
        }


        /// <summary>
        /// 通过构造参数列表进行构造参数设置
        /// </summary>
        /// <param name="args"></param>
        /// <param name="baseSystem"></param>
        /// <returns></returns>
        private List<ConstructorArgument> ProcessConstructorArgs(List<ConstructorArgument> args, StrategyData baseSystem)
        {
            List<ConstructorArgument> list = new List<ConstructorArgument>();
            foreach (ConstructorArgument argument in args)
            {
                double num;
                ConstructorArgument item = argument;
                if ((((argument.Type == ConstructorArgumentType.Integer) || (argument.Type == ConstructorArgumentType.Double)) || (argument.Type == ConstructorArgumentType.Int64)) && ((argument.Value is string) && !double.TryParse(argument.Value as string, out num)))
                {
                    item = argument.Clone();
                    if (argument.Type == ConstructorArgumentType.Integer)
                    {
                        item.Value = (int)baseSystem.StrategyParameters[argument.Value as string];
                    }
                    else if (argument.Type == ConstructorArgumentType.Int64)
                    {
                        item.Value = (long)baseSystem.StrategyParameters[argument.Value as string];
                    }
                    else
                    {
                        item.Value = baseSystem.StrategyParameters[argument.Value as string];
                    }
                }
                list.Add(item);
            }
            return list;
        }



        /// <summary>
        /// 新建一个Strategy
        /// 每个策略需要有一个对应的OrderTracker/PositionTracker与之对应用于储存
        /// 策略运行所产生的委托/成交等信息
        /// </summary>
        public void CreatedNewStrategy(SharedSystemRunData data, IDataStore dataStore)
        {
            debug(PROGRAME +":CreatedNewStrategy创建交易系统实例与模拟数据集");
            using (new Profile("CreateNewSystem"))
            {
                
                //获得frequencyplugin 设置
                //data.RunSettings.BarFrequency = data.GetOrCreateSystemFrequencyPlugin();
                
                //如果是实盘模式 设定对应日期
                if (data.InternalSettings.LiveMode)
                {
                    data.RunSettings.TradeStartDate = DateTime.MaxValue;
                    data.RunSettings.EndDate = DateTime.MaxValue;
                }
                if (((data.RunSettings.DataStartDate > DateTime.MinValue) && (data.RunSettings.TradeStartDate > DateTime.MinValue)) && (data.RunSettings.LeadBars > 0))
                {
                    data.RunSettings.LeadBars = 0;
                }

                this.FindAccountInfoSymbols(data);

                List<SecurityFreq> symbols = data.RunSettings.Symbols.Cast<SecurityFreq>().ToList();
                //生成历史数据streame用于回测时得到对应的bar/tick数据 来驱动StrategyRunner
                
                _barDataStreamer = new BarDataStreamer(dataStore, symbols, data.RunSettings.DataStartDate, data.RunSettings.TradeStartDate, data.RunSettings.LeadBars, data.RunSettings.EndDate, data.InternalSettings.LiveMode, data.RunSettings.UseTicksForSimulation);
                this._barDataStreamer.LoadLeadBars();

                data.RunSettings.DataStartDate = this._barDataStreamer.DataStartDate;
                data.RunSettings.TradeStartDate = this._barDataStreamer.TradeStartDate;

                this.LoadAccountInfoSymbols(data, dataStore);

                //data.InternalSettings.SynchronizationContext = this._synchContext;

                //生成策略实例
                _system = (IStrategy)this._systemConstructor.Invoke(null);
                //_system.SendDebugEvent +=new DebugDelegate(debug);

                //生成策略运行数据环境
                _strategydata = new StrategyData(data.RunSettings, data.InternalSettings);
                _strategydata.SendDebugEvent +=new DebugDelegate(debug);
 

                //利用策略实例,策略运行数据环境,策略运行设置生成一个StrategyRunner
                _runner = new StrategyRunner(_system, _strategydata, data.RunSettings,debug);
                //_runner.SendDebugEvent +=new DebugDelegate(debug);
                //处理runner对应的系统事件

            }
    


        
        }

        bool _needupdateprogress = true;
        private delegate BarStreamData GetStreamData();

        GetStreamData GetSteamDataFunc;
        /// <summary>
        /// 按照一定的配置方式运行某个策略
        /// </summary>
        public void RunStrateggy(StrategyData strategydata, SharedSystemRunData runData, IServiceFactory brokerFactory)
        {
            debug(PROGRAME+":RunStrateggy 运行回测过程...");
            using (new Profile("RunSystem"))
            {
                //初始化相关模块 在这里运行相关初始化过程,然后启动策略
                this.InitializeModule(strategydata, runData, brokerFactory);

                DateTime startloop = DateTime.Now;
                DateTime now = DateTime.Now;
                DateTime minValue = DateTime.MinValue;
                long num = 0;
                debug(PROGRAME +":RunStrategy 初始化完毕,开始回测历史数据");
                using (new Profile("RunSystem loop"))
                {
                    Profiler.Instance.EnterSection("MainLoop");
                    debug(PROGRAME + ":加载回测数据 " + _barDataStreamer.TotalBars.ToString() + " Bars   |   " + _barDataStreamer.TotalTicks.ToString() + " Ticks");
                     DateTime barStartTime = DateTime.MinValue;
                     //int num2=0;
                    //通过载入数据的不同获得相应最合适的获得数据的函数调用
                     int totalItems= (int)(_barDataStreamer.TotalBars + _barDataStreamer.TotalTicks);
                    //回放bar数据
                     if (_barDataStreamer.TotalBars > 0 && _barDataStreamer.TotalTicks == 0)
                     {
                         GetSteamDataFunc = _barDataStreamer.GetNextBarItem;
                     }
                    //只回放Tick数据
                     else if (_barDataStreamer.TotalTicks > 0 && _barDataStreamer.TotalBars == 0)
                     {
                         GetSteamDataFunc = _barDataStreamer.GetNextTickItem;
                     }
                     //混合回放
                     else
                     {
                         GetSteamDataFunc = _barDataStreamer.GetNextItem;
                     }
                        
                     int i = 0;
                     while(true)
                     //while(i<20)
                     {
                         //Thread.Sleep(200);
                         //i++;

                         BarStreamData nextItem = GetSteamDataFunc();//_barDataStreamer.GetQuickNextTickItem();//单合约80tick/secend;//.GetNextTickItem();// 通过快速Tick数据回放 加速到74万/secned.GetNextItem();//获得一个bar或者一个tick事件
                        
                        if (nextItem == null)
                        {
                            //结束 跳出循环
                            break;
                        }
                        //debug("tick:" + nextItem.Tick.Tick.datetime.ToString() + nextItem.Tick.Tick.ToString());
                        //continue;//80万tick/sencend
                        
                        //更新进度状态
                        if(_needupdateprogress)
                        {
                            //tick计数
                            if (!nextItem.IsBarEvent)
                            {
                                num++;
                                //barStartTime = nextItem.NewBar.;
                            }
                            
                            using (new Profile("RunSystem.UpdateProgress"))
                            {
                                //Profiler.Instance.EnterSection("UpdateProgress");

                                if (DateTime.Now.Subtract(minValue).TotalSeconds > 0.5)//降低更新频率
                                {
                                    //只有在更新界面的时候 我们才需要时间信息
                                    if (nextItem.IsBarEvent)
                                    {
                                        barStartTime = nextItem.NewBar.BarStartTime;
                                    }
                                    else
                                    {
                                        barStartTime = Util.ToDateTime(nextItem.Tick.Tick.date, nextItem.Tick.Tick.time);
                                    }
                                    int currentItem = 0;
                                    //int totalItems = 1;
                                    //if ((_barDataStreamer.TotalBars + _barDataStreamer.TotalTicks) < int.MaxValue)
                                    {
                                        //totalItems = (int)(_barDataStreamer.TotalBars + _barDataStreamer.TotalTicks);
                                        currentItem = (int)(_barDataStreamer.BarsProcessed + num);
                                    }
                                    if (currentItem > totalItems)
                                    {
                                        throw new QSQuantError(string.Concat(new object[] { "Current progress (", currentItem, ") was greater than max progress (", totalItems, ")" }));
                                    }
                                    this.Callbacks.UpdateSimulateProgress(currentItem, totalItems, barStartTime);
                                    minValue = DateTime.Now;
                                }
                                //Profiler.Instance.LeaveSection();
                            }
                        }
                        
                        //Profiler.Instance.EnterSection("StrategyRunner");
                        //调用strategyrunner的对应接口运行newtick/newbar 驱动交易系统
                        if (nextItem.IsBarEvent)
                        {
                            DateTime time3 = new DateTime(0x7d9, 7, 0x11, 12, 0x2d, 0);
                            bool flag1 = nextItem.NewBar.BarStartTime >= time3;
                            this._runner.ProcessBar(nextItem.NewBar);
                            
                        }
                        else
                        {
                            //1.TickStream回放tick的速度 2.processTick的速度
                            //if(nextItem.Tick.Tick !=null)
                            this._runner.ProcessTick(nextItem.Tick.Symbol,nextItem.Tick.Tick);
                            //this._systemRunner.ProcessTick(nextItem.TickSymbol, nextItem.Tick);
                            //num++;
                        }
                        //DateTime time6 = DateTime.Now;
                        //num2++;
                        //Profiler.Instance.LeaveSection();
                     
                        
                     }
                     debug("Total process tick:" + num.ToString());
                     Profiler.Instance.LeaveSection();
                } //end runsystem.loop

                //debug("Finsh Baskting loop");
                DateTime endloop = DateTime.Now;
                double elaps = (endloop - startloop).TotalSeconds;
                int totalimtes = (int)(_barDataStreamer.TotalBars + _barDataStreamer.TotalTicks);
                debug("Total Time:" + elaps +" TotalItem:"+totalimtes.ToString() +"   Speed:"+(totalimtes / elaps).ToString() + " / secend");


                //debug("##################################################################\r\n");
                //_strategydata.StratgyHistory.ShowStatistics();
                //MessageBox.Show(_strategydata.StratgyHistory.StrategyStatistics.ToString());
                //this._runner.SendFinalBars();
                //4.更新显示图表
                using (new Profile("UpdateCharts"))
                {
                 
                    this._strategydata.IndicatorManager.UpdateCharts();
                }
                
                //this._systemData.PositionManager.GetPendingPositions();
                
                
                //5.获得系统运行的结果systemresoult用于下一步的分析交易信息 获得交易系统评价指标
                debug(PROGRAME +":生成策略回测结果文件");
                this._results = new StrategyResults(strategydata, runData.RunSettings);
                if (this._strategydata.LiveMode)
                {
                    //this._results.RiskResults = new List<RiskAssessmentResults>();
                }
                else
                {
                    using (new Profile("DoRiskAssessment"))
                    {
                        //this._results.RiskResults = new List<RiskAssessmentResults>(this._finder.DoRiskAssessment(this._results.Data, runData.InternalSettings.RiskAssessmentPluginIDs).Values);
                    }
                }
                if (runData.InternalSettings.ShutDownWhenDone)
                {
                    this.Shutdown();
                    _strategydata.Broker = null;
                }
                
            }
            GC.Collect();//回收系统资源
        }//end function RunStrateggy

        #endregion



        private sealed class AccountInfoSymbol
        {
            // Fields
            private SharedSystemRunData _runData;
            public bool AlreadyIncluded;
            public CurrencyType Currency;
            public SymbolSetup Symbol;
            public SymbolType Type;

            // Methods
            public AccountInfoSymbol(SharedSystemRunData runData, SymbolType type, CurrencyType currency)
            {
                this._runData = runData;
                this.Type = type;
                this.Currency = currency;
                PluginToken.Create(typeof(TimeFrequency), typeof(FrequencyPlugin));
                TimeFrequency orCreateSystemFrequencyPlugin = this._runData.GetOrCreateSystemFrequencyPlugin() as TimeFrequency;
                if (orCreateSystemFrequencyPlugin != null)
                {
                    this.Frequency = (int)orCreateSystemFrequencyPlugin.BarLength.TotalMinutes;
                }
                if (this.Frequency == 0)
                {
                    this.Frequency = 0x5a0;
                }
            }

            public void ProcessSymbol(SymbolSetup newSymbol)
            {
                /*
                if (this.Type == SymbolType.Interest)
                {
                    if ((newSymbol.Symbol.AssetClass != AssetClass.InterestRate) || (newSymbol.Symbol.CurrencyType != this.Currency))
                    {
                        return;
                    }
                }
                else if (this.Type == SymbolType.ExchangeRate)
                {
                    if (newSymbol.Symbol.AssetClass != AssetClass.Forex)
                    {
                        return;
                    }
                    if (((newSymbol.Symbol.CurrencyType != this.Currency) || (newSymbol.Symbol.BaseCurrency != this.AccountCurrency)) && ((newSymbol.Symbol.CurrencyType != this.AccountCurrency) || (newSymbol.Symbol.BaseCurrency != this.Currency)))
                    {
                        return;
                    }
                }
                if (this.Symbol == null)
                {
                    this.Symbol = newSymbol;
                }
                else
                {
                    int frequency = this.Symbol.Frequency;
                    int num2 = newSymbol.Frequency;
                    bool flag = false;
                    if ((num2 == this.Frequency) && (frequency != this.Frequency))
                    {
                        flag = true;
                    }
                    else if ((num2 > this.Frequency) && (frequency < this.Frequency))
                    {
                        flag = true;
                    }
                    else if ((num2 < this.Frequency) && (frequency < this.Frequency))
                    {
                        int num3 = this.Frequency - num2;
                        int num4 = this.Frequency - frequency;
                        if (num3 < num4)
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        this.Symbol = newSymbol;
                    }
                }**/
            }

            // Properties
            private CurrencyType AccountCurrency
            {
                get
                {
                    return this._runData.RunSettings.AccountCurrency;
                }
            }

            private int Frequency { get; set; }

            // Nested Types
            public enum SymbolType
            {
                Interest,
                ExchangeRate
            }
        }


    }



}
