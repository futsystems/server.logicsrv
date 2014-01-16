using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouter
    {
        public event OrderErrorDelegate GotOrderErrorEvent;
        public event OrderActionErrorDelegate GotOrderActionErrorEvent;
        public event FillDelegate GotFillEvent;
        public event OrderDelegate GotOrderEvent;
        public event LongDelegate GotCancelEvent;


        /// <summary>
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter-X>Broker(order/cancel/trade/)
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter->ordermessage
        /// ordermessage和simbroker原先的问题一样,ansycserver的消息一直通过处理brokerrouter,brokerrouter如果又直接返回消息
        /// 进入tradingserver会形成消息闭路，我们有必要将消息进行缓存中断
        /// </summary>
        #region 对外转发交易信息
        const int buffersize = 1000;
        RingBuffer<Order> _ordercache = new RingBuffer<Order>(buffersize);
        RingBuffer<long> _cancelcache = new RingBuffer<long>(buffersize);
        RingBuffer<Trade> _fillcache = new RingBuffer<Trade>(buffersize);//成交缓存
        RingBuffer<OrderErrorPack> _errorordernotifycache = new RingBuffer<OrderErrorPack>(buffersize);//委托消息缓存

        Thread msgoutthread = null;
        bool msgoutgo = false;
        void msgoutprocess()
        {
            while (msgoutgo)
            {
                try
                {
                    //转发委托
                    while (_ordercache.hasItems)
                    {
                        if (GotOrderEvent != null)
                            GotOrderEvent(_ordercache.Read());
                    }
                    //转发成交
                    while (_fillcache.hasItems & !_ordercache.hasItems)
                    {
                        Trade fill = _fillcache.Read();
                        _tifengine.GotFill(fill);
                        if (GotFillEvent != null)
                            GotFillEvent(fill);
                    }
                    //转发取消
                    while (_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        long oid = _cancelcache.Read();
                        _tifengine.GotCancel(oid);
                        _ordHelper.GotCancel(oid);//发单辅助引擎得到委托
                        if (GotCancelEvent != null)
                            GotCancelEvent(oid);
                    }
                    //转发委托错误
                    while (_errorordernotifycache.hasItems & !_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        OrderErrorPack error = _errorordernotifycache.Read();
                        if (GotOrderErrorEvent != null)
                            GotOrderErrorEvent(error.Order, error.RspInfo);
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    debug(PROGRAM + ":process message out error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }
        void StartProcessMsgOut()
        {
            if (msgoutgo) return;
            msgoutgo = true;
            msgoutthread = new Thread(msgoutprocess);
            msgoutthread.IsBackground = true;
            msgoutthread.Name = "BrokerRouter MessageOut Thread";
            msgoutthread.Start();
            ThreadTracker.Register(msgoutthread);

        }

        void StopProcessMsgOut()
        {
            if (!msgoutgo) return;
            ThreadTracker.Unregister(msgoutthread);
            msgoutgo = false;
            int mainwait = 0;
            while (msgoutthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            msgoutthread.Abort();
            msgoutthread = null;
        }
        #endregion

    }
}
