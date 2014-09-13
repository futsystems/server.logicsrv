//using System;
//using System.Collections.Generic;
//using System.Text;

//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using TradingLib.Common;

//using TradingLib.API;
//using TradingLib.MySql;
//using System.Data;
//using System.IO;
//using TradingLib.LitJson;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 交易服务器内核,提供客户端注册,交易委托,回报,撤销等功能
//    /// </summary>
//    [System.ComponentModel.DesignerCategory("")]
//    public class TLServer_MQ : TLServer_Base,ITrdService
//    {
//        public TLServer_MQ(string name,string server, int port)
//            : base(name,server, port)
//        {
            

//        }

//        #region 扩展函数部分
//        public IClientInfo[] Clients {

//            get {
//                return _clients.Clients;
//            }
//        }
//        /// <summary>
//        /// 返回所有注册symbol的basket,用于服务器重启时,重新注册市场数据
//        /// </summary>
//        public SymbolBasket AllClientBasket
//        {
//            get
//            {
//                return null;
//                //Basket b = new BasketImpl();
//                //foreach(IClientInfo c in _clients.Clients)
//                //{
//                //    b.Add(BasketImpl.FromString((c as TrdClientInfo).Stocks));
//                //}
//                //return b;
//            }
//        }

//        /// <summary>
//        /// 返回某个client所注册的symbol
//        /// </summary>
//        /// <param name="client"></param>
//        /// <returns></returns>
//        public string ClientSymbols(string address)
//        {
//            TrdClientInfo cinfo = _clients[address] as TrdClientInfo;
//            if (cinfo == null) return "";
//            return cinfo.Stocks;
//        }
//        #endregion

        
//        public override void  Init()
//        {
//            PROGRAME = "TLServer_MQ";
//        }

//        #region 定时将clientlist文本化到本地文件 用于服务器崩溃时从文本恢复

//        const string clientlistfn = @"cache\TrdClientList";
//        int sessiondelay = IPUtil.HEARTBEATPERIOD;

//        [TaskAttr("保存客户端Sessoin",60,"将客户端Session信息保存用于服务端崩溃后恢复客户端连接数据")]
//        public void Task_SaveSession()
//        {
//            try
//            {
//                //实例化一个文件流--->与写入文件相关联  
//                using (FileStream fs = new FileStream(clientlistfn, FileMode.Create))
//                {
//                    //实例化一个StreamWriter-->与fs相关联  
//                    using (StreamWriter sw = new StreamWriter(fs))
//                    {

//                        foreach (IClientInfo info in _clients.Clients)
//                        {
//                            TrdClientInfo tinfo = info as TrdClientInfo;
//                            string str = tinfo.ToString();

//                            sw.WriteLine(str);
//                        } 
//                        sw.Flush();
//                        sw.Close();
//                    }
//                    fs.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                debug("Cache clientlist error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion


//        #region 从Cache得到session用于母函数恢复连接
//        public override List<IClientInfo> LoadSessions()
//        {
//            List<IClientInfo> cinfolist = new List<IClientInfo>();
//            try
//            {
//                //实例化一个文件流--->与写入文件相关联  
//                using (FileStream fs = new FileStream(clientlistfn, FileMode.Open))
//                {
//                    //实例化一个StreamWriter-->与fs相关联  
//                    using (StreamReader sw = new StreamReader(fs))
//                    {
//                        while (sw.Peek() > 0)
//                        {
//                            string str = sw.ReadLine();
//                            cinfolist.Add(TrdClientInfo.FromString(str));
//                        }
//                        sw.Close();
//                    }
//                    fs.Close();
//                }
//                return cinfolist;
//            }
//            catch (Exception ex)
//            {
//                debug("Error In Restoring (Session):"+ex.ToString(),QSEnumDebugLevel.ERROR);
//                return cinfolist;
//            }
            
//        }

//        #endregion


 




       

//        #region client-->TLServer消息所引发的各类操作
        
//        /// <summary>
//        /// 系统接受到客户端发送过来的委托
//        /// 1.检查客户端是否Register如果没有register则clientlist不存在该ClientID
//        /// 2.检查账户是否登入,如果登入 检查委托账号与登入账户是否一致
//        /// 这里的检查逻辑保证了委托对应的地址是系统内注册过的客户端地址,并且客户端已经登入成功,而且委托Account与请求服务的Account一致
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        public override void SrvSendOrder(string msg, string address)
//        {
//            //我们需要检查某个地址发上来的委托是否与account相对应
//            Order o = OrderImpl.Deserialize(msg);
//            ////1.通过会话标识获得client信息,如果不存在,表明该ID没有通过注册连接到我们的服务端
//            TrdClientInfo cinfo = _clients[address] as TrdClientInfo;
//            if (cinfo == null)
//            {
//                debug("系统拒绝委托:" + o.ToString() + "系统没有该注册端:" + address + "|" + o.Account, QSEnumDebugLevel.WARNING);
//                return;
//            }


//            //2.检查委托是否有效
//            if (o == null || !o.isValid)
//            {
//                debug("系统拒绝委托:" + (o == null ? "Null" : o.ToString()) + "委托无效:" + address + "|" + o.Account, QSEnumDebugLevel.WARNING);
//                //TLSend("系统拒绝，委托无效", MessageTypes.POPMESSAGE, address);//发送客户端提示信息
//                CacheMessage(QSMessageHelper.Message(QSEnumOrderMessageType.REJECT, QSMessageContent.ORDER_INVALID), MessageTypes.ORDERMESSAGE, address);//发送客户端提示信息
//                return;
//            }
//            debug("tlmq order go to here..........:"+cinfo.IsLoggedIn.ToString() +" accountid:"+cinfo.AccountID, QSEnumDebugLevel.INFO);

//            //3.如果本地客户端列表中存在该ID,则我们需要比较该ID所请求的交易账号与其所发送的委托账号是否一致,这里防止客户端发送他人的account的委托
//            //只有当客户端通过请求账户 提供正确的账户 与密码 系统才会将address(ClientID与Account进行绑定)
//            if ((!cinfo.IsLoggedIn) || (cinfo.AccountID != o.Account))//客户端没有登入或者登入ID与委托ID不符
//            {
//                CacheMessage(QSMessageHelper.Message(QSEnumOrderMessageType.REJECT, QSMessageContent.ORDER_ACCOUNT_CLIENT_NOT_MATCH), MessageTypes.ORDERMESSAGE, address);//发送客户端提示信息
//                return;
//            }
//            //4.标注来自客户端的原始委托
//            o.OrderSource = QSEnumOrderSource.CLIENT;
//            //设定发单时间
//            o.date = Util.ToTLDate(DateTime.Now);
//            o.time = Util.ToTLTime(DateTime.Now);

//            //5.对外层触发委托事件
//            if (newSendOrderRequest != null)
//                newSendOrderRequest(o);

//        }
//        /// <summary>
//        /// 取消委托,参数为全局委托编号
//        /// </summary>
//        /// <param name="msg"></param>
//        public override void SrvCancelOrder(string msg,string address)
//        {
//            //通过address(ClientID)查询本地客户端列表是否存在该ID
//            TrdClientInfo cinfo = _clients[address] as TrdClientInfo;
//            if (cinfo == null) return;
            
//            long id = 0;
//            if (long.TryParse(msg, out id) && (newOrderCancelRequest != null))
//                newOrderCancelRequest(id);
//        }

//        /// <summary>
//        /// 客户端请求注册symbol数据
//        /// </summary>
//        /// <param name="cname"></param>
//        /// <param name="stklist"></param>
//        void SrvRegStocks(string address, string stklist)
//        {
//            TrdClientInfo cinfo = _clients[address] as TrdClientInfo;
//            if (cinfo == null) return;

//            debug("Client:" + address + " Request Mktdata: " + stklist, QSEnumDebugLevel.INFO);
//            cinfo.Stocks = stklist;
//            SrvBeatHeart(address);
//            if (newRegisterSymbols != null)
//            {
//                newRegisterSymbols(address, stklist);
//            }
//        }

//        /// <summary>
//        /// 客户端请求清除已注册的symbol
//        /// </summary>
//        /// <param name="cname"></param>
//        void SrvClearStocks(string address)
//        {
//            TrdClientInfo cinfo = _clients[address] as TrdClientInfo;
//            if (cinfo == null) return;
//            cinfo.Stocks = "";
//            SrvBeatHeart(address);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="c">客户端信息</param>
//        /// <param name="loginid">登入ID可以是邮件地址/手机帐号/交易帐号等</param>
//        /// <param name="pass">主帐户密码</param>
//        /// <param name="authtype">认证类型 -1系统只能识别 0邮件 1手机 2username 3uid 4account</param>
//        /// <param name="type">服务类别,用于识别某个客户端登入交易系统后授权的是那种类型的服务,比如比赛,配资返回帐户下对应的交易帐号</param>
//        public override void AuthLogin(IClientInfo c, string loginid, string pass,QSEnumTLServiceType type)
//        {
//            if (newLoginRequest == null) return;//回调外部loginRquest的实现函数

//            //1.生成response对象
//            ILoginResponse response = new LoginResponse(c.Address, loginid, type);

//            //调用业务层进行认证逻辑 提供loginid,pass,service 用授权信息对进行登入验证,并且指定服务类型
//            if (!newLoginRequest(loginid, pass,ref response))
//            {
//                debug("账户: " + loginid + " 验证失败",QSEnumDebugLevel.WARNING);
//                //1.回报登入通知给客户端
//                CacheMessage(JsonMapper.ToJson(response).ToString(), MessageTypes.LOGINRESPONSE, c.Address);//向客户端返回请求交易账户确认
//                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINFAILED, QSMessageContent.LOGIN_FAILED), MessageTypes.SYSMESSAGE, c.Address);//发送客户端提示信息
//                return;
//            }
//            else
//            {
//                //################客户端验证成功后的一系列操作#############################################
//                debug("账户: " + loginid + " 验证成功",QSEnumDebugLevel.INFO);
//                string account = response.Account;
//                //在同一个前置类型中,每个帐号只能登入一个回话,
//                //初始状态:向某个Account发送消息时,只获得一个地址，因此当某个客户端提供的Account验证成功后,我们需要将缓存中的绑定了该Account的客户端进行清除，一个Account只可以由一个client使用,这样才可以保证通过Account所找到的Client唯一，否则会出现缓存找到排序在前面的client，该client可能已经由于某些原因与服务器断开
//                debug("检查相同接入类型同名客户端:" + response.Account);//保证客户端地址唯一
                
//                //1.将前置中同一帐号的其他回话删除
//                _clients.DelClientByAccount(response.Account, c.FrontType, true);//从注册客户端列表中删除该登入名的客户端记录
                
//                //2.将登入回报相关内容解析到Client
//                ParseResponse(ref c, ref response);

//                //3.回报登入通知给客户端
//                CacheMessage(JsonMapper.ToJson(response).ToString(), MessageTypes.LOGINRESPONSE, c.Address);//向客户端返回请求交易账户确认
//                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINSUCCESS, QSMessageContent.LOGIN_SUCCESS), MessageTypes.SYSMESSAGE, c.Address);

//                //4.更新回话登入状态信息，登入成功后更新账户的登入信息 如果是自动重连,则第一次登入是没有采集到ip地址,硬件地址.当客户端更新本地数据时，会再次调用updatelogininfo，来更新该信息
//                UpdateLoginInfo(account, true, c);
//            }
//        }
//        /// <summary>
//        /// 将登入信息中的用户状态解析到Client对象中
//        /// </summary>
//        /// <param name="client"></param>
//        /// <param name="response"></param>
//        void ParseResponse(ref IClientInfo client, ref ILoginResponse response)
//        { 
//            //交易类服务将交易帐号绑定到Client
//            if(response.Service == QSEnumTLServiceType.Service_Race || response.Service == QSEnumTLServiceType.Service_FinService)
//            {
//                client.AccountID = response.Account;
//            }
//            client.LoginID = response.LoginID;
//            client.UID = response.UID;
//        }
        
//        /// <summary>
//        /// 请求交易功能特征列表
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        public override void SrvReqFuture(string msg,string address)
//        {
//            string msf = "";
//            List<MessageTypes> f = new List<MessageTypes>();
            
//            f.Add(MessageTypes.REGISTERCLIENT);//注册客户端
//            f.Add(MessageTypes.LOGINREQUEST);//请求登入
//            f.Add(MessageTypes.CLEARCLIENT);//注销客户端

//            f.Add(MessageTypes.HEARTBEAT);//发送心跳包
//            f.Add(MessageTypes.HEARTBEATREQUEST);//心跳请求
//            f.Add(MessageTypes.HEARTBEATRESPONSE);
//            f.Add(MessageTypes.VERSION);//服务器版本
//            //f.Add(MessageTypes.BROKERNAME);//服务端标识
//            f.Add(MessageTypes.FEATUREREQUEST);//请求功能特征
//            f.Add(MessageTypes.FEATURERESPONSE);//回报功能请求

//            f.Add(MessageTypes.REGISTERSTOCK);//请求行情数据
//            f.Add(MessageTypes.CLEARSTOCKS);//取消行情数据
            
//            List<string> mf = new List<string>();
//            foreach (MessageTypes t in f)
//            {
//                int ti = (int)t;
//                mf.Add(ti.ToString());
//            }
//            if (newFeatureRequest != null)
//            {
//                MessageTypes[] f2 = newFeatureRequest();
//                foreach (MessageTypes t in f2)
//                {
//                    int ti = (int)t;
//                    mf.Add(ti.ToString());
//                }
//            }
//            msf = string.Join(",", mf.ToArray());
//            CacheMessage(msf, MessageTypes.FEATURERESPONSE, address);
//        }

//        /// <summary>
//        /// 处理母类所不处理的消息类型
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="msg"></param>
//        /// <param name="address"></param>
//        /// <returns></returns>
//        public override long handle(MessageTypes type, string msg, ISession session)
//        {
//            long result = NORETURNRESULT;
//            switch (type)
//            {
//                case MessageTypes.REGISTERSTOCK:
//                    //debug(PROGRAME + "#Got REGISTERSTOCK From " + address + "  " + msg.ToString());
//                    //string[] m2 = msg.Split('+');
//                    SrvRegStocks(session.SessionID,msg);
//                    break;
//                case MessageTypes.CLEARSTOCKS:
//                    //debug(PROGRAME + "#Got CLEARSTOCKS From " + address + "  " + msg.ToString());
//                    SrvClearStocks(msg);
//                    break;
//                default:
//                    //将默认server没有实现的功能通过default路由到外层的event handler处理函数中去
//                    if (newUnknownRequestSource != null)
//                        result = newUnknownRequestSource(type, msg,session);
//                    //else if (newUnknownRequest != null)
//                    //    result = newUnknownRequest(type, msg);
//                    else
//                        result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
//                    break;
//            }
//            return result;
//        }
//        #endregion


//        #region TLServer -->client发送相应回报
//        /// <summary>
//        /// 向客户端发送委托消息 
//        /// </summary>
//        /// <param name="o"></param>
//        /// <param name="msg"></param>
//        public void newOrderMessage(Order o, string msg)
//        {
//            if (o.Account == null || o.Account == string.Empty) return;
//            TLSend(msg + "@" + o.id.ToString(), MessageTypes.ORDERMESSAGE, _clients.AddListForAccount(o.Account));
//            debug("send orderMessage to client " + msg + "@" + o.id.ToString());
//        }
//        /// <summary>
//        /// 向客户端发送系统消息
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="clientid"></param>
//        public void newSysMessage(string msg, string address)
//        {
//            TLSend(msg, MessageTypes.SYSMESSAGE, address);
//            debug("send sysmessage to client" + msg + " address:" + address);
//        }

//        /// <summary>
//        /// 指定客户端编号进行发送委托回报 用于登入时后的交易信息恢复
//        /// </summary>
//        /// <param name="o"></param>
//        /// <param name="clientID"></param>
//        public void RestoreOrder(Order o, string address)
//        {
//            if (o == null || !o.isValid)
//            {
//                debug("invalid order: " + (o == null ? "Null Order" : o.ToString()), QSEnumDebugLevel.WARNING);
//                return;
//            }
//            if (o.Account == null || o.Account == string.Empty) return;//如果委托不含有Account信息则直接返回
//            TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, address);
//            debug("send ordernotify to client | " + o.ToString());
//        }
//        /// <summary>
//        /// 服务端向客户端发送委托回报
//        /// </summary>
//        /// <param name="o"></param>
//        public void newOrder(Order o)
//        {
//            if (o==null ||!o.isValid)
//            {
//                debug("invalid order: " + (o==null ? "Null Order":o.ToString()),QSEnumDebugLevel.WARNING);
//                return;
//            }
//            if (o.Account == null || o.Account == string.Empty) return;//如果委托不含有Account信息则直接返回
//            TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, _clients.AddListForAccount(o.Account));
//            debug("send ordernotify to client | "+o.ToString());
//            /*
//             通过o.account反查交易客户端的通讯地址,如果列表中存在2个clientinfo拥有同样的account那么只会返回排序在前面的address
//             当客户端注册到服务端后,如果非正常关闭会造成服务端不知道清理该客户端，造成通过account返查同通讯地址得到的是以前老的地址
//             因此在其请求交易账号的时,我们需要将拥有该account的所有client端信息删除
//             * */
//        }

//        public void RestoreFill(Trade trade, string address)
//        {
//            // make sure our trade is filled and initialized properly
//            if (trade == null || !trade.isValid)
//            {
//                debug("invalid trade: " + (trade == null ? "Null Trade" : trade.ToString()));
//                return;
//            }
//            if (trade.Account == null || trade.Account == string.Empty) return;
//            TLSend(TradeImpl.Serialize(trade), MessageTypes.EXECUTENOTIFY, address);
//            debug("send Filld to client | " + trade.ToString());
//        }

//        /// <summary>
//        /// 向客户端发送成交回报
//        /// </summary>
//        /// <param name="trade">The trade to include in the notification.</param>
//        public void newFill(Trade trade)
//        {
//            // make sure our trade is filled and initialized properly
//            if (trade==null || !trade.isValid)
//            {
//                debug("invalid trade: " + (trade==null?"Null Trade":trade.ToString()));
//                return;
//            }
//            if (trade.Account == null || trade.Account == string.Empty) return;
//            TLSend(TradeImpl.Serialize(trade), MessageTypes.EXECUTENOTIFY, _clients.AddListForAccount(trade.Account));
//            debug("send Filld to client | "+trade.ToString());
//        }

//        public void RestoreCancel(Order o, string address)
//        {
//            if (o.Account == null || o.Account == string.Empty) return;
//            TLSend(o.id.ToString(), MessageTypes.ORDERCANCELRESPONSE, address);
//            debug("send Cancel to client | " + o.ToString());
//        }

//        /// <summary>
//        /// 向客户端发送取消回报
//        /// </summary>
//        /// <param name="orderid_being_cancled"></param>
//        public void newCancel(Order o)
//        {
//            if (o.Account == null || o.Account == string.Empty) return;
//            TLSend(o.id.ToString(), MessageTypes.ORDERCANCELRESPONSE, _clients.AddListForAccount(o.Account));
//            debug("send Cancel to client | "+o.ToString());
//        }

//        /// <summary>
//        /// 服务端给某个客户端恢复昨日持仓
//        /// 客户端数据先恢复昨日持仓，然后推送
//        /// </summary>
//        /// <param name="pos"></param>
//        /// <param name="cliendid"></param>
//        public void RestorePosition(Position pos, string address)
//        {
//            if (pos.Account == null || pos.Account == string.Empty) return;
//            TLSend(PositionImpl.Serialize(pos), MessageTypes.POSITIONRESPONSE, address);
//            debug("send position to client|" + pos.ToString());
//        }

//        /// <summary>
//        /// 向客户端发送持仓状态更新回报
//        /// </summary>
//        /// <param name="pos"></param>
//        public void newPosition(Position pos)
//        {
//            if (pos.Account == null || pos.Account == string.Empty) return;
//            TLSend(PositionImpl.Serialize(pos), MessageTypes.POSITIONUPDATE, _clients.AddListForAccount(pos.Account));
//            debug("send positionupdate to client|" + pos.ToString());

//        }
                
//        #endregion

//        //服务事件列表
//        public event LoginRequestDel newLoginRequest;//登入服务器
//        public event OrderDelegate newSendOrderRequest;//发送委托
//        public event LongDelegate newOrderCancelRequest;//发送取消
//        public event SymbolRegisterDel newRegisterSymbols;//订阅市场数据
//        public event MessageArrayDelegate newFeatureRequest;//请求功能列表
//        public event UnknownMessageDelegateSession newUnknownRequestSource;//未知带返回地址消息

//    }
//}
