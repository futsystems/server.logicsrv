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
        
        const int buffersize = 1000;
        RingBuffer<Order> _ordercache = new RingBuffer<Order>(buffersize);
        RingBuffer<long> _cancelcache = new RingBuffer<long>(buffersize);
        RingBuffer<Trade> _fillcache = new RingBuffer<Trade>(buffersize);//成交缓存
        RingBuffer<OrderErrorPack> _errorordernotifycache = new RingBuffer<OrderErrorPack>(buffersize);//委托错误缓存
        RingBuffer<OrderActionErrorPack> _actionerrorcache = new RingBuffer<OrderActionErrorPack>(buffersize);//委托操作错误缓存
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
                        Order order = _ordercache.Read();
                        TLCtxHelper.EventRouter.FireOrderEvent(order);
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
                    //转发委托操作错误
                    while (_actionerrorcache.hasItems & !_cancelcache.hasItems & !_ordercache.hasItems & !_fillcache.hasItems)
                    {
                        OrderActionErrorPack error = _actionerrorcache.Read();
                        TLCtxHelper.EventRouter.FireOrderActionErrorEvent(error.OrderAction, error.RspInfo);
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
