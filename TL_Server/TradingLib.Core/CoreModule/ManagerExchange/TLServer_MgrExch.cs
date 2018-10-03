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
                if (packet.Type == MessageTypes.MGR_RSP_CONTRIB)
                {
                    string x = "";
                }
                return notify.Locatioins.ToArray();
            }
            
            //2.通知类型的以Accout为目标的搜索 逻辑 某个管理端有权限查看该帐户则向该帐户进行发送
            if (packet.PacketType == QSEnumPacketType.NOTIFYRESPONSE)
            {
                if (QryNotifyLocationsViaAccount != null)
                {
                    return QryNotifyLocationsViaAccount(notify.Account);
                }
            
            }
            return new ILocation[]{};
        }


        public override bool ValidRegister(RegisterClientRequest request)
        {
            if (string.IsNullOrEmpty(request.VersionToken)) return false;
            return request.VersionToken.Equals(GlobalConfig.VersionToken);
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
                //验证root超级密码
                //if (request.LoginID.Equals("root"))
                //{
                //    re = request.Passwd.Equals(GlobalConfig.SuperPass);
                //}
                //当前密码检验未通过则验证储存的mgr密码
                if (!re)
                {
                    re = mgr.Pass.Equals(request.Passwd);
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
                    if (m.Deleted)
                    {
                        response.RspInfo.Fill("MGR_INACTIVE");
                    }
                    
                    //管理员所属分区编号大于授权可开最大编号则不允许登入 返回不存在提示
                    if (m.domain_id > LicenseConfig.Instance.DomainCNT)
                    {
                        response.RspInfo.Fill("MGR_NOT_EXIST");
                    }

                    if (m.Domain.IsExpired())//域过期 分区时间统一由授权时间进行更新了
                    {
                        response.RspInfo.Fill("PLATFORM_EXPIRED");
                    }
                    else
                    {
                        response.LoginResponse.LoginID = m.Login;
                        response.LoginResponse.Mobile = m.Profile.Mobile;
                        response.LoginResponse.Name = m.Profile.Name;
                        response.LoginResponse.QQ = m.Profile.QQ;
                        response.LoginResponse.ManagerType = m.Type;
                        response.LoginResponse.MGRID = m.ID;//mgrid
                        response.LoginResponse.BaseMGRFK = m.mgr_fk;//主域id


                        //获得界面访问权限列表
                        response.LoginResponse.Permission = m.Permission;
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

            response.Add(f.ToArray());
            TLSend(response);
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
            Manager manager = session.GetManager();
            long result = NORETURNRESULT;
            if (NewPacketRequest != null)
                NewPacketRequest(session, packet, manager);
            else
                result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
            return result;


            //long result = NORETURNRESULT;
            //switch (packet.Type)
            //{
            //    case MessageTypes.SENDORDER:
            //        SrvOnOrderInsert(session,packet as OrderInsertRequest);
            //        break;
            //    case MessageTypes.SENDORDERACTION:
            //        SrvOnOrderAction(packet as OrderActionRequest);
            //        break;

            //    default:
            //        if (newPacketRequest != null)
            //            newPacketRequest(session,packet);
            //        else
            //            result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
            //        break;
            //}
            //return result;
        }

        #region 底层基础事件
        /// <summary>
        /// 业务数据包
        /// </summary>
        public event Action<ISession,IPacket,Manager> NewPacketRequest;

        /// <summary>
        /// 通过交易账户查找需要推送的管理员地址列表
        /// </summary>
        public event Func<string,ILocation[]> QryNotifyLocationsViaAccount;
        #endregion

    }

}
