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
        //建立连接 注意这里的数据库读写是单线程的 是符合线程安全的
        //mysqlDBTransaction mysql;// = new mysqlDB();
        //FastDBTransaction fastdb;
        RingBuffer<Order> _ocache;
        RingBuffer<Order> _oupdatecache;
        RingBuffer<Trade> _tcache;
        RingBuffer<long> _ccache;
        RingBuffer<IPositionRound> _postranscache;
        RingBuffer<OrderAction> _oactioncache;
        RingBuffer<PositionCloseDetail> _posclosecache;
        //RingBuffer<PositionRound> _postransopencache;

        public int OrderInCache { get { return _ocache.Count; } }
        public int TradeInCache { get { return _tcache.Count; } }
        public int CancleInCache { get { return _ccache.Count; } }
        public int OrderUpdateInCache { get { return _oupdatecache.Count; } }
        public int PosTransInCache { get { return _postranscache.Count; } }
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
            debug("插入委托时间消耗:"+ordert.Elapsed.ToString(),QSEnumDebugLevel.MUST);
            debug("更新委托时间消耗:" + updateordert.Elapsed.ToString(), QSEnumDebugLevel.MUST);
            debug("插入成交时间消耗:" + tradet.Elapsed.ToString(), QSEnumDebugLevel.MUST);
            debug("插入取消时间消耗:" + tradet.Elapsed.ToString(), QSEnumDebugLevel.MUST);
            
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
                        try
                        {
                            //插入委托
                            while (_ocache.hasItems)
                            {
                                bool re = false;
                                Order o = _ocache.Read();
                                try
                                {
                                    ordert.Start();
                                    re = ORM.MTradingInfo.InsertOrder(o);
                                    ordert.Stop();
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(),QSEnumDebugLevel.ERROR);
                                    throw (new QSTranLogOrderError(o));
                                }
                                string s = "交易日志:Order Inserted:" + o.GetOrderInfo();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + "失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);

                            }
                            //更新委托状态
                            while (!_ocache.hasItems && _oupdatecache.hasItems)
                            {
                                bool re = false;
                                Order o = _oupdatecache.Read();
                                try
                                {
                                    updateordert.Start();
                                    re = ORM.MTradingInfo.UpdateOrderStatus(o);
                                    updateordert.Stop();
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                    throw (new QSTranLogOrderUpdateError(o));
                                }
                                string s = "交易日志:Order Update:" + o.GetOrderStatus();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + "失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);
                            }
                            //插入成交
                            while (!_ocache.hasItems && _tcache.hasItems)
                            {
                                bool re = false;
                                Trade f = _tcache.Read();
                                try
                                {
                                    tradet.Start();
                                    re = ORM.MTradingInfo.InsertTrade(f);
                                    tradet.Stop();
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                    throw (new QSTranLogFillError(f));
                                }
                                string s = "交易日志:Trade Inserted:" + f.GetTradeInfo();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + " 失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);

                            }
                            //插入取消
                            while (!_ocache.hasItems && _ccache.hasItems)
                            {
                                bool re = false;
                                long oid = _ccache.Read();
                                try
                                {
                                    canclet.Start();
                                    re = ORM.MTradingInfo.InsertCancel(Util.ToTLDate(DateTime.Now), Util.ToTLTime(DateTime.Now), oid);
                                    canclet.Stop();
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                    throw (new QSTranLogCancelError(oid));
                                }
                                string s = "交易日志:Cancle inserted:" + oid.ToString();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + " 失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);
                            }

                            while (!_ocache.hasItems && _oactioncache.hasItems)
                            {
                                bool re = false;
                                OrderAction action = _oactioncache.Read();
                                try
                                {
                                    re = ORM.MTradingInfo.InsertOrderAction(action);
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                }

                                string s = "交易日志:OrderAction Inserted" + OrderActionImpl.Serialize(action);
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + " 失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);

                            }

                            //插入取消
                            while (_postranscache.hasItems)
                            {
                                bool re = false;
                                IPositionRound pr = _postranscache.Read();
                                try
                                {
                                    re = ORM.MTradingInfo.InsertPositionRound(pr);
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                    //throw (new QSTranLogCancelError(oid));
                                }
                                string s = "交易日志:PosTransaction Inserted:";// +oid.ToString();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + " 失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);
                            }

                            while (_posclosecache.hasItems)
                            {
                                bool re = false;
                                PositionCloseDetail close = _posclosecache.Read();
                                try
                                {
                                    re = ORM.MSettlement.InsertPositionCloseDetail(close);
                                }
                                catch (Exception ex)
                                {
                                    debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                }
                                string s = "平仓明细日志:PositionCloseDetail Inserted:";// +oid.ToString();
                                if (!re)
                                {
                                    _nrt++;
                                    debug(s + " 失败", QSEnumDebugLevel.ERROR);
                                }
                                else
                                {
                                    debug(s + " 成功", QSEnumDebugLevel.INFO);
                                }
                                Thread.Sleep(_delay);
                            }



                            // clear current flag signal
                            _logwaiting.Reset();

                            // wait for a new signal to continue reading
                            _logwaiting.WaitOne(SLEEP);
                            //Thread.Sleep(1000);
                        }
                        //以下代码段通过捕捉交易日志插入部分的异常,将没有正常插入的数据重新返回到缓存队列
                        //mysql则通过不断尝试进行数据库连接,当连接成功后重新将日志插入数据库
                        catch (QSTranLogOrderError ex)
                        {
                            debug(ex.OFail.ToString(), QSEnumDebugLevel.ERROR);
                            debug(PROGRAME +":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                             //this.newOrder(ex.OFail);
                        }
                        catch (QSTranLogOrderUpdateError ex)
                        {
                            debug(ex.OFail.ToString(), QSEnumDebugLevel.ERROR);
                            debug(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            //this.updateOrder(ex.OFail);
                        }
                        catch (QSTranLogFillError ex)
                        {
                            debug(ex.FFail.ToString(), QSEnumDebugLevel.ERROR);
                            debug(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            //this.newTrade(ex.FFail);
                        }
                        catch (QSTranLogCancelError ex)
                        {
                            debug(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            //this.newCancle(ex.CFail);
                        }
                        catch (Exception ex)
                        {
                            debug(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    catch (Exception ex)
                    {
                        //在异常捕捉时，我们会将没有记录的交易信息返回缓存进行处理,如果在返回遗漏交易信息的时候发生错误,则我们这里还会产生异常,导致系统崩溃
                        debug(PROGRAME + ":异常捕捉产生错误,会发生遗漏委托记录" + ex.ToString());
                    }
                }
        }

        public const int SLEEPDEFAULTMS = 10;
        int _sleep = SLEEPDEFAULTMS;
        /// <summary>
        /// sleep time in milliseconds between checking read buffer
        /// </summary>
        public int SLEEP { get { return _sleep; } set { _sleep = value; } }

        /// <summary>
        /// 将新的需要记录的数据记录下来 从而实现异步处理防止阻塞通讯主线程
        /// 数据记录需要copy模式,否则引用对象得其他线程访问时候会出现数据错误 比如成交数目与实际成交数目无法对应等问题。
        /// </summary>
        /// <param name="k"></param>
        public void newOrder(Order o)
        {
            //debug("插入委托数据到数据库");
            Order oc = new OrderImpl(o);//复制委托 防止委托参数发生变化
            _ocache.Write(oc);
            newlog();
        }
        public void updateOrder(Order o)
        {
            //debug("插入委托数据到数据库");
            Order oc = new OrderImpl(o);
            _oupdatecache.Write(oc);
            newlog();
        }
        public void newTrade(Trade f)
        {
            Trade nf = new TradeImpl(f);
            _tcache.Write(nf);
            newlog();
        }
        public void newCancle(long id)
        {
            _ccache.Write(id);
            newlog();
        }

        public void newOrderAction(OrderAction action)
        {
            _oactioncache.Write(action);
            newlog();
        }

        public void newPositonRound(IPositionRound pr)
        {
            _postranscache.Write(pr);
            newlog();
        }

        public void newPositionCloseDetail(PositionCloseDetail pc)
        {
            _posclosecache.Write(pc);
            newlog();
        }
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
            _tcache = new RingBuffer<Trade>(maxbr);
            _ccache = new RingBuffer<long>(maxbr);
            _postranscache = new RingBuffer<IPositionRound>(maxbr);
            _oactioncache = new RingBuffer<OrderAction>(maxbr);
            _posclosecache = new RingBuffer<PositionCloseDetail>(maxbr);
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
