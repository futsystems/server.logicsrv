using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    
    /// <summary>
    /// 原来行情通道通过交易所进行路由识别
    /// 比如订阅某个行情,按照其交易所找到设置中的行情通道 然后执行订阅
    /// 对于这一步理论上设计比较好，不过目前所有的行情都是通过FastTick进行传送
    /// FastTickSrv对接所有的行情源然后形成统一的订阅与传输格式对外传出，这样避免了在多点去维护不同的行情与订阅信息
    /// 这里只要针对FastTick进行订阅就可以了
    /// </summary>
    public class DataFeedRouter:BaseSrvObject,IDataRouter
    {
        const string ComponentName = "DataFeedRouter";
        TickWatcher _tickwatcher;
        List<MktTime> _mkttimespanlist = new List<MktTime>();

        ConfigDB _cfgdb;
        int _symidlespan = 5;
        int _massalertspan = 5;
        int _alertspan = 10;
        public DataFeedRouter()
            : base(ComponentName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(ComponentName);
            if (!_cfgdb.HaveConfig("SymbolIdleSpan"))
            {
                _cfgdb.UpdateConfig("SymbolIdleSpan", QSEnumCfgType.Int,10, "合约行情处于非激活状态的判定时间,多少秒后该合约判定为行情异常");
            }
            _symidlespan = _cfgdb["SymbolIdleSpan"].AsInt();

            if (!_cfgdb.HaveConfig("MassAlertSpan"))
            {
                _cfgdb.UpdateConfig("MassAlertSpan", QSEnumCfgType.Int,5, "行情通道整体异常,多少秒内在行情时间端内没有任何Tick,则行情通道触发MassAlert");
            }
            _massalertspan = _cfgdb["MassAlertSpan"].AsInt();

            if (!_cfgdb.HaveConfig("AlertSpan"))
            {
                _cfgdb.UpdateConfig("AlertSpan", QSEnumCfgType.Int,10, "单个合约行情异常，合约当前行情时间距离上次行情时间超过设定时间后，则触发行情Alert");
            }
            _alertspan = _cfgdb["AlertSpan"].AsInt();

            _ticktracker = new TickTracker();
            asynctick = new AsyncResponse("DataFeedRouter");
            asynctick.GotTick += new TickDelegate(asynctick_GotTick);
            //建立合约列表用于记录所维护的行情数据
            mb = new SymbolBasketImpl();
            //生成TickWatcher用于维护行情数据状态
            _tickwatcher = new TickWatcher(true, _ticktracker);


            _tickwatcher.SymbolIdleSpan = _symidlespan;//单个行情延迟5秒 则该合约处于非活动状态
            _tickwatcher.AlertThreshold = _alertspan;//行情时间间隔超过5秒触发报警
            _tickwatcher.GotAlert += new SymDelegate(_tickwatcher_GotAlert);
            _tickwatcher.GotFirstTick += new SymDelegate(_tickwatcher_GotFirstTick);

            _tickwatcher.MassAlertThreshold = _massalertspan;//行情整体报警 5秒内没有更新任何行情则报警
            _tickwatcher.GotMassAlert += new Int32Delegate(_tickwatcher_GotMassAlert);
            _tickwatcher.GotMassAlertCleard += new Int32Delegate(_tickwatcher_GotMassAlertCleard);

            _snapshotcahefile = Path.Combine(new string[] { "cache", "ticksnapshot" });
            _mkttimespanlist.Add(new MktTime(90000, 113000));//上午 9:00:00-11:30:00
            _mkttimespanlist.Add(new MktTime(130000, 151500));//下午13:00:00-15:15:00
            _mkttimespanlist.Add(new MktTime(210000, 235959));//夜盘23:59:59秒
            _mkttimespanlist.Add(new MktTime(0, 23000));//凌晨00:00:00-2:30:00

        }

        /// <summary>
        /// 获得路由状态
        /// </summary>
        /// <returns></returns>
        public DataFeedRouterStatus GetRouterStatus()
        {
            DataFeedRouterStatus status = new DataFeedRouterStatus();

            if (TLCtxHelper.ServiceRouterManager.DefaultDataFeed == null)//如果行情通道不存在 则为false
            {
                status.IsDefaultDataFeedLive = false;
            }
            else
            {
                status.IsDefaultDataFeedLive = TLCtxHelper.ServiceRouterManager.DefaultDataFeed.IsLive;//如果行情通道存在 则设定DefaultDataFeed的当前工作状态
            }

            status.MassAlert = _tickwatcher.isMassAlerting;//是否处于报警状态
            status.IsTickSpan = _tickwatcher.TimeSpanSetted;//是否设定了行情有效时间段

            return status;

        }
        
        #region 行情自我诊断与维护系统

        /* 行情监控系统
         * 1.在设定的开始于结束时间段内 如果最新行情时间与当前时间偏差达到设定阀值时 触发行情异常警报 如果行情恢复 则警报解除
         * 2.在某个交易时间段前更新时间段 准备等待行情
         * 
         * 
         * 
         * 
         * 
         * */
        /// <summary>
        /// 更新当前行情时间段
        /// </summary>
        [TaskAttr("更新TickWatcher", 1, 0, "自动更新TickWatch时间段设置")]
        public void Task_UpdateTickWatcher()
        {
            bool isinspan = false;
            foreach (MktTime t in _mkttimespanlist)
            {
                //如果当前时间在某个时间段内，则更新tickwatcher的开始与结束时间
                if(t.IsInSpan(Util.ToTLTime()))
                {
                    isinspan = true;
                    //如果TickWatcher没有设定开始于结束时间 则设定
                    if (!_tickwatcher.TimeSpanSetted)
                    {
                        Util.Debug("Now we are in Tick TimeSpan,but have not set TickWatcher,set TimeSpan as:" + t.ToString(), QSEnumDebugLevel.WARNING);
                        _tickwatcher.UpdateTimeSpan(t.StartTime, t.EndTime);
                    }
                }
            }
            //debug("now:" + Util.ToTLTime() + " inspan:" + isinspan.ToString(), QSEnumDebugLevel.INFO);
            //如果不在时间段内 则需要判断当前时间 是否是某个时间段的前5秒,在某个时间段的前30秒更新TickWatcher的TimeSpan
            if (!isinspan)
            {
                bool preset = false;
                foreach (MktTime t in _mkttimespanlist)
                {
                    int diff = t.StartDiff;//距离开始还有多少秒
                    //Util.Debug("Time diff  Now:" + now.ToString() + " Start:" + t.StartTime.ToString() + " Diff:" + diff.ToString(), QSEnumDebugLevel.WARNING);
                    if (diff<=5)
                    {
                        preset = true;
                        if (!_tickwatcher.TimeSpanSetted || (_tickwatcher.TimeSpanSetted && (_tickwatcher.StartAlertTime != t.StartTime || _tickwatcher.StopAlertTime != t.EndTime)))
                        {
                            Util.Debug("Tick will come in less than 5 secends,set TimeSpan first:" + t.ToString(), QSEnumDebugLevel.WARNING);
                            _tickwatcher.UpdateTimeSpan(t.StartTime, t.EndTime);
                        }
                        
                        
                        //debug(t.ToString() + " will be active in less than 30s.. diff:"+diff.ToString(), QSEnumDebugLevel.WARNING);
                    }
                }
                //如果不再行情覆盖时间段内
                if (!preset && _tickwatcher.TimeSpanSetted)
                {
                    Util.Debug("we are levae Tick TimeSpan ,reset TickWatcher", QSEnumDebugLevel.WARNING);
                    _tickwatcher.Reset();
                }
            }
            
        }

        /// <summary>
        /// 行情异常解除
        /// </summary>
        /// <param name="val"></param>
        void _tickwatcher_GotMassAlertCleard(int val)
        {
            debug("MassAlart cleard, the latest tick time:" + val.ToString(), QSEnumDebugLevel.WARNING);
        }

        /// <summary>
        /// 行情异常报警
        /// 报警后我们要执行相关任务 比如重启行情通道等
        /// </summary>
        /// <param name="val"></param>
        void _tickwatcher_GotMassAlert(int val)
        {
            debug("MassAlert," +"the latest tick time:"+_tickwatcher.RecentTime.ToString()+" there is no tick in "+_tickwatcher.MassAlertThreshold.ToString()+" secends"  , QSEnumDebugLevel.WARNING);
            
            /* 如果我们在夜盘没有订阅任何数据 则接口会一直重启
            IDataFeed df = TLCtxHelper.Ctx.RouterManager.DefaultDataFeed;
            debug("Reconnect Default DataFeed:"+df.Token, QSEnumDebugLevel.INFO);
            if (df != null)
            {
                if (df.IsLive)
                {
                    df.Stop();
                    Util.sleep(500);
                }
                df.Start();
            }
            **/
        }

        /// <summary>
        /// 首个行情到达
        /// </summary>
        /// <param name="sym"></param>
        void _tickwatcher_GotFirstTick(string sym)
        {
            debug("symbol:" + sym + "'s tick first arive..", QSEnumDebugLevel.WARNING);
        }

        /// <summary>
        /// 单个行情延迟警报
        /// </summary>
        /// <param name="sym"></param>
        void _tickwatcher_GotAlert(string sym)
        {
            Util.Debug("symbol:" + sym + " have no tick in " + _tickwatcher.AlertThreshold.ToString()+" secends.", QSEnumDebugLevel.WARNING);
        }

        #endregion


        /// <summary>
        /// 加载行情通道
        /// 绑定行情通道的 行情到达事件和行情连接事件
        /// </summary>
        /// <param name="datafeed"></param>
        public void LoadDataFeed(IDataFeed datafeed)
        { 
            //将数据通道的Tick转发到datafeedrouter然后再转发到TradingServer
            debug("load datafeed:xxxxxxxxxxxxxxxxxxxxxx", QSEnumDebugLevel.INFO);
            datafeed.GotTickEvent +=new TickDelegate(GotTick);
            datafeed.Connected += new IConnecterParamDel(datafeed_Connected);
            
        }


        #region 获得某个合约的行情通道
        /// <summary>
        /// 获得某个合约的行情数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IDataFeed GetDataFeed(Symbol symbol)
        {
            //如果默认的行情路由存在并且处于活动状态 则通过该行情通道订阅行情数据
            if (TLCtxHelper.ServiceRouterManager.DefaultDataFeed != null && TLCtxHelper.ServiceRouterManager.DefaultDataFeed.IsLive)
                return TLCtxHelper.ServiceRouterManager.DefaultDataFeed;

            //通过预设Selector逻辑 找到对应的行情通道
            string dfname = "demo";
            return GetDataFeedViaToken(dfname);
        }

        IDataFeed GetDataFeedViaToken(string token)
        {
            return TLCtxHelper.ServiceRouterManager.FindDataFeed(token);
        }
        #endregion



        /// <summary>
        /// 某个datafeed连接成功事件
        /// datafeed内部维护了一个合约列表,当有数据接口连接成功时,向该接口注册由该接口维护的合约列表
        /// </summary>
        /// <param name="connecter"></param>
        void datafeed_Connected(string token)
        {
            IDataFeed df = GetDataFeedViaToken(token);//connecter as IDataFeed;
            if (df == null || (!df.IsLive)) return;//如果通道为空 或 通道不处于工作状态则直接返回
            //通过查找本地需要注册的合约 当通道链接成功时进行注册
            SymbolBasket basket = GetDataFeedSymbols(df);

            //如果通过该通道已注册的行情为0 则加载所有默认合约进行注册
            if (basket.Count == 0)
            {
                debug(string.Format("DataFeed[{0}] have not registed any symbol,register all symbol we trade",df.Token), QSEnumDebugLevel.INFO);
                SymbolBasket nb = new SymbolBasketImpl();

                foreach (Symbol sym in BasicTracker.SymbolTracker.BasketAvabile)
                {
                    IDataFeed d = GetDataFeed(sym);
                    if (df.Equals(d))//需要当前的df进行比对 查找的d可能为空即某个合约没有绑定对应的行情通道
                    {
                        nb.Add(sym);
                    }
                }
                this.RegisterSymbols(nb);
            }
            else//如果有对应的合约 则重新注册
            {
                debug("DataFeed:" + df.Token + "connected,register symbols: " + string.Join(",", basket.ToSymArray()), QSEnumDebugLevel.INFO);
                df.RegisterSymbols(basket);
            }
        }

        /// <summary>
        /// 获得已注册合约中 获得某个通道的注册合约
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        SymbolBasket GetDataFeedSymbols(IDataFeed df)
        {
            if (datafeedbasktmap.Keys.Contains(df))
                return datafeedbasktmap[df];
            else
                return new SymbolBasketImpl();
        }

        #region 本地向DataFeed发送数据请求
        //建立本地缓存basket如果包含了该security则不重复进行发送
        SymbolBasket mb;
        /// <summary>
        /// 用于维护通道与Basket 映射关系
        /// </summary>
        ConcurrentDictionary<IDataFeed, SymbolBasket> datafeedbasktmap = new ConcurrentDictionary<IDataFeed, SymbolBasket>();
        public void RegisterSymbols(SymbolBasket b)
        {
            if (b.Count <= 0) return;
            try
            {
                //遍历所有的security,然后选择对应的数据通道请求行情数据
                debug("request market data to datafeed:" + string.Join(",", b.ToSymArray()), QSEnumDebugLevel.INFO);
                //将请求的合约按行情通道进行分组,然后统一调用行情通道的订阅合约函数
                Dictionary<IDataFeed, SymbolBasket> registermap = new Dictionary<IDataFeed, SymbolBasket>();
                //遍历合约列表然后按照对应的接口进行分类,最后统一进行注册防止过度调用接口的注册函数[假设是多个接口 多个行情字头可能每个合约对应的数据接口都不一致]
                foreach (Symbol sym in b.ToArray())
                {
                    if (mb.HaveSymbol(sym.Symbol)) continue;//注册过的行情字头不再进行注册
                    IDataFeed d = GetDataFeed(sym);//通过合约查找对应的数据接口
                    if (d != null)
                    {
                        if (registermap.Keys.Contains(d))
                        {
                            registermap[d].Add(sym);
                        }
                        else//第一次生成该接口对应的合约列表
                        {
                            registermap[d] = new SymbolBasketImpl();
                            registermap[d].Add(sym);
                        }
                    }
                }
                foreach (IDataFeed df in registermap.Keys)
                {
                    if (!df.IsLive)
                    {
                        debug(PROGRAME + ":DataFeed[" + df.Token + "]  is not connected well, please make the connection first", QSEnumDebugLevel.WARNING);
                        continue;
                    }
                    //调用行情通道订阅合约组
                    df.RegisterSymbols(registermap[df]);
                    //如果没有该通道键值 增加记录
                    if(!datafeedbasktmap.Keys.Contains(df))
                    {
                        datafeedbasktmap.TryAdd(df,new SymbolBasketImpl());
                    }
                    datafeedbasktmap[df].Add(registermap[df]);
                    //记录曾经注册过的合约
                    mb.Add(registermap[df]);
                }
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":regist symbols error:" + ex.ToString());
            }

        }


        
        


        #endregion


        #region 获得行情数据
        /// <summary>
        /// 获得所有当前市场数据快照
        /// 当前订阅的Basket mb 对应的Tick 行情由TickTracker维护
        /// </summary>
        /// <returns></returns>
        public Tick[] GetTickSnapshot()
        {
            if (mb.Count > 0)
            {
                string[] syms = mb.ToSymArray();
                List<Tick> ticks = new List<Tick>();

                foreach (string sym in syms)
                {
                    Tick k = _ticktracker[sym];
                    if (k.isValid)
                        ticks.Add(k);
                }
                return ticks.ToArray();
            }
            else
            {
                return _ticktracker.GetTicks();
            }
        }

        /// <summary>
        /// 获得某个symbol的行情快照信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick GetTickSnapshot(string symbol)
        {
            return _ticktracker[symbol];
        }

        /// <summary>
        /// 判定某个合约的市场行情是否处于活动状态
        /// 合约行情故障类型
        /// 1.行情整体没有获得过任何行情 live为false
        /// 2.当前行情整体处于报警状态
        /// 3.单个合约 行情严重延迟
        /// 在不同的行情时间端内切换时,是否工作正常。比如live状态如何重置
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool IsTickLive(string symbol)
        {
            //debug("tickwatch live:" + _tickwatcher.isLive.ToString() + " massalert:" + _tickwatcher.isMassAlerting.ToString() + " symbollive:" + _tickwatcher.IsSymbolTickLive(symbol).ToString(),QSEnumDebugLevel.INFO);
            //if (!_tickwatcher.isLive) return false;//如果行情整体处于idle状态则 单个合约一定处于非活动状态
            //如果处于行情整体报警状态 则处于非活动状态
            if (_tickwatcher.isMassAlerting)
            {
                return false;
            }
            return _tickwatcher.IsSymbolTickLive(symbol);
        }

        /// <summary>
        /// 获得某个合约的有效价格
        /// 如果返回-1则价格无效
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string symbol)
        {
            Tick k = GetTickSnapshot(symbol);//获得当前合约的最新数据
            if (k == null) return -1;

            decimal price = somePrice(k);

            //如果价格有效则返回价格 否则返回-1无效价格
            return price > 0 ? price : -1;
        }

        /// <summary>
        /// 从Tick数据采获当前可用的价格
        /// 优先序列 最新价/ ask / bid 如果均不可用则返回价格0
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private decimal somePrice(Tick k)
        {
            if (k.isTrade)
                return k.Trade;
            if (k.hasAsk)
                return k.AskPrice;
            if (k.hasBid)
                return k.BidPrice;
            else
                return -1;
        }
        #endregion



        #region 将行情快照保存到本地文件 用于服务器恢复是获得当前数据
        string _snapshotcahefile = "";
        //const string basketfn = @"cache\basket";

        DateTime _lastsnapshottime = DateTime.Now;

        [TaskAttr("保存行情快照", 30, 0, "保存行情路由系统的行情快照,用于系统崩溃胡快速重启恢复到最近的行情数据,没有行情数据风控系统可能会出现错误平仓")]
        public void Task_SaveTickSnapshot()
        {
            try
            {
                if ((DateTime.Now - _lastsnapshottime).TotalSeconds > 60)
                {
                    _lastsnapshottime = DateTime.Now;
                    using (FileStream fs = new FileStream(_snapshotcahefile, FileMode.Create))
                    {
                        //实例化一个StreamWriter-->与fs相关联  
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            foreach (Tick k in this.GetTickSnapshot())
                            {
                                if (k != null && k.isValid)
                                {
                                    string str = TickImpl.Serialize(k);
                                    sw.WriteLine(str);
                                }
                            }
                            sw.Flush();
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug("Error In savetick:" + ex.ToString(), QSEnumDebugLevel.ERROR);  
            }

        }

        /// <summary>
        /// 加载行情快照
        /// 主要用于盘中程序崩溃后从行情快照文件恢复最近的行情价格,用于防治行情错乱导致其他风控,策略等执行异常
        /// </summary>
        public void LoadTickSnapshot()
        {
            try
            {
                //LibUtil.Debug("##############load ticksnapshot...................");
                //实例化一个文件流--->与写入文件相关联  
				using (FileStream fs = new FileStream(_snapshotcahefile, FileMode.OpenOrCreate))
                {
                    //实例化一个StreamWriter-->与fs相关联  
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        while (sw.Peek() > 0)
                        {
                            string str = sw.ReadLine();
                            Tick k = TickImpl.Deserialize(str);
                            //通过缓存行情快照可以恢复到重启时间点的状态,这样浮动盈亏计算就不会应为缺失数据而计算错误，从而导致错误触发强平
                            //this.GotTick(k);//需要对外触发Tick然后驱动position的浮动盈亏
                            newtick(k, true);
                            //_ticktracker.GotTick(k);//维护每个symbol的tick快照
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug("Error In loadtick:" + ex.ToString(), QSEnumDebugLevel.ERROR);  
            }
        }
        #endregion


        #region DataFeed向本地发送tick数据
        /// <summary>
        /// 当有数据连接有数据到达
        /// </summary>
        //public event TickDelegate GotTickEvent;

        private TickTracker _ticktracker = null;
        AsyncResponse asynctick = null;


        public void DemoTick(decimal lastsettle,decimal settleprice)
        {
            Tick k = new TickImpl();
            k.Trade = 2404.0M;
            k.Size = 10;
            k.Symbol = "IF1411";
            k.AskPrice=2381.0M;
            k.AskSize=10;
            k.BidPrice = 2381.0M;
            k.BidSize = 2;
            k.Open = 2381.0M;
            k.Low=2398.0M;
            k.High=2412.0M;
            k.Exchange = "demo";
            k.Vol=2000;
            k.OpenInterest=100;
            k.PreOpenInterest=120;
            k.Date = Util.ToTLDate();
            k.Time = Util.ToTLTime();
            k.PreSettlement = lastsettle;
            k.Settlement = settleprice;
            GotTick(k);

            Tick k2 = new TickImpl();
            k2.Trade = 85.1M;
            k2.Size = 10;
            k2.Symbol = "IO1406-C-2150";
            k2.AskPrice = 85.4M;
            k2.AskSize = 10;
            k2.BidPrice = 85.3M;
            k2.BidSize = 2;
            k2.Open = 84.0M;
            k2.Low = 80.0M;
            k2.High = 90M;
            k2.Vol = 2000;
            k2.OpenInterest = 100;
            k2.PreOpenInterest = 120;
            k2.Date = Util.ToTLDate();
            k2.Time = Util.ToTLTime();

            GotTick(k2);

        }

        /// <summary>
        /// 所有的数据接口将tick数据集中到datafeedrouter的gottick进行处理,datafeedrouter内建一个异步处理gottick的缓存
        /// df接口统一直接将tick数据发送到datafeedrouter的gottick,然后datafeedrouter在内建的缓存中进行异步处理tick
        /// </summary>
        /// <param name="k"></param>
        void GotTick(Tick k)
        {
            //利用异步处理tick组件来接受并处理tick数据,这样当有多个数据源时,就不会存在线程问题。
            //在asynctick中统一缓存然后由唯一的线程对外发送tick数据
            //debug("datafeed got tick ??????????????", QSEnumDebugLevel.INFO);
            asynctick.newTick(k);
        }

        public void ExcludeSymbol(string symbol)
        {
            if (!excludesymbol.Contains(symbol))
                excludesymbol.Add(symbol);
        }

        public void IncludeSymbol(string symbol)
        {
            if (excludesymbol.Contains(symbol))
                excludesymbol.Remove(symbol);
        }
        List<string> excludesymbol = new List<string>();
        //通过建立asynctick 使得tick在微观级别上是单线程处理,在同一时刻排除了多个tick进入系统的可能
        //维持了其他基于tick数据操作的线程安全 因为有多个datafeed时完全有可能在同一时刻有2个线程在调用ontick函数
        void asynctick_GotTick(Tick k)
        {
            if (excludesymbol.Contains(k.Symbol))
                return;
            //debug("it is tick here..............", QSEnumDebugLevel.INFO);
            newtick(k, false);

        }
        void newtick(Tick k,bool ishist=false)
        {
            //debug("got tick:" + TickImpl.Serialize(k), QSEnumDebugLevel.INFO);
            try
            {
                if (GlobalConfig.IsDevelop)//如果处于开发环境则需要替换行情的时间
                {
                    k.Date = Util.ToTLDate();
                    k.Time = Util.ToTLTime();
                }

                _ticktracker.GotTick(k);//维护每个symbol的tick快照

                //如果是历史行情加载，tickwatcher不用监控该tick
                if(!ishist)
                    _tickwatcher.GotTick(k);

                TLCtxHelper.EventRouter.FireTickEvent(k);

                //if (GotTickEvent != null)
                //{
                    //debug("fire raw tick event", QSEnumDebugLevel.INFO);
                //    GotTickEvent(k);
                //}
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":async got tick error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        public void Reset()
        {
            //行情快照需要加载，同时结算时需要切换结算价格 用于维持最新市场状态

            //清空行情快照，?为什么不清空呢？
            //_ticktracker.Clear();
            //清除当前订阅列表 
            mb.Clear();
        }
        #endregion 


        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            asynctick.Start();
            _tickwatcher.Start();
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            asynctick.Stop();
            _tickwatcher.Stop();
        }

        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            asynctick.GotTick -= new TickDelegate(asynctick_GotTick);
            _tickwatcher.GotAlert -= new SymDelegate(_tickwatcher_GotAlert);
            _tickwatcher.GotFirstTick -= new SymDelegate(_tickwatcher_GotFirstTick);

            _tickwatcher.GotMassAlert -= new Int32Delegate(_tickwatcher_GotMassAlert);
            _tickwatcher.GotMassAlertCleard -= new Int32Delegate(_tickwatcher_GotMassAlertCleard);
        }

    }
}
