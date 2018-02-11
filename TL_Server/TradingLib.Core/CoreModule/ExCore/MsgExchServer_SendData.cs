//Copyright 2013 by FutSystems,Inc.
//20161223 将数据发送类操作整理到一个文件中

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MsgExchServer
    {
        public void Send(IPacket packet)
        {
            _packetcache.Write(packet);
        }
        /// <summary>
        /// 缓存应答数据包
        /// 客户端提交查询或操作时 将应答数据缓存到发送队列
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="islat"></param>
        void CacheRspResponse(RspResponsePacket packet, bool islat = true)
        {
            packet.IsLast = islat;
            CachePacket(packet);
        }
        /// <summary>
        /// 缓存数据包
        /// 此处为业务数据包发送唯一入口(交易数据通过Notify函数写入特定缓存进行发送) 
        /// </summary>
        /// <param name="packet"></param>
        void CachePacket(IPacket packet)
        {
            _packetcache.Write(packet);
        }

        /// <summary>
        /// 通知实时行情
        /// </summary>
        /// <param name="k"></param>
        protected void NotifyTick(Tick k)
        {
            tl.newTick(k);
        }

        /// <summary>
        /// 通知委托
        /// </summary>
        /// <param name="o"></param>
        protected void NotifyOrder(Order o)
        {
            //如果需要将委托状态通知发送到客户端 则设置needsend为true
            //路由中心返回委托回报时,发送给客户端的委托需要进行copy 否则后续GotOrderEvent事件如果对委托有修改,则会导致发送给客户端的委托发生变化,委托发送是在线程内延迟执行
            _ocache.Write(o);//new OrderImpl(o));
        }

        /// <summary>
        /// 通知成交数据
        /// </summary>
        /// <param name="f"></param>
        protected void NotifyFill(Trade f)
        {
            _fcache.Write(f);//new TradeImpl(f));
        }

        /// <summary>
        /// 通知持仓更新
        /// </summary>
        /// <param name="pos"></param>
        protected void NotifyPositionUpdate(Position pos)
        {
            _posupdatecache.Write(pos.GenPositionEx());

        }

        /// <summary>
        /// 通知委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void NotifyOrderError(Order o, RspInfo e)
        {
            _errorordercache.Write(new OrderErrorPack(o, e));
        }

        /// <summary>
        /// 通知委托操作错误
        /// </summary>
        /// <param name="a"></param>
        /// <param name="e"></param>
        protected void NotifyOrderActionError(OrderAction a, RspInfo e)
        {
            _erractioncache.Write(new OrderActionErrorPack(a, e));
        }

        protected void NotifyCancel(long oid)
        { 
            
        }

        /// <summary>
        /// 向客户端发送委托更新回报
        /// </summary>
        /// <param name="o"></param>
        protected void NotifyBOOrder(BinaryOptionOrder o)
        {
            BOOrderNotify notify = ResponseTemplate<BOOrderNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;

            CachePacket(notify);

        }

        /// <summary>
        /// 向客户端发送委托错误回报
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void NotifyBOOrderError(BinaryOptionOrder o, RspInfo e)
        {
            BOOrderErrorNotify notify = ResponseTemplate<BOOrderErrorNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;
            notify.RspInfo = e;

            CachePacket(notify);

        }

        /// <summary>
        /// 将数据通知前置
        /// </summary>
        /// <param name="packet"></param>
        //protected void NotifyFront(IPacket packet)
        //{
        //    _frontNotifyCache.Write(packet);
        //}

        /// <summary>
        /// 出入金通知
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        void NotifyCashOperation(string account, QSEnumCashOperation type, decimal amount)
        {
            logger.Info(string.Format("帐户:{0}发生:{1} {2}", account, type, amount));
            CashOperationNotify notify = ResponseTemplate<CashOperationNotify>.SrvSendNotifyResponse(account);
            notify.Amount = amount;
            notify.OperationType = type;

            CachePacket(notify);

            TLCtxHelper.EventAccount.FireAccountTradingNoticeEvent(account, "出入金内容");
        }

        void NotifyTradingNotice(string account, string content)
        {
            logger.Info(string.Format("发送通知到帐户:{0} 内容:{1}", account, content));
            TradingNoticeNotify notify = ResponseTemplate<TradingNoticeNotify>.SrvSendNotifyResponse(account);
            notify.SendTime = Util.ToTLTime();
            notify.NoticeContent = content;

            CachePacket(notify);
        }
    }
}
