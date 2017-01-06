//Copyright 2013 by FutSystems,Inc.
//20161223  将功能特征请求与登入验证全部放入TLServer中处理,其他逻辑业务全部抛到外层处理
//          

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
    /// 交易服务器核心
    /// </summary>
    public class TLServer_Exch :TLServer_Generic<TrdClientInfo>
    {
        public TLServer_Exch(string name,string server, int port, bool verb)
            : base(name, server, port, verb)
        {
            

        }

        ///// <summary>
        ///// 业务逻辑请求事件
        ///// </summary>
        public event Action<ISession, IPacket, IAccount> newPacketRequest;

        int _loginTerminalNum = 6;
        /// <summary>
        /// 交易账户同时可登入终端数量
        /// </summary>
        public int ClientLoginTerminalLimit { get { return _loginTerminalNum; } set { _loginTerminalNum = value; } }

        /// <summary>
        /// 注销某个交易帐户的所有登入终端
        /// </summary>
        /// <param name="account"></param>
        public void ClearTerminalsForAccount(string account)
        {
            foreach (TrdClientInfo info in this.ClientsForAccount(account).ToArray())
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
            //logger.Info("交易帐户:" + notify.Account + " 链接端数量:" + locationlist.Length.ToString());
            logger.Warn(string.Format("Notify Account:{0} Client Num:{1} IDs:{2}", notify.Account, locationlist.Length, string.Join(" ", locationlist.Select(l => l.ClientID).ToArray())));
            //通知地址为零 输出日志信息
            if (locationlist.Length == 0)
            { 
                foreach(var client in this._clients.Clients)
                {
                    logger.Warn(string.Format("ClientID:{0} Account:{1}", client.Location.ClientID, client.Account.ID));
                }
            }
            return locationlist;
        }

        string GetAuthTypeStr(int type)
        {
            if (type == 1) return "LocalDB-1";
            if (type == 0) return "UCenter-0";
            return "Unknown";
        }

        /// <summary>
        /// 服务客户端请求登入认证函数
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public override void AuthLogin(LoginRequest request,TrdClientInfo client)
        {
            LoginResponse response = ResponseTemplate<LoginResponse>.SrvSendRspResponse(request);
            response.FrontUUID = client.Location.FrontID;
            response.ClientUUID = client.Location.ClientID;
            response.FrontIDi = client.FrontIDi;
            response.SessionIDi = client.SessionIDi;
            response.TradingDay = TLCtxHelper.ModuleSettleCentre.Tradingday;

            bool login = false;
            IAccount account = null;
            logger.Info(string.Format("RequestLogin ID:{0} Password:{1} IP:{3} MAC:{4} Product:{5} AuthType:{2}", request.LoginID, request.Passwd, GetAuthTypeStr(request.LoginType), request.IPAddress, request.MAC, request.ProductInfo));
            if (request.LoginType == 1)
            {
                //获得当前登入终端数量
                int loginnums = this.ClientsForAccount(request.LoginID).Count();
                //如果当前登入数量大于等于 系统允许单个账户登入数量 则拒绝登入
                if (loginnums >= this.ClientLoginTerminalLimit)
                {
                    logger.Warn(string.Format("MaxLoginNum:{0} account:{1} current logined num:{2}", this.ClientLoginTerminalLimit, request.LoginID, loginnums));
                    response.Authorized = false;
                    response.RspInfo.Fill("TERMINAL_NUM_LIMIT");
                }
                else
                {
                    account = TLCtxHelper.ModuleAccountManager[request.LoginID];
                    if (account == null)
                    {
                        login = false;
                    }
                    else
                    {
                        login = account.Pass.Equals(request.Passwd);
                    }
                    response.Authorized = login;
                    //登入成功后填充 登入回报信息
                    if (login)
                    {
                        response.LoginID = request.LoginID;
                        response.Account = request.LoginID;
                        response.AccountType = account.Category;
                    }
                    else
                    {
                        response.RspInfo.Fill("INVALID_LOGIN");
                    }
                }
            }
            else
            {
                response.Authorized = false;
                response.RspInfo.Fill("LOGINTYPE_NOT_SUPPORT");
            }

            //检查域和管理员对象 进行域过期和管理是否激活进行限制
            if (account != null)
            {
                if (account.Domain.IsExpired())//域过期
                {
                    response.Authorized = false;
                    response.RspInfo.Fill("PLATFORM_EXPIRED");
                }
                Manager mgr = BasicTracker.ManagerTracker[account.Mgr_fk];
                if (mgr == null || (!mgr.Active))
                {
                    response.Authorized = false;
                    response.RspInfo.Fill("PLATFORM_EXPIRED");
                }
            }

            //2.检查验证结果 并将对应的数据储存到对应的Client对象
            if (response.Authorized)
            {
                //将登入回报中的相关信息填充到ClientInfo对象中
                client.IPAddress = request.IPAddress;
                client.HardWareCode = request.MAC;
                client.ProductInfo = request.ProductInfo;
                client.BindState(account);

                UpdateClientLoginStatus(client, true);

            }

            //对外触发登入事件
            if (response.Authorized)
            {
                TLCtxHelper.EventSession.FireAccountLoginSuccessEvent(response.Account);
            }
            else
            {
                TLCtxHelper.EventSession.FireAccountLoginFailedEvent(response.Account);
            }

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
            List<MessageTypes> f = new List<MessageTypes>();

            f.Add(MessageTypes.REGISTERCLIENT);//注册客户端
            f.Add(MessageTypes.LOGINREQUEST);//请求登入
            f.Add(MessageTypes.CLEARCLIENT);//注销客户端

            f.Add(MessageTypes.HEARTBEAT);//发送心跳包
            f.Add(MessageTypes.HEARTBEATREQUEST);//心跳请求
            f.Add(MessageTypes.HEARTBEATRESPONSE);
            f.Add(MessageTypes.VERSIONREQUEST);//服务器版本
            f.Add(MessageTypes.FEATUREREQUEST);//请求功能特征
            f.Add(MessageTypes.FEATURERESPONSE);//回报功能请求

            f.Add(MessageTypes.REGISTERSYMTICK);//请求行情数据
            f.Add(MessageTypes.UNREGISTERSYMTICK);//取消行情数据

            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.EXECUTENOTIFY);

            //客户端主动发起请求的功能
            f.Add(MessageTypes.SENDORDER);//发送委托
            //f.Add(MessageTypes.ORDERCANCELREQUEST);//发送取消委托
            f.Add(MessageTypes.QRYACCOUNTINFO);//查询账户信息
            //f.Add(MessageTypes.QRYCANOPENPOSITION);//查询可开
            f.Add(MessageTypes.REQCHANGEPASS);//请求修改密码
            
            response.Add(f.ToArray());
            SendOutPacket(response);
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
            IAccount account = session.GetAccount();
            long result = NORETURNRESULT;
            if (newPacketRequest != null)
                newPacketRequest(session, packet, account);
            else
                result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
            return result;
        }

        #endregion


        #region TLServer -->client发送相应回报
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
            logger.Info(string.Format("Notify Order To Client:{0}", o.GetOrderInfo()));
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
            logger.Info(string.Format("Notify Order Error To Client:{0} / RspInfo:{1}", notify.Order.GetOrderInfo(), notify.RspInfo));
        }

        internal void newOrderActionError(ErrorOrderActionNotify notify)
        {
            TLSend(notify);
            logger.Info(string.Format("Notify OrderAction Error To Client:{0} / RspInfo:{1}","",notify.RspInfo));
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
            logger.Info(string.Format("Notify Trade To Client:{0}", trade.GetTradeInfo()));
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
            logger.Info(string.Format("Notify OrderAction To Client:{0}", action.ToString()));
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
            logger.Info(string.Format("Notify Postion To Client:{0}", pos.ToString()));

        }
        #endregion




    }
}
