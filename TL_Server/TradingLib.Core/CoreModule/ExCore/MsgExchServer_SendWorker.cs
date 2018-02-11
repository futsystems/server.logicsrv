//Copyright 2013 by FutSystems,Inc.
//20161223  整理消息发送线程

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class MsgExchServer
    {
        /// <summary>
        /// 
        /// </summary>
        void StartSendWorker()
        {
            if (_sendgo) return;
            _sendgo = true;
            messageoutthread = new Thread(messageout);
            messageoutthread.IsBackground = true;
            messageoutthread.Name = "TradingServer MessageOut Thread";
            messageoutthread.Start();
            ThreadTracker.Register(messageoutthread);
        }

        void StopSendWorker()
        {
            if (!_sendgo) return;
            _sendgo = false;
            ThreadTracker.Unregister(messageoutthread);
            int mainwait = 0;
            while (messageoutthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            messageoutthread.Abort();
            messageoutthread = null;

        }
        const int BUFFERSIZE = 20000;
        RingBuffer<Order> _ocache = new RingBuffer<Order>(BUFFERSIZE);//委托缓存
        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(BUFFERSIZE);//成交缓存
        RingBuffer<OrderErrorPack> _errorordercache = new RingBuffer<OrderErrorPack>(BUFFERSIZE);//委托错误缓存
        RingBuffer<OrderActionErrorPack> _erractioncache = new RingBuffer<OrderActionErrorPack>(BUFFERSIZE);//委托操作错误缓存
        RingBuffer<PositionEx> _posupdatecache = new RingBuffer<PositionEx>(BUFFERSIZE);//持仓更新缓存

        RingBuffer<IPacket> _packetcache = new RingBuffer<IPacket>(BUFFERSIZE);//数据包缓存队列
        //RingBuffer<IPacket> _frontNotifyCache = new RingBuffer<IPacket>(BUFFERSIZE);//发往前置的业务通知队列

        bool _sendgo = false;
        Thread messageoutthread;

        /// <summary>
        /// 所有需要转发到客户端的消息均通过缓存进行，这样避免了多个线程同时操作一个ZeroMQ socket
        /// 所有发送消息的过程产生的异常将在这里进行捕捉
        /// </summary>
        void messageout()
        {
            while (_sendgo)
            {
                try
                {
                    //发送委托
                    while (_ocache.hasItems)
                    {
                        tl.newOrder(_ocache.Read());
                    }
                    //转发委托错误
                    while (!_ocache.hasItems && _errorordercache.hasItems)
                    {
                        OrderErrorPack e = _errorordercache.Read();
                        ErrorOrderNotify notify = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(e.Order.Account);
                        notify.Order = e.Order;
                        notify.RspInfo = e.RspInfo;

                        tl.newOrderError(notify);
                    }
                    //转发委托操作错误
                    while (!_ocache.hasItems && !_errorordercache.hasItems && _erractioncache.hasItems)
                    {
                        OrderActionErrorPack e = _erractioncache.Read();
                        ErrorOrderActionNotify notify = ResponseTemplate<ErrorOrderActionNotify>.SrvSendNotifyResponse(e.OrderAction.Account);
                        notify.OrderAction = e.OrderAction;
                        notify.RspInfo=e.RspInfo;
                        tl.newOrderActionError(notify);
                    }

                    //发送成交
                    while (!_ocache.hasItems && _fcache.hasItems)
                    {
                        tl.newFill(_fcache.Read());

                    }

                    //发送持仓更新信息
                    while (_posupdatecache.hasItems && !_ocache.hasItems && !_fcache.hasItems)
                    {
                        tl.newPositionUpdate(_posupdatecache.Read());

                    }

                    //当优先级的消息缓存没有消息的时候才发送其他消息
                    while (_packetcache.hasItems)
                    {
                        tl.TLSend(_packetcache.Read());
                    }

                    //while (_frontNotifyCache.hasItems)
                    //{
                    //    tl.TLNotifyFront(_frontNotifyCache.Read());
                    //}
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    logger.Error("消息发送线程出错 " + ex.ToString());
                }
            }

        }
    }
}
