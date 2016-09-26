using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Common.Logging;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
namespace Broker.SIM
{
    public class BOSIMTrader : TLBrokerBase, IBroker
    {

        bool _working = false;
        const int buffersize = 1000;
        RingBuffer<BinaryOptionOrder> _ordercache = new RingBuffer<BinaryOptionOrder>(buffersize);
        RingBuffer<BinaryOptionOrder> _ordernotifycache = new RingBuffer<BinaryOptionOrder>(buffersize);


        BOOrderBook orderbook = null;

        

        ConfigDB _cfgdb;
        public BOSIMTrader()
        {
            _cfgdb = new ConfigDB("BOSIMTrader");

            


            orderbook = new BOOrderBook();
            orderbook.OrderExitEvent += new Action<BinaryOptionOrder>(OnOrderExitEvent);

            //定时任务
            ITask task = new TaskProc("","定时判定委托","0 * * * * ?",OnSchedule);
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);

            //订阅行情数据
            TLCtxHelper.EventRouter.GotTickEvent +=new TickDelegate(GotTick);
            
        }

        void OnSchedule()
        {
            long dt = DateTime.Now.ToTLDateTime();
            logger.Info("BOPTT Schedule:" + dt);
            orderbook.GotTime(dt);
            
        }

        void GotTick(Tick k)
        {
            //logger.Info("got tick:" + k.ToString());
            orderbook.GotTick(k);
        }


        void OnOrderExitEvent(BinaryOptionOrder obj)
        {
            logger.Info("BOPTT Server Exit Order:" + obj.ToString());
            _ordernotifycache.Write(obj);
            NewNotify();
        }


        static ManualResetEvent _processwaiting = new ManualResetEvent(false);
        static ManualResetEvent _notifywaiting = new ManualResetEvent(false);
        public const int SLEEPDEFAULTMS = 10;
        int _sleep = SLEEPDEFAULTMS;
        /// <summary>
        /// sleep time in milliseconds between checking read buffer
        /// </summary>
        public int SLEEP { get { return _sleep; } set { _sleep = value; } }


        public void Start()
        {
            string msg = string.Empty;
            bool success = this.Start(out msg);
        }

        public bool Start(out string msg)
        {
            logger.Info("Start BinaryOption Match Engine");
            msg = string.Empty;
            StartProcess();
            StartProcOut();
            NotifyConnected();
            return true;
        }
        public void Stop()
        {
            if (!_working) return;
            _working = false;

            NotifyDisconnected();
        }

        public bool IsLive
        {
            get
            {
                return _working;
            }
            set
            {
                _working = value;
            }
        }






        #region 二元期权 开权 与数据回报线程

        Thread processthread = null;

        void StartProcess()
        {
            if (_working) return;
            _working = true;

            processthread = new Thread(process);
            processthread.IsBackground = true;
            processthread.Start();
        }

        void process()
        {
            while (_working)
            {
                try
                {
                    //关于环形缓冲使用,如果不等待信号或者一定时间会导致cpu一直尝试读取数据,导致每次获得的对象为空,即读写序号变化时 对象并没有放入对应的位置
                    while (_ordercache.hasItems)
                    {
                        BinaryOptionOrder o = _ordercache.Read();
                        if (o != null)
                        {
                            EntryOrder(o);
                        }
                    }

                    // clear current flag signal
                    _processwaiting.Reset();

                    // wait for a new signal to continue reading
                    _processwaiting.WaitOne(SLEEP);

                }
                catch (Exception ex)
                {
                    logger.Error("move order from ringbufeer to queue error:" + ex.ToString());
                }
            }
        
        }


        private void NewProcess()
        {
            if ((processthread != null) && (processthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _processwaiting.Set();
            }

        }

        void EntryOrder(BinaryOptionOrder o)
        {
            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot("",o.BinaryOption.Symbol);
            if(k == null)
            {
                //回报错误
                o.Status = EnumBOOrderStatus.Reject;
                o.Comment = "品种行情不存在";

                _ordernotifycache.Write(new BinaryOptionOrderImpl(o));
                NewNotify();
            }
            else
            {
                
                BinaryOptionOrder tmp = new BinaryOptionOrderImpl(o);
                //开权 并加入委托维护器
                BinaryOptionOrderImpl.EntryOrder(tmp, k);
                orderbook.GotOrder(tmp);
                logger.Info("BOPTT Server Entry Order:" + tmp);

                _ordernotifycache.Write(new BinaryOptionOrderImpl(tmp));
                NewNotify();
            }

        }
        #endregion

        #region 信息对外发送线程
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
                    while (_ordernotifycache.hasItems)
                    {
                        BinaryOptionOrder o = _ordernotifycache.Read();
                        NotifyBOOrder(o);
                    }

                    // clear current flag signal
                    _notifywaiting.Reset();

                    // wait for a new signal to continue reading
                    _notifywaiting.WaitOne(SLEEP);
                }
                catch (Exception ex)
                {
                    logger.Error("SIMBroker回传交易信息错误:" + ex.ToString());
                }
            }

        }

        private void NewNotify()
        {
            if ((_readthread != null) && (_readthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _notifywaiting.Set();
            }

        }
        #endregion


        #region 交易接口
        public void SendOrder(Order o)
        {
            throw new NotImplementedException();
        }


        int i = 0;
        public void SendOrder(BinaryOptionOrder o)
        {
            logger.Info("BOPTT Server Got Order: " + o.ToString());

            BinaryOptionOrder tmp = null;
            
            //提交委托时 设定localorderid remoteorderid
            o.Status = EnumBOOrderStatus.Submited;

            tmp = new BinaryOptionOrderImpl(o);
            _ordercache.Write(tmp);
            NewProcess();
        }

        public void CancelOrder(long id)
        {
   
            throw new NotImplementedException();
        }


        
        #endregion











        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public virtual IEnumerable<Order> Orders { get { return new List<Order>(); } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public virtual IEnumerable<Trade> Trades { get { return new List<Trade>(); } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public virtual IEnumerable<Position> Positions { get { return new List<Position>(); } }


        #region 持仓状态数据与判定
        /// <summary>
        /// 返回所有持仓状态统计数据
        /// </summary>
        public virtual IEnumerable<PositionMetric> PositionMetrics { get { return new List<PositionMetric>(); } }

        /// <summary>
        /// 获得某个合约的持仓状态统计数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual PositionMetric GetPositionMetric(string symbol)
        {
            return null;
        }

        /// <summary>
        /// 返回持仓预计调整量
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual int GetPositionAdjustment(Order o)
        {
            if (o.IsEntryPosition)
                return o.UnsignedSize;
            else
                return o.UnsignedSize * -1;
        }

        #endregion
    }
}
