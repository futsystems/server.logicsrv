using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.FinService
{
    /// <summary>
    /// 异步记录收费记录
    /// </summary>
    public class FeeChargeItemLogger : BaseSrvObject
    {
        //注当缓存超过后系统记录的交易信息就会发生错误。因此这里我们需要放大缓存大小并且在控制面板中需要监视。
        const int MAXLOG = 100000;
        //建立连接 注意这里的数据库读写是单线程的 是符合线程安全的
        //mysqlDBTransaction mysql;// = new mysqlDB();
        //FastDBTransaction fastdb;
        RingBuffer<FeeChargeItem> _feechargecache = new RingBuffer<FeeChargeItem>(MAXLOG);
        

        public int FeeChargeInCache { get { return _feechargecache.Count; } }
        

        static ManualResetEvent _logwaiting = new ManualResetEvent(false);
        Thread _logthread = null;
        public bool isValid { get { return _loggo; } }

        int _nwt;
        int _nrt;

        bool _loggo = false;
        int _delay = 0;

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
                    //插入收费记录
                    while (_feechargecache.hasItems)
                    {
                        bool re = false;
                        FeeChargeItem o = _feechargecache.Read();
                        try
                        {
                            re = ORM.MFeeChargeItem.InsertFeeChargeItem(o);
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                        string s = "服务收费日志:FeeCharge inserted:" + o.ToString();
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

                    // clear current flag signal
                    _logwaiting.Reset();

                    // wait for a new signal to continue reading
                    _logwaiting.WaitOne(SLEEP);
                    //Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    debug(PROGRAME + ":服务收费日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
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
        public void newFeeChargeItem(FeeChargeItem charge)
        {
            _feechargecache.Write(charge);
            newlog();
        }

        private void newlog()
        {
            if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set();
            }
        }


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
        public FeeChargeItemLogger() : this(MAXLOG) { }
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        /// <param name="maximb"></param>
        public FeeChargeItemLogger(int maxbr)
            : base("AsyncFeeChargeItemLoger")
        {
            
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
            if (_loggo) return;
            _loggo = true;
            _logthread = new Thread(this.readedata);
            _logthread.Name = "AsyncFeeChargeItemLoger logger";
            _logthread.IsBackground = true;
            ThreadTracker.Register(_logthread);
            _logthread.Start();

        }

    }

}
