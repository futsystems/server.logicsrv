using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouterPassThrough
    {
        /// <summary>
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter-X>Broker(order/cancel/trade/)
        /// ansyaserver->tlserver_mq->tradingserver->Brokerrouter->ordermessage
        /// ordermessage和simbroker原先的问题一样,ansycserver的消息一直通过处理brokerrouter,brokerrouter如果又直接返回消息
        /// 进入tradingserver会形成消息闭路，我们有必要将消息进行缓存中断
        /// </summary>
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
                        TLCtxHelper.EventRouter.FireOrderEvent(_ordercache.Read());
                    }
                    //转发成交
                    while (_fillcache.hasItems & !_ordercache.hasItems)
                    {
                        Trade fill = _fillcache.Read();
                        TLCtxHelper.EventRouter.FireFillEvent(fill);
                    }
                    //转发取消
                    while (_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        long oid = _cancelcache.Read();
                        TLCtxHelper.EventRouter.FireCancelEvent(oid);
                    }
                    //转发委托错误
                    while (_errorordernotifycache.hasItems & !_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        OrderErrorPack error = _errorordernotifycache.Read();
                        TLCtxHelper.EventRouter.FireOrderError(error.Order, error.RspInfo);
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    logger.Info(PROGRAME + ":process message out error:" + ex.ToString());
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
    }
}
