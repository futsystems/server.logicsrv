using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 运行某个交易策略
    /// 以某种设置运行某个策略
    /// SIM/LIVE运行策略 均在StrategyRunner下进行运行
    /// IBroker用于执行策略的委托与取消等操作 IDataFeed用于执行策略的Tick回放等数据操作
    /// 数据优化就是利用优化配置,遍历生成可能的策略集 Strategs,然后在多个线程中运行StrategyRunner来进行模拟执行
    /// 最后完成后统一得到运行结构 进行数据比较
    /// StrategyRunner承载了系统外部数据传入给Strategy,同时接受IStrategy上传过来的数据 
    /// StrateBase 主要是提供简单/统一的策略操作入口给策略实体。提供仓位查询/委托查询等等
    /// 
    /// </summary>
    public class StrategyRunner:IFrequencyProcessor
    {

        const string PROGRAME = "StrategyRunner";
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        IStrategy _strategy;

        #region 属性

        public FrequencyPlugin SystemFrequency
        {
            get
            {
                return this._strategydata.BarFrequency;
            }
        }
        
        public StrategyRunSettings Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }
        public List<SecurityFreq> SymbolFreqs
        {
            get
            {
                List<SecurityFreq> sf = new List<SecurityFreq>();
                foreach (SymbolSetup s in _setting.Symbols)
                {
                    sf.Add(new SecurityFreq(s.Security, s.Frequency));
                }
                return sf;
            }
        }
        public bool UseTicksForSimulation { get; set; }



        #endregion

        StrategyRunSettings _setting;
        StrategyData _strategydata;
        TickGenerator _tickgenerator;//Tick数据发生器,1.live转发Tick数据 2.Bar回测将Bar分解成模拟的tick数据
        
        private EventHandler<FrequencyNewBarEventArgs> NewBars;
        private static Func<FrequencyManager.FreqInfo, DateTime> getupdatetimeFunc;
        private static Func<DateTime, DateTime> gettimeFunc;
        private static Func<FrequencyNewBarEventArgs, IEnumerable<SingleBarEventArgs>> getsingleeventFunc;
        private static Func<SingleBarEventArgs, DateTime> getsingleeventendtimeFunc;











        string LocalAccount = string.Empty;
        /// <summary>
        /// 初始化爱一个strategyrunner
        /// Strategy为策略提供了一个运行环境，包括策略实例,策略运行参数,策略运行数据
        /// 
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="strategydata"></param>
        /// <param name="runSetting"></param>
        public StrategyRunner(IStrategy strategy,StrategyData strategydata ,StrategyRunSettings runSetting,DebugDelegate debug=null)
        {
            LocalAccount = "0001";
            SendDebugEvent = debug;
            //this.debug("初始化StrategyRunner....###################################################");
            _strategy = strategy;//策略
            _strategydata = strategydata;//策略运行的数据环境
            _setting = runSetting;//策略运行/模拟配置文件

            
            Dictionary<Security, BarConstructionType> symbols = new Dictionary<Security, BarConstructionType>();
            foreach (Security symbol in _strategydata.Symbols)
            {
                symbols[symbol] = _strategydata.GetBarConstruction(symbol);
            }

            _tickgenerator = new TickGenerator(symbols);
            _tickgenerator.CreateTicks = _strategydata.CreateTicksFromBars;
            _tickgenerator.HighBeforeLow = runSetting.HighBeforeLowDuringSimulation;
            _tickgenerator.NewBar += new EventHandler<NewBarEventArgs>(_tickgenerator_NewBar);
            _tickgenerator.NewTick += new EventHandler<NewTickEventArgs>(_tickgenerator_NewTick);
            //绑定FrequencProcess到FrequencyManager用于相应FrequencyManager生成的tick数据等信息
            _strategydata.FrequencyManager.SetFrequencyProcessor(this);

            if (runSetting.UseTicksForSimulation)
            {
                UseTicksForSimulation = true;
            }
            else
            { 
                
            
            
            }

            
            
        
        }
        #region tickgenerator 事件回调
        //tickgenerator 调用
        void _tickgenerator_NewTick(object sender, NewTickEventArgs e)
        {

             this._strategydata.FrequencyManager.ProcessTick(e.Symbol, e.Tick);

        }

        //回测的时候tickgenerator会得到NewBar
        void _tickgenerator_NewBar(object sender, NewBarEventArgs e)
        {
            foreach (Security symbol in e.Symbols)
            {
                SingleBarEventArgs newBar = new SingleBarEventArgs(symbol, e[symbol], e.BarEndTime, e.TicksWereSent);
                this._strategydata.FrequencyManager.ProcessBar(newBar);
            }
            this._strategydata.FrequencyManager.UpdateTime(Util.ToTLDateTime(e.BarEndTime));//e.BarEndTime);
        }
        #endregion

        #region 策略newTick/newBar事件回调
        /// <summary>
        /// 调用策略newBar
        /// </summary>
        /// <param name="args"></param>
        private void CallSystemNewBar(FrequencyNewBarEventArgs args)
        {
            //bool flag = false;
            //SingleBarEventArgs bararg;
            
            //只发送系统主频率的Bar数据到策略,其他频率的数据的Bar事件需要单独订阅
            foreach (FreqKey key in args.FrequencyEvents.Keys)
            {
                if (key.Settings.Equals(this.SystemFrequency))
                {
                    //flag = true;
                    //MessageBox.Show(args.FrequencyEvents[key].Bar.ToString());
                    this._strategy.GotBar(args.FrequencyEvents[key].Bar);
                }
            }

            //if (flag)
            //{
            //    this._strategy.GotBar();
                //this._systemData.PositionManager.SetCountdownStarted();
           // }

            EventHandler<FrequencyNewBarEventArgs> newBars = this.NewBars;
            if (newBars != null)
            {
                newBars(this, args);
            }
        }

        int currentdate = 0;
        int currenttime = 0;
        /// <summary>
        /// 由FrequencyManager调用 fm处理完tick数据生成必要的bar数据后转发到这里传递给strategy
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tick"></param>
        public void CallSystemNewTick(Security symbol, Tick tick)//策略系统空载运行26-27万
        {
            //FrequencyManager检查了tick对应的频率 只发送主频率对应的Tick
            //Frequency frequency = this._strategydata.FrequencyManager.GetFrequency(symbol, this.SystemFrequency);
            //if (frequency.Bars.PartialBar == null)
            //{
            //    throw new QSQuantError(string.Format("Partial bar null for tick {0} {1} {2}", symbol, tick.time.ToString(), tick.ToString()));
            //}

            currentdate = tick.date;
            currenttime = tick.time;
            this.UpdateObjects(symbol,tick);//指标管理器更新指标计算数据速度1.指标本身的算法设计2.指标所需要用到的数值的获取 是否是最低层 最快(通过有限的数据封装将低层数据快速读取暴露给指标 这样才可以加快指标数据的更新熟读)
            this._strategydata.TradingInfoTracker.GotTick(tick);
            this._strategy.GotTick(symbol,tick,null);
           
        }
        #endregion



        #region 接收外部数据输入
        /// <summary>
        /// 响应live/回测 DataStream的Tick数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tick"></param>
        public void ProcessTick(Security symbol, Tick tick)
        {
            //Profiler.Instance.EnterSection("RunProcessTick");
            //this.VerifyBrokerConnected();//验证broker是否处于连接状态
            //_broker.SimTick(symbol, tick);
            //this._strategydata.FrequencyManager.ProcessTick(symbol, tick);//2.14 
            _frequencyManager.ProcessTick(symbol, tick);
            /*如果系统含有该symbol则才处理该symbol数据 这里是通过统一格式调用tickgenerator来转了一次 降低了速度 去掉
            if (this._strategydata.ContainsSymbol(symbol))
            {
                NewTickEventArgs args = new NewTickEventArgs
                {
                    Tick = tick,
                    Symbol = symbol,
                    PartialBar = null
                };
                this._tickgenerator.ProcessTick(args);
            }**/
            //Profiler.Instance.LeaveSection();
        }

 

        /// <summary>
        /// 响应回测系统的Bar数据,用于系统回测
        /// </summary>
        /// <param name="newBars"></param>
        public void ProcessBar(NewBarEventArgs newBars)
        {
            Profiler.Instance.EnterSection("Run ProcessBar");
            //debug("StrategyRunner processBar");
            //this.VerifyBrokerConnected();
            if (newBars.Symbols.Any<Security>(new Func<Security, bool>(this._strategydata.ContainsSymbol)))
            {
                NewBarEventArgs args = new NewBarEventArgs();
                foreach (Security symbol in newBars.Symbols)
                {
                    if (this._strategydata.ContainsSymbol(symbol))//如果systemdata含有该Bar则将该bar加入args
                    {
                        args.AddBar(symbol, newBars[symbol]);
                    }
                    else if (!newBars[symbol].EmptyBar)//如果该bar不为空
                    {
                        //this._systemData.AccountInfoSymbols[symbol].Add(newBars[symbol]);
                    }
                }
                args.BarStartTime = newBars.BarStartTime;
                args.BarEndTime = newBars.BarEndTime;
                args.TicksWereSent = newBars.TicksWereSent;
                newBars = args;
            }
            //FM没有使用多频率,tickgenerator不用生成Tick 并且系统不在live则调用FM直接处理Bar数据进行回测
            if ((!this._strategydata.FrequencyManager.UsingMultipleFrequencies && !this._tickgenerator.CreateTicks) && (!this._strategydata.LiveMode))
            {
                //debug("ProcessRunner 调用 FrequencyManager .ProcessBarsDirectly");
                this._strategydata.FrequencyManager.ProcessBarsDirectly(newBars);
            }
            else
            {
                //通过Tickgenerator处理Bar数据
                this._tickgenerator.ProcessBar(newBars);
            }
            Profiler.Instance.LeaveSection();
        }


        #endregion


        /// <summary>
        /// 处理Bar事件，FrequencyManager产生Bar数据后由FrequencyManager进行调用
        /// </summary>
        /// <param name="eventList"></param>
        public void ProcessBarEvents(IEnumerable<FrequencyNewBarEventArgs> eventList)
        {
            if (getsingleeventFunc == null)
            {
                getsingleeventFunc = new Func<FrequencyNewBarEventArgs, IEnumerable<SingleBarEventArgs>>(StrategyRunner.getSingleBarEvent);
            }
            if (getsingleeventendtimeFunc == null)
            {
                getsingleeventendtimeFunc = new Func<SingleBarEventArgs, DateTime>(StrategyRunner.getSingleBarEndTime);
            }
            DateTime barEndTime = eventList.SelectMany<FrequencyNewBarEventArgs, SingleBarEventArgs>(getsingleeventFunc).Max<SingleBarEventArgs, DateTime>(getsingleeventendtimeFunc);
            
            /*
            if (this._systemData.EnableTradeOnClose)
            {
                if (xcb0642a72e75e08e == null)
                {
                    xcb0642a72e75e08e = new Func<FrequencyNewBarEventArgs, IEnumerable<SingleBarEventArgs>>(SystemRunner.x5b5378ac540b495c);
                }
                if (x9c6ba31e2758ebf4 == null)
                {
                    x9c6ba31e2758ebf4 = new Func<SingleBarEventArgs, DateTime>(SystemRunner.x9cb5768df35ee544);
                }
                DateTime time3 = BarUtils.GetBarTime(eventList.SelectMany<FrequencyNewBarEventArgs, SingleBarEventArgs>(xcb0642a72e75e08e).Min<SingleBarEventArgs, DateTime>(x9c6ba31e2758ebf4), barEndTime, 1.0);
                this.CheckDate(time3);
                foreach (FrequencyNewBarEventArgs args in eventList)
                {
                    this.ProcessBarsInPaperBroker(args);
                }
                foreach (FrequencyNewBarEventArgs args2 in eventList)
                {
                    this.UpdateObjects(args2);
                    this.DoBarClose(args2);
                }
                this.CheckDate(barEndTime);
                foreach (FrequencyNewBarEventArgs args3 in eventList)
                {
                    this.CallSystemNewBar(args3);
                }
            }
            else**/
            {
                this.CheckDate(barEndTime);
                //Profiler.Instance.EnterSection("PaperTrade");
                //模拟broker处理bar撮合交易
                foreach (FrequencyNewBarEventArgs args4 in eventList)
                {
                    
                    //debug("StrategyRunner 准备调用paperbroker.processbar");
                    this.ProcessBarsInPaperBroker(args4);
                    
                }
                //Profiler.Instance.LeaveSection();

                //Profiler.Instance.EnterSection("Strategy");
                //调用策略newBar 某个频率下可能有多个延迟发送的Bar 一般情况 只有一个Bar被发送
                foreach (FrequencyNewBarEventArgs args5 in eventList)
                {
                    
                    //Profiler.Instance.EnterSection("Update Object");
                    this.UpdateObjects(args5);
                    //Profiler.Instance.LeaveSection();

                    //Profiler.Instance.EnterSection("CallStrategy");
                    this.CallSystemNewBar(args5);
                    //Profiler.Instance.LeaveSection();
                }
                //Profiler.Instance.LeaveSection();
            }
        }
        /// <summary>
        /// 以Bar基础更新指标等数据计算信息
        /// </summary>
        /// <param name="args"></param>
        private void UpdateObjects(FrequencyNewBarEventArgs args)
        {
            
            NewBarEventArgs systemFrequencyBars = this.GetSystemFrequencyBars(args);
            if (systemFrequencyBars != null)
            {
                this._strategydata.StratgyHistory.SimNewBar(systemFrequencyBars);
            }

            //调用指标管理器运算指标结构
            this._strategydata.IndicatorManager.NewBar(args);
            //this._systemData.Triggers.NewBar(args);
        }
        /// <summary>
        /// 以Tick为数据基础更新指标等数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="k"></param>
        private void UpdateObjects(Security symbol,Tick k)
        {
            //Profiler.Instance.EnterSection("runner Ind");
            this._strategydata.IndicatorManager.NewTick(symbol,k);
            //Profiler.Instance.LeaveSection();
        }






        /// <summary>
        /// 由FrequencyManager调用 paperbaroker处理tick
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="tick"></param>
        public void ProcessTickInPaperBroker(Security symbol,Tick tick)
        {
            //this.CheckDate(tick.time);
            this._strategydata.Broker.SimTick(symbol, tick);
            //this._processPosts();
        }

 


        /// <summary>
        /// 由内部函数processBarEvents处理Bar调用simbroker用于相应市场数据
        /// </summary>
        /// <param name="args"></param>
        private void ProcessBarsInPaperBroker(FrequencyNewBarEventArgs args)
        {
            NewBarEventArgs systemFrequencyBars = this.GetSystemFrequencyBars(args);
            if (systemFrequencyBars != null)
            {
                //log.DebugFormat("ProcessBarsInPaperBroker {0} {1}", systemFrequencyBars.BarStartTime, systemFrequencyBars.BarEndTime);
                //this._strategydata.Broker.SimBar(systemFrequencyBars);
                //this._processPosts();
            }
        }

        public void SendFinalBars()
        {
            if (!this._strategydata.LiveMode)
            {
                if (getupdatetimeFunc == null)
                {
                    getupdatetimeFunc = new Func<FrequencyManager.FreqInfo, DateTime>(StrategyRunner.getNextUpdateTime);
                }
                if (gettimeFunc == null)
                {
                    gettimeFunc = new Func<DateTime, DateTime>(StrategyRunner.getTime);
                }
                foreach (DateTime time in this._strategydata.FrequencyManager.GetAllFrequencies().Select<FrequencyManager.FreqInfo, DateTime>(getupdatetimeFunc).OrderBy<DateTime, DateTime>(gettimeFunc).Distinct<DateTime>().ToList<DateTime>())
                {
                    this._strategydata.FrequencyManager.UpdateTime(Util.ToTLTime(time));//time);
                    //this._processPosts();
                }
            }
        }


        #region 功能函数
        private void CheckDate(DateTime xccf8b068badcb542)
        {
            /*
            if (xccf8b068badcb542 < this._strategydata.CurrentDate)
            {
                throw new QSQuantError(string.Format("Out of order time: {0}", xccf8b068badcb542) + "  Current time: " + this._systemData.CurrentDate);
            }
            if (this._systemData.InLeadBars && (xccf8b068badcb542 >= this._systemData.TradeStartDate))
            {
                this._systemData.InLeadBars = false;
            }
            if (xccf8b068badcb542 > this._systemData.CurrentDate)
            {
                this_strategydata.CurrentDate = xccf8b068badcb542;
            }
            if (this._bNeedsStartDate)
            {
                this._strategydata.TradeStartDate = xccf8b068badcb542;
                if (this._systemData.DataStartDate == DateTime.MinValue)
                {
                    this._systemData.DataStartDate = xccf8b068badcb542;
                }
                this._bNeedsStartDate = false;
            }**/
        }

 
        /// <summary>
        /// 将一组frequencyNewBarEventArgs合并成 一个NewBarEventArgs
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private NewBarEventArgs GetSystemFrequencyBars(FrequencyNewBarEventArgs args)
        {
            NewBarEventArgs args2 = new NewBarEventArgs();
            bool flag = false;
            foreach (KeyValuePair<FreqKey, SingleBarEventArgs> pair in args.FrequencyEvents)
            {
                if (pair.Key.Settings.Equals(this.SystemFrequency))
                {
                    flag = true;
                    args2.AddBar(pair.Key.Symbol, pair.Value.Bar);
                    if (pair.Value.TicksWereSent)
                    {
                        args2.TicksWereSent = true;
                    }
                    if (pair.Value.BarEndTime > args2.BarEndTime)
                    {
                        args2.BarEndTime = pair.Value.BarEndTime;
                    }
                }
            }
            if (!flag)
            {
                return null;
            }/*
            foreach (Security symbol in this._strategydata.Symbols)
            {
                if (!args2.BarDictionary.ContainsKey(symbol) && (this._strategydata.Bars[symbol].Count > 0))
                {
                    Bar data2;
                    Bar current = this._strategydata.Bars[symbol].Last;
                    data2 = new BarImpl(this.SystemFrequency.BarFrequency.Interval);
                    
                    {
                        Open = data2.High = data2.Low = data2.Close = current.Close,
                        Bid = current.Bid,
                        Ask = current.Ask
                    };
                    args2.AddBar(symbol, data2);
                }
            }**/
            return args2;
        }

        /// <summary>
        /// 获得当前指标的最新数值
        /// </summary>
        /// <returns></returns>
        public Dictionary<Security, Dictionary<string, double>> GetLatestIndicatorValues()
        {
            return this._strategydata.IndicatorManager.GetLatestIndicatorValues();
        }

        /// <summary>
        /// 获得某个合约 某个频率的当前Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public Bar GetPartialBar(Security symbol, FrequencyPlugin frequency)
        {
            Frequency frequency2 = this._strategydata.FrequencyManager.GetFrequency(symbol, frequency);
            if (frequency2.WriteableBars.PartialBar != null)
            {
                return frequency2.WriteableBars.PartialBar;
            }
            return null;
        }
        private static DateTime getTime(DateTime t)
        {
            return t;
        }


        private static DateTime getNextUpdateTime(FrequencyManager.FreqInfo freqinfo)
        {
            DateTime nextTimeUpdateNeeded = freqinfo.Generator.NextTimeUpdateNeeded;
            if (nextTimeUpdateNeeded >= DateTime.MaxValue.Subtract(TimeSpan.FromDays(365.0)))
            {
                nextTimeUpdateNeeded = nextTimeUpdateNeeded.Subtract(TimeSpan.FromDays(365.0));
            }
            return nextTimeUpdateNeeded;
        }

        private static IEnumerable<SingleBarEventArgs> getSingleBarEvent(FrequencyNewBarEventArgs fe)
        {
            return fe.FrequencyEvents.Values;
        }

        private static DateTime getSingleBarEndTime(SingleBarEventArgs se)
        {
            return se.BarEndTime;
        }

        #endregion



        FrequencyManager _frequencyManager = null;
        /// <summary>
        /// 启动策略
        /// </summary>
        public void Start()
        {
            try
            {
                debug(PROGRAME + ":StrategyRunner 初始化易策略");

                //4.注册策略主频率
                _strategydata.FrequencyManager.RegisterFrequencies(this.SystemFrequency);
                _frequencyManager = _strategydata.FrequencyManager;//直接获得frequencymanaer 避免每次调用时都要访问 FrequencyManager属性中的get 与相关比较逻辑

                //1.执行strategy.start的代码部分,进行策略的初始化,在策略初始化的过程中,我们便已经注册了一些指标
                //启动完毕后 再调用indicatorManager.initialize
                _strategy.Start(_strategydata);



                //2.初始化指标管理器
                _strategydata.IndicatorManager.Initialize();//初始化指标管理器
                //3.更新图表
                _strategydata.IndicatorManager.UpdateCharts();//更新图表



                //
                _strategydata.StrategyStarted = true;//标志策略已经运行

                _tickgenerator.CreateTicks = _strategydata.CreateTicksFromBars;//设定是否需要从Bar数据生成模拟Tick数据


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        //Broker回报事件先路由给TradingInfoTracker用于记录信息并整理相关信息
        void ERouter_Broker2TradingInfoTracker()
        {
            _broker.GotOrderEvent += (Order o) => { _strategydata.TradingInfoTracker.GotOrder(o); };
            _broker.GotFillEvent += (Trade fill) => { _strategydata.TradingInfoTracker.GotFill(fill); };
            _broker.GotCancelEvent += (long val) => { _strategydata.TradingInfoTracker.GotCancel(val); };
            
        }
        //将tradingInfo的交易信息事件 转发给 策略
        void ERouter_TradingInfoTracker2Strategy()
        {
            _strategydata.TradingInfoTracker.GotOrderEvent += (Order o) => { _strategy.GotOrder(o); };
            _strategydata.TradingInfoTracker.GotCancelEvent += (long val) => { _strategy.GotOrderCancel(val); };
            _strategydata.TradingInfoTracker.GotFillEvent += (Trade fill) => { _strategy.GotFill(fill); };
            _strategydata.TradingInfoTracker.SendEntryPosiitonEvent += (Trade f,PositionDataPair data) => { _strategy.GotEntryPosition(f,data); };
            _strategydata.TradingInfoTracker.SendExitPositionEvent += (Trade f,PositionDataPair data) => { _strategy.GotExitPosiiton(f,data); };
        }
        //将策略的请求转发到本地sendorder / cancelorder
        //
        void ERouter_TradingInfoTracker2History()
        {
            _strategydata.TradingInfoTracker.SendHistoryPositionDataEvent += (Trade f, PositionDataPair data) => { _strategydata.StratgyHistory.GotFill(f, data); };
        }

        //取消委托
        /*
        void SendCancel(long val, int source)
        {
            _broker.CancelOrder(val);
        }
        //发送委托
        void SendOrder(Order o,int source)
        {
            o.date = currentdate;
            o.time = currenttime;
            o.Account = LocalAccount;
            _broker.SendOrder(o);
        }**/

        //实际发单的成交接口
        IExBroker _broker;
        public void SetBroker(IExBroker broker)
        {
            _broker = broker;
            //将交易接口绑定到StrategySupport :用于封装发单功能
            (_strategydata.StrategySupport as StrategySupport).SetBroker(_broker);
            //将具体的Broker事件进行绑定
            
            ERouter_Broker2TradingInfoTracker();//将borker事件转发到trainginfotracker
            ERouter_TradingInfoTracker2History();//将TradingInfo的成交事件转发到StrategyHistory进行
            ERouter_TradingInfoTracker2Strategy();//将tradinginfotracker事件转发到策略
            
        }

        /*关于系统内交易信息路由的传递规划
         * Broker组件承接了对外发送委托/取消  以及转发从实体Broker传递过来的委托回报/成交回报/以及其他相关信息
         * StrategyRunner提供了策略运行的环境 初始化时 需要IStrategy(策略) StratgyData(策略运行环境数据) StrategyRunSetting(策略运行配置)
         * 
         * 理论上Broker应该与StrategyRunner进行对接,在StrategyRunner里面IStrategy将委托 成交等按统一格式转换通过Broker的接口对外发送
         * 从Broker接口过来的交易信息 先通过TradingInfo记录器进行记录 然后再转发给对应的策略
         * 
         * 在实盘运行过程中可能会出现多个策略通过不同的接口下单。
         * 这里需要有一个交易路由服务存在,交易路由服务封装成一个虚拟Broker该broker可以通过设置把策略发送过来的委托正确的发送到对应的接口上去
         * 然后把不同接口返回过来的交易信息正确的转发到不同的策略。同时该路由服务需要将交易信息进行本地化储存
         * 在实盘模式下strategyrunner并不需要每次都生成不同的broker实例子，实盘运行时候只是生成了一个虚拟的Broker安全引用。然后通过它对外下单。
         * 同时返回过来的委托 在策略内部再通过同样的方式进行 交易信息收集
         * 
         * 
         * 
         * 系统性能 策略空载/broker空载 50/s
         * 
         * 是否有表要整理一个类似的PositionManager用于封装一些下单操作 方便操作反手 /手数 为0的自定义计算 / 委托定时发送拓展等
         * 
         * */
    }
}
