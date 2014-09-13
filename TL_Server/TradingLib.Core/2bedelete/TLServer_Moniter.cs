using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using TradingLib.Common;
using TradingLib.API;
using System.Data;
using System.IO;

namespace TradingLib.Core
{
    public class TLServer_Moniter
    {
        //public void Start();
        //public void Stop();

    }
    ///// <summary>
    ///// 交易服务器内核,提供客户端注册,交易委托,回报,撤销等功能
    ///// </summary>
    //public class TLServer_Moniter:TLServer_Base,IMgrService
    //{
    //    public TLServer_Moniter(string name,string server, int port)
    //        : base(name,server, port)
    //    {
                
            
    //    }
        
    //    public event FindAccountDel SendFindAccountEvent;
    //    public event FindOrderDel SendFindOrderEvent;

    //    private IAccount FindAccount(string acc)
    //    {
    //        if (SendFindAccountEvent != null)
    //            return SendFindAccountEvent(acc);
    //        return null;
    //    }

    //    private Order FindOrder(long oid)
    //    {
    //        if (SendFindOrderEvent != null)
    //            return SendFindOrderEvent(oid);
    //        return null;
    //    }
    //    ~TLServer_Moniter()
    //    {
    //        try
    //        {
    //            Stop();

    //        }
    //        catch { }
    //    }

        



        
    //    //权限检查
    //    PermissionCheck _permCheck;

    //    //mysqlDBClientSession mysqldb;
    //    public override void Init()
    //    {
    //        PROGRAME = "TLServer_Mgr";
    //        _permCheck = new PermissionCheck();
    //    }


      

    //    //TLServer触发的交易事件 这些事件由上层server分发到不同的Broker进行执行
    //    //public event StringDelegate newAcctRequest;
    //    //public event AccountRequestDel newAcctRequest;

    //    public event LoginRequestDel newLoginRequest;//登入服务器
    //    public event FindCustomerDataSet newCustomerSetRequest;//查找customer对应的数据集
    //    public event OrderDelegate newSendOrderRequest;//发送委托请求
    //    public event LongDelegate newOrderCancelRequest;//取消委托请求
    //    //public event PositionArrayDelegate newPosList;
    //    public event SymbolRegisterDel newRegisterSymbols;
    //    public event MessageArrayDelegate newFeatureRequest;//请求功能列表

    //    public event UnknownMessageDelegateSession newUnknownRequestSource;//未知请求带地址
    //    //public event AccountUpdateDel SendAccountUpdateEvent;
    //    public event LoginInfoDel SendLoginInfoEvent;


    //    #region 从session的保存与恢复
    //    const string clientlistfn = @"cache\MgrClientList";
    //    int sessiondelay = IPUtil.HEARTBEATPERIOD;//默认客户端发送一批心跳包 则我们重新建立一次cache

    //    [TaskAttr("保存管理端Sessoin", 60, "将管理端Session信息保存用于服务端崩溃后恢复客户端连接数据")]
    //    public void SaveSession()
    //    {
    //        try
    //        {
    //            //实例化一个文件流--->与写入文件相关联  
    //            using (FileStream fs = new FileStream(clientlistfn, FileMode.Create))
    //            {
    //                //实例化一个StreamWriter-->与fs相关联  
    //                using (StreamWriter sw = new StreamWriter(fs))
    //                {
    //                    foreach (IClientInfo info in _clients.Clients)
    //                    {
    //                        MgrClientInfo tinfo = info as MgrClientInfo;
    //                        string str = tinfo.ToString();
    //                        sw.WriteLine(str);
    //                    }
    //                    sw.Flush();
    //                    sw.Close();
    //                }
    //                fs.Close();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            debug(PROGRAME + "Cache clientlist error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
    //        }
    //    }
    //    #region 定时将clientlist文本化到本地文件 用于服务器崩溃时从文本恢复

    //    /*
    //    Thread sessionthread;
    //    bool sessiongo = false;
    //    int sessiondelay = IPUtil.HEARTBEATPERIOD;//默认客户端发送一批心跳包 则我们重新建立一次cache
    //    void StartSessionLog()
    //    {
    //        if (sessiongo) return;
    //        sessiongo = true;
    //        sessionthread = new Thread(new ThreadStart(sessionproc));
    //        sessionthread.IsBackground = true;
    //        sessionthread.Name = "MGR SaveSessionFile Thread";
    //        sessionthread.Start();
    //        ThreadTracker.Register(sessionthread);
    //    }
    //    const string clientlistfn = @"cache\MgrClientList";
    //    void sessionproc()
    //    {
    //        while (sessiongo)
    //        {
    //            Thread.Sleep(1000 * sessiondelay);

    //            try
    //            {
    //                //实例化一个文件流--->与写入文件相关联  
    //                FileStream fs = new FileStream(clientlistfn, FileMode.Create);
    //                //实例化一个StreamWriter-->与fs相关联  
    //                StreamWriter sw = new StreamWriter(fs);

    //                foreach (IClientInfo info in _clients.Clients)
    //                {
    //                    MgrClientInfo tinfo = info as MgrClientInfo;
    //                    string str = tinfo.ToString();

    //                    sw.WriteLine(str);



    //                }
    //                sw.Flush();
    //                sw.Close();
    //                fs.Close();
    //            }
    //            catch (Exception ex)
    //            {
    //                debug(PROGRAME + "Cache clientlist error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
    //            }

    //        }

    //    }
    //    **/
    //    #endregion

    //    public override List<IClientInfo> LoadSessions()
    //    {
    //        List<IClientInfo> cinfolist = new List<IClientInfo>();
    //        try
    //        {
    //            //实例化一个文件流--->与写入文件相关联  
    //            using (FileStream fs = new FileStream(clientlistfn, FileMode.Open))
    //            {
    //                //实例化一个StreamWriter-->与fs相关联  
    //                using (StreamReader sw = new StreamReader(fs))
    //                {
    //                    while (sw.Peek() > 0)
    //                    {
    //                        string str = sw.ReadLine();
    //                        cinfolist.Add(MgrClientInfo.FromString(str));
    //                    }
    //                    sw.Close();
    //                }
    //                fs.Close();
    //            }
    //            //从Cache恢复客户端连接后然后启动session记录线程
    //            //TaskCentre.RegisterTask(new TaskProc("定时保存客户端Session", new TimeSpan(0, 0, 1000 * sessiondelay), SaveSession));
    //            return cinfolist;
    //        }
    //        catch (Exception ex)
    //        {
    //            debug("Error In Restoring (Session):" + ex.ToString(), QSEnumDebugLevel.ERROR);
    //            //TaskCentre.RegisterTask(new TaskProc("定时保存客户端Session", new TimeSpan(0, 0, 1000 * sessiondelay), SaveSession));
    //            return cinfolist;
    //        }
           
    //    }


    //    #endregion

    //    /// <summary>
    //    /// 过滤Account信息,检查某个Account是否绑定给了该客户端
    //    /// </summary>
    //    /// <param name="clientId"></param>
    //    /// <param name="a"></param>
    //    /// <returns></returns>
    //    public bool ViewAccountRight(string clientId, IAccount a)
    //    {
    //        try
    //        {
    //            //debug("检查管理端:" + clientId + " 查看交易账户:" + a.ID + " 权限");
    //            IClientInfo c = _clients[clientId];
    //            //MgrClientInfo c =  as MgrClientInfo;
    //            //debug("clientID:" + c.ClientID);
    //            MgrClientInfo mc = c as MgrClientInfo;
    //            if (c == null || a == null) return false;

    //            //debug("accounts:" + mc.Accounts.ToString());
    //            if (mc.Accounts == "ALL") return true;//"ALL" 代表可以观察所有的交易account
    //            if (mc.Accounts.Contains(a.ID)) return true;//检查accounts队列中是否包含有该a.id如果包含则返回true
    //            return false;
    //        }
    //        catch (Exception ex)
    //        {
    //            debug(ex.ToString());
    //            return false;
    //        }
        
    //    }
    //    /// <summary>
    //    /// 检查某个客户端是否运行某个messagetype类型的请求
    //    /// </summary>
    //    /// <param name="mt"></param>
    //    /// <param name="c"></param>
    //    /// <returns></returns>
    //    public bool PermissionCheck(MessageTypes mt,MgrClientInfo c)
    //    {
    //        return _permCheck.checkPermission(mt, c);
    //    }

        

    //    #region client-->TLServer消息所引发的各类操作

    //    public  override void SrvRegClient(string front,string address)
    //    {
    //        IClientInfo cinfo = _clients[address];
    //        if (cinfo == null)
    //        {
    //            //如果不存在address对应的客户端 则直接将该客户端缓存到本地
    //            _clients.RegistClient(new MgrClientInfo(address));
    //        }
    //        else
    //        {
    //            //如果存在该address对应的客户端 则将老客户端删除然后缓存到本地
                
    //            _clients.UnRegistClient(address);
    //            _clients.RegistClient(new MgrClientInfo(address));
    //        }
    //        SrvBeatHeart(address);
    //        debug(PROGRAME +":Client" + address + " Registed To Server",QSEnumDebugLevel.INFO);
    //    }

    //    /// <summary>
    //    /// 服务客户端请求登入认证函数
    //    /// </summary>
    //    /// <param name="c"></param>
    //    /// <param name="loginid"></param>
    //    /// <param name="pass"></param>
    //    public override void AuthLogin(IClientInfo c, string loginid, string pass, QSEnumTLServiceType type)
    //    {
    //        if (newLoginRequest == null || newCustomerSetRequest==null) return;//回调外部loginRquest的实现函数
    //        //1.生成response对象
    //        ILoginResponse response = new LoginResponse(c.Address, loginid, type);

    //        string account = "";
    //        if (!newLoginRequest(loginid, pass,ref response))
    //        {
    //            debug("账户: " + loginid + "验证失败");
    //            CacheMessage("False", MessageTypes.LOGINRESPONSE, c.Address);//向客户端返回请求交易账户确认
    //            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINFAILED, "账户验证失败,请确认密码或联系管理员"), MessageTypes.SYSMESSAGE, c.Address);//发送客户端提示信息
    //            return;
    //        }
    //        else
    //        {
    //            debug("账户: " + loginid + "验证成功");
    //            DataSet ds = newCustomerSetRequest(loginid);//mysqldb.getCustomer(loginid);
    //            //debug("账户信息行数:" + ds.Tables["customers"].Rows.Count.ToString());
    //            DataRow dr = ds.Tables["customers"].Rows[0];
    //            (c as MgrClientInfo).Accounts = dr["accounts"].ToString();
    //            (c as MgrClientInfo).CustomerType = (QSEnumCustomerType)Enum.Parse(typeof(QSEnumCustomerType), dr["type"].ToString());
               
    //            _clients.DelClientByAccount(loginid,c.FrontType,false);//不允许单个账户多个session进行登入
    //            c.AccountID = loginid;
    //            //CacheMessage(loginid, MessageTypes.MGRCUSTOMERRLOGINREP, c.ClientID);//向客户端返回请求交易账户确认
    //            CacheMessage("True", MessageTypes.LOGINRESPONSE, c.Address);//向客户端返回请求交易账户确认
    //            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.LOGGINSUCCESS, "账户验证成功！"), MessageTypes.SYSMESSAGE, c.Address);

    //            //如果登入账户是Admin则我们需要主动发送委托检查,账户检查等配置文件
    //            if ((c as MgrClientInfo).CustomerType == QSEnumCustomerType.ADMIN)
    //            {
    //                debug(PROGRAME + ":传输委托检查规则给客户端:" + c.Address);
    //                string so = XMLTransportHelper.XML2String(OrderCheckTracker.getXMLDoc());
    //                CacheMessage(GZip.Compress(so), MessageTypes.MGRORDERRULEFILEREP, c.Address);
    //                debug(PROGRAME + ":传输账户检查规则给客户端:" + c.Address);
    //                string sa = XMLTransportHelper.XML2String(AccountCheckTracker.getXMLDoc());
    //                CacheMessage(GZip.Compress(sa), MessageTypes.MGRACCOUNTRULEFILEREP, c.Address);
    //                debug(PROGRAME + ":传输默认合约列表给客户端:" + c.Address);
    //                //string ss = XMLTransportHelper.XML2String(SecurityTracker.getXMLDoc());
    //                //CacheMessage(GZip.Compress(ss), MessageTypes.MGRDEFAULTSECFILEREP, c.Address);


    //            }
    //            //TLSend("账户验证成功,祝你交易愉快！", MessageTypes.POPMESSAGE, ClientID);
    //            //updateAccountConnect(acname, true, ClientID);
    //        }
    //    }

    //    /// <summary>
    //    /// 请求交易功能特征列表
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    /// <param name="address"></param>
    //    public override void SrvReqFuture(string msg,string address)
    //    {
    //        string msf = "";
    //        List<MessageTypes> f = new List<MessageTypes>();
    //        f.Add(MessageTypes.REGISTERCLIENT);//注册客户端
    //        f.Add(MessageTypes.VERSION);//服务器版本
    //        f.Add(MessageTypes.BROKERNAME);//服务端标识
    //        f.Add(MessageTypes.LOGINREQUEST);//请求登入
    //        f.Add(MessageTypes.CLEARCLIENT);//注销客户端
    //        f.Add(MessageTypes.HEARTBEATREQUEST);//心跳请求
    //        f.Add(MessageTypes.HEARTBEATRESPONSE);

    //        f.Add(MessageTypes.FEATUREREQUEST);//请求功能特征
    //        f.Add(MessageTypes.FEATURERESPONSE);//回报功能请求

    //        f.Add(MessageTypes.REGISTERSTOCK);//请求行情数据
    //        f.Add(MessageTypes.CLEARSTOCKS);//取消行情数据
            
    //        List<string> mf = new List<string>();
    //        foreach (MessageTypes t in f)
    //        {
    //            int ti = (int)t;
    //            mf.Add(ti.ToString());
    //        }
    //        if (newFeatureRequest != null)
    //        {
    //            MessageTypes[] f2 = newFeatureRequest();
    //            foreach (MessageTypes t in f2)
    //            {
    //                int ti = (int)t;
    //                mf.Add(ti.ToString());
    //            }
    //        }
    //        msf = string.Join(",", mf.ToArray());
    //        TLSend(msf, MessageTypes.FEATURERESPONSE, address);
    //    }
    //    /// <summary>
    //    /// 发送委托
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    /// <param name="address"></param>
    //    public override void SrvSendOrder(string msg, string address)
    //    {
            
    //        //我们需要检查某个地址发上来的委托是否与account相对应
    //        Order o = OrderImpl.Deserialize(msg);
    //        //通过address(ClientID)查询本地客户端列表是否存在该ID
    //        //MgrClientInfo cinfo = _clients[address] as MgrClientInfo;
    //        IClientInfo cinfo = _clients[address];
    //        //如果不存在该ID表明该ID没有通过注册,连接到我们的服务端，我们输出日志,然后直接返回
    //        if (cinfo == null)
    //        {
    //            debug("TLServer:" + "系统拒绝委托:" + o.ToString() + "系统没有该注册端:" + address + "|" + o.Account);
    //            return;
    //        }
    //        if (o == null || !o.isValid)
    //        {
    //            debug("TLServer:" + "系统拒绝委托:" + (o == null ? "Null" : o.ToString()) + "委托无效:" + address + "|" + o.Account);
    //            //TLSend("系统拒绝，委托无效", MessageTypes.POPMESSAGE, address);//发送客户端提示信息
    //            return;
    //        }

    //        //从管理端发上来的委托 我们需要返查到对应的账户 然后检查 该管理端是否有对该账户的监控权限.这里需要细化一些细节
    //        //正常交易服务器是检查注册端的登入ID与该注册端发上来的委托的Account是否一致
    //        //如果本地客户端列表中存在该ID,则我们需要比较该ID所请求的交易账号与其所发送的委托账号是否一致,这里防止客户端发送他人的account的委托
    //        //只有当客户端通过请求账户 提供正确的账户 与密码 系统才会将address(ClientID与Account进行绑定)
    //        //if (cinfo.LoginID != o.Account)
    //        //{
    //            //TLSend("系统拒绝，系统注册端地址与委托单账户不符", MessageTypes.POPMESSAGE, address);//发送客户端提示信息
    //        //    return;
    //        //}

    //        //_log.GotDebug(o.ToString());
    //        if (newSendOrderRequest != null)
    //            newSendOrderRequest(o);
    //    }
    //    /// <summary>
    //    /// 取消委托
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    public override void SrvCancelOrder(string msg, string address)
    //    {
    //        //这里需要通过委托来返查到委托 然后检查是否具有权限
    //        long id = 0;
    //        if (long.TryParse(msg, out id) && (newOrderCancelRequest != null))
    //            newOrderCancelRequest(id);
    //    }

    //    #endregion


    //    #region TLServer -->client发送相应回报

    //    /// <summary>
    //    /// 向所有的管理端转发帐户更新通知
    //    /// </summary>
    //    /// <param name="account"></param>
    //    public void newAccountSettingChanged(IAccount account)
    //    {
    //        foreach (IClientInfo c in _clients.Clients)
    //        {
    //            if (!ViewAccountRight(c.Address, FindAccount(account.ID))) continue;
    //            //向所有注册到管理端的客户端发送委托信息
    //            TLSend(AccountBase.Series(account), MessageTypes.MGRREQACCOUNTSRESPONSE, c.Address);
    //            //TLSend(RaceInfo.Serialize(ri), MessageTypes.RACEINFORESPONSE, c.ClientID);
    //        }
    //    }
    //    //public void newRaceInfo(IRaceInfo ri)
    //    //{
    //    //    foreach (IClientInfo c in _clients.Clients)
    //    //    {
    //    //        if (!ViewAccountRight(c.Address, FindAccount(ri.AccountID))) continue;
    //    //        //向所有注册到管理端的客户端发送委托信息
    //    //        TLSend(RaceInfo.Serialize(ri), MessageTypes.RACEINFORESPONSE, c.Address);
    //    //    }
    //    //}
    //    /*
    //    public void newHealth(IHealthInfo h)
    //    {
    //        foreach (MgrClientInfo c in _clients.Clients)
    //        {
    //            if (c.CustomerType == QSEnumCustomerType.ADMIN)
    //            {
    //                //向所有注册到管理端的客户端发送委托信息
    //                TLSend(HealthInfo.Series(h), MessageTypes.MGRSRVHEALTH, c.ClientID);
    //            }
    //        }
    //    }**/
    //    /// <summary>
    //    /// 服务端向客户端发送委托回报
    //    /// </summary>
    //    /// <param name="o"></param>
    //    public void newOrder(Order o) 
    //    {
    //        if (!o.isValid)
    //        {
    //            debug("invalid order: " + o.ToString());
    //            return;
    //        }
    //        if (o.Account == null || o.Account == string.Empty) return;//如果委托不含有Account信息则直接返回
    //        foreach (IClientInfo c in _clients.Clients)
    //        {
    //            if (!ViewAccountRight(c.Address, FindAccount(o.Account))) continue;
    //            //向所有注册到管理端的客户端发送委托信息
    //            TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, c.Address);
    //        }
    //    }

    //    public void newOrderMessage(Order o, string msg)
    //    { 
        
    //    }

    //    public void newSysMessage(string msg, string clientid)
    //    {
            
    //    }
    //    public void newPosition(Position pos)
    //    {
            
    //    }


    //    /// <summary>
    //    /// 向所有管理端转发成交回报
    //    /// </summary>
    //    /// <param name="trade">The trade to include in the notification.</param>
    //    public void newFill(Trade trade)
    //    {
    //        if (!trade.isValid)
    //        {
    //            debug("invalid trade: " + trade.ToString());
    //            return;
    //        }
    //        if (trade.Account == null || trade.Account == string.Empty) return;
    //        foreach (IClientInfo c in _clients.Clients)
    //        {
    //            if (!ViewAccountRight(c.Address, FindAccount(trade.Account))) continue;
    //            //向所有注册到管理端的客户端发送委托信息
    //            TLSend(TradeImpl.Serialize(trade), MessageTypes.EXECUTENOTIFY, c.Address);
    //        }
    //    }

    //    /// <summary>
    //    /// 向所有管理端转发取消确认
    //    /// </summary>
    //    /// <param name="id"></param>
    //    public void newCancel(Order o)
    //    {
    //        foreach (MgrClientInfo c in _clients.Clients)
    //        {
    //            if (!ViewAccountRight(c.Address, FindAccount(o.Account))) continue;
    //            TLSend(o.id.ToString(), MessageTypes.ORDERCANCELRESPONSE, c.Address);
    //        }
    //    }
    //    /// <summary>
    //    /// 向服务端转发客户登入,注销事件
    //    /// </summary>
    //    /// <param name="accId"></param>
    //    /// <param name="online"></param>
    //    /// <param name="clientID"></param>
    //    public void newSessionUpdate(string accId, bool online, string address)
    //    {
    //        foreach (MgrClientInfo c in _clients.Clients)
    //        {
    //            IAccount a = FindAccount(accId);
    //            //debug("account:"+a.ID);
    //            bool r = ViewAccountRight(c.Address, a);
    //            //debug("权限检查结果:" + r.ToString());
    //            if (!r) continue;
    //            //TLSend(accId + "," + online.ToString() + "," + address, MessageTypes.MGRSESSIONUPDATE, c.ClientID);
    //            TLSend(accId + "," + online.ToString() + "," + address, MessageTypes.LOGINUPDATE, c.Address);
    //        }
    //    }

    //    #endregion


    //    #region Message消息处理逻辑

    //    public override bool onPermissionCheck(MessageTypes mt, string address)
    //    {
    //        //return false;
    //        try
    //        {
                
    //            MgrClientInfo mc=null;
    //            IClientInfo c = _clients[address];// as MgrClientInfo;
    //            if (c != null)
    //            {
    //                mc = c as MgrClientInfo;
    //                debug("customer info:" + c.Address + " loginID:" + c.AccountID + " isloggedin:" + c.IsLoggedIn.ToString());
    //            }
    //            bool ok = PermissionCheck(mt, mc);
    //            //debug("权限检查结果:"+ok.ToString());
    //            if (!ok)
    //            {
    //                debug("权限检查不通过 " + "Customer:" + (mc != null ? mc.Address : "New Customer") + " OP:" + mt.ToString());
    //                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.OPERATIONREJECT, "权限不够或者没有登入"), MessageTypes.SYSMESSAGE, address);

    //                return false;
    //            }
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            debug("权限检查异常: "+ex.ToString());
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// 处理母类所不处理的消息类型
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <param name="msg"></param>
    //    /// <param name="address"></param>
    //    /// <returns></returns>
    //    public override long handle(MessageTypes type, string msg,ISession session)
    //    {
    //        long result = NORETURNRESULT;
    //        switch (type)
    //        {
    //            case MessageTypes.REGISTERSTOCK:
    //                //debug(PROGRAME + "#Got REGISTERSTOCK From " + address + "  " + msg.ToString());
    //                string[] m2 = msg.Split('+');
    //                //SrvRegStocks(m2[0], m2[1]);
    //                break;
    //            case MessageTypes.CLEARSTOCKS:
    //                //debug(PROGRAME + "#Got CLEARSTOCKS From " + address + "  " + msg.ToString());
    //                //SrvClearStocks(msg);
    //                break;
    //            default:
    //                //将默认server没有实现的功能通过default路由到外层的event handler处理函数中去
                    
    //                if (newUnknownRequestSource != null)
    //                    result = newUnknownRequestSource(type, msg,session);
    //                else
    //                    result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
    //                break;
    //        }
    //        return result;
    //    }


    //    #endregion

    //    //如果将系统客户端链接信息持久缓存,我们需要将当前的注册端信息持久化到数据库,当服务端故障重新启动的时候,我们任然可以
    //    //正确的将消息发送到对应的客户端

    //    /// <summary>
    //    /// 
    //    /// </summary>
       

        




    //}
    ///// <summary>
    ///// 权限检查
    ///// </summary>
    //public class PermissionCheck
    //{
    //    public List<MessageTypes> _defaultmt=new List<MessageTypes>();
    //    public List<MessageTypes> _adminmt=new List<MessageTypes>();
    //    public List<MessageTypes> _stuffmt=new List<MessageTypes>();
    //    public List<MessageTypes> _custmt = new List<MessageTypes>();

    //    List<MessageTypes> _trademt = new List<MessageTypes>();//交易所需要用到的操作类型

    //    List<MessageTypes> _accmgtmt = new List<MessageTypes>();//账户管理需要用到的操作类型

    //    List<MessageTypes> _accriskmt = new List<MessageTypes>();//账户风控管理
    //    public PermissionCheck()
    //    {
    //        _defaultmt.Add(MessageTypes.REGISTERCLIENT);//注册客户端
    //        //_defaultmt.Add(MessageTypes.MGRCUSTOMERRLOGINREQ);//请求登入
    //        _defaultmt.Add(MessageTypes.CLEARCLIENT);//注销客户端
    //        _defaultmt.Add(MessageTypes.FEATUREREQUEST);//请求功能列表
    //        _defaultmt.Add(MessageTypes.VERSION);//请求版本
    //        _defaultmt.Add(MessageTypes.BROKERNAME);//请求服务端标识
    //        _defaultmt.Add(MessageTypes.HEARTBEATREQUEST);//请求心跳标识
    //        _defaultmt.Add(MessageTypes.HEARTBEAT);//请求心跳标识
    //        _defaultmt.Add(MessageTypes.LOGINREQUEST);//请求登入
    //        //_defaultmt.Add(MessageTypes.MGRREQACCOUNTS);//请求恢复监控或观察的账户数据

    //        //交易请求
    //        _trademt.Add(MessageTypes.SENDORDER);//发送委托
    //        _trademt.Add(MessageTypes.ORDERCANCELREQUEST);//发送取消委托

    //        //账户操作
    //        _accmgtmt.Add(MessageTypes.MGRACTIVEACCOUNT);//激活账户
    //        _accmgtmt.Add(MessageTypes.MGRINACTIVEACCOUNT);//禁止账户
    //        _accmgtmt.Add(MessageTypes.MGRUPDATEACCOUNTCATEGORY);//修改账户类型
    //        _accmgtmt.Add(MessageTypes.MGRUPDATEACCOUNTBUYMUPLITER);//修改账户购买乘数
    //        _accmgtmt.Add(MessageTypes.MGRUPDATEACCOUNTINTRADAY);//修改日内交易属性
    //        _accmgtmt.Add(MessageTypes.MGRCASHOPERATION);//账户出入金操作

    //        //风控
    //        _accriskmt.Add(MessageTypes.MGRCLEARORDERCHECK);
    //        _accriskmt.Add(MessageTypes.MGRADDORDERCHECK);
    //        _accriskmt.Add(MessageTypes.MGRDELORDERCHECK);
    //        _accriskmt.Add(MessageTypes.MGRCLEARACCOUNTCHECK);
    //        _accriskmt.Add(MessageTypes.MGRADDACCOUNTCHECK);
    //        _accriskmt.Add(MessageTypes.MGRDELACCOUNTCHECK);

            

            



    //    }

    //    public bool checkPermission(MessageTypes mt, MgrClientInfo c)
    //    {
    //        if (_defaultmt.Contains(mt)) return true;//默认功能列表的请求予以支持
    //        if (c == null || !c.IsLoggedIn) return false;//当客户端成功注册到服务器进行功能请求时,我们进行检查,如果C为空或则没有登入则直接拒绝功能操作
    //        switch (c.CustomerType)
    //        { 
    //            case QSEnumCustomerType.ADMIN:
    //                return true;
    //            case QSEnumCustomerType.VIEWER:
    //                return _custmt.Contains(mt);
    //            default:
    //                return false;
    //        }
    //    }

    //}
}
