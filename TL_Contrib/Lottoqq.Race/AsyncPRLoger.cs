﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;//记得加入此引用
using TradingLib.Common;
using TradingLib.API;
using TradingLib;

namespace Lottoqq.Race
{
    public class AsyncPRLoger : BaseSrvObject
    {
        //注当缓存超过后系统记录的交易信息就会发生错误。因此这里我们需要放大缓存大小并且在控制面板中需要监视。
        const int MAXLOG = 100000;
        
        RingBuffer<IPositionRound> _prcache;
      

        public int PRInCache { get { return _prcache.Count; } }
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
                    //插入委托
                    while (_prcache.hasItems)
                    {
                            
                        IPositionRound pr = _prcache.Read();

                        RaceService rs = RaceHelper.RaceServiceTracker[pr.Account];

                        if (rs == null || (!rs.IsActive))
                            continue;

                        try
                        {
                            debug("insert position round....", QSEnumDebugLevel.INFO);

                            TradingLib.ORM.MRacePositionTransaction.InsertPositionTransaction(pr);
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString(), QSEnumDebugLevel.ERROR);
                                
                        }
                        //string s = "交易日志:Order inserted:" + o.ToString();
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
                    debug(PROGRAME + ":交易日志持久化发生错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
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
        public void newPR(IPositionRound pr)
        {
            _prcache.Write(pr);
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
            else**/ if ((_logthread != null) && (_logthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _logwaiting.Set(); // signal ReadIt thread to read now
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
        public AsyncPRLoger() : this(MAXLOG) { }
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        /// <param name="maximb"></param>
        public AsyncPRLoger(int maxbr)
            : base("AsyncTransactionLoger")
        {
            _prcache = new RingBuffer<IPositionRound>(maxbr);


            
            //_brcache = new RingBuffer<BarRequest>(maxbr);
            //_brcache.BufferOverrunEvent += new VoidDelegate(_brcache_BufferOverrunEvent);
            //_readbarrequestthread = new Thread(this.ReadBarRequest);

        }

        void _brcache_BufferOverrunEvent()
        {
            if (GotBarRequestOverrun != null)
                GotBarRequestOverrun();
        }

        public void Start()
        {
            if (_loggo) return;
            _loggo = true;
            _logthread = new Thread(this.readedata);
            _logthread.Name = "AsyncTransaction logger";
            ThreadTracker.Register(_logthread);

            _logthread.Start();
        }
        /// <summary>
        /// stop the read threads and shutdown (call on exit)
        /// </summary>
        public void Stop()
        {
            if (!_loggo) return;
            ThreadTracker.Unregister(_logthread);
            _loggo = false;
            Util.WaitThreadStop(_logthread);

            try
            {
                //_brcache = new RingBuffer<BarRequest>(MAXLOG);
                _logwaiting.Reset();
            }
            catch { }

            /*
            _loggo = false;
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
        }

    }
}