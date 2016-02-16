using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using TradingLib.API;

/*
 * 模拟成交引擎
 * 1.接收外部委托与取消
 * 2.接收行情数据进行委托成交
 * 在操作量不是很大，频率不是很高的情况下成交情况正常，数据没有问题。但是如果操作频率很高，数据很多，
 * 会产生委托无法成交，无法撤单的问题。这里需要仔细研究，将模拟成交引擎改造成高处理量，高稳定的结构
 * 
 * 
 * 
 * 
 * **/
namespace Broker.SIM
{
    /// <summary>
    /// route your orders through this component to paper trade with a live data feed
    /// 模拟交易
    //当client 发送 order 到 服务端  AsycnServer中的Worker线程会调用TLserver中的handlemessage 处理消息,handlemessage处理消息后会触发相关事件
    //事件触发后在CTPServer中对事件的绑定,会进行对Order的检查与papter trading.paper trading回对提交的order cancle 以及fill 反馈出来。
    //这个过程中流程比较负责,paper trading会对order进行修改,同时order记录会出现问题.因此paper trading/ asynclog在记录order fill等信息的时候需要copy新建实体。否则
    //会产生对个线程对某个对象进行访问 出现数据部同步的问题。
    //系统将交易记录储存到数据库中，储存过程是异步进行的,交易记录用于结算与分析
    //
    /// </summary>
    public class SIMTrader:TLBrokerBase, IBroker
    {

        public const string PROGRAM = "Broker.SIM";

        int _fillseq = 2000;
        Random random = new Random();
        object _fillseqobj = new object();
        int _steplow = 50;
        int _stephigh = 100;
        int NextFillSeq
        {
            get
            {
                lock (_fillseqobj)
                {
                    _fillseq += random.Next(_steplow, _stephigh);
                    return _fillseq;
                }

            }
        }

        ConfigDB _cfgdb;
        bool _fillall = false;
        /// <summary>
        /// 模拟交易服务,这里基本实现了委托,取消以及通过行情来成交委托的功能，但是单个tick能否成交多个委托的问题这里没有考虑
        /// 同时这里加入了委托 平仓检查,对于超买 超卖 会给出平今仓位不足的提示。
        /// 需要参考其他模拟交易引擎,对该引擎进行优化
        /// 每个Tick进来 执行process然后再process中循环所有的委托进行成交检查
        /// 可以改进成每个Tick进行只对该tick.symbol所对应的委托进行检查,其余委托不做检查
        /// </summary>
        /// <param name="idt"></param>
        /// <param name="verb"></param>
        public SIMTrader()
        {
            _cfgdb = new ConfigDB("SIMBroker");
            if (!_cfgdb.HaveConfig("FillAll"))
            {
                _cfgdb.UpdateConfig("FillAll", QSEnumCfgType.Bool,false, "是否成交所有");
            }
            _fillall = _cfgdb["FillAll"].AsBool();

            if (!_cfgdb.HaveConfig("UseBikAsk"))
            {
                _cfgdb.UpdateConfig("UseBikAsk", QSEnumCfgType.Bool, true, "是否按盘口成交");
            }
            _useBikAsk = _cfgdb["UseBikAsk"].AsBool();

            
            _fillseq = random.Next(1000, 4000);
        }

        /* 参数
         * 行情扫描成交还是Tick成交  是否使用盘口价还是最新价  模拟挂单成交(过价成交)
         * FillOrderTickByTick,     UseBidAsk,              SimLimit
         * 
         * */
        /// <summary>
        /// 模拟成交用Tick数据逐个成交还是用市场切片快速成交
        /// </summary>
        //bool _fillOrderTickByTick = false;

        /// <summary>
        /// 模拟成交用盘口数据成交还是用最新价成交
        /// </summary>
        bool _useBikAsk = true;

        /// <summary>
        /// 是否模拟真实挂单
        /// 盘口卖11 买10，挂单10买，如果严格按照盘口来成交 则需要等到卖盘变成10才会成交。但是实际有主动卖出的单子，会以当时挂单10买来成交 但是卖盘口还是11
        /// 因此挂单需要用最新价来成交
        /// 市价单用 盘口价来成交
        /// 
        /// 实盘成交过程
        /// 当有挂单时,如果价格上下跳动,我们不能马上以该成交价格成交,应为这里存在排队机制。
        /// 挂10买的时候记录当时盘口,并且需要记录该价位的成交数量,如果累计成交的数量 超过了该委托下达时当时的盘口厚度，则给予成交
        /// </summary>
        bool _simLimitReal = false;

        /// <summary>
        /// 产生新的tick用于引擎Fill Order
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            if (this._simLimitReal)
            {
                UpdateLimitOrderQuoteStatus(k);
            }
        }



        #region 限价委托 盘口记录器 
        Dictionary<long, LimitOrderQuoteStatus> _quotestatus = new Dictionary<long, LimitOrderQuoteStatus>();
        /// <summary>
        /// 用tick数据驱动 限价委托的盘口数据
        /// </summary>
        /// <param name="k"></param>
        void UpdateLimitOrderQuoteStatus(Tick k)
        {
            try
            {
                foreach (LimitOrderQuoteStatus qs in _quotestatus.Values)
                {
                    qs.GotTick(k);
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        /// <summary>
        /// 当有限价委托提交到队列中时,我们为该委托生成一个盘口记录器
        /// </summary>
        /// <param name="o"></param>
        void onLimitOrderQuened(Order o)
        {
            try
            {
                _quotestatus.Remove(o.id);
                //debug("限价委托 记录盘口....", QSEnumDebugLevel.MUST);
                Tick tick = FindTickSnapshot(o.Symbol);
                if (tick != null && tick.IsValid())
                {
                    _quotestatus.Add(o.id, new LimitOrderQuoteStatus(o, tick));
                }
            }
            catch (Exception ex)
            {
                logger.Error("onLimitOrderQuened error" + ex.ToString());
            }
        }

        /// <summary>
        /// 当该限价单全部成交时,删除对应的盘口记录器
        /// </summary>
        /// <param name="o"></param>
        void onLimitOrderFilled(Order o)
        {
            //debug("限价委托 全部成交", QSEnumDebugLevel.MUST);
            try
            {
                _quotestatus.Remove(o.id);
            }
            catch(Exception ex)
            {
                logger.Error("onLimitOrderFilled error" + ex.ToString());
            }
        }

        /// <summary>
        /// 当该限价单取消时,删除对应的盘口记录器
        /// </summary>
        /// <param name="id"></param>
        void onLimitOrderCancelled(long id)
        {
            try
            {
                _quotestatus.Remove(id);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
        /// <summary>
        /// 获得对应的开平标识
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        //QSEnumOffsetFlag GetOffsetFlag(Order o)
        //{
        //    switch (o.OffsetFlag)
        //    { 
        //        case QSEnumOffsetFlag.OPEN:
        //            return QSEnumOffsetFlag.OPEN;

        //        case QSEnumOffsetFlag.CLOSETODAY:
        //            return QSEnumOffsetFlag.CLOSETODAY;
        //        case QSEnumOffsetFlag.CLOSEYESTERDAY:
        //            return QSEnumOffsetFlag.CLOSEYESTERDAY;

        //        case QSEnumOffsetFlag.CLOSE:
        //            {
        //                //如果是上期所 则需要区分平今 平昨
        //                if (o.oSymbol.SecurityFamily.Exchange.EXCode == "SHFE")
        //                { 
                            
        //                }
        //                return QSEnumOffsetFlag.CLOSE;
        //            }
        //    }
        //}
        #region 市场快照扫描成交
        //市场端面扫描模拟成交
        /*
         * 接受委托先提交到ringbuffer这样 提交委托是线程安全的，在模拟成交内部由一个唯一的线程将ringbuffer中的委托移到queue[非线程安全]
         * 移动完毕后再遍历所有的委托 并用最新的市场快照去进行成交。
         * 这样操作的方式 数据循环量减少 可以提高处理量
         * 
         * 
         * **/
        void processPaperTrading()
        { 
             try
            {
                Order[] orders;
                long[] cancels;
                //debug("papertrading order num:"+aq.Count.ToString(), QSEnumDebugLevel.ERROR);
                if(aq.Count==0) return;//如果队列中没有委托 则直接返回

                orders = aq.ToArray();
                aq.Clear();
                
                cancels = can.ToArray();
                can.Clear();
                
                //列表用于放置未成交的委托
                List<Order> unfilled = new List<Order>();

                // 遍历所有的委托 查询是否存在该委托对应的取消,如果存在该委托的取消,则取消该委托
                for (int i = 0; i < orders.Length; i++)
                {
                    //获得对应的委托
                    Order o = orders[i];

                    //1.取消检查 查询是否有该委托的取消,若有取消 则直接返回,不对委托进行成交检查
                    //遍历所有的委托 会将所有的有效取消消耗,若还有取消没有消耗,则是因为其他原因造成的死取消
                    int cidx = gotcancel(o.id, cancels);
                    if (cidx >= 0)
                    {
                        logger.Info("PTT Server canceled: " + o.id);
                        cancels[cidx] = 0;
                        o.Status = QSEnumOrderStatus.Canceled;
                        _ocache.Write(o);
                        _ccache.Write(o.id);
                        continue;
                    }


                    //2.成交检查 用Tick数据成交该委托 注意我们是遍历所有的委托 然后取对应的tick数据 去进行成交
                    bool filled = false;

                    Tick tick = FindTickSnapshot(o.oSymbol.TickSymbol);

                    //如果是挂单 并且需要模拟实盘进行成交
                    if (tick != null && tick.IsValid())
                    {

                        //1.以对方盘口价格进行成交
                        filled = o.Fill(tick, _useBikAsk, false, _fillall);

                        //2.限价单如果没有成交我们按累计的盘口检查是否可以用最新价进行成交
                        LimitOrderQuoteStatus qs = null;

                        _quotestatus.TryGetValue(o.id, out qs);
                        if (this._simLimitReal && o.isLimit && qs != null && qs.IsTradeFill)
                        {
                            filled = o.Fill(tick, false, false, _fillall);
                        }

                    }

                    // 如果没有成交,则直接返回
                    if (!filled)
                    {
                        unfilled.Add(o);
                        continue;
                    }
                    else
                    {
                        //检查全部成交还是部分成交
                        Trade fill = (Trade)o;
                        //模拟成交需要按照交易所设定对应的开平标识
                        //生成成交编号
                        fill.BrokerTradeID = NextFillSeq.ToString();//交易所成交编号 Broker端的成交编号
                        //Util.Debug("@@@@@@@@@@@@@@@@@@trade date:" + fill.xDate.ToString() + " tradetime:" + fill.xTime.ToString(),QSEnumDebugLevel.ERROR);
                        logger.Info("PTT Server Filled: " + fill.GetTradeDetail());

                        bool partial = fill.UnsignedSize != o.UnsignedSize;//如果是部分成交 则需要将剩余未成交的委托 返还到委托队列
                        if (partial)
                        {
                            o.Status = QSEnumOrderStatus.PartFilled;//标识 部分成交
                            o.Size = (o.UnsignedSize - fill.UnsignedSize) * (o.Side ? 1 : -1);
                            o.FilledSize += fill.UnsignedSize;//将成交的数量累加到委托中的 filled标识段
                            unfilled.Add(o);
                        }
                        else
                        {
                            o.Status = QSEnumOrderStatus.Filled;//标识 全部成交
                            o.Size = (o.UnsignedSize - fill.UnsignedSize) * (o.Side ? 1 : -1);//此时o.unsigendsize-fill.unsignedsize ==0委托手数为0
                            o.FilledSize += fill.UnsignedSize;

                            //如果模拟实盘成交奥 则委托全部成交时我们删除该委托的盘口跟踪
                            if(o.isLimit && this._simLimitReal)
                                onLimitOrderFilled(o);
                        }
                        //委托成交后 回报委托最新状态
                        _ocache.Write(new OrderImpl(o));
                        // 发送委托回报
                        Trade nf = new TradeImpl(fill);
                        //关于这里复制后再发出order，这里的process循环 fill order. fill是对order的一个引用，后面修改的数据会覆盖到前面的数据,而gotfillevent触发的时候可能是在另外一个线程中
                        //运行的比如发送回client,或者记录信息等。这样就行程了多个线程对一个对象的访问可能会存在数据部同步的问题。
                        //logger.Info("~~~~~~~~~~ cache fill,fill==null" + (nf == null).ToString());
                        //debug("cache fill:" + nf.GetTradeDetail(), QSEnumDebugLevel.WARNING);
                        _fcache.Write(nf);
                        //logger.Info("_fcache count:" + _fcache.Count.ToString());
                    }
                }
                // add orders back
                for (int i = 0; i < unfilled.Count; i++)
                {
                    aq.Enqueue(unfilled[i]);
                }
                
                // add cancels back
                //取消均在委托发送之后才会取消,因此如果遍历了所有的委托都没有取消成功的取消，表明该委托已经被成交或者其他原因。
                //我们将余下的取消默认取消成功
                for (int i = 0; i < cancels.Length; i++)
                {
                    if (cancels[i] != 0)
                    {
                        can.Enqueue(cancels[i]);
                        //_ccache.Write(cancels[i]);//？
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("模拟撮合成交错误:" + ex.ToString()+ex.StackTrace.ToString());
            }
        
        }
        //查询取消列表中是否有某个委托ID,如有则返回序号，无返回-1
        int gotcancel(long id, long[] ids)
        {
            if (id == 0) return -1;
            for (int i = 0; i < ids.Length; i++)
                if (id == ids[i]) return i;
            return -1;
        }

        #endregion


        #region 委托 取消 入队 支持多线程操作该broker. queue操作并非线程安全,因此我们通过rb建立多线程间的同步
        RingBuffer<Order> _orderinCache = new RingBuffer<Order>(buffersize);
        RingBuffer<long> _cancelinCache = new RingBuffer<long>(buffersize);

        //原先撮合线程与接受委托线程会同时操作queue,通过加锁方式实现,当前通过隔离 接受请求与处理请求 达到线程安全
        //我们用消息放到队列在其他线程中进行。主干线程就是任务明确 稳定 高效率
        Queue<Order> aq = new Queue<Order>(buffersize);
        Queue<long> can = new Queue<long>(buffersize);
        /// <summary>
        /// 将接受到的委托与取消缓存 放到内部队列中
        /// 由唯一的线程进行调用,用ringbuffer进行中间缓存是为可能有多个work调用tradingserver->neworderrequest->brokerrouter.sendorder
        /// 从而导致有多个线程同时访问SendOrder/CancelOrder
        /// 利用ringbuffer进行缓存可以让SimBroker的sendorder/cancelorder 多线程安全。
        /// 如果委托入队正常,则其状态为Opened或PartFilled
        /// </summary>
        void processIn()
        {
            try
            {
                while (_orderinCache.hasItems)
                {
                    queueOrder(_orderinCache.Read());
                }
                while (_cancelinCache.hasItems)
                {
                    queueCancel(_cancelinCache.Read());
                }
            }
            catch (Exception ex)
            {
                logger.Error("move order from ringbufeer to queue error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 接收brokerrouter路由过来的委托
        /// 模拟成交接口的相关字段规则
        /// Broker 为模拟成交接口的Token
        /// BrokerLocalOrderID 成交端的本地编号为空
        /// BrokerRemoteOrderID 成交端的对端编号在委托入队列时 设定为系统内部设定的OrderSeq
        /// 
        /// OrderSysID 设定为BrokerRemoteOrderID
        /// 
        /// 成交时 由Order产生Trade,Trade中的Broker属性以及Order属性均来自于委托的相关字段 模拟成交接口需要赋值一个模拟的递增成交编号
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            logger.Error("PTT Server Got Order: " + o.GetOrderInfo());
            //提交委托时 设定localorderid remoteorderid
            o.BrokerLocalOrderID = "";
            o.Status = QSEnumOrderStatus.Submited;
            Order tmp = new OrderImpl(o);
            tmp.Comment = string.Empty;
            //将委托comment清空，模拟成交不携带comment信息，在messageexchange中根据委托状态进行设定comment信息
            _orderinCache.Write(tmp);//复制委托 委托的模拟成交变化不会改变初始委托数据

        }

        void queueOrder(Order o)
        {
            logger.Info("PTT Server Queue Order: " + o.GetOrderInfo());
            //模拟交易对新提交上来的order进行复制后保留,原来的order形成事件触发.这样本地的order fill process对order信息的修改就不会影响到对外触发事件的order.
            o.Status = QSEnumOrderStatus.Opened;//标识 委托状态为open,等待成交
            //将委托置入队列时 相当于交易所接受了委托 设定BrokerRemoteOrderID
            //模拟成交接口中 远端委托编号 与 系统内设定的委托流水相同
            o.BrokerRemoteOrderID = o.OrderSeq.ToString();
            //成交接口发送委托到交易所，交易所会返回RemoteOrderID(成交侧的远端委托编号)
            //此处远端编号与BrokerRemoteOrderID一致
            o.OrderSysID = o.BrokerRemoteOrderID;
            
            Order oc = new OrderImpl(o);
            aq.Enqueue(oc);//放入委托队列

            //如果模拟实盘成交 限价单 则记录盘口状态 这里onLimitOrderQuened函数可能会产生异常,如果发生异常则委托就会在这里丢失,不会回报
            if (oc.isLimit && this._simLimitReal)
                onLimitOrderQuened(oc);

            _ocache.Write(o);//发送委托回报
        }

        /// <summary>
        /// 接收brokerrouter路由过来的取消委托
        /// </summary>
        /// <param name="id"></param>
        public void CancelOrder(long id)
        {
            logger.Info(" PTT Got Cancel: " + id);
            _cancelinCache.Write(id);
        }

        public void queueCancel(long id)
        {
            if (id == 0) return;
            if (IsCancedQueued(id))
            {
                logger.Info("PTT have queued cancel:" + id.ToString());//已经队列过取消
                return;
            }
            can.Enqueue(id);//取消如队列
            logger.Info(" PTT queueing cancel: " + id);
        }

        /// <summary>
        /// 判断取消队列中是否已经包含了某个委托的取消
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsCancedQueued(long id)
        {
            return can.ToArray().Where(c => c == id).Count() !=0;
        }

        #endregion

        

        Thread ptthread = null;
        bool ptgo = false;
        void PTEngine()
        {
            while (ptgo)
            {
                processIn();//将委托与取消从ringbuffer移动到Queue中等待执行成交或取消 由于移动委托操作与委托模拟成交操作是在同一个线程内进行，因此这里queue不存在多线程操作的安全问题
                processPaperTrading();
                Thread.Sleep(200);//每隔200毫秒进行一次模拟撮合
            }
        }

        void StartPTEngine()
        {
            if (ptgo) return;
            ptgo = true;
            ptthread = new Thread(new ThreadStart(PTEngine));
            ptthread.IsBackground = true;
            ptthread.Name = "SimBroker PTEngine";
            ptthread.Start();
            ThreadTracker.Register(ptthread);
        }


        #region 成交接口交易数据
        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public IEnumerable<Order> Orders { get { return new List<Order>(); } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public IEnumerable<Trade> Trades { get { return new List<Trade>(); } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public IEnumerable<Position> Positions { get { return new List<Position>(); } }

        /// <summary>
        /// 返回所有持仓状态统计数据
        /// </summary>
        public  IEnumerable<PositionMetric> PositionMetrics { get { return new List<PositionMetric>(); } }

        /// <summary>
        /// 获得某个合约的持仓状态统计数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public  PositionMetric GetPositionMetric(string symbol)
        {
            return null;
        }

        /// <summary>
        /// 返回持仓预计调整量
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public  int GetPositionAdjustment(Order o)
        {
            if (o.IsEntryPosition)
                return o.UnsignedSize;
            else
                return o.UnsignedSize * -1;
        }

        #endregion





        #region 信息对外发送线程
        const int buffersize = 20000;
        RingBuffer<Order> _ocache = new RingBuffer<Order>(buffersize);
        RingBuffer<long> _ccache = new RingBuffer<long>(buffersize);
        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(buffersize);
        void StartProcOut()
        {
            if (_read) return;//原来这里没有启动标识,则系统在长期运行后 次日重新启动模拟成交引擎,会导致有2个线程在处理procout
            _read = true;
            _readthread = new Thread(new ThreadStart(procout));
            _readthread.IsBackground = true;
            _readthread.Name = "SimBroker MessageOut";
            _readthread.Start();
            ThreadTracker.Register(_readthread);
        }
        bool _read = false;
        Thread _readthread = null;
        void procout()
        {
            while (_read)
            {
                try
                {
                    while (_ocache.hasItems)
                    {
                        //debug("PPT fire order Event.............", QSEnumDebugLevel.INFO);
                        Order o = _ocache.Read();
                        NotifyOrder(o);
                    }
                    while (!_ocache.hasItems && _fcache.hasItems)
                    {
                        //debug("PPT fire Trade Event.............", QSEnumDebugLevel.INFO);
                        //debug("Procout fill out,cnt:" + _fcache.Count.ToString());
                        Trade f = _fcache.Read();
                        //debug("read fill, fill==null " + (f == null).ToString());
                        NotifyTrade(f);

                    }
                    while (!_ocache.hasItems && _ccache.hasItems)
                    {
                        long c = _ccache.Read();
                        NotifyCancel(c);

                    }
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    logger.Error("SIMBroker回传交易信息错误:"+ex.ToString());
                }
            }
            
        }
        #endregion

       
        /// <summary>
        /// 将当日的交易信息恢复到内存,注意所有的交易信息均有清算中心维护
        /// </summary>
        void Restore()
        {
            try
            {
                logger.Info("从清算中心加载模拟交易记录");
                //模拟委托直接加载分帐户侧委托
                IEnumerable<Order> olist = this.ClearCentre.GetOrdersViaBroker(this.Token);//加载
                lock (aq)
                {
                    foreach (Order o in olist)
                    {
                        //将等待成交的委托放入队列 注意这里加入一个规则 如果是市价委托则不加载 等待成交的均是limit或stop order.
                        //如果有一些委托没有得到正确的委托标识,则系统会重复成交
                        if (o.Status == QSEnumOrderStatus.PartFilled || o.Status == QSEnumOrderStatus.Opened)//处于Opened或者Partilled才会被加载
                        {
                            //debug("加载委托:" + o.ToString());
                            aq.Enqueue(o);
                        }
                    }
                }

                IEnumerable<Trade> trades = this.ClearCentre.GetTradesViaBroker(this.Token);
                //tryparse用于避免非数字brokerkey造成的异常
                _fillseq = trades.Count() > 0 ? trades.Max(f => { int seq = 0; int.TryParse(f.BrokerTradeID, out seq); return seq; }) : _fillseq;
                logger.Info("Order Cnt:"+olist.Count().ToString() +" Trade Cnt:"+trades.Count().ToString()+" Max Fill Seq:" + _fillseq.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Resotore error:" + ex.ToString());
            }
        }
        bool _working = false;

        public void Stop()
        {
            aq.Clear();
            can.Clear();

            _working = false;
            NotifyDisconnected();
        }

        public void Start()
        { 
            string msg = string.Empty;
            bool success = this.Start(out msg);
        }
        /*
         注意:在系统托管过程中会停止或重启服务,这里需要注意不能重启多个ptengine或者procout否则会造成数据错误
         */
        public bool Start(out string msg)
        {
            msg = string.Empty;

            //初始化参数
            ParseConfigInfo();
            //_useBikAsk = true;
            _simLimitReal = false;


            logger.Info("模拟引擎启动,成交方式:市场断面扫描 取价方式:"+(_useBikAsk?"盘口价":"最新价") +" 模拟实盘取价:"+ _simLimitReal.ToString());
            StartPTEngine();//启动模拟成交引擎
            StartProcOut();//启动对外消息发送线程

            _working = true;
            Restore();
            NotifyConnected();
            return true;
        }

        public bool IsLive
        {
            get {
                return _working;
            }
            set 
            {
                _working = value;
            }
        }
        


    }

    /**
     * 关于成交有效性的问题
     * 限价
     * 1.最新价打穿盘口
     * 2.对手盘口符合价格要求
     * 满足以上2点才可以成交，而且这个价格基本都会走到。
     * 
     * 唯一出现的问题就是
     * 某个价格在盘口上出现过,但是没有实际成交。而我们的模拟成交取到了那个价格。则这个问题就会发生价格偏差的问题
     * 10 - 11
     * 当11卖的盘口吃干净,仍然有主动买(超价委托)的单子则会迟到12的盘口并给出12积累的盘口厚度。
     * 价格变化到 11-12 注意:出现12则表明已经有人在12的价格成交了单子。
     * 是否可以建立15秒K线 然后当一个15秒的bar生成后,检查在15秒内成交是否在该15秒的K线范围内。
     * 
     * 限价 限定价格10元买，在15秒周期最低价格为11元，则对客户有利，客户不会去投诉。同时由于是限价格，盘口出现对应的价格 则表明一定有对应的成交。
     * 因此限价不用去处理
     * 
     * 如果是市价，不在该bar范围内,则我们需要调整其成交价格。
     * 关键是需要有非常精确的Tick数据 才可以保证成交的正常
     * 
     * 同时要挖掘模拟比赛中的错误成交。
     * 
     * 解决以上问题那么系统就应该比较完善了。
     * 
     * */


    /**
     * A#关于市价单的问题
     * 当前市价单是以对手盘口的价格来立即成交,但是可能会出现当时最新价格没有该价格。这个问题比较难以处理
     * 买10元 卖11元，当前成交压制在10元附近，即主动卖的比较多，若我们买一手，则会出现11元成交，但是当时市场没有11元这个成交价格。
     * 不过买卖多半是来回波动 这个只能通过限制主力合约来达到 变动快速 使得选手无法识别
     * 
     * 
     * */
    /// <summary>
    /// A#限价单盘口检查 基本解决
    /// 1.可以按照当时盘口厚度已经成交来递减盘口,从而模拟实际的挂单成交
    /// 2.盘口会根据时间来进行模拟撤单动作
    /// 3.需要增加不同深度的报价,从而实现在成交密集区 上下浮动几个点位 可以模拟对应的盘口数量，目前差一个点位则没有盘口厚度，见价则成交
    /// </summary>
    class LimitOrderQuoteStatus
    {
        public Order Order{get;set;}//对应的委托
        public int PendingSize{get;set;}//委托下达时该价格盘口的数量

        /// <summary>
        /// 理论上应该给入一个level2数据，判断盘口大小按照1 2 3 4 5 6 来进行盘口数据的模拟
        /// 当前 买盘口10元 盘口200手 挂10元买 则盘口厚度200手，如果挂9元则应该设置多少盘口厚度呢？
        /// 这里就需要程序动态记录合约的不同深度的盘口来获得对应的盘口数据
        /// </summary>
        /// <param name="o"></param>
        /// <param name="k"></param>
        public LimitOrderQuoteStatus(Order o, Tick k)
        {
            Order = o;
            decimal partprice = o.Side ? k.BidPrice : k.AskPrice;
            if (o.LimitPrice >= partprice)
                PendingSize = o.Side ? k.BidSize : k.AskSize;
            else
                PendingSize = 0;
            //LibUtil.Debug("模拟成交记录委托盘口:" + Order.ToString() + " 盘口数量:" + PendingSize.ToString() + " 价格:" + partprice.ToString());
        }

        bool s5 = false;
        bool s10 = false;
        bool s30 = false;
        bool s60 = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            if (k.Symbol != Order.Symbol) return;
            if (k.Trade == Order.LimitPrice)
            {
                PendingSize = PendingSize - k.Size;//如果成交价格等于该委托限价格,则从盘口厚度中减去该成交数量
                //LibUtil.Debug("价格:" + Order.price.ToString() + " 成交:" + k.size.ToString() + " 修改盘口厚度:" + PendingSize.ToString());
            }
            int secends = Util.FTDIFF(Order.Time, k.Time);
            
            if (secends >60*60 && !s60)//30分钟后盘口撤单70%
            {
                s60 = true;
                PendingSize = (int)(PendingSize * 0.2);
                return;
            }
            if (secends > 30 * 60 && !s30)//30分钟后盘口撤单50%
            {
                s30 = true;
                PendingSize = (int)(PendingSize * 0.5);
                return;
            }
            if (secends > 10 * 60 && !s10)//10分钟后盘口撤单20%
            {
                s10 = true;
                PendingSize = (int)(PendingSize * 0.8);
                return;
            }
            if (secends > 5 * 60 && !s5)//10分钟后盘口撤单10%
            {
                s5 = true;
                PendingSize = (int)(PendingSize * 0.9);
                return;
            }

        }

        /// <summary>
        /// 当盘口单子被吃掉后,则我们可以用最新价进行成交
        /// </summary>
        public bool IsTradeFill
        { 
            get
            {
                if (PendingSize <= 0)
                    return true;
                return false;
            }
        }

        /**市价单的一个变通处理
         * 记录每个合约最近2秒的Tick序列,
         * 盘口 10 11，如果当时成交价集中在11元附近,而客户执行市价卖出,则按照目前的成交方式则会成交在10元,但是在这个tick时间点上
         * 市场的成交价格是11元。因此我们需要获得前面2秒的tick数据具体时间可以设定
         * 
         * 3    10-11  10   主动卖
         * 2    10-11  10   主动卖
         * 1    10-11  11   主动买
         * 0    10-11  11   主动买
         * 第0tick上的数据如果按照目前的成交我们卖出 则我们以bid价10元成交,但是当前成交价是11与实际有冲突,
         * 成交策略
         * 1.回溯前面2秒的tick看有没有10的价格,如果有10的价格用该tick进行成交
         * 2.如果回溯后没有10这个价格,则我们以最新成交价成交
         * 
         * */

        class TickHistory
        {
            public string Symbol { get; set; }
        }

        class TickPip
        {
            const int num = 4;
            int idx = 0;
            ThreadSafeList<Tick> tickpip = new TradingLib.Common.ThreadSafeList<Tick>();

            public TickPip()
            {
                for (int i = 0; i < num; i++)
                {
                    tickpip.Add(new TickImpl());
                }
            }
            public void GotTick(Tick k)
            {
                tickpip[idx] = k;
                idx++;
                if (idx >= num)
                    idx = 0;
            }

            /// <summary>
            /// 某个市价委托下达时 当前Tick 与对应的委托
            /// 查询当前tick管道内的数据获得对应的Tick
            /// </summary>
            /// <param name="k"></param>
            /// <param name="side"></param>
            public void QueryTick(Tick k, Order o,out Tick tickused)
            { 
                tickused = null;
                //idx=3 3,2,1,0
                //idx=2 2,1,0,3
                //idx=1 1,0,3,2
                bool side = o.Side;
                decimal price = o.Side ? k.AskPrice : k.BidPrice;

                List<int> idlist = new List<int>();
                //遍历管道内的所有 tick,查找最符合的tick，即当前盘口下主动买或者主动卖
                foreach (int i in idlist)
                { 
                    //if(k.isTrade )
                    Tick tick = tickpip[i];
                    if (k != null && tick.IsValid())
                    {
                        if (pairtick(tick, price, side))
                        {
                            tickused = tick;
                            return;
                        }
                    }
                }
                //如果没有符合的盘口以及成交类别,则找是否有当前的成交价格,10-11主动卖 则找成交价格为10的tick
                foreach (int i in idlist)
                {
                    //if(k.isTrade )
                    Tick tick = tickpip[i];
                    if (k != null && tick.IsValid())
                    {
                        if (pairticktrade(tick, price))
                        {
                            tickused = tick;
                            return;
                        }
                    }
                }
                //如果找不到该成交价格,则以最新的成交价格成交

                foreach (int i in idlist)
                {
                    //if(k.isTrade )
                    Tick tick = tickpip[i];
                    if (tick != null && tick.IsValid())
                    {
                        if (pairtickhavetrade(tick))
                        {
                            tickused = tick;
                            return;
                        }
                    }
                }

            }

            bool pairtickhavetrade(Tick k)
            {
                if (k.IsTrade())
                {
                    return true;
                }
                return false;
            }

            bool pairticktrade(Tick k, decimal price)
            {
                if (k.Trade == price)
                    return true;
                return false;
            }
            /// <summary>
            /// 检查主动买或者主动卖在该tick上是否可以成交
            /// </summary>
            /// <param name="k"></param>
            /// <param name="price"></param>
            /// <param name="side"></param>
            /// <returns></returns>
            bool pairtick(Tick k, decimal price, bool side)
            { 
                //主动买
                if (side)
                {
                    if (k.Trade == k.AskPrice && k.Trade == price)//当前成交价 卖盘 与下委托时的盘口价格一致
                    {
                        return true;
                    }
                    return false;
                }
                else//主动卖
                {
                    if (k.Trade == k.BidPrice && k.Trade == price)//当前成交价 买盘 与下委托时的盘口价格一致
                    {
                        return true;
                    }
                    return false;
                }

            }
        }
    }
}
