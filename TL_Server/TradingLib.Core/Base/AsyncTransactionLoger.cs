using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;//记得加入此引用
using TradingLib.Common;
using TradingLib.API;
using TradingLib;

namespace TradingLib.Core
{
    

    /*
     * 数据核心产生的一些问题
     * 1.系统会产生一些重复的单子(大多数是因为CTP回报Order多次,数据库没有进行对应检查,而进行的多次插入)
     * 2.模拟交易账号应该不会存在重复单,但是也存在了重复单
     * 
     * 
     * 
     * 
     * 
     * */
    /// <summary>
    ///模拟交易Broker用于将Order cancle trade交易信息安全的记录到数据库
    ///通过数据库操作缓存队列以及数据库重试连击技术，可以使得交易日志能够在数据库断开连接或者重新启动数据库的情况下交易日志信息部丢失
    /// </summary>
    public class AsyncTransactionLoger:BaseSrvObject
    {
        //注当缓存超过后系统记录的交易信息就会发生错误。因此这里我们需要放大缓存大小并且在控制面板中需要监视。
        const int MAXLOG = 100000;

        RingBuffer<Order> _ocache;//委托插入缓存
        RingBuffer<Order> _oupdatecache;//委托更新缓存
        RingBuffer<Trade> _tcache;//成交插入缓存
        RingBuffer<OrderAction> _oactioncache;//委托操作缓存
        RingBuffer<PositionCloseDetail> _posclosecache;//平仓明细缓存
        RingBuffer<PositionDetail> _posdetailcache;//持仓明细缓存
        RingBuffer<ExchangeSettlement> _exsettlementcache;//交易所结算缓存
        RingBuffer<CashTransaction> _cashtxncache;//出入金记录缓存

        RingBuffer<Order> _osettlecache;//委托结算缓存
        RingBuffer<Trade> _tsettlecache;//成交结算缓存
        RingBuffer<PositionDetail> _pdsettledcache;//持仓明细结算缓存
        RingBuffer<ExchangeSettlement> _exsettlecache;//交易所结算结算缓存
        RingBuffer<CashTransaction> _cashtxnsettlecash;//出入金操作结算缓存
        RingBuffer<DataRepositoryLog> _datareperrorcache;//数据储存异常缓存


        public int OrderInCache { get { return _ocache.Count; } }
        public int TradeInCache { get { return _tcache.Count; } }

        public int OrderUpdateInCache { get { return _oupdatecache.Count; } }
        public int PosCloseInCache { get { return _posclosecache.Count; } }

        /// <summary>
        /// fired when barrequest is read asychronously from buffer
        /// </summary>
        //public event BarRequestDel GotBarRequest;//有新的barRequest处理事件
        /// <summary>
        ///  fired when buffer is empty
        /// </summary>
        public event VoidDelegate GotBRQueueEmpty;
        /// <summary>
        /// fired when buffer is written
        /// </summary>
        public event VoidDelegate GotBRQueued;
        /// <summary>
        /// should be zero unless buffer too small
        /// </summary>
        
        static ManualResetEvent _logwaiting = new ManualResetEvent(false);
        Thread _logthread = null;
        public bool isValid { get { return _loggo; } }

        int _nwt;
        int _nrt;

        bool _loggo=false;
        int _delay = 0;
        Stopwatch ordert = new Stopwatch();
        Stopwatch updateordert = new Stopwatch();
        Stopwatch tradet = new Stopwatch();
        Stopwatch canclet = new Stopwatch();

        public void DisplayTimer()
        {
            logger.Info("插入委托时间消耗:" + ordert.Elapsed.ToString());
            logger.Info("更新委托时间消耗:" + updateordert.Elapsed.ToString());
            logger.Info("插入成交时间消耗:" + tradet.Elapsed.ToString());
            logger.Info("插入取消时间消耗:" + tradet.Elapsed.ToString());
            
        }

        public void demoinsertorder()
        {
            //fastdb.insertOrder();
        }
        /// <summary>
        /// 异步交易信息记录系统拥有1000条的缓存数据,实验的时候发现插入数据错误,后来研究发现 插入数据的先后有关系
        /// 当我们在trade cache停留的时候 一致有新的交易被送进来，但是ordercache里面的order却没有发送到数据库 从而造成的问题就是 当trade里面出现对应的order时候 我们并没有插入到该order
        /// 因此我们在插入数据的时候要检查 Order是优先插入的 有了order我们才可以先交易。 因此Order被完全插入完毕后我们才开始插入交易数据
        /// </summary>
        void readedata()
        {
            while (_loggo)
            {
                try
                {
                    #region 交易记录插入与更新
                    //插入委托
                    while (_ocache.hasItems)
                    {
                        Order o = _ocache.Read();
                        try
                        {
                            bool re = ORM.MTradingInfo.InsertOrder(o);
                            if (re)
                            {
                                logger.Debug(string.Format("Insert Order Success:{0}", o.GetOrderInfo()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertOrder, o, ex);
                        }
                        Thread.Sleep(_delay);
                    }

                    //更新委托状态
                    while (!_ocache.hasItems && _oupdatecache.hasItems)
                    {
                        Order o = _oupdatecache.Read();
                        try
                        {
                            bool re = ORM.MTradingInfo.UpdateOrderStatus(o);
                            if (re)
                            {
                                logger.Debug(string.Format("Update Order Success:{0}", o.GetOrderInfo()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.UpdateOrder, o, ex);
                        }
                        Thread.Sleep(_delay);
                    }

                    //插入成交
                    while (!_ocache.hasItems && _tcache.hasItems)
                    {
                        Trade f = _tcache.Read();
                        try
                        {
                            bool re = ORM.MTradingInfo.InsertTrade(f);
                            if (re)
                            {
                                logger.Debug(string.Format("Insert Trade Success:{0}", f.GetTradeInfo()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertTrade, f, ex);
                        }
                        Thread.Sleep(_delay);
                    }

                    //委托操作插入
                    while (!_ocache.hasItems && _oactioncache.hasItems)
                    {
                        OrderAction action = _oactioncache.Read();
                        try
                        {
                            bool re = ORM.MTradingInfo.InsertOrderAction(action);
                            if (re)
                            {
                                logger.Debug(string.Format("Insert OrderAction Success:{0}", action.ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertOrderAction, action, ex);
                        }
                        Thread.Sleep(_delay);
                    }

                    //插入平仓明细数据
                    while (_posclosecache.hasItems)
                    {
                        PositionCloseDetail close = _posclosecache.Read();
                        try
                        {
                            bool re = ORM.MSettlement.InsertPositionCloseDetail(close);
                            if (re)
                            {
                                logger.Debug(string.Format("Insert PositionCloseDetail Success:{0}", close.ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertPositionCloseDetail, close, ex);
                        }
                        Thread.Sleep(_delay);
                    }
                    //插入持仓明细数据
                    while (_posdetailcache.hasItems)
                    {
                        PositionDetail pd = _posdetailcache.Read();
                        try
                        {
                            ORM.MSettlement.InsertPositionDetail(pd);
                            logger.Debug(string.Format("Insert PositionDetail Success:{0}", pd.ToString()));
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertPositionDetail, pd, ex);
                        }
                        Thread.Sleep(_delay);
                    }
                    //插入交易所结算记录
                    while (_exsettlementcache.hasItems)
                    {
                        ExchangeSettlement settle = _exsettlementcache.Read();
                        try
                        {
                            ORM.MSettlement.InsertExchangeSettlement(settle);
                            logger.Debug(string.Format("Insert ExchangeSettlement Success:{0}", settle.ToString()));
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertExchangeSettlement, settle, ex);
                        }
                        Thread.Sleep(_delay);
                    }
                    //插入出入金记录
                    while (_cashtxncache.hasItems)
                    {
                        CashTransaction txn = _cashtxncache.Read();
                        try
                        {
                            ORM.MCashTransaction.InsertCashTransaction(txn);
                            logger.Debug(string.Format("Insert CashTransaction Success:{0}", txn.ToString()));
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.InsertCashTransaction, txn, ex);
                        }
                        Thread.Sleep(_delay);
                    }
                    #endregion


                    #region 更新结算标识
                    //更新委托结算标识
                    while (!_ocache.hasItems && !_oupdatecache.hasItems && _osettlecache.hasItems)
                    {
                        Order o = _osettlecache.Read();
                        try
                        {
                            ORM.MTradingInfo.MarkOrderSettled(o);
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.SettleOrder, o, ex);
                        }
                    }
                    //更新成交结算标识
                    while (!_ocache.hasItems && !_tcache.hasItems && _tsettlecache.hasItems)
                    {
                        Trade f = _tsettlecache.Read();
                        try
                        {
                            ORM.MTradingInfo.MarkeTradeSettled(f);
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.SettleTrade, f, ex);
                        }
                    }
                    //更新持仓明细结算标识
                    while (_pdsettledcache.hasItems)
                    {
                        PositionDetail pd = _pdsettledcache.Read();
                        try
                        {
                            ORM.MSettlement.MarkPositionDetailSettled(pd);
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.SettlePositionDetail, pd, ex);
                        }
                    }
                    //更新交易所结算标识
                    while (_exsettlecache.hasItems)
                    {
                        ExchangeSettlement settle = _exsettlecache.Read();
                        try
                        {
                            ORM.MSettlement.MarkExchangeSettlementSettled(settle);
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.SettleExchangeSettlement,settle, ex);
                        }

                    }
                    //更新出入金结算标识
                    while (_cashtxnsettlecash.hasItems)
                    {
                        CashTransaction txn = _cashtxnsettlecash.Read();
                        try
                        {
                            ORM.MCashTransaction.MarkeCashTransactionSettled(txn);
                        }
                        catch (Exception ex)
                        {
                            throw new DataRepositoryException(EnumDataRepositoryType.SettleCashTransaction, txn, ex);
                        }
                    }

                    #endregion

                    // clear current flag signal
                    _logwaiting.Reset();

                    // wait for a new signal to continue reading
                    _logwaiting.WaitOne(SLEEP);
                    //Thread.Sleep(1000);
                }
                
                //以下代码段通过捕捉交易日志插入部分的异常,将没有正常插入的数据重新返回到缓存队列
                //mysql则通过不断尝试进行数据库连接,当连接成功后重新将日志插入数据库
                catch (DataRepositoryException ex)
                {
                    _datareperrorcache.Write(new DataRepositoryLog(ex.RepositoryType, ex.RepositoryData));
                    logger.Error(string.Format("数据储存发生错误 Method:{0} Data:{1} Error:{2}", ex.RepositoryType, ex.RepositoryData.ToString(), ex.InnerException.ToString()));
                }
                catch (Exception ex)
                {
                    logger.Error(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString());
                }
            }
        }

        public const int SLEEPDEFAULTMS = 10;
        int _sleep = SLEEPDEFAULTMS;
        /// <summary>
        /// sleep time in milliseconds between checking read buffer
        /// </summary>
        public int SLEEP { get { return _sleep; } set { _sleep = value; } }


        #region 结算标识
        /// <summary>
        /// 结算委托
        /// </summary>
        /// <param name="o"></param>
        public void MarkOrderSettled(Order o)
        {
            Order oc = new OrderImpl(o);
            _osettlecache.Write(oc);
            newlog();
        }
        /// <summary>
        /// 结算成交
        /// </summary>
        /// <param name="f"></param>
        public void MarkTradeSettled(Trade f)
        {
            Trade nf = new TradeImpl(f);
            _tsettlecache.Write(nf);
            newlog();
        }
        /// <summary>
        /// 结算持仓明细
        /// </summary>
        /// <param name="pd"></param>
        public void MarkPositionDetailSettled(PositionDetail pd)
        {
            _pdsettledcache.Write(pd);
            newlog();
        }
        /// <summary>
        /// 结算交易所结算
        /// </summary>
        /// <param name="settle"></param>
        public void MarkExchangeSettlementSettled(ExchangeSettlement settle)
        {
            _exsettlecache.Write(settle);
            newlog();
        }

        /// <summary>
        /// 结算出入金记录
        /// </summary>
        /// <param name="txn"></param>
        public void MarkCashTransactionSettled(CashTransaction txn)
        {
            _cashtxnsettlecash.Write(txn);
            newlog();
        }
        #endregion

        #region 插入或更新交易数据
        /// <summary>
        /// 将新的需要记录的数据记录下来 从而实现异步处理防止阻塞通讯主线程
        /// 数据记录需要copy模式,否则引用对象得其他线程访问时候会出现数据错误 比如成交数目与实际成交数目无法对应等问题。
        /// </summary>
        /// <param name="k"></param>
        public void NewOrder(Order o)
        {
            //debug("插入委托数据到数据库");
            Order oc = new OrderImpl(o);//复制委托 防止委托参数发生变化
            _ocache.Write(oc);
            newlog();
        }
        public void UpdateOrder(Order o)
        {
            //debug("插入委托数据到数据库");
            Order oc = new OrderImpl(o);
            _oupdatecache.Write(oc);
            newlog();
        }
        
        public void NewTrade(Trade f)
        {
            Trade nf = new TradeImpl(f);
            _tcache.Write(nf);
            newlog();
        }
        
        public void NewOrderAction(OrderAction action)
        {
            _oactioncache.Write(action);
            newlog();
        }


        public void NewPositionCloseDetail(PositionCloseDetail pc)
        {
            _posclosecache.Write(pc);
            newlog();
        }
        public void NewPositionDetail(PositionDetail pd)
        {
            _posdetailcache.Write(pd);
            newlog();
        }

        public void NewExchangeSettlement(ExchangeSettlement settle)
        {
            _exsettlementcache.Write(settle);
            newlog();
        }

        public void NewCashTransaction(CashTransaction txn)
        {
            _cashtxncache.Write(txn);
            newlog();
        }
        #endregion

        private void newlog()
        {
            /*
            if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.Unstarted))
            {
                _loggo = true;
                _logthread.IsBackground = true;
                
                _logthread.Start();
                
            }
            else if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set(); // signal ReadIt thread to read now
            }**/
            if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set();
            }
            
        }

        /// <summary>
        /// called if bad barrequest is written or read.
        /// check bad counters to see if written or read.
        /// </summary>
        public event VoidDelegate GotBadBR;
        /// <summary>
        /// called if buffer set is too small
        /// </summary>
        public event VoidDelegate GotBarRequestOverrun;
        /// <summary>
        /// # of null barrequest ignored at write
        /// </summary>
        public int BadBRWritten { get { return _nwt; } }
        /// <summary>
        /// # of null barrequest ignored at read
        /// </summary>
        public int BadBRRead { get { return _nrt; } }



        /// <summary>
        /// create an asynchronous responder
        /// </summary>
        public AsyncTransactionLoger() : this(MAXLOG) { }
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        /// <param name="maximb"></param>
        public AsyncTransactionLoger(int maxbr) : base("AsyncTransactionLoger")
        {
            _ocache = new RingBuffer<Order>(maxbr);
            _oupdatecache = new RingBuffer<Order>(maxbr);
            _osettlecache = new RingBuffer<Order>(maxbr);
            _tcache = new RingBuffer<Trade>(maxbr);
            _tsettlecache = new RingBuffer<Trade>(maxbr);

            _oactioncache = new RingBuffer<OrderAction>(maxbr);
            _posclosecache = new RingBuffer<PositionCloseDetail>(maxbr);
            _posdetailcache = new RingBuffer<PositionDetail>(maxbr);
            _exsettlementcache = new RingBuffer<ExchangeSettlement>(maxbr);
            _cashtxncache = new RingBuffer<CashTransaction>(maxbr);

            _pdsettledcache = new RingBuffer<PositionDetail>(maxbr);
            _exsettlecache = new RingBuffer<ExchangeSettlement>(maxbr);
            _cashtxnsettlecash = new RingBuffer<CashTransaction>(maxbr);

            _datareperrorcache = new RingBuffer<DataRepositoryLog>(maxbr);
        }

        void _brcache_BufferOverrunEvent()
        {
            if (GotBarRequestOverrun != null)
                GotBarRequestOverrun();
        }
        /// <summary>
        /// stop the read threads and shutdown (call on exit)
        /// </summary>
        public void Stop()
        {
            if (!_loggo) return;
            _loggo = false;
            ThreadTracker.Unregister(_logthread);

            /*
            try
            {
                if ((_logthread != null) && ((_logthread.ThreadState != System.Threading.ThreadState.Stopped) && (_logthread.ThreadState != System.Threading.ThreadState.StopRequested)))
                    _logthread.Interrupt();
            }
            catch { }
            try
            {
                //_brcache = new RingBuffer<BarRequest>(MAXLOG);
                _logwaiting.Reset();
            }
            catch { }**/
            int mainwait = 0;
            while (_logthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
             try
            {
                //_brcache = new RingBuffer<BarRequest>(MAXLOG);
                _logwaiting.Reset();
            }
            catch { }


            _logthread.Abort();
            _logthread = null;
        }

        public void Start()
        { 
            if(_loggo) return;
            _loggo  = true;
            _logthread = new Thread(this.readedata);
            _logthread.Name = "AsyncTransaction logger";
            _logthread.IsBackground = true;
            ThreadTracker.Register(_logthread);
            _logthread.Start();
 
        }

    }


}
