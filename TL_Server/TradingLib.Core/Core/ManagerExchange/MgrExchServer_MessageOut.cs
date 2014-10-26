using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {



        public void Send(IPacket packet)
        {
            CachePacket(packet);
        }

        void CachePacket(IPacket packet)
        {
            _packetcache.Write(packet);
        }

        void CacheRspResponse(RspResponsePacket packet, bool islat = true)
        {
            packet.IsLast = islat;
            CachePacket(packet);
        }

        /// <summary>
        /// 消息对外发送线程,由于ZeroMQ并不是线程安全的，因此我们不能运行多个线程去操作一个socket,当我们对外发送的时候统一调用了
        /// TLSend _trans.Send(data, clientID)
        /// 1.TradingServer形成的交易信息,我们需要通过MgrServer转发到对应的客户端TradingSrv->MgrServer->TLSend
        /// 2.客户端注册操作 以及回补相关账户交易信息时，也会形成MgrServer对外的信息发送Client->MgServer->TLSend
        /// 这样Mgr其实有多个线程通过MgrServer对外发送信息,因此我们需要在这里建立信息缓存，将多个线程需要转发的消息一并放入
        /// 对应的缓存中，然后由单独的线程不间断的将这些缓存的信息统一发送出去
        /// </summary>
        void StartMessageRouter()
        {

            if (_readgo) return;
            _readgo = true;
            messageoutthread = new Thread(messageout);
            messageoutthread.IsBackground = true;
            messageoutthread.Name = "MGR Server Message SendOut Thread";
            messageoutthread.Start();
            ThreadTracker.Register(messageoutthread);

        }

        void StopMessageRouter()
        {

            if (!_readgo) return;
            ThreadTracker.Unregister(messageoutthread);
            _readgo = false;
            int mainwait = 0;
            while (messageoutthread.IsAlive && mainwait < 10)
            {
                Thread.Sleep(1000);
                mainwait++;
            }
            messageoutthread.Abort();
            messageoutthread = null;
        }

        const int buffize = 5000;

        //实时交易信息缓存
        
        RingBuffer<Order> _ocache = new RingBuffer<Order>(buffize);//委托缓存
        RingBuffer<ErrorOrder> _errorordercache = new RingBuffer<ErrorOrder>(buffize);//委托错误缓存
        RingBuffer<OrderAction> _occache = new RingBuffer<OrderAction>(buffize);//取消缓存
        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(buffize);//成交缓存

        //恢复交易帐号交易信息数据
        RingBuffer<MGRResumeAccountRequest> _resumecache = new RingBuffer<MGRResumeAccountRequest>(buffize);//请求恢复交易帐户数据队列
        RingBuffer<IPacket> _packetcache = new RingBuffer<IPacket>(buffize);//其余消息的缓存队列
        RingBuffer<SessionInfo> _sessioncache = new RingBuffer<SessionInfo>(buffize);//客户端登入 登出缓存
        RingBuffer<IAccount> _accountchangecache = new RingBuffer<IAccount>(buffize);//帐户变动缓存
        RingBuffer<RspMGRQryAccountResponse> _accqrycache = new RingBuffer<RspMGRQryAccountResponse>(buffize);//交易帐户列表查询缓存需要比其他数据提高发送优先级，其他数据依赖与该数据


        //关于交易信息转发,交易信息转发时,我们需要区分是实时发生的交易信息转发还是请求回补的信息转发。
        //实时交易信息通过客户端权限检查自动将交易信息发送到所有有权,而回补信息则是针对不同的管理端进行的回补请求,若统一由tl.neworder转发会造成不同的管理端之间信息重复接收
        bool _readgo = false;
        Thread messageoutthread;

        /// <summary>
        /// 当没有任何回补信息的时候 我们才可以发送实时信息
        /// </summary>
        /// <returns></returns>
        bool noresumeinfo()
        {
            return !_resumecache.hasItems && !_accqrycache.hasItems;
        }

        bool noaccountqry()
        {
            return !_accqrycache.hasItems;
        }


        /// <summary>
        /// 所有需要转发到客户端的消息均通过缓存进行，这样避免了多个线程同时操作一个ZeroMQ socket
        /// </summary>
        void messageout()
        {
            while (_readgo)
            {
                try
                {
                    #region 发送交易账户列表 以及 回补某个交易账户的交易信息
                    //发送账户列表信息
                    while (_accqrycache.hasItems)
                    {
                        IPacket packet = _packetcache.Read();
                        tl.TLSend(packet);
                    }

                    //恢复交易帐户日内数据
                    while (_resumecache.hasItems && noaccountqry())
                    {
                        MGRResumeAccountRequest request = _resumecache.Read();
                        ResumeAccountTradingInfo(request);
                    }
                    #endregion

                    #region 转发管理端选中账户的交易信息
                    //转发委托
                    while (_ocache.hasItems && noresumeinfo())
                    {
                        //debug("send realime order message~~~~~~~~~~~~~~~~", QSEnumDebugLevel.INFO);
                        Order o = _ocache.Read();
                        //遍历所有管理端的infoex,查看哪些管理端订阅了该交易帐户
                        foreach (CustInfoEx cst in customerExInfoMap.Values)
                        {
                            if (cst.NeedPushTradingInfo(o.Account))
                            {
                                //debug("send realime order notify to manager client~~~~~~~~~~~~~~~~", QSEnumDebugLevel.INFO);
                                OrderNotify notify = ResponseTemplate<OrderNotify>.SrvSendNotifyResponse(cst.Location);
                                notify.Order = o;
                                tl.TLSend(notify);
                            }
                        }
                    }
                    while (_errorordercache.hasItems && !_ocache.hasItems && noresumeinfo())
                    {
                        ErrorOrder error = _errorordercache.Read();
                        foreach (CustInfoEx cst in customerExInfoMap.Values)
                        {
                            if (cst.NeedPushTradingInfo(error.Order.Account))
                            {
                                ErrorOrderNotify notify = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(cst.Location);
                                error.Fill(notify);
                                tl.TLSend(notify);
                            }
                        }
                    }
                    //转发成交
                    while (_fcache.hasItems && !_ocache.hasItems && noresumeinfo())
                    {
                        Trade f = _fcache.Read();
                        foreach (CustInfoEx cst in customerExInfoMap.Values)
                        {
                            if (cst.NeedPushTradingInfo(f.Account))
                            {
                                TradeNotify notify = ResponseTemplate<TradeNotify>.SrvSendNotifyResponse(cst.Location);
                                notify.Trade = f;
                                tl.TLSend(notify);
                            }
                        }

                    }
                    #endregion
                   
                    //发送其他类型的信息
                    while (_packetcache.hasItems && noresumeinfo())
                    {
                        
                        IPacket packet = _packetcache.Read();
                        if (packet.Type == MessageTypes.MGRCONTRIBRESPONSE)
                        {
                            string x = "";
                        }
                        //debug("发送消息: 类型:" + packet.Type.ToString() + " 发送消息:" + packet.Content, QSEnumDebugLevel.INFO);
                        tl.TLSend(packet);
                    }
                    Thread.Sleep(100);

                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                }
            }
        }

        void ResumeAccountTradingInfo(MGRResumeAccountRequest request)
        { 
       
            string acc = request.ResumeAccount;
            debug("resuem account:" + acc, QSEnumDebugLevel.INFO);
            try
            {
                            
                IAccount account = clearcentre[acc];
                if (account == null)
                {
                    debug("Resume Account:" + acc + " do not exist", QSEnumDebugLevel.ERROR);
                    return;
                }

                //1.发送恢复数据开始标识
                RspMGRResumeAccountResponse response = ResponseTemplate<RspMGRResumeAccountResponse>.SrvSendRspResponse(request);
                response.ResumeAccount = request.ResumeAccount;
                response.ResumeStatus = QSEnumResumeStatus.BEGIN;
                tl.TLSend(response);
                
                ILocation location = new Location(request.FrontID,request.ClientID);
                //转发昨日持仓信息
                foreach (Position p in account.Positions)
                {
                    if (p.PositionDetailYdRef.Count() != 0)
                    {
                        IEnumerable<Position> poslist = PositionImpl.FromPositionDetail(p.PositionDetailYdRef);
                        foreach (Position pos in poslist)
                        {
                            HoldPositionNotify notify = ResponseTemplate<HoldPositionNotify>.SrvSendNotifyResponse(location);
                            notify.Position = pos.GenPositionEx();
                            tl.TLSend(notify);
                        }
                    }

                    
                }
                foreach (PositionDetail pos in account.YdPositions)
                {
                    //HoldPositionNotify notify = ResponseTemplate<HoldPositionNotify>.SrvSendNotifyResponse(location);
                    //notify.Position = pos.GenPositionEx();
                    //tl.TLSend(notify);
                }
                //转发当日委托
                foreach (Order o in account.Orders)
                {
                    OrderNotify notify = ResponseTemplate<OrderNotify>.SrvSendNotifyResponse(location);
                    notify.Order = o;
                    tl.TLSend(notify);
                }
                //转发当日成交
                foreach (Trade f in account.Trades)
                {
                    TradeNotify notify = ResponseTemplate<TradeNotify>.SrvSendNotifyResponse(location);
                    notify.Trade = f;
                    
                    //debug("转发当日成交:" + f.ToString() +" side:"+f.side.ToString(), QSEnumDebugLevel.INFO);
                    tl.TLSend(notify);
                }
                //转发当日取消
                //foreach (long oid in clearcentre.getCancels(acc))
                //{
                //    //tl.RestoreCancel(_clearcentre.SentOrder(oid), resume.Source);
                //}

                //3.发送恢复数据结束标识
                response.ResumeStatus = QSEnumResumeStatus.END;
                tl.TLSend(response);
            }
            catch (Exception ex)
            {
                debug("恢复交易帐户:" + acc + "出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

    }



}


