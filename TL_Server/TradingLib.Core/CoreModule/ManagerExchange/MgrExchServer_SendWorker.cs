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
        public void Send(IPacket packet,bool fireSend)
        {
            CachePacket2(packet, fireSend);
        }
        void CachePacket(IPacket packet)
        {
            CachePacket2(packet);
        }
        void CachePacket2(IPacket packet,bool firesend=true)
        {
            _packetcache.Write(packet);
            if (firesend)
                NewMessageItem();
            
        }

        void CacheRspResponse(RspResponsePacket packet, bool islat = true)
        {
            packet.IsLast = islat;
            CachePacket2(packet,islat);
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
            //messageoutthread.IsBackground = true;
            messageoutthread.Name = "MGR Server Message SendOut Thread";
            messageoutthread.Start();
            ThreadTracker.Register(messageoutthread);

        }

        void StopMessageRouter()
        {

            if (!_readgo) return;
            ThreadTracker.Unregister(messageoutthread);
            _readgo = false;
            messageoutthread.Join();
            messageoutthread = null;
        }

        const int buffize = 5000;//

        //数据缓存
        RingBuffer<Order> _ocache = new RingBuffer<Order>(buffize);//委托缓存
        RingBuffer<OrderErrorPack> _errorordercache = new RingBuffer<OrderErrorPack>(buffize);//委托错误缓存
        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(buffize);//成交缓存
        RingBuffer<MgrClientInfoEx> _resumecache = new RingBuffer<MgrClientInfoEx>(buffize);//请求恢复交易帐户数据
        RingBuffer<IPacket> _packetcache = new RingBuffer<IPacket>(buffize);//其余消息的缓存队列

        bool _readgo = false;
        Thread messageoutthread;

        /// <summary>
        /// 当没有任何回补信息的时候 我们才可以发送实时信息
        /// </summary>
        /// <returns></returns>
        bool NoResumeRequest()
        {
            return !_resumecache.hasItems;
        }

        static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
        const int SLEEPDEFAULTMS = 500;
        void NewMessageItem()
        {
            if ((messageoutthread != null) && (messageoutthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                _sendwaiting.Set();
            }
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
                    if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.EnterSection("MgrExchange_SendWorker");

                    #region 发送交易账户列表 以及 回补某个交易账户的交易信息
                    //恢复交易帐户日内数据
                    while (_resumecache.hasItems)
                    {
                        var info = _resumecache.Read();
                        ResumeAccountTradingInfo(info);
                    }
                    #endregion

                    #region 转发管理端选中账户的交易信息
                    //转发委托
                    while (_ocache.hasItems && NoResumeRequest())
                    {
                        Order o = _ocache.Read();
                        foreach (var cst in customerExInfoMap.Values)
                        {
                            if (cst.NeedPushTradingInfo(o.Account))
                            {
                                OrderNotify notify = ResponseTemplate<OrderNotify>.SrvSendNotifyResponse(cst.Location);
                                notify.Order = o;
                                tl.TLSend(notify);
                            }
                        }
                    }
                    //转发委托错误
                    while (_errorordercache.hasItems && !_ocache.hasItems && NoResumeRequest())
                    {
                        OrderErrorPack pack = _errorordercache.Read();
                        foreach (var cst in customerExInfoMap.Values)
                        {
                            if (cst.NeedPushTradingInfo(pack.Order.Account))
                            {
                                ErrorOrderNotify notify = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(cst.Location);
                                notify.Order = pack.Order;
                                notify.RspInfo = pack.RspInfo;
                                tl.TLSend(notify);
                            }
                        }
                    }
                    //转发成交
                    while (_fcache.hasItems && !_ocache.hasItems && NoResumeRequest())
                    {
                        Trade f = _fcache.Read();
                        foreach (var cst in customerExInfoMap.Values)
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
                    while (_packetcache.hasItems && NoResumeRequest())
                    {
                        IPacket packet = _packetcache.Read();
                        tl.TLSend(packet);
                    }

                    // clear current flag signal
                    _sendwaiting.Reset();
                    if (GlobalConfig.ProfileEnable) RunConfig.Instance.Profile.LeaveSection();
                    //logger.Info("process send");
                    // wait for a new signal to continue reading
                    _sendwaiting.WaitOne(SLEEPDEFAULTMS);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    
                }
            }
        }

        void ResumeAccountTradingInfo(MgrClientInfoEx info)
        {
            try
            {
                
                IAccount account = info.AccountSelected;
                if (account == null)
                {
                    logger.Error(string.Format("Manager:{0} do not have account selected",info.Manager.Login));
                    return;
                }
                logger.Info(string.Format("Resume Trading Info Of Account:{0}", info.AccountSelected.ID));

                //1.发送恢复数据开始标识
                NotifyMGRContribNotify resumeNotify = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(info.Location);
                resumeNotify.ModuleID = this.CoreId;
                resumeNotify.CMDStr = "NotifyResume";
                resumeNotify.Result = 0.SerializeObject();
                tl.TLSend(resumeNotify);

                ILocation location = info.Location;
                //转发昨日持仓信息
                foreach (Position p in account.Positions)
                {
                    foreach (PositionDetail pd in p.PositionDetailYdRef)
                    {
                        HoldPositionNotify notify = ResponseTemplate<HoldPositionNotify>.SrvSendNotifyResponse(location);
                        notify.PositionDetail = pd;
                        tl.TLSend(notify);
                    }
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
                    tl.TLSend(notify);
                }
                //3.发送恢复数据结束标识
                //response.ResumeStatus = QSEnumResumeStatus.END;
                //tl.TLSend(response);
                resumeNotify.Result = 1.SerializeObject();
                tl.TLSend(resumeNotify);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Resume Account:{0} Error", info.AccountSelected.ID));
            }
        }

    }



}


