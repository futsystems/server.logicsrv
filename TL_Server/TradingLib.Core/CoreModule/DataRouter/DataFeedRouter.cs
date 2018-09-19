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
    public class DataFeedRouter:BaseSrvObject,IModuleDataRouter
    {
        const string CoreName = "DataFeedRouter";
        public string CoreId { get { return this.PROGRAME; } }

        ConfigDB _cfgdb;
        public DataFeedRouter()
            : base(DataFeedRouter.CoreName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(DataFeedRouter.CoreName);
            

            _ticktracker = new TickTracker();
            asynctick = new AsyncResponse("DataFeedRouter");
            asynctick.GotTick += new TickDelegate(asynctick_GotTick);
            //建立合约列表用于记录所维护的行情数据
            mb = new List<Symbol>();
            
            _snapshotcahefile = Path.Combine(new string[] { "cache", "ticksnapshot" });
            //订阅重置事件
            TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);

        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            logger.Info("重置行情路由服务");
            this.Reset();
        }

        /// <summary>
        /// 获得路由状态
        /// </summary>
        /// <returns></returns>
        //public DataFeedRouterStatus GetRouterStatus()
        //{
        //    DataFeedRouterStatus status = new DataFeedRouterStatus();

        //    if (TLCtxHelper.ServiceRouterManager.DefaultDataFeed == null)//如果行情通道不存在 则为false
        //    {
        //        status.IsDefaultDataFeedLive = false;
        //    }
        //    else
        //    {
        //        status.IsDefaultDataFeedLive = TLCtxHelper.ServiceRouterManager.DefaultDataFeed.IsLive;//如果行情通道存在 则设定DefaultDataFeed的当前工作状态
        //    }

        //    status.MassAlert = false; //_tickwatcher.isMassAlerting;//是否处于报警状态
        //    status.IsTickSpan = false;// _tickwatcher.TimeSpanSetted;//是否设定了行情有效时间段

        //    return status;

        //}
        

        /// <summary>
        /// 加载行情通道
        /// 绑定行情通道的 行情到达事件和行情连接事件
        /// </summary>
        /// <param name="datafeed"></param>
        public void LoadDataFeed(IDataFeed datafeed)
        { 
            //将数据通道的Tick转发到datafeedrouter然后再转发到TradingServer
            logger.Info("Load datafeed:"+datafeed.Name);
            datafeed.GotTickEvent +=new TickDelegate(GotTick);
            datafeed.Connected += new IConnecterParamDel(datafeed_Connected);
            datafeed.Disconnected += new IConnecterParamDel(datafeed_Disconnected);
            
        }

        void datafeed_Disconnected(string tocken)
        {
            
        }


        #region 获得某个合约的行情通道
        /// <summary>
        /// 获得某个合约的行情数据
        /// 默认通过FastDatafeed进行注册并管理所有市场行情数据 具体的注册流程通过TickPubSrv去管理
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
            List<Symbol> basket = GetDataFeedSymbols(df);

            //该通道没有注册过任何合约 则注册所有可交易合约
            if (basket.Count == 0)
            {
                logger.Info(string.Format("DataFeed[{0}] have not registed any symbol,register all symbols tradable", df.Token));
                List<Symbol> nb = new List<Symbol>();

                foreach (Symbol sym in BasicTracker.SymbolTracker.BasketAvabile)
                {
                    IDataFeed d = GetDataFeed(sym);
                    if (df.Equals(d))//需要当前的df进行比对 查找的d可能为空即某个合约没有绑定对应的行情通道
                    {
                        nb.Add(sym);
                    }
                }
                Action<List<Symbol>> del = new Action<List<Symbol>>(this.RegisterSymbols);
                del.BeginInvoke(nb, (re) => { logger.Info("RegisterSymbol Complate"); }, null);

                //this.RegisterSymbols(nb);
                //logger.Info("Register Basket async");
            }
            else//如果有对应的合约 则重新注册
            {
                logger.Info("DataFeed:" + df.Token + "connected,register symbols: " + string.Join(",", basket.Select(s=>s.Symbol).ToArray()).Truncat(50,"...."));
                df.RegisterSymbols(basket);
            }
        }

        /// <summary>
        /// 获得已注册合约中 获得某个通道的注册合约
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        List<Symbol> GetDataFeedSymbols(IDataFeed df)
        {
            if (datafeedbasktmap.Keys.Contains(df))
                return datafeedbasktmap[df];
            else
                return new List<Symbol>();
        }

        #region 本地向DataFeed发送数据请求
        //建立本地缓存basket如果包含了该security则不重复进行发送
        List<Symbol> mb;
        /// <summary>
        /// 用于维护通道与Basket 映射关系
        /// </summary>
        ConcurrentDictionary<IDataFeed, List<Symbol>> datafeedbasktmap = new ConcurrentDictionary<IDataFeed, List<Symbol>>();
        public void RegisterSymbols(List<Symbol> b)
        {
            if (b.Count <= 0) return;
            try
            {
                //遍历所有的security,然后选择对应的数据通道请求行情数据
                logger.Info("request market data to datafeed:" + string.Join(",", b.Select(sym=>sym.Symbol).ToArray()).Truncat(50, "...."));
                //将请求的合约按行情通道进行分组,然后统一调用行情通道的订阅合约函数
                Dictionary<IDataFeed, List<Symbol>> registermap = new Dictionary<IDataFeed, List<Symbol>>();
                //遍历合约列表然后按照对应的接口进行分类,最后统一进行注册防止过度调用接口的注册函数[假设是多个接口 多个行情字头可能每个合约对应的数据接口都不一致]
                foreach (Symbol sym in b)
                {
                    if (mb.Contains(sym)) continue;//注册过的行情字头不再进行注册
                    if (sym.SymbolType != QSEnumSymbolType.Standard) continue;//非标准合约不注册行情，非标准合约行情本地逻辑生成
                    IDataFeed d = GetDataFeed(sym);//通过合约查找对应的数据接口
                    if (d != null)
                    {
                        if (registermap.Keys.Contains(d))
                        {
                            registermap[d].Add(sym);
                        }
                        else//第一次生成该接口对应的合约列表
                        {
                            registermap[d] = new List<Symbol>();
                            registermap[d].Add(sym);
                        }
                    }
                }
                foreach (IDataFeed df in registermap.Keys)
                {
                    if (!df.IsLive)
                    {
                        logger.Warn(PROGRAME + ":DataFeed[" + df.Token + "]  is not connected well, please make the connection first");
                        continue;
                    }
                    //调用行情通道订阅合约组
                    df.RegisterSymbols(registermap[df]);
                    //如果没有该通道键值 增加记录
                    if(!datafeedbasktmap.Keys.Contains(df))
                    {
                        datafeedbasktmap.TryAdd(df, new List<Symbol>());
                    }
                    datafeedbasktmap[df] = datafeedbasktmap[df].Concat(registermap[df]).ToList();
                    //记录曾经注册过的合约
                    foreach(var t in registermap[df])
                    {
                        if(!mb.Contains(t))
                            mb.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(PROGRAME + ":regist symbols error:" + ex.ToString());
            }

        }


        
        


        #endregion


        #region 获得行情数据
        //TODO SymbolKey
        /// <summary>
        /// 获得所有当前市场数据快照
        /// 当前订阅的Basket mb 对应的Tick 行情由TickTracker维护
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tick> GetTickSnapshot()
        {
            return _ticktracker.GetTicks();
        }

        /// <summary>
        /// 获得某个symbol的行情快照信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick GetTickSnapshot(string exchange,string symbol)
        {
            return _ticktracker[exchange,symbol];
        }

        //TODO SymbolKey
        /// <summary>
        /// 获得某个合约的有效价格
        /// 如果返回-1则价格无效
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string exchange,string symbol)
        {
            Tick k = GetTickSnapshot(exchange,symbol);//获得当前合约的最新数据
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
            if (k.IsTrade())
                return k.Trade;
            if (k.HasAsk())
                return k.AskPrice;
            if (k.HasBid())
                return k.BidPrice;
            else
                return -1;
        }
        #endregion



        #region 将行情快照保存到本地文件 用于服务器恢复是获得当前数据
        string _snapshotcahefile = "";
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
                                if (k != null && k.IsValid())
                                {
                                    string str = TickImpl.Serialize2(k);
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
                logger.Error("Error In savetick:" + ex.ToString());  
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
                            Tick k = TickImpl.Deserialize2(str);
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
                logger.Error("Error In loadtick:" + ex.ToString());  
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
            //历史结算不响应外部行情数据
            if (TLCtxHelper.ModuleSettleCentre.SettleMode == QSEnumSettleMode.StandbyMode)
            {
                asynctick.newTick(k);
            }
        }


        Dictionary<string, SymbolImpl> _monthSymbolPair = null;
        bool _cacheMonthSymbol = false;
        //通过建立asynctick 使得tick在微观级别上是单线程处理,在同一时刻排除了多个tick进入系统的可能
        //维持了其他基于tick数据操作的线程安全 因为有多个datafeed时完全有可能在同一时刻有2个线程在调用ontick函数
        void asynctick_GotTick(Tick k)
        {

            if (GlobalConfig.ProfileEnable)  RunConfig.Instance.Profile.EnterSection("DataFeedGotTick");
            newtick(k, false);

            if(_monthSymbolPair == null && !_cacheMonthSymbol)
            {
                _cacheMonthSymbol = true;
                DomainImpl super = BasicTracker.DomainTracker.SuperDomain;
                if (super != null)
                {
                    _monthSymbolPair = BasicTracker.SymbolTracker[super.ID].MonthContinuousAvabile;
                }
                
            }
            if (_monthSymbolPair != null)
            {
                SymbolImpl month = null;
                if (_monthSymbolPair.TryGetValue(k.Symbol, out month))
                {
                    Tick k2 = TickImpl.Copy(k);
                    k2.Symbol = month.Symbol;
                    newtick(k2, false);
                }
            }
            if (GlobalConfig.ProfileEnable)  RunConfig.Instance.Profile.LeaveSection();
        }

        void newtick(Tick k,bool ishist=false)
        {
            try
            {
                if (GlobalConfig.IsDevelop)//如果处于开发环境则需要替换行情的时间
                {
                    k.Date = Util.ToTLDate();
                    k.Time = Util.ToTLTime();
                }

                _ticktracker.GotTick(k);//维护每个symbol的tick快照

                TLCtxHelper.EventRouter.FireTickEvent(k);

                //if (k.UpdateType == "X" || k.UpdateType == "Q" || k.UpdateType == "F" || k.UpdateType == "S")
                //{
                //    //转发实时行情
                //    Tick snapshot = _ticktracker[k.Exchange, k.Symbol];

                //    //向系统内触发快照信息
                //    TLCtxHelper.EventRouter.FireTickEvent(TickImpl.NewTick(snapshot,"S"));
                //}
                

            }
            catch (Exception ex)
            {
                logger.Error(PROGRAME + ":async got tick error:" + ex.ToString());
            }
        }

        public void Reset()
        {
            //清除当前订阅列表 
            mb.Clear();
        }
        #endregion 


        public void Start()
        {
            logger.StatusStart(this.PROGRAME);
            asynctick.Start();
        }

        public void Stop()
        {
            logger.StatusStop(this.PROGRAME);
            asynctick.Stop();
        }

        public override void Dispose()
        {
            logger.StatusDestory(this.PROGRAME);
            base.Dispose();
            asynctick.GotTick -= new TickDelegate(asynctick_GotTick);
        }

    }
}
