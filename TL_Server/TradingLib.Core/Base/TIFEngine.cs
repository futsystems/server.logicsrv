using System;
using System.Collections.Generic;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using System.Diagnostics;

namespace TradingLib.Core
{
    /// <summary>
    /// Order TIF引擎,用于追踪委托的有效时间,然后针对性的撤单，在系统内部实现Time in Force功能
    /// GenericTracker<long>,GenericTrackerLong, 
    /// 关于TIF引擎的拓展
    /// 我们可以定义一些简单行为 比如取消,
    /// </summary>
    public class TIFEngine : SendOrderIndicator, SendCancelIndicator, GotCancelIndicator, GotFillIndicator
    {
        public TIFEngine() : this(new IdTracker()) { }
        public TIFEngine(IdTracker id)
        {
            _idt = id;
        }
        public event LongDelegate SendCancelEvent;
        public event OrderDelegate SendOrderEvent;
        public event DebugDelegate SendDebugEvent;
        int _tif = 0;
        public int DefaultTif { get { return _tif; } set { _tif = value; } }//默认的Tif数值
        IdTracker _idt = null;
        const int NOIDX = -1;
        int _lasttime = 0;

        /// <summary>
        /// 获得某个Order的tif值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int gettif(long id) { return tiflist.FindByOrderID(id).TIF; }
        //ringbuffer可以再多个线程间传递数据,而不用lock。该类型是线程安全的
        RingBuffer<TIFInfo> finishedOrder = new RingBuffer<TIFInfo>(500);
        RingBuffer<TIFInfo> insertOrder = new RingBuffer<TIFInfo>(500);
        TIFInfoList tiflist = new TIFInfoList();
        //检查Tif值
        void checktifs()
        {
            // can't check until time is set
            if (_lasttime == 0) return;

            while (finishedOrder.hasItems)
            {
                //委托取消 成交 将会直接从列表中删除
                tiflist.Remove(finishedOrder.Read());
            }
            while (insertOrder.hasItems)
            {
                tiflist.Add(insertOrder.Read());
            }

            foreach (TIFInfo tifinfo in tiflist)
            {
                // skip if tif is zero
                int tif = tifinfo.TIF;
                if (tif == 0)
                {
                    finishedOrder.Write(tifinfo);
                    continue;
                }

                int senttime = tifinfo.SentTime;//获得序列中的发送Order时间
                //debug("senttime:" + senttime.ToString() + "_lastTime:" + _lasttime.ToString());
                int diff = Util.FTDIFF(senttime, _lasttime);//比较发送时间与当前时间的偏差 如果时间消逝大于Tif则取消
                if (diff >= tif)
                {
                    debug("[TIFEngine]Tif expired for: " + tifinfo.OrderID.ToString() + " at time: " + _lasttime + " from: " + senttime + " secs: " + diff);
                    if (SendCancelEvent != null)
                        SendCancelEvent(tifinfo.OrderID);
                    else
                        debug("[TIFEngine]SendCancel unhandled! can't enforce TIF!");
                }

                //debug(DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString());
                //watch.Stop();
                //debug("监测TIF消耗时间:"+watch.Elapsed.ToString());

            }
        }

        public void GotCancel(long id)
        {
            TIFInfo tifinfo = tiflist.FindByOrderID(id);
            if (tifinfo == null) return;
            debug("[TIFEngine]cancel received for tif(sec) order: " + id);
            //不需要在监护该委托，将其数据从缓存清除
            finishedOrder.Write(tifinfo);
        }

        public void GotFill(Trade fill)
        {
            long id = fill.id;
            TIFInfo tifinfo = tiflist.FindByOrderID(id);
            if (tifinfo == null) return;
            tifinfo.FilledSize += Math.Abs(fill.xsize);
            // get symbol
            string sym = fill.symbol;
            // see if completed
            if (tifinfo.FilledSize == tifinfo.SentSize)
            {
                // mark as done
                debug("[TIFEngine] "+sym+" completely filled: " + id);
                //不需要在监护该委托，将其数据从缓存清除
                finishedOrder.Write(tifinfo);
            }
            else
                debug("[TIFEngine] " + sym + " partial fill: " + id);
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 清空内存缓存数据
        /// </summary>
        public void Clear()
        { 
        
        }

        public static Stopwatch watch = new Stopwatch(); 
        public void SendOrder(Order o)
        {
            // get tif from order
            int tif = 0;
            if (int.TryParse(o.TIF, out tif))
                SendOrderTIF(o, tif);
            else
                SendOrderTIF(o, 0);
        }

        public void SendOrderTIF(Order o, int TIF)
        {
            // update time
            if ((o.time != 0) && (o.time > _lasttime))
                _lasttime = o.time;
            if ((o.time == 0) && (_lasttime == 0))
            {
                debug("[TIFEngine]No time available!  Can't enforce tif!");
                return;
            }
            // make sure it has an id
            if (o.id == 0)
            {
                o.id = _idt.AssignId;
            }
            // 如果TIF引擎没有该委托,则我们生成tifinfo并将其放置到insertOrder缓存中
            if (!tiflist.HaveOrder(o.id))
            {
                debug("[TIFEngine]tracking tif for: " + o.id + " " + o.symbol + " tif:" + o.TIF);
                TIFInfo tifinfo = new TIFInfo(o.id, _lasttime, TIF, o.UnsignedSize, 0);
                insertOrder.Write(tifinfo);
            }
            // pass order along if required
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        #region TIF检查线程 在一定的时间尺度上对委托进行检查
        Thread tifthread = null;
        bool _tifgo = false;
        int _freq = 500;
        int Freq { get { return _freq; } set { _freq = value; } }
        void tiffun()
        {
            while (_tifgo)
            {
                _lasttime = Util.ToTLTime(DateTime.Now);
                checktifs();
                Thread.Sleep(_freq);
            }
        }
        public void Start()
        {
            if (_tifgo) return;
            _tifgo = true;
            tifthread = new Thread(new ThreadStart(tiffun));
			tifthread.Name="TIF Engine ";
            tifthread.Start();
			ThreadTracker.Register(tifthread);
        }

        public void Stop()
        {
            if (!_tifgo) return;
            _tifgo = false;
            tifthread.Abort();
        }
        #endregion

    }


    class TIFInfoList:List<TIFInfo>
    {

        public bool HaveOrder(long orderid)
        {
            foreach (TIFInfo tif in this)
            {
                if (tif.OrderID == orderid)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 通过OrderId找到对应的tifinfo
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public TIFInfo FindByOrderID(long orderid)
        {
            foreach (TIFInfo tif in this)
            {
                if (tif.OrderID == orderid)
                    return tif;
            }
            return null;
        }
        
    }
    class TIFInfo
    {
        public TIFInfo(long orderid,int senttime,int tif,int sentsize ,int filledsize)
        {
            OrderID = orderid;
            SentTime = senttime;
            TIF = tif;
            SentSize = sentsize;
            FilledSize = filledsize;
        }
        public long OrderID;
        public int SentTime;
        public int TIF;
        public int SentSize;
        public int FilledSize;

    }
}
