using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TradingLib.Common;

using TradingLib.API;
using System.Data;
using System.IO;
namespace TradingLib.Core
{
    /// <summary>
    /// 交易服务器内核,提供客户端注册,交易委托,回报,撤销等功能
    /// </summary>
    public class TLServer_Exch :TLServer_Generic<TrdClientInfo>
    {
        public TLServer_Exch(string name,string server, int port, bool verb)
            : base(name, server, port, verb)
        {
            

        }

        /// <summary>
        /// 注销某个交易帐户的所有登入终端
        /// </summary>
        /// <param name="account"></param>
        public void ClearTerminalsForAccount(string account)
        {
            foreach (TrdClientInfo info in this.ClientsForAccount(account))
            {
                _clients.UnRegistClient(info.Location.ClientID);
            }
        }
        /// <summary>
        /// 查找所有以交易帐号account登入的客户端连接
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<TrdClientInfo> ClientsForAccount(string account)
        {
            return _clients.Clients.Where(client =>(client.Account !=null && client.Account.ID.Equals(account)));
        }

        /// <summary>
        /// 查找某个地址的ClientInfo
        /// 返回登入列表的第一客户端信息
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public TrdClientInfo GetClient(string clientid)
        {
            return _clients.Clients.Where(c => c.Location.ClientID.Equals(clientid)).FirstOrDefault();
        }
       

        #region client-->TLServer消息所引发的各类操作

        public override void TLSendOther(IPacket packet)
        {
            //base.TLSendOther(packet);
        }

        /// <summary>
        /// 获得某个通知包的通知地址
        /// 1.交易通知逻辑包 有Account字段
        /// 2.交易TrdClientInfo包含有该客户端登入时候所绑定的Account
        /// 通过搜索获得所有某个Account登入的客户端地址
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override ILocation[] GetNotifyTargets(IPacket packet)
        {
            NotifyResponsePacket notify= packet as NotifyResponsePacket;
            //查找以交易帐户登入的地址列表
            ILocation[] locationlist = ClientsForAccount(notify.Account).Select(client =>client.Location).ToArray();
            logger.Info("交易帐户:" + notify.Account + " 链接端数量:" + locationlist.Length.ToString());
            return locationlist;
        }
        /// <summary>
        /// 服务客户端请求登入认证函数
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public override void AuthLogin(LoginRequest request,TrdClientInfo client)
        {
            //1.检查外部调用绑定
            if (newLoginRequest == null)
            {
                logger.Error("外部认证事件没有绑定");
                return;//回调外部loginRquest的实现函数
            }

            LoginResponse response = ResponseTemplate<LoginResponse>.SrvSendRspResponse(request);
            response.FrontUUID = client.Location.FrontID;
            response.ClientUUID = client.Location.ClientID;
            response.FrontIDi = client.FrontIDi;
            response.SessionIDi = client.SessionIDi;
            response.Date = TLCtxHelper.ModuleSettleCentre.Tradingday;
            newLoginRequest(client, request, ref response);

            //2.检查验证结果 并将对应的数据储存到对应的client informaiton中去
            if (response.Authorized)
            {
                //################客户端验证成功后的一系列操作#############################################
                //检查当个客户端的登入次数超过次数给出提示 不予登入

                //将登入回报中的相关信息填充到ClientInfo对象中
                client.IPAddress = request.IPAddress;
                client.HardWareCode = request.MAC;
                client.ProductInfo = request.ProductInfo;

                IAccount account = TLCtxHelper.ModuleAccountManager[response.Account];
                client.BindState(account);

                //登入成功后我们更新账户的登入信息 如果是自动重连,则第一次登入是没有采集到ip地址,硬件地址.当客户端更新本地数据时，会再次调用updatelogininfo，来更新该信息
                UpdateClientLoginInfo(client, true);
                

            }
            logger.Info(client.ToString());
            SendOutPacket(response);
        }


        /// <summary>
        /// 请求交易功能特征列表
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public override void SrvReqFuture(FeatureRequest req,TrdClientInfo client)
        {
            FeatureResponse response = ResponseTemplate<FeatureResponse>.SrvSendRspResponse(req);

            string msf = "";
            List<MessageTypes> f = new List<MessageTypes>();

            f.Add(MessageTypes.REGISTERCLIENT);//注册客户端
            f.Add(MessageTypes.LOGINREQUEST);//请求登入
            f.Add(MessageTypes.CLEARCLIENT);//注销客户端

            f.Add(MessageTypes.HEARTBEAT);//发送心跳包
            f.Add(MessageTypes.HEARTBEATREQUEST);//心跳请求
            f.Add(MessageTypes.HEARTBEATRESPONSE);
            f.Add(MessageTypes.VERSIONREQUEST);//服务器版本
            //f.Add(MessageTypes.BROKERNAME);//服务端标识
            f.Add(MessageTypes.FEATUREREQUEST);//请求功能特征
            f.Add(MessageTypes.FEATURERESPONSE);//回报功能请求

            f.Add(MessageTypes.REGISTERSYMTICK);//请求行情数据
            f.Add(MessageTypes.UNREGISTERSYMTICK);//取消行情数据

            
            if (newFeatureRequest != null)
            {
                MessageTypes[] f2 = newFeatureRequest();
                foreach (MessageTypes t in f2)
                {
                    if (f.Contains(t))
                        continue;
                    f.Add(t);
                }
            }
            response.Add(f.ToArray());
            SendOutPacket(response);
        }

        //Profiler orderInsertProfile = new Profiler();
        /// <summary>
        /// 系统接受到客户端发送过来的委托
        /// 1.检查客户端是否Register如果没有register则clientlist不存在该ClientID
        /// 2.检查账户是否登入,如果登入 检查委托账号与登入账户是否一致
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public  void SrvOnOrderInsert(ISession session,OrderInsertRequest request)
        {
            TLCtxHelper.Profiler.EnterSection("OrderInsert");
            logger.Info("Got Order:" + request.Order.ToString());

            //检查插入委托请求是否有效
            if (!request.IsValid)
            {
                logger.Warn("请求无效");
                return;
            }
            IAccount account = session.GetAccount();
            //如果请求没有指定交易帐号 则根据对应的Client信息自动绑定交易帐号
            if (string.IsNullOrEmpty(request.Order.Account))
            {
                request.Order.Account = account.ID;
            }
            //如果指定交易帐号与登入交易帐号不符 则回报异常
            if (account.ID!= request.Order.Account)//客户端没有登入或者登入ID与委托ID不符
            {
                logger.Warn("客户端对应的帐户:" + account.ID + " 与委托帐户:" + request.Order.Account + " 不符合");
                return;
            }

            //标注来自客户端的原始委托
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.id = 0;
            order.OrderSource = QSEnumOrderSource.CLIENT;

            //设定委托达到服务器时间
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();

            //设定TotalSize为 第一次接受到委托时候的Size
            order.TotalSize = order.Size;

            //设定分帐户端信息
            order.FrontIDi = session.FrontIDi;
            order.SessionIDi = session.SessionIDi;
            order.RequestID = request.RequestID;

            //order.Domain_ID = account.Domain.ID;
            //对外层触发委托事件
            if (newSendOrderRequest != null)
                newSendOrderRequest(order);

            TLCtxHelper.Profiler.LeaveSection();
        }

        /// <summary>
        /// 取消委托,参数为全局委托编号
        /// </summary>
        /// <param name="msg"></param>
        public void SrvOnOrderAction(ISession session, OrderActionRequest request)
        {
            if (newOrderActionRequest != null)
                newOrderActionRequest(request);
        }

        /// <summary>
        /// 客户端请求注册symbol数据
        /// </summary>
        /// <param name="cname"></param>
        /// <param name="stklist"></param>
        void SrvRegStocks(ISession session, RegisterSymbolTickRequest request)
        {
            logger.Info("Client:" + request.ClientID + " Request Mktdata: " + request.Content);
            if (newRegisterSymbols != null)
            {
                newRegisterSymbols(request.ClientID, request.Symbols);
            }
        }

        /// <summary>
        /// 客户端请求清除已注册的symbol
        /// </summary>
        /// <param name="cname"></param>
        void SrvClearStocks(ISession session, UnregisterSymbolTickRequest request)
        {
            //SrvBeatHeart(client);
        }


        public override void OnSessionCreated(Client2Session session)
        {
            session.SessionType = QSEnumSessionType.CLIENT;
        }

        public override void OnSessionStated(Client2Session session, TrdClientInfo client)
        {
            session.BindAccount(client.Account);
        }

        /// <summary>
        /// 处理母类所不处理的消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public override long handle(ISession session,IPacket packet)
        {
            long result = NORETURNRESULT;
            switch (packet.Type)
            {
                case MessageTypes.SENDORDER:
                    SrvOnOrderInsert(session,packet as OrderInsertRequest);
                    break;
                case MessageTypes.SENDORDERACTION:
                    SrvOnOrderAction(session,packet as OrderActionRequest);
                    break;
                case MessageTypes.REGISTERSYMTICK:
                    SrvRegStocks(session,packet as RegisterSymbolTickRequest);
                    break;
                case MessageTypes.UNREGISTERSYMTICK:
                    SrvClearStocks(session,packet as UnregisterSymbolTickRequest);
                    break;
                default:
                    if (newPacketRequest != null)
                        newPacketRequest(session,packet);
                    else
                        result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
                    break;
            }
            return result;
        }
        #endregion


        #region TLServer -->client发送相应回报
        /* 服务端向客户端发送消息是通过MsgExch中的唯一线程进行操作的
         * 将不同数据缓存队列中的消息包通过唯一的线程按一定顺序发送出去
         * newXXXX这组函数是由msgexch在发送线程内进行调用
         * 在tlserver_exch中如果需要对外发送消息都要调用SendOutPacket通过将数据包缓存到队列中统一发送
         * 否则会造成多个线程操作底层tlsend,造成崩溃
         * 
         * 
         * 
         * */
        /// <summary>
        /// 服务端向客户端发送委托回报
        /// </summary>
        /// <param name="o"></param>
        internal void newOrder(Order o)
        {
            if (o==null ||!o.isValid)
            {
                logger.Warn("invalid order: " + (o == null ? "Null Order" : o.ToString()));
                return;
            }

            if (string.IsNullOrEmpty(o.Account)) return;

            OrderNotify notify = ResponseTemplate<OrderNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;

            TLSend(notify);
            logger.Info("send ordernotify to client | " + o.ToString());
        }

        internal void newOrderError(ErrorOrderNotify notify)
        {
            if (notify.Order == null || !notify.Order.isValid)
            {
                logger.Info("invalid ordererror:" + notify.ToString());
                return;
            }
            if (string.IsNullOrEmpty(notify.Order.Account)) return;

            TLSend(notify);
            logger.Info("send order error notify to client | " + notify.ToString());
        }

        internal void newOrderActionError(ErrorOrderActionNotify notify)
        {
            TLSend(notify);
            logger.Info("send orderaction error notify to client | " + notify.ToString());
        }
        /// <summary>
        /// 向客户端发送成交回报
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        internal void newFill(Trade trade)
        {
            if (trade==null || !trade.isValid)
            {
                logger.Info("invalid trade: " + (trade == null ? "Null Trade" : trade.ToString()));
                return;
            }

            if (string.IsNullOrEmpty(trade.Account)) return;

            TradeNotify notify = ResponseTemplate<TradeNotify>.SrvSendNotifyResponse(trade.Account);
            notify.Trade = trade;

            TLSend(notify);
            logger.Info("send Filld to client | " + trade.ToString());
        }


        /// <summary>
        /// 向客户端发送OrderAction回报
        /// </summary>
        /// <param name="action"></param>
        internal void newOrderAction(OrderAction action)
        {
            if (string.IsNullOrEmpty(action.Account)) return;
            OrderActionNotify response = ResponseTemplate<OrderActionNotify>.SrvSendNotifyResponse(action.Account);
            response.OrderAction = action;

            TLSend(response);
            logger.Info("send Cancel to client | " + action.ToString());
        }

        /// <summary>
        /// 向客户端发送持仓状态更新回报
        /// </summary>
        /// <param name="pos"></param>
        internal void newPositionUpdate(PositionEx pos)
        {
            if (string.IsNullOrEmpty(pos.Account)) return;
            PositionNotify notify = ResponseTemplate<PositionNotify>.SrvSendNotifyResponse(pos.Account);
            notify.Position = pos;

            TLSend(notify);
            logger.Info("send positionupdate to client|" + pos.ToString());

        }
        #endregion

        #region 交易消息交换服务端事件列表
        public event LoginRequestDel<TrdClientInfo> newLoginRequest;//登入服务器
        public event OrderDelegate newSendOrderRequest;//发送委托
        public event OrderActionRequestDel newOrderActionRequest;//发送委托操作
        public event SymbolRegisterDel newRegisterSymbols;//订阅市场数据
        public event MessageArrayDelegate newFeatureRequest;//请求功能列表
        public event PacketRequestDel newPacketRequest;//其他逻辑宝请求
        #endregion


        
    }
    public delegate void OrderActionRequestDel(OrderActionRequest request);
    
}
