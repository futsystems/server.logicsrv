using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant;
using TradingLib.Quant.Base;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Lifetime;
using System.ComponentModel;

using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 交易模块wrapper对StrategyModule进一步进行封装
    /// 在加载回测策略的时候 是新建一个appDomain,在改AppDomain中运行回测
    /// </summary>
    public class TradingModuleWrapper
    {
        const string PROGRAME = "TradingModuleWrapper";
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        DateTime _latestBarTime;
        TradingModuleWrapperArgs _args;
        private Dictionary<Security, int> _subscriptionCounts = new Dictionary<Security, int>();
        private AppDomain _systemDomain;
        private bool _systemLoaded = false;
        private StrategyWrapper _wrapper;
        private DelegateHolder _delegateHolder;
        private DelegateWrapper _delegateWrapper;

        




        public TradingModuleWrapper(TradingModuleWrapperArgs args)
        {
            //this._subscriptionCounts = new Dictionary<Symbol, int>();
            this._latestBarTime = DateTime.MinValue;
            this._args = args;
            this._delegateHolder = new DelegateHolder();
            this._delegateHolder.debug = debug;
            this._delegateWrapper = new DelegateWrapper(this._delegateHolder);//实现不同app域之间程序响应
        }

        void DebugUpdateSimProgress(int currentItem, int totalItems, DateTime currentTime)
        {
            debug("current:" + currentItem.ToString() + " total:" + totalItems.ToString() + " SimTime:" + currentItem.ToString());
        }

        /// <summary>
        /// 获得回测结果文件
        /// </summary>
        /// <param name="resultsFilename"></param>
        /// <returns></returns>
        public SingleRunResults GetRunResults(string resultsFilename)
        {
            return this.Wrapper.GetRunResults(resultsFilename);
        }
        /// <summary>
        /// 从回测结果文件加载交易系统回测结果
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static StrategyResults LoadResultsFromFile(string filename)
        {
            return StrategyResults.Load(filename);
        }



        /// <summary>
        /// 加载插件
        /// </summary>
        private void LoadPlugin()
        {
            //debug(PROGRAME + ":加载策略所需组件...");
            string[] strArray;
            this.UnloadDomain();//先卸载appdomain
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string directoryName = Path.GetDirectoryName(this._args.SystemFilename);
            //debug("filename:" + this._args.SystemFilename + "directoryName:" + directoryName);
            if (directoryName != string.Empty)
            {
                strArray = new string[] { directoryName };
            }
            else
            {
                strArray = new string[0];
            }

            //debug("生成 AppDomain");
            this._systemDomain = PluginGlobals.PluginManager.CreateAppDomain("BackTest Domain", strArray);
            debug(PROGRAME + ":生成StrategyWrapper ["+this._args.SystemFilename+"] 并初始化" );
            this._wrapper = (StrategyWrapper)this._systemDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(StrategyWrapper).FullName);
            //this._wrapper.SendDebugEvent +=new DebugDelegate(debug);

            //debug(PROGRAME + ":初始化wrapper");//将主appDomain/callback/delegateWrapper传递个StrategyWrapper
            this._wrapper.Initialize(currentDomain, this._delegateWrapper, this._args.SystemFilename, this._args.SystemClassName, BaseGlobals.AppDataDirectory);
            this._systemLoaded = true;

        }

        //从文件运行回测
        private void Run(string filename)
        {
            DateTime now = DateTime.Now;
            //debug("获得StrategyWrapper");
            StrategyWrapper wrapper = this.Wrapper;
            debug(PROGRAME + ":生成回测BrokerFactory");
            IServiceFactory brokerFactory = this._args.BrokerFactoryFactory.CreateFactoryInDomain(this.SystemDomain) as IServiceFactory;
            debug(PROGRAME + ":运行StrategyWrapper 进行回测");
            wrapper.RunSystem(filename, brokerFactory, this._args.DataStoreSettings);


        }

        /// <summary>
        /// 从rundata运行策略回测
        /// 1.主系统通过配置文件以及选择的symbol得到临时systemRunData
        /// 2.将sysRunData写入tmp文件 实现不同app domain之间的数据共享(为每个回测建立一个新的app domain便于程序管理以及程序运行安全)
        /// 
        /// </summary>
        /// <param name="systemRunData"></param>
        public void RunSystem(SharedSystemRunData systemRunData)
        {
            string tempFileName = Path.GetTempFileName();
            //debug(PROGRAME + ":将RunData写入临时文件:" + tempFileName);
            using (FileStream stream = new FileStream(tempFileName, FileMode.Truncate, FileAccess.Write))
            {
                using (SerializationWriter writer = new SerializationWriter(stream))
                {
                    systemRunData.SerializeOwnedData(writer, new StreamingContext());
                }
            }
            this.Run(tempFileName);//运行策略回测
            //IDictionary<Security, double> fallbackPrices = null;

            /*
            if (systemRunData.InternalSettings.ExistingPositions != null)
            {
                fallbackPrices = systemRunData.InternalSettings.ExistingPositions.CurrentPrices;
            }
            using (SerializationReader reader = new SerializationReader(this.Wrapper.GetAccountInfoSymbols()))
            {
                dictionary2 = SerializationUtils.Specialized.ReadAccountInfoSymbols(reader);
            }
            this._accountInfo = new WrapperAccountInfo(systemRunData.RunSettings.AccountCurrency, systemRunData.RunSettings.DataStartDate, dictionary2, fallbackPrices);
            **/
            File.Delete(tempFileName);
        }



        public  StrategyProgressUpdate UpdateProgressFunction
        {
            set { _delegateHolder.updateDelegate = value; }
        }


        public void SaveOpenPositions(string fileName)
        {
            //this.Wrapper.SaveOpenPositions(fileName);
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public void Shutdown()
        {
            if (this._wrapper != null)
            {
                this._wrapper.Shutdown();
            }
        }
        /// <summary>
        /// 销毁回测domain
        /// </summary>
        public void Dispose()
        {
            this.ShutdownDomain(true);
        }

        /// <summary>
        /// 关闭domain
        /// </summary>
        /// <param name="clearDelegates"></param>
        private void ShutdownDomain(bool clearDelegates)
        {
            try
            {
                if (clearDelegates && (this._delegateWrapper != null))
                {
                    this._delegateWrapper.UnhookDelegates();
                }
                if (this._wrapper != null)
                {
                    this._wrapper.DisposeBrokerService();
                    this._wrapper.Dispose();
                }
            }
            finally
            {
                this.UnloadDomain();
                this._wrapper = null;
            }
        }
        //卸载appdomain
        private void UnloadDomain()
        {
            if (this._systemDomain != null)
            {
                AppDomain.Unload(this._systemDomain);
                this._systemDomain = null;

            }
        }



        #region 属性
        private AppDomain SystemDomain
        {
            get
            {
                if (!this._systemLoaded)
                {
                    this.LoadPlugin();
                }
                return this._systemDomain;
            }
        }
 

        private StrategyWrapper Wrapper
        {
            get
            {
                if (!this._systemLoaded)
                {
                    this.LoadPlugin();
                }
                return this._wrapper;
            }
        }
 


        #endregion

        // Nested Types
        /// <summary>
        /// 封装了不同的几个委托/将函数绑定到该委托
        /// </summary>
        private class DelegateHolder
        {
            // Fields
            //public EventHandler<ConnectionEventArgs> connectionStateChanged;
            //public EventHandler<ExceptionEventArgs> exceptionDelegate;
            //public EventHandler<SystemOutputEventArgs> outputDelegate;
            public StrategyProgressUpdate updateDelegate;
            public DebugDelegate debug;
        }

        /// <summary>
        /// 将delegateholder进行封装 实现TradingModuleWrapper.ICallBacks.
        /// 这样在其他appDomian中的的对象就可以调用ICallbacks中相关函数 对maindomain进行操作
        /// 实现的一个ICallBack供策略在内部调用
        /// </summary>
        private class DelegateWrapper : MarshalByRefObject, ICallbacks, ISponsor
        {
            // Fields
            private TradingModuleWrapper.DelegateHolder _delegates;

            // Methods
            public DelegateWrapper(TradingModuleWrapper.DelegateHolder delegates)
            {
                this._delegates = delegates;
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            
            public TimeSpan Renewal(ILease lease)
            {
                return TimeSpan.FromSeconds(60.0);
            }
            /*
            public void SendConnectionStateChange(object sender, ConnectionEventArgs args)
            {
                EventHandler<ConnectionEventArgs> connectionStateChanged = this._delegates.connectionStateChanged;
                if (connectionStateChanged != null)
                {
                    connectionStateChanged(sender, args);
                }
            }

            public void SendException(object sender, ExceptionEventArgs args)
            {
                EventHandler<ExceptionEventArgs> handler = (this._delegates == null) ? null : this._delegates.exceptionDelegate;
                if (handler == null)
                {
                    throw new RightEdgeError(args.ex.Message, args.ex);
                }
                handler(sender, args);
            }

            public void SendOutput(object sender, SystemOutputEventArgs args)
            {
                EventHandler<SystemOutputEventArgs> handler = (this._delegates == null) ? null : this._delegates.outputDelegate;
                if (handler != null)
                {
                    handler(sender, args);
                }
            }**/

            public void UnhookDelegates()
            {
                this._delegates = null;
            }

            public void Debug(string msg)
            {
                DebugDelegate debug = (this._delegates == null) ? null : this._delegates.debug;
                if (debug != null)
                {
                    debug(msg);
                }
            }
            public void UpdateSimulateProgress(int currentItem, int totalItems, DateTime currentTime)
            {
                StrategyProgressUpdate update = (this._delegates == null) ? null : this._delegates.updateDelegate;
                if (update != null)
                {
                    update(currentItem, totalItems, currentTime);
                }
            }

        }



        



    }
}
