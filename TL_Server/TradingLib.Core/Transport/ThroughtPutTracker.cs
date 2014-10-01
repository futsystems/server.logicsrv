using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{

    /// <summary>
    /// 记录某个信息的通过量,经过修改可以做系统的流控
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThroughputTracker<T>
    {
        int StartNum = Const.TPStartNum;//当消息数量累计到多少时开始启动检测
        int CheckNum = Const.TPCheckNum;//启动检测后跟踪消息的数目(在这个数目内计算TP)
        double RejectValue = Const.TPRejectValue;//TP数值达到多少后拒绝该地址的消息
        double StopValue = Const.TPStartNum;//Tp数值降低到多少后停止检测

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        public ThroughputTracker() { }
        DateTime _start;
        int _count=0;
        DateTime _stop;
        public int Count { get { return _count; } }
        bool _started = false;
        public void Start() { if (_started) return; _start = DateTime.Now; _count = 0; _started = true; _stop = DateTime.MinValue; }
        public void Stop() { if (!_started) return; _stop = DateTime.Now; _count = 0; _started = false; }

        DateTime _heartbeattime=DateTime.Now;
        /// <summary>
        /// 最近一个消息达到时间,用于检测该客户端地址是否已经死亡
        /// </summary>
        DateTime LastBeatTime { get { return _heartbeattime; } }
        public bool IsDead
        {
            get
            {
                return DateTime.Now.Subtract(_heartbeattime).TotalSeconds > Const.CLIENTDEADTIME;
            }
        }

        /// <summary>
        /// 计算每秒的通过量
        /// </summary>
        public double Throughput
        {
            get
            {
                double elap = (_stop == DateTime.MinValue) ?
                    DateTime.Now.Subtract(_start).TotalSeconds :
                    _stop.Subtract(_start).TotalSeconds;
                if (elap == 0) return 0;
                return _count / elap;
            }
        }
        object _startstop = new object();
        /// <summary>
        /// 记录通过信息,用于计算流控TP值
        /// </summary>
        /// <param name="item"></param>
        public void newItem(T item)
        {
            _heartbeattime = DateTime.Now;
            _count++;
            if (_count > StartNum & !_started)//如果消息数大于一定阀值,并且没有开始监控则开始计算TP
            {
                lock (_startstop)
                {
                    //debug("start xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    Start();
                }
            }
        }

        public bool IsOverLoad
        {
            get
            {
                if (!_started) return false;//如果没有开始检测 则直接返回(消息数没有超过阀值,流控值TP默认不超标)
                //debug("TP:" + this.Throughput.ToString() + " xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                if (this.Throughput > RejectValue) return true;//如果TP超速 则拒绝消息
                //如果TP数值较小并且检测了一定数量的消息,则可以关闭监控
                if (this.Throughput < StopValue && _count > CheckNum)//启动之后_count会置0,那么我们就不可以直接根据throughtput<1.(刚开始由于基数小会得到TP=0) 需要检测一定数量的信息
                {
                    if (_started)
                    {
                        lock (_startstop)
                        {
                            //debug("stop xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                            Stop();//如果每秒小于1,则停止记录
                        }
                    }
                }
                //当TP小于stopvalue后系统就开始接受数据
                return false;
            }
        }
    }
}
