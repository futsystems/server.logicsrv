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
        public override void AuthLogin(MgrClientInfo c, LoginRequest request)
        {
            //1.检查外部调用绑定
            if (newLoginRequest == null)
            {
                debug("外部认证事件没有绑定", QSEnumDebugLevel.ERROR);
                return;//回调外部loginRquest的实现函数
            }

            LoginResponse response = ResponseTemplate<LoginResponse>.SrvSendRspResponse(request);
            newLoginRequest(c, request, ref response);

            //2.检查验证结果 并将对应的数据储存到对应的client informaiton中去
            if (!response.Authorized)
            {
                debug("账户: " + request.LoginID + " 验证失败", QSEnumDebugLevel.WARNING);

                //CacheMessage(rspinstance.Serialize(), MessageTypes.LOGINRESPONSE, c);//向客户端返回请求交易账户确认
                //CacheMessage(QSMessageHelper.Message(QSSysMessage.LOGGINFAILED,"账户验证失败,请确认密码或联系管理员"), MessageTypes.SYSMESSAGE, c.ClientID);//发送客户端提示信息
                c.AuthorizedFail();
                //SendPacket(response);
            }
            else
            {
                //################客户端验证成功后的一系列操作#############################################
                debug("账户: " + request.LoginID + " 验证成功", QSEnumDebugLevel.INFO);
                //当某个客户端提供的Account验证成功后,我们需要将缓存中的绑定了该Account的客户端进行清除，一个Account只可以由一个client使用,这样才可以保证通过Account所找到的Client唯一
                //否则会出现缓存找到排序在前面的client，该client可能已经由于某些原因与服务器断开
                debug("检查相同接入类型同名客户端:" + request.LoginID, QSEnumDebugLevel.INFO);//保证客户端地址唯一

                //检查当个客户端的登入次数超过次数给出提示 不予登入
                ///_clients.DelClientByLoginID(loginid, c.FrontType, true);//从注册客户端列表中删除该登入名的客户端记录
                c.AuthorizedSuccess();
                //c.Account = response.Account;

                //debug("ClientInfo account:" + c.Account + " author:" + c.Authorized.ToString() + " response account:" + response.Account + " loginid:" + response.LoginID.ToString(), QSEnumDebugLevel.INFO);
                //SendPacket(response);
                //CacheMessage(rspinstance.Serialize(), MessageTypes.LOGINRESPONSE, c.ClientID);//向客户端返回请求交易账户确认
                //CacheMessage(QSMessageHelper.Message(QSSysMessage.LOGGINSUCCESS, "账户验证成功,祝你交易愉快！"), MessageTypes.SYSMESSAGE, c.ClientID);
                //登入成功后我们更新账户的登入信息 如果是自动重连,则第一次登入是没有采集到ip地址,硬件地址.当客户端更新本地数据时，会再次调用updatelogininfo，来更新该信息
                //UpdateLoginInfo(loginid, true, c);
                //if (ClientLoginEvent != null)
                //    ClientLoginEvent(c);


            }
            TLSend(response);
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
            TLSend(response);
        }


        /// <summary>
        /// 系统接受到客户端发送过来的委托
        /// 1.检查客户端是否Register如果没有register则clientlist不存在该ClientID
        /// 2.检查账户是否登入,如果登入 检查委托账号与登入账户是否一致
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        public void SrvOnOrderInsert(OrderInsertRequest request)
        {
            debug("got order:" + request.Order.ToString(), QSEnumDebugLevel.INFO);
            MgrClientInfo cinfo = _clients[request.ClientID];
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
                return;
            }

            //3.如果本地客户端列表中存在该ID,则我们需要比较该ID所请求的交易账号与其所发送的委托账号是否一致,这里防止客户端发送他人的account的委托
            //只有当客户端通过请求账户 提供正确的账户 与密码 系统才会将address(ClientID与Account进行绑定)
            if (!cinfo.Authorized)
            {
                debug("客户端:" + cinfo.Location.ClientID + "未登入,无法请求委托", QSEnumDebugLevel.ERROR);
                return;
            }

            //标注来自客户端的原始委托
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.OrderSource = QSEnumOrderSource.QSMONITER;
            order.TotalSize = order.size;
            order.date = Util.ToTLDate();
            order.time = Util.ToTLTime();
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
            debug("got order action:" + request.ToString(), QSEnumDebugLevel.INFO);
            MgrClientInfo cinfo = _clients[request.ClientID];
            if (cinfo == null) return;

            if (newOrderActionRequest != null)
                newOrderActionRequest(request.OrderAction);
        }

        void SrvOnMGRLoginRequest(MGRLoginRequest request)
        {
            debug("got login request:" + request.ToString(), QSEnumDebugLevel.INFO);
            MgrClientInfo clientinfo = _clients[request.ClientID];
            
            bool re = ORM.MManager.ValidManager(request.LoginID, request.Passwd);

            RspMGRLoginResponse response = ResponseTemplate<RspMGRLoginResponse>.SrvSendRspResponse(request);
            response.IsLast = true;
            response.Authorized = re;
            
            //如果验证通过返回具体的管理信息
            if (response.Authorized)
            {
                Manager m = BasicTracker.ManagerTracker[request.LoginID];
                if (m != null)
                {
                    response.LoginID = m.Login;
                    response.Mobile = m.Mobile;
                    response.Name = m.Name;
                    response.QQ = m.QQ;
                    response.ManagerType = m.Type;

                }
                clientinfo.AuthorizedSuccess();
                clientinfo.ManagerID = request.LoginID;
            }
            else
            {
                clientinfo.AuthorizedFail();
            }

            SendOutPacket(response);
            
            
        }
        /// <summary>
        /// 处理母类所不处理的消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public override long handle(IPacket packet,MgrClientInfo clientinfo)
        {
            long result = NORETURNRESULT;
            switch (packet.Type)
            {
                case MessageTypes.SENDORDER:
                    SrvOnOrderInsert(packet as OrderInsertRequest);
                    break;

                case MessageTypes.MGRLOGINREQUEST:
                    SrvOnMGRLoginRequest(packet as MGRLoginRequest);
                    break;

                case MessageTypes.SENDORDERACTION:
                    SrvOnOrderAction(packet as OrderActionRequest);
                    break;

                default:
                    //1.生成对应的Session 用于减少ClientInfo的暴露防止错误修改相关参数
                    Client2Session sesssion = new Client2Session(clientinfo);

                    //针对Manager设置相关属性
                    sesssion.SessionType = QSEnumSessionType.MANAGER;
                    sesssion.ManagerID = clientinfo.ManagerID;
                    

                    //2.通过managerid来获得manager对象,并传递到逻辑包处理函数
                    Manager manager = BasicTracker.ManagerTracker[clientinfo.ManagerID];
                    if (manager == null)
                    {
                        debug("manager:" + clientinfo.ManagerID + " do not exist!", QSEnumDebugLevel.ERROR);
                        return -1;
                    }
                    if (newPacketRequest != null)
                        newPacketRequest(packet,sesssion,manager);
                    else
                        result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
                    break;
            }
            return result;
        }

        #region TLServer->TLClient发送消息


        /// <summary>
        /// 当有新的委托时,通知所有的管理端
        /// </summary>
        /// <param name="o"></param>
        public void newOrder(Order o)
        {

        }

        /// <summary>
        /// 当系统有新的成交时,通知所有的管理端
        /// </summary>
        /// <param name="f"></param>
        public void newFill(Trade f)
        { 
            
        }

        /// <summary>
        /// 当有持仓更新时,通知所有管理端
        /// </summary>
        /// <param name="pos"></param>
        public void newPositionUpdate(Position pos)
        {

        }

        /// <summary>
        /// 当有新的委托操作时,通知所有管理端
        /// </summary>
        /// <param name="action"></param>
        public void newOrderAction(Order action)
        { 
        
        }


        /// <summary>
        /// 调用tlserver_generic发送逻辑包 packet
        /// </summary>
        /// <param name="packet"></param>
        //public void SendPacket(IPacket packet)
        //{
        //    TLSend(packet);
        //}
        #endregion

        #region 底层基础事件
        public event LoginRequestDel<MgrClientInfo> newLoginRequest;//登入服务器
        public event OrderDelegate newSendOrderRequest;//发送委托
        public event OrderActionDelegate newOrderActionRequest;//发送委托操作
        public event MessageArrayDelegate newFeatureRequest;//请求功能列表
        public event MgrPacketRequestDel newPacketRequest;
        public event LocationsViaAccountDel GetLocationsViaAccountEvent;//
        #endregion

    }

    /// <summary>
    /// 管理服务器的逻辑包委托
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="session"></param>
    /// <param name="manager"></param>
    public delegate void MgrPacketRequestDel(IPacket packet,ISession session,Manager manager);
    /// <summary>
    /// 通过某个交易帐号获得对应的管理端通知列表
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public delegate ILocation[] LocationsViaAccountDel(string account);
}
