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
        Queue<BinaryOptionOrder> aq = new Queue<BinaryOptionOrder>(buffersize);

        RingBuffer<BinaryOptionOrder> _ordernotifycache = new RingBuffer<BinaryOptionOrder>(buffersize);


        BOOrderBook orderbook = null;
        
        public BOSIMTrader()
        {
            
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
            long dt = DateTime.Now.ToTLTime();
            logger.Info("on schedule run:" + dt);
            orderbook.GotTime(dt);
            
        }

        void GotTick(Tick k)
        {
            logger.Info("got tick:" + k.ToString());
            orderbook.GotTick(k);
        }


        void OnOrderExitEvent(BinaryOptionOrder obj)
        {
            _ordernotifycache.Write(obj);
        }



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
            try
            {
                while (_working)
                {
                    while (_ordercache.hasItems)
                    {
                        EntryOrder(_ordercache.Read());
                    }

                    //对外通知回报
                    while (_ordernotifycache.hasItems)
                    {
                        NotifyBOOrder(_ordernotifycache.Read());
                    }

                }
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error("move order from ringbufeer to queue error:" + ex.ToString());
            }
        }


        void EntryOrder(BinaryOptionOrder o)
        {
            logger.Info("BOPTT Server Entry Order: " + o.ToString());
            Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Symbol);
            if(k == null)
            {
                //回报错误
                o.Status = EnumBOOrderStatus.Reject;
                o.Comment = "品种行情不存在";
                _ordernotifycache.Write(new BinaryOptionOrderImpl(o));
            }
            else
            {
                BinaryOptionOrder tmp = new BinaryOptionOrderImpl(o);
                //开权 并加入委托维护器
                BinaryOptionOrderImpl.EntryOrder(tmp, k);
                orderbook.GotOrder(tmp);
            }

        }
        #endregion




        #region 交易接口
        public void SendOrder(Order o)
        {
            throw new NotImplementedException();
        }

        

        public void SendOrder(BinaryOptionOrder o)
        {
            logger.Error("BOPTT Server Got Order: " + o.ToString());
            //提交委托时 设定localorderid remoteorderid

            o.Status = EnumBOOrderStatus.Submited;

            BinaryOptionOrder tmp = new BinaryOptionOrderImpl(o);
            _ordercache.Write(tmp);
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
