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
        /// 通知实时行情
        /// </summary>
        /// <param name="k"></param>
        protected override void NotifyTick(Tick k)
        {
            tl.newTick(k);
        }

        /// <summary>
        /// 通知委托
        /// </summary>
        /// <param name="o"></param>
        protected override void  NotifyOrder(Order o)
        {
            //如果需要将委托状态通知发送到客户端 则设置needsend为true
            //路由中心返回委托回报时,发送给客户端的委托需要进行copy 否则后续GotOrderEvent事件如果对委托有修改,则会导致发送给客户端的委托发生变化,委托发送是在线程内延迟执行
            _ocache.Write(new OrderImpl(o));
        }

        /// <summary>
        /// 通知成交数据
        /// </summary>
        /// <param name="f"></param>
        protected override void NotifyFill(Trade f)
        {
            _fcache.Write(new TradeImpl(f));
        }

        /// <summary>
        /// 通知持仓更新
        /// </summary>
        /// <param name="pos"></param>
        protected override void NotifyPositionUpdate(Position pos)
        {
            _posupdatecache.Write(pos.GenPositionEx());

        }

        /// <summary>
        /// 通知委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected override void NotifyOrderError(Order o, RspInfo e)
        { 
            _errorordercache.Write(new OrderErrorPack(o,e));
        }

        /// <summary>
        /// 通知委托操作错误
        /// </summary>
        /// <param name="a"></param>
        /// <param name="e"></param>
        protected override void NotifyOrderActionError(OrderAction a, RspInfo e)
        {
            _erractioncache.Write(new OrderActionErrorPack(a, e));
        }


        /// <summary>
        /// 向客户端发送委托更新回报
        /// </summary>
        /// <param name="o"></param>
        protected override void NotifyBOOrder(BinaryOptionOrder o)
        {
            BOOrderNotify notify = ResponseTemplate<BOOrderNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;

            this.Send(notify);

        }

        /// <summary>
        /// 向客户端发送委托错误回报
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected override void NotifyBOOrderError(BinaryOptionOrder o, RspInfo e)
        {
            BOOrderErrorNotify notify = ResponseTemplate<BOOrderErrorNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;
            notify.RspInfo = e;

            this.Send(notify);

        }





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
