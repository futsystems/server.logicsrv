using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于异步执行tick处理,主线程直接将Tick放入ringbuffer中,然后在单独的线程中去处理Tick
    /// </summary>
    public class AsyncResponse
    {
        const int MAXTICK = 10000;
        const int MAXIMB = 100000;
        RingBuffer<Tick> _tickcache;

        /// <summary>
        /// 当从缓存中异步读取一个tick时触发
        /// </summary>
        public event TickDelegate GotTick;
        /// <summary>
        ///  当缓存内没有数据时触发
        /// </summary>
        public event VoidDelegate GotTickQueueEmpty;
        /// <summary>
        /// 当有新的Tick数据写入缓存时触发
        /// </summary>
        public event VoidDelegate GotTickQueued;
        /// <summary>
        /// 当缓存太小,Tick数据溢出时触发
        /// </summary>
        public int TickOverrun { get { return _tickcache.BufferOverrun; } }

        static ManualResetEvent _tickswaiting = new ManualResetEvent(false);
        Thread _readtickthread = null;

        volatile bool _readtick = false;
        int _nrt = 0;
        int _nwt = 0;

        /// <summary>
        /// 是否处于有效工作状态
        /// </summary>
        public bool isValid { get { return _readtick; } }
        /// <summary>
        /// 是否处于工作状态
        /// </summary>
        public bool IsRunning { get { return _readtick; } }
        void ReadTick()
        {
            while (_readtick)
            {
                try
                {

                    if (_tickcache.hasItems && (GotTickQueued != null))
                        GotTickQueued();

                    while (_tickcache.hasItems)
                    {
                        if (!_readtick)
                            break;
                        Tick k = _tickcache.Read();
                        if (k == null)
                        {
                            _nrt++;
                            if (GotBadTick != null)
                                GotBadTick();
                            continue;
                        }
                        if (GotTick != null)
                            GotTick(k);
                    }

                    // send event that queue is presently empty
                    if (_tickcache.isEmpty && (GotTickQueueEmpty != null))
                        GotTickQueueEmpty();
                    // clear current flag signal
                    _tickswaiting.Reset();
                    // wait for a new signal to continue reading
                    _tickswaiting.WaitOne(SLEEP);

                }
                catch (Exception ex)
                {
                    
                    Util.Debug("process tick error:" + ex.ToString());
                }
            }
        }

        public const int SLEEPDEFAULTMS = 10;
        int _sleep = SLEEPDEFAULTMS;
        /// <summary>
        /// 每隔多少时间检查tick buffer中是否有新的行情数据
        /// </summary>
        public int SLEEP { get { return _sleep; } set { _sleep = value; } }

        /// <summary>
        /// 当有行情到达时调用该函数,将行情tick放入Ringbuffer中,用于后台线程异步处理
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            if (k == null)
            {
                _nwt++;
                if (GotBadTick != null)
                    GotBadTick();
                return;
            }
            _tickcache.Write(k);
            
            if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.Unstarted))
            {
                _readtick = true;
                _readtickthread.Start();

            }
            else
            if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                _tickswaiting.Set(); // signal ReadIt thread to read now
            }
        }

        /// <summary>
        /// 当有异常tick读取时 触发
        /// </summary>
        public event VoidDelegate GotBadTick;
        /// <summary>
        /// 当缓存大小太小时候触发
        /// </summary>
        public event VoidDelegate GotTickOverrun;
        /// <summary>
        /// # of null ticks ignored at write
        /// </summary>
        public int BadTickWritten { get { return _nwt; } }
        /// <summary>
        /// # of null ticks ignored at read
        /// </summary>
        public int BadTickRead { get { return _nrt; } }

        
        /// <summary>
        /// create an asynchronous responder
        /// </summary>
        public AsyncResponse(string name) : this(name,MAXTICK) { }

        string _name = "";
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        public AsyncResponse(string name,int maxticks)
        {
            _name = name;
            _tickcache = new RingBuffer<Tick>(maxticks);
            _tickcache.BufferOverrunEvent += new VoidDelegate(_tickcache_BufferOverrunEvent);
           
            //_readtickthread = new Thread(this.ReadTick);
            //_readtickthread.Name = "AsyncTickResponse-"+_name;
            //ThreadTracker.Register(_readtickthread);
        }

        void _tickcache_BufferOverrunEvent()
        {
            if (GotTickOverrun != null)
                GotTickOverrun();
        }
        /// <summary>
        /// stop the read threads and shutdown (call on exit)
        /// </summary>
        public void Stop()
        {
            /*
            _readtick = false;
            try
            {
                if ((_readtickthread != null) && ((_readtickthread.ThreadState != ThreadState.Stopped) && (_readtickthread.ThreadState != ThreadState.StopRequested)))
                    _readtickthread.Interrupt();
            }
            catch { }
            try
            {
                _tickcache = new RingBuffer<Tick>(MAXTICK);
                _tickswaiting.Reset();
            }
            catch { }
             * **/
            if (!_readtick) return;
            ThreadTracker.Unregister(_readtickthread);
            _readtick = false;
            int mainwait = 0;
            while (_readtickthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            try
            {
                _tickswaiting.Reset();
            }
            catch { }

            _readtickthread.Abort();
            _readtickthread = null;
            //_readtickthread
        }


        public void Start()
        {
            if (_readtick) return;
            _readtick = true;
            _readtickthread = new Thread(this.ReadTick);
            _readtickthread.Name = "AsyncTickResponse-" + _name;
            ThreadTracker.Register(_readtickthread);

            _readtickthread.Start();
            
        }

    }
}
