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
        public TLServer_Exch(string server, int port, bool verb)
            : base("Exch",server, port, verb)
        {
            

        }

        /// <summary>
        /// 查找所有以交易帐号account登入的客户端连接
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public TrdClientInfo[] ClientsForAccount(string account)
        {
            return _clients.Clients.Where(client => (client.Account.Equals(account))).ToArray();
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
            debug("交易帐户:" + notify.Account + " 链接端数量:" + locationlist.Length.ToString(),QSEnumDebugLevel.DEBUG);
            return locationlist;
        }
        /// <summary>
        /// 服务客户端请求登入认证函数
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public override void AuthLogin(TrdClientInfo c, LoginRequest request)
        {
            //1.检查外部调用绑定
            if (newLoginRequest == null)
            {
                debug("外部认证事件没有绑定", QSEnumDebugLevel.ERROR);
                return;//回调外部loginRquest的实现函数
            }
            
            LoginResponse response = ResponseTemplate<LoginResponse>.SrvSendRspResponse(request);
            response.FrontUUID = c.Location.FrontID;
            response.ClientUUID = c.Location.ClientID;
            response.FrontIDi = c.FrontIDi;
            response.SessionIDi = c.SessionIDi;
            response.Date = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;
            newLoginRequest(c,request, ref response);

            //2.检查验证结果 并将对应的数据储存到对应的client informaiton中去
            if (!response.Authorized)
            {
                c.AuthorizedFail();
            }
            else
            {
                //################客户端验证成功后的一系列操作#############################################
                //检查当个客户端的登入次数超过次数给出提示 不予登入
                c.AuthorizedSuccess();
                //将登入回报中的相关信息填充到ClientInfo对象中
                c.Account = response.Account;
                c.IPAddress = request.IPAddress;
                c.HardWareCode = request.MAC;
                c.ProductInfo = request.ProductInfo;

                //登入成功后我们更新账户的登入信息 如果是自动重连,则第一次登入是没有采集到ip地址,硬件地址.当客户端更新本地数据时，会再次调用updatelogininfo，来更新该信息
                UpdateClientLoginInfo(c, true);
                

            }
            debug(c.ToString(), QSEnumDebugLevel.INFO);
            SendOutPacket(response);
        }


        /// <summary>
        /// 请求交易功能特征列表
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public override void SrvReqFuture(FeatureRequest req)
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

            f.Add(MessageTypes.REGISTERSTOCK);//请求行情数据
            f.Add(MessageTypes.CLEARSTOCKS);//取消行情数据

            
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


        /// <summary>
        /// 系统接受到客户端发送过来的委托
        /// 1.检查客户端是否Register如果没有register则clientlist不存在该ClientID
        /// 2.检查账户是否登入,如果登入 检查委托账号与登入账户是否一致
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public  void SrvOnOrderInsert(OrderInsertRequest request ,TrdClientInfo clientinfo)
        {
            debug("Got Order:" + request.Order.ToString(), QSEnumDebugLevel.DEBUG);
            //通过address(ClientID)查询本地客户端列表是否存在该ID
            TrdClientInfo cinfo = _clients[request.ClientID];
            //1.如果不存在,表明该ID没有通过注册连接到我们的服务端
            if (cinfo == null)
            {
                debug("系统拒绝委托:" + request.Order.ToString() + "系统没有该注册端:" + request.ClientID + "|" + request.Order.Account, QSEnumDebugLevel.WARNING);
                return;
            }
            

            //2.检查插入委托请求是否有效
            if (!request.IsValid)
            {
                debug("请求无效", QSEnumDebugLevel.ERROR);
                //debug("系统拒绝委托:" + (o == null ? "Null" : o.ToString()) + "委托无效:" + address + "|" + o.Account,QSEnumDebugLevel.WARNING);
                //TLSend("系统拒绝，委托无效", MessageTypes.POPMESSAGE, address);//发送客户端提示信息
                //CacheMessage(QSMessageHelper.Message(QSOrderMessage.REJECT,"系统拒绝，委托无效"), MessageTypes.ORDERMESSAGE, address);//发送客户端提示信息
                return;
            }

            //3.如果本地客户端列表中存在该ID,则我们需要比较该ID所请求的交易账号与其所发送的委托账号是否一致,这里防止客户端发送他人的account的委托
            //只有当客户端通过请求账户 提供正确的账户 与密码 系统才会将address(ClientID与Account进行绑定)
            if (!cinfo.Authorized)
            {
                debug("客户端:" + cinfo.Location.ClientID + "未登入,无法请求委托", QSEnumDebugLevel.ERROR);
                return;
            }

            if (cinfo.Account != request.Order.Account)//客户端没有登入或者登入ID与委托ID不符
            {
                debug("客户端对应的帐户:" + cinfo.Account + " 与委托帐户:" + request.Order.Account + " 不符合", QSEnumDebugLevel.ERROR);
                return;
            }

            //标注来自客户端的原始委托
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.OrderSource = QSEnumOrderSource.CLIENT;

            //设定委托达到服务器时间
            order.date = Util.ToTLDate();
            order.time = Util.ToTLTime();

            //设定TotalSize为 第一次接受到委托时候的Size
            order.TotalSize = order.size;

            //设定委托FrontID和SessioinID
            order.FrontIDi = clientinfo.FrontIDi;
            order.SessionIDi = clientinfo.SessionIDi;

            //对外层触发委托事件
            if (newSendOrderRequest != null)
                newSendOrderRequest(order);
        }

        /// <summary>
        /// 取消委托,参数为全局委托编号
        /// </summary>
        /// <param name="msg"></param>
        public  void SrvOnOrderAction(OrderActionRequest request)
        {
            //通过address(ClientID)查询本地客户端列表是否存在该ID
            TrdClientInfo cinfo = _clients[request.ClientID];
            if (cinfo == null) return;

            if (newOrderActionRequest != null)
                newOrderActionRequest(request);
        }

        /// <summary>
        /// 客户端请求注册symbol数据
        /// </summary>
        /// <param name="cname"></param>
        /// <param name="stklist"></param>
        void SrvRegStocks(RegisterSymbolsRequest request)
        {
            TrdClientInfo cinfo = _clients[request.ClientID] as TrdClientInfo;
            if (cinfo == null) return;

            debug("Client:" + request.ClientID + " Request Mktdata: " + request.Content, QSEnumDebugLevel.INFO);
            //cinfo.Stocks = request.Content;
            SrvBeatHeart(request.ClientID);
            if (newRegisterSymbols != null)
            {
                newRegisterSymbols(request.ClientID, request.Symbols);
            }
        }

        /// <summary>
        /// 客户端请求清除已注册的symbol
        /// </summary>
        /// <param name="cname"></param>
        void SrvClearStocks(UnregisterSymbolsRequest request)
        {
            TrdClientInfo cinfo = _clients[request.ClientID] as TrdClientInfo;
            if (cinfo == null) return;
            //cinfo.Stocks = "";
            SrvBeatHeart(request.ClientID);
        }



        /// <summary>
        /// 处理母类所不处理的消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public override long handle(IPacket packet,TrdClientInfo clientinfo)
        {
            long result = NORETURNRESULT;
            switch (packet.Type)
            {
                case MessageTypes.SENDORDER:
                    SrvOnOrderInsert(packet as OrderInsertRequest, clientinfo);
                    break;
                case MessageTypes.SENDORDERACTION:
                    SrvOnOrderAction(packet as OrderActionRequest);
                    break;
                case MessageTypes.REGISTERSTOCK:
                    SrvRegStocks(packet as RegisterSymbolsRequest);
                    break;
                case MessageTypes.CLEARSTOCKS:
                    SrvClearStocks(packet as UnregisterSymbolsRequest);
                    break;
                default:
                    //1.生成对应的Session 用于减少ClientInfo的暴露防止错误修改相关参数
                    Client2Session sesssion = new Client2Session(clientinfo);
                    sesssion.AccountID = clientinfo.Account;


                    if (newPacketRequest != null)
                        newPacketRequest(packet,sesssion);
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
                debug("invalid order: " + (o==null ? "Null Order":o.ToString()),QSEnumDebugLevel.WARNING);
                return;
            }

            if (string.IsNullOrEmpty(o.Account)) return;

            OrderNotify notify = ResponseTemplate<OrderNotify>.SrvSendNotifyResponse(o.Account);
            notify.Order = o;

            TLSend(notify);
            debug("send ordernotify to client | "+o.ToString());
        }

        internal void newOrderError(ErrorOrderNotify notify)
        {
            if (notify.Order == null || !notify.Order.isValid)
            {
                debug("invalid ordererror:" + notify.ToString(), QSEnumDebugLevel.INFO);
                return;
            }
            if (string.IsNullOrEmpty(notify.Order.Account)) return;

            TLSend(notify);
            debug("send error order notify to client | " + notify.ToString());
        }
        /// <summary>
        /// 向客户端发送成交回报
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        internal void newFill(Trade trade)
        {
            if (trade==null || !trade.isValid)
            {
                debug("invalid trade: " + (trade==null?"Null Trade":trade.ToString()));
                return;
            }

            if (string.IsNullOrEmpty(trade.Account)) return;

            TradeNotify notify = ResponseTemplate<TradeNotify>.SrvSendNotifyResponse(trade.Account);
            notify.Trade = trade;

            TLSend(notify);
            debug("send Filld to client | "+trade.ToString());
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
            debug("send Cancel to client | " + action.ToString());
        }

        /// <summary>
        /// 向客户端发送持仓状态更新回报
        /// </summary>
        /// <param name="pos"></param>
        internal void newPositionUpdate(Position pos)
        {
            if (string.IsNullOrEmpty(pos.Account)) return;
            PositionNotify notify = ResponseTemplate<PositionNotify>.SrvSendNotifyResponse(pos.Account);
            notify.Position = pos.GenPositionEx();

            TLSend(notify);
            debug("send positionupdate to client|" + pos.ToString());

        }
        #endregion

        #region 交易消息交换服务端事件列表
        public event LoginRequestDel<TrdClientInfo> newLoginRequest;//登入服务器
        public event OrderDelegate newSendOrderRequest;//发送委托
        public event OrderActionRequestDel newOrderActionRequest;//发送委托操作
        public event SymbolRegisterDel newRegisterSymbols;//订阅市场数据
        public event MessageArrayDelegate newFeatureRequest;//请求功能列表
        public event TrdPacketRequestDel newPacketRequest;//其他逻辑宝请求
        #endregion


        
    }
    public delegate void OrderActionRequestDel(OrderActionRequest request);
    public delegate void TrdPacketRequestDel(IPacket packet, ISession session);
}
