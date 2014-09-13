//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;

//using TradingLib.API;

//using System.Threading;
//using System.Reflection;
//using System.Xml;
//using System.IO;
//using TradingLib.Common;
//using TradingLib.Contrib;


//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 管理服务
//    /// 线程1：负责将所有的消息按照次序 逐个向客户端进行发送,消息的发送有一定的先后次序，因此需要在逻辑部分建立该线程 不能放在底层消息发送组件内
//    /// 线程2：消息采集线程,除了客户端主动获得消息外,还有一部分消息是客户端主动推送的，按照一定时频采集消息然后放到缓存中，由线程1统一对外发送
//    /// </summary>
//    public class MgrExchServer : BaseSrvObject, IMessageMgr, ICore
//    {

//        const string CoreName = "MgrExchServer";

//        #region 对外触发相关事件
//        /// <summary>
//        /// 管理端触发的提交委托事件
//        /// </summary>
//        public event OrderDelegate SendOrderEvent;
//        /// <summary>
//        /// 管理端提交的取消委托事件
//        /// </summary>
//        public event LongDelegate SendOrderCancelEvent;
//        /// <summary>
//        /// 管理端获得比赛信息
//        /// </summary>
//        //public event GetRaceInfoDel GetRaceInfoEvent;
//        /// <summary>
//        /// 获得用户对应的个人信息
//        /// </summary>
//        //public event GetUserProfileDel GetUserProfileEvent;
//        /// <summary>
//        /// 获得比赛中心中所有的比赛状态数据
//        /// </summary>
//        public event GetRaceStatisticStrListDel GetRaceStatisticStrListEvent;

//        /// <summary>
//        /// 获得某个账户的配资服务信息
//        /// </summary>
//        //public event GetFinServiceInfoDel GetFinServiceInfoEvent;
//        /// <summary>
//        /// 激活配资服务
//        /// </summary>
//        public event AccountParamDel ActiveFinServiceEvent;
//        /// <summary>
//        /// 冻结配资服务
//        /// </summary>
//        public event AccountParamDel InActiveFinServiceEvent;
//        /// <summary>
//        /// 更新配资服务
//        /// </summary>
//        public event UpdateFinServiceDel UpdateFinServiceEvent;
//        #endregion


//        public TLServer_Moniter tl;
//        public IDebug MgrService { get { return tl; } }
//        Dictionary<string, Type> dicOrderRule;
//        Dictionary<string, Type> dicAccountRule;
//        Log _log = new Log("MgrServer", true, true, "d:\\QSLOG", true);//日志组件


//        public int CustomerNum { get { return tl.NumClients; } }
//        public int CustomerLoggedInNum { get { return tl.NumClientsLoggedIn; } }

//        IClearCentreSrv _clearcentre;
//        RiskCentre _riskcentre;
//        MsgExchServer _srv;
        
//        //ReportCentre _report;
//        //public void BindReport(ReportCentre r)
//        //{
//            //_report = r;
//        //}
       
//        //用于管理管理客户端地址与对应信息的映射
//        //管理终端注册到系统建立映射条目,管理终端注销时删除映射条目
//        ConcurrentDictionary<string, CustInfoEx> customerExInfoMap = null;

//        public string CoreId { get { return this.PROGRAME; } }

//        ConfigDB _cfgdb;


//        /// <summary>
//        /// 管理端内联了Mgr通讯组件,清算中心,风控中心,比赛中心,信息中心 通过与客户端的交互形成
//        /// 对服务端的操作
//        /// </summary>
//        /// <param name="tls"></param>
//        /// <param name="c"></param>
//        /// <param name="risk"></param>
//        /// <param name="rc"></param>
//        /// <param name="ic"></param>
//        /// <param name="dbserver"></param>
//        /// <param name="dbuser"></param>
//        /// <param name="dbpass"></param>
//        public MgrExchServer(MsgExchServer s, IClearCentreSrv c, RiskCentre risk)
//            : base(MgrExchServer.CoreName)
//        {

//            //1.加载配置文件
//            _cfgdb = new ConfigDB(MgrExchServer.CoreName);
//            if (!_cfgdb.HaveConfig("TLServerIP"))
//            {
//                _cfgdb.UpdateConfig("TLServerIP", QSEnumCfgType.String, "*", "TLServer_Moniter监听IP地址");
//            }
//            if (!_cfgdb.HaveConfig("TLPort"))
//            {
//                _cfgdb.UpdateConfig("TLPort", QSEnumCfgType.Int, 6670, "TLServer_Moniter监听Base端口");
//            }

//            customerExInfoMap =  new ConcurrentDictionary<string, CustInfoEx>();
//            TLServer_Moniter tls = new TLServer_Moniter(MgrExchServer.CoreName, _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt());

//            tls.NumWorkers = 1;
//            tls.EnableTPTracker = false;



//            //加载委托规则 与 账户规则
//            loadOrderRuleSet();
//            loadAccountRuleSet();

//            _srv = s;
//            _clearcentre = c;
//            _riskcentre = risk;

//            tl = tls;
//            //tl.TLMode = QSEnumTLMode.MgrSrv;
//            tl.ProviderName = Providers.QSManager;//set provider

//            tl.SendFindAccountEvent += new FindAccountDel(tl_SendFindAccountEvent);
//            tl.SendFindOrderEvent += new FindOrderDel(tl_SendFindOrderEvent);
//            tl.SendCacheMessage +=new CacheMessageDel(CacheMessage);

//            tl.newLoginRequest += new LoginRequestDel(tl_newLoginRequest);
//            tl.newCustomerSetRequest += new FindCustomerDataSet(tl_newCustomerSetRequest);
//            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
//            tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);//处理Order发送
//            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);//处理Order取消

//            //处理其他请求消息
//            tl.newUnknownRequestSource += new UnknownMessageDelegateSession(tl_newUnknownRequestSource);
//            tl.ClientRegistedEvent += new ClientParamDel(tl_ClientRegistedEvent);
//            tl.ClientUnRegistedEvent += new ClientParamDel(tl_ClientUnRegistedEvent);

//            //启动对外消息发送线程
            
//            //启动当前信息泵 按照不同的频率采集信息,主动向管理端推送消息
//            //StartInfoPump();
//            //向任务中心注册帐户实时采集
//            //TaskCentre.RegisterTask(new TaskProc("采集帐户信息", new TimeSpan(0, 0, 3), Task_CollectAccountInfo));

        
//        }

//        //终端从系统注销
//        void tl_ClientUnRegistedEvent(IClientInfo c)
//        {
//            debug("Client unregist to tlserver", QSEnumDebugLevel.INFO);
//            CustInfoEx o=null;
//            customerExInfoMap.TryRemove(c.Address, out o);
//        }
//        //终端注册到系统
//        void tl_ClientRegistedEvent(IClientInfo c)
//        {
//            debug("Client regist from tlserver", QSEnumDebugLevel.INFO);
//            customerExInfoMap[c.Address] = new CustInfoEx(c.Address);
//        }
//        bool _valid = false;

        

//        //启动服务
//        public void Start()
//        {
//            StartMessageOut();

//            debug("##########启动 Manager Server###################",QSEnumDebugLevel.INFO);
//            try
//            {
//                tl.Start();
//                (tl as TLServer_Moniter).LoadSessions();
//            }
//            catch (Exception ex)
//            {
//                _valid = false;
//                return;
//            }
//            _valid = true;
//            if (_valid)
//            {
//                debug("Trading Server Starting success");
//            }
//            else
//                debug("Trading Server Starting failed.");
//        }
//        public void Stop()
//        {
//            StopMessageRouter();

//            debug("#########停止 Manager Server ##########", QSEnumDebugLevel.INFO);
//            if (tl != null && tl.IsLive)
//            {
//                tl.Stop();
//            }
//            debug("Manger server stopped....");
//        }
//        /*
//        void tl_SendLoginInfoEvent(string loginID, bool isloggedin, string clientid)
//        {
//            throw new NotImplementedException();
//        }**/


//        public override void Dispose()
//        {

            

//            base.Dispose();
//            tl.Dispose();
            
        
        
//        }
//        #region 管理端-->TL_Server的请求操作

//        System.Data.DataSet tl_newCustomerSetRequest(string customer)
//        {
//            try
//            {
//                return null;// _clearcentre.getCustomer(customer);
//            }
//            catch (Exception ex)
//            {
//                debug("验证custoemr错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//            return null;
//        }
//        //管理端请求登入
//        bool tl_newLoginRequest(string loginid, string pass,ref ILoginResponse response)
//        {
//            //account = "";
//            try
//            {
//                return false;// _clearcentre.validCustomer(loginid, pass);
//            }
//            catch (Exception ex)
//            {
//                debug("验证custoemr错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//            return false;
//        }

//        /// <summary>
//        /// 通过委托id查询委托
//        /// </summary>
//        /// <param name="oid"></param>
//        /// <returns></returns>
//        Order tl_SendFindOrderEvent(long oid)
//        {
//            return _clearcentre.SentOrder(oid);
//        }
//        /// <summary>
//        /// 通过account查询账户
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        IAccount tl_SendFindAccountEvent(string account)
//        {
//            return _clearcentre[account];
//        }

//        /// <summary>
//        /// 管理服务的消息路由处理,不同的消息 按不同的处理逻辑新型处理
//        /// </summary>
//        /// <param name="t"></param>
//        /// <param name="msg"></param>
//        /// <param name="source"></param>
//        /// <returns></returns>
//        long tl_newUnknownRequestSource(MessageTypes t, string msg,ISession session)
//        {
//            string source = session.SessionID;
//            switch (t)
//            {
//                #region 监控账户类操作
//                //请求返回账户列表,返回该管理有权查看的所有账户列表
//                case MessageTypes.MGRREQACCOUNTS:
//                    {
//                        debug("管理服务器请求发送账户列表: " + msg,QSEnumDebugLevel.INFO);
//                        foreach (IAccount a in _clearcentre.Accounts)
//                        {
//                            if (!tl.ViewAccountRight(session.SessionID,a)) continue;//检查某个地址是否有权限查看某个账户
//                            _ascache.Write(new AccountSource(a, session.SessionID));
//                        }
//                        CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "获得帐户列表成功"), MessageTypes.SYSMESSAGE, session.SessionID);//发送客户端提示信息
//                    }
//                    return 0;
                

//                //请求设置推送账户列表,只有在列表中的账户才推送对应的财务实时数据
//                case MessageTypes.MGRWATCHACCOUNTS:
//                    {
//                        try
//                        {
//                            debug("管理服务器请求推送账户实时数据列表: " + msg,QSEnumDebugLevel.INFO);

//                            CustInfoEx c = customerExInfoMap[session.SessionID];
//                            if (c == null) return 0;
//                            c.Watch(msg);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "设定观察帐户列表成功"), MessageTypes.SYSMESSAGE, session.SessionID);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器请求推送账户实时数据列表出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                        return 0;
//                    }
                
//                //管理端请求回补某个账户交易信息操作
//                case MessageTypes.MGRRESUMEACCOUNT:
//                    {
//                        try
//                        {
//                            debug("管理服务器请求回补账户当日交易记录: " + msg, QSEnumDebugLevel.INFO);
//                            string account = msg;
//                            IAccount acc = _clearcentre[account];
//                            if (acc == null) return 0;//不存在对应的帐号信息
//                            //检查账户是否有权利查看该账户
//                            if (!tl.ViewAccountRight(session.SessionID, acc)) return 0;
//                            _resumecache.Write(new AccountSource(acc, session.SessionID));

//                            CustInfoEx c = customerExInfoMap[session.SessionID];
//                            if (c == null) return 0;
//                            c.Resume(account);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "设定回补账户成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器请求回补账户当日交易记录出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
                        
//                    }
//                    return 0;
//                #endregion

//                #region 增加或删除账户
//                //请求添加账户
//                case MessageTypes.MGRADDACCOUNT://添加账户操作
//                    {
//                        try
//                        {
//                            debug("管理服务器创建新的交易账户:" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(',');

//                            QSEnumAccountCategory type = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory),p[0]);
//                            string agentcode = "0000";
//                            if (p.Length >= 2)
//                            {
//                                agentcode = p[1];
//                            }
//                            string accid = null;
//                            if (type == QSEnumAccountCategory.DEALER)
//                            {
//                                //accid = _clearcentre.AddNewAccount("0", CoreGlobal.DefaultPass);
//                            }
//                            else if (type == QSEnumAccountCategory.LOANEE)
//                            {
//                                //accid = (_clearcentre as ClearCentre).AddNewFinAccount("0",null, agentcode);
//                            }

//                            if (accid == null) return -1;
//                            IAccount a = _clearcentre[accid];
//                            this.newAccountChanged(a);//帐户设置变动 通过该发送发送到有权利查看该帐户的所有管理端

//                            //if (tl.ViewAccountRight(source, a))
//                            //{
//                            //    _ascache.Write(new AccountSource(a, source));
//                            //    //CacheMessage(AccountBase.Series(a), MessageTypes.MGRREQACCOUNTSRESPONSE, source);
//                           // }
//                            //CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "添加账户成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器增加账户出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                #endregion

//                #region 修改交易账户性质
//                //请求激活某个账户
//                case MessageTypes.MGRACTIVEACCOUNT://激活账户操作
//                    {
//                        try
//                        {
//                            debug("管理服务器激活账户:" + msg, QSEnumDebugLevel.INFO);
//                            _clearcentre.ActiveAccount(msg);

//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "激活账户成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器激活账户出错:" + msg, QSEnumDebugLevel.INFO);
//                        }
//                    }
//                    return 0;
//                //请求禁止某个账户
//                case MessageTypes.MGRINACTIVEACCOUNT://禁止账户操作
//                    {
//                        try
//                        {
//                            _clearcentre.InactiveAccount(msg);
//                            debug("管理服务器禁止账户:" + msg, QSEnumDebugLevel.INFO);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "静止账户成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器冻结账户出错:" + msg, QSEnumDebugLevel.INFO);
//                        }
//                    }
//                    return 0;
//                //请求更新某个账户的类型 模拟 实盘
//                case MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE://更新账户类型操作
//                    {
//                        try
//                        {
//                            string[] p = msg.Split(':');
//                            string id = p[0];
//                            QSEnumOrderTransferType type = (QSEnumOrderTransferType)Enum.Parse(typeof(QSEnumOrderTransferType), p[1]);
//                            _clearcentre.UpdateAccountRouterTransferType(id, type);
//                            debug("管理服务器更新账户转发路由类型:" + id + " 类型为:" + type.ToString(), QSEnumDebugLevel.INFO);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "设定路由转发类别成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("修改账户路由类别出错" + msg, QSEnumDebugLevel.INFO);
//                        }
//                    }
//                    return 0;
//                //请求更新某个账户日内交易
//                case MessageTypes.MGRUPDATEACCOUNTINTRADAY:
//                    {
//                        try
//                        {
//                            debug("管理服务器修改账户日内属性:" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(':');
//                            string acc = p[0];
//                            bool intraday = Convert.ToBoolean(p[1]);
//                            _clearcentre.UpdateAccountIntradyType(acc, intraday);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "设定日内/隔夜成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("修改账户日内/隔夜交易出错:" + msg, QSEnumDebugLevel.INFO);
//                        }
//                    }   
//                    return 0;
//                //更新账户类别
//                case MessageTypes.MGRUPDATEACCOUNTCATEGORY:
//                    {
//                        try
//                        {
//                            debug("管理服务器修改账户类别属性:" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(':');
//                            string acc = p[0];
//                            QSEnumAccountCategory type = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), p[1]);
//                            _clearcentre.UpdateAccountCategory(acc,type);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "更新账户类别成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("修改日账户类别属性错误:" + ex.ToString());
//                        }

//                    }
//                    return 0;
//                /*
//                //更新账户购买乘数
//                case MessageTypes.MGRUPDATEACCOUNTBUYMUPLITER:
//                    {
//                        try
//                        {
//                            debug("管理服务器修改账户购买乘数:" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(':');
//                            string acc = p[0];
//                            int mp = Convert.ToInt16(p[1]);
//                            //QSEnumAccountCategory type = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), p[1]);
//                            _clearcentre.UpdateAccountBuyMultiplier(acc,mp);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "更新账户购买乘数成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("修改日账户类别属性错误:" + ex.ToString());
//                        }

//                    }
//                    return 0;**/
//                #endregion

//                #region 出入金操作
//                //请求出入金操作
//                case MessageTypes.MGRCASHOPERATION://出入金操作
//                    {
//                        try
//                        {
//                            string[] p = msg.Split(',');
//                            string acc = p[0];
//                            decimal ammount = decimal.Parse(p[1]);
//                            string comment = p[2];
//                            _clearcentre.CashOperation(acc, ammount, comment);
//                            debug("管理服务器为账户:" + acc + (ammount > 0 ? " 入金" : " 出金") + "  " + ammount.ToString() + "  " + comment.ToString(), QSEnumDebugLevel.INFO);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "出入金操作成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息

//                        }
//                        catch (Exception ex)
//                        {
//                            debug("出入金操作出错:" + msg, QSEnumDebugLevel.INFO);
//                        }
//                    }
//                    return 0;
//                //请求重置资金
//                case MessageTypes.MGRRESETEQUITY://重置资金
//                    {
//                        try
//                        {
//                            string[] p = msg.Split(':');
//                            string acc = p[0];
//                            decimal v = decimal.Parse(p[1]);
//                            debug("管理服务器重置账户:" + acc + " 权益至:" + v.ToString(), QSEnumDebugLevel.INFO);
//                            _clearcentre.ResetEquity(acc, v);
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("重置账户权益出错:" + msg, QSEnumDebugLevel.INFO);
//                        }

//                    }
//                    return 0;
//                #endregion

//                #region 账户风控规则
//                case MessageTypes.MGRCLEARORDERCHECK://清空委托检查操作
//                    {
//                        _clearcentre[msg].ClearOrderCheck();//缓存清除
//                        OrderCheckTracker.delRuleFromAccount(msg);//本地文本记录清除
//                        debug("管理服务器清空账户:" + msg + " 的委托检查", QSEnumDebugLevel.INFO);
//                    }
//                    return 0;
//                case MessageTypes.MGRADDORDERCHECK://添加委托检查操作
//                    {
//                        string[] p = msg.Split('+');
//                        string acc = p[0];
//                        string rcstr = p[1];
//                        string[] p1 = rcstr.Split(',');
//                        string rsname = p1[0];
//                        IOrderCheck rc = ((IOrderCheck)Activator.CreateInstance(dicOrderRule[rsname])).FromText(rcstr) as IOrderCheck;
//                        //将规则记录到文本
//                        OrderCheckTracker.addRuleIntoAccount(acc, rc);
//                        rc.Account = new AccountAdapterToExp(_clearcentre[acc]);
//                        //将规则添加到对应账户的规则集
//                        _clearcentre[acc].AddOrderCheck(rc);
//                        debug("管理服务器为账户:" + msg + " 添加委托检查 " + rc.RuleDescription, QSEnumDebugLevel.INFO);
                        
//                    }
//                    return 0;
//                case MessageTypes.MGRDELORDERCHECK://删除委托检查操作
//                    {
//                        string[] p = msg.Split('+');
//                        string acc = p[0];
//                        string rcstr = p[1];
//                        string[] p1 = rcstr.Split(',');
//                        string rsname = p1[0];
//                        IOrderCheck rc = ((IOrderCheck)Activator.CreateInstance(dicOrderRule[rsname])).FromText(rcstr) as IOrderCheck;
//                        //将规则从xml文件中删除
//                        OrderCheckTracker.delRuleFromAccount(acc, rc);
//                        _clearcentre[acc].DelOrderCheck(rc);
//                        debug("管理服务器为账户:" + msg + " 删除委托检查 " + rc.RuleDescription, QSEnumDebugLevel.INFO);
//                    }
//                    return 0;
//                case MessageTypes.MGRCLEARACCOUNTCHECK://清空账户检查操作
//                    {
//                        _clearcentre[msg].ClearAccountCheck();
//                        AccountCheckTracker.delRuleFromAccount(msg);
//                        debug("管理服务器清空账户:" + msg + " 的账户检查", QSEnumDebugLevel.INFO);
//                    }
//                    return 0;
//                case MessageTypes.MGRADDACCOUNTCHECK://添加账户检查操作
//                    {
//                        string[] p = msg.Split('+');
//                        string acc = p[0];
//                        string rcstr = p[1];
//                        string[] p1 = rcstr.Split(',');
//                        string rsname = p1[0];
//                        IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicAccountRule[rsname])).FromText(rcstr) as IAccountCheck;
//                        //将规则记录到文本
//                        AccountCheckTracker.addRuleIntoAccount(acc, rc);
//                        rc.Account = new AccountAdapterToExp(_clearcentre[acc]);
//                        //将规则添加到对应账户的规则集
//                        _clearcentre[acc].AddAccountCheck(rc);
//                        debug("管理服务器为账户:" + msg + " 添加账户检查 " + rc.RuleDescription, QSEnumDebugLevel.INFO);
                    
//                    }
//                    return 0;
//                case MessageTypes.MGRDELACCOUNTCHECK://删除账户检查操作
//                    {
//                        string[] p = msg.Split('+');
//                        string acc = p[0];
//                        string rcstr = p[1];
//                        string[] p1 = rcstr.Split(',');
//                        string rsname = p1[0];
//                        IAccountCheck rc = ((IAccountCheck)Activator.CreateInstance(dicAccountRule[rsname])).FromText(rcstr) as IAccountCheck;
//                        //将规则记录到文本
//                        AccountCheckTracker.delRuleFromAccount(acc,rc);
//                        rc.Account = new AccountAdapterToExp(_clearcentre[acc]);
//                        //将规则添加到对应账户的规则集
//                        _clearcentre[acc].DelAccountCheck(rc);
//                        debug("管理服务器为账户:" + msg + " 删除账户检查 " + rc.RuleDescription, QSEnumDebugLevel.INFO);
                    
//                    }
//                    return 0;
//                #endregion



//                //请求比赛信息
//                case MessageTypes.MGRRACESTATISTICREQ:
//                    {
//                        debug("管理服务器发送比赛统计信息", QSEnumDebugLevel.INFO);

//                        if (GetRaceStatisticStrListEvent != null)
//                        {
//                            foreach (string rstr in GetRaceStatisticStrListEvent())
//                            {
//                                CacheMessage(rstr, MessageTypes.MGRRACESTATISTICREP, source);
//                            }
//                        }
//                    }
//                    return 0;

//                #region 合约与手续费设置
//                //请求下载默认合约信息
//                case MessageTypes.MGRDOWNLOADDEFAULTSECFILE:
//                    {
//                        try
//                        {
//                            debug("管理端请求下载默认合约列表" + msg,QSEnumDebugLevel.INFO);
//                            //string so = XMLTransportHelper.XML2String(SecurityTracker.getXMLDoc());
//                            //将费率信息转发到对应的管理端
//                            //CacheMessage(GZip.Compress(so), MessageTypes.MGRDEFAULTSECFILEREP, source);
//                            //CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "下载全局合约成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求下载默认合约列表出错"+ex.ToString(),QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                //请求上传默认合约信息
//                case MessageTypes.MGRUPLOADDEFAULTSECFILE:
//                    {
//                        try
//                        {
//                            debug("管理端请求上传默认合约列表" + msg, QSEnumDebugLevel.INFO);

//                            string s = GZip.Uncompress(msg);
//                            XmlDocument doc = XMLTransportHelper.String2XML(s);
//                            string filename = @"config\Security.xml";
//                            //检查文件是否存在
//                            if (File.Exists(filename))
//                                File.Delete(filename);
//                            doc.Save(filename);
//                            //_clearcentre.LoadXMLTable();
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "上传全局合约成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求上传默认合约列表出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
                    
//                    }
//                    return 0;
//                //请求上传账户合约信息
//                case MessageTypes.MGRUPLOADCOMMISSIONTABLE:
//                    {
//                        try
//                        {
//                            debug("管理端上传合约信息" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split('@');
//                            string account = p[0];
//                            string s = GZip.Uncompress(p[1]);
//                            XmlDocument doc = XMLTransportHelper.String2XML(s);
//                            string filename = @"D:\QSConfiguration\Account\" + account + @"\security.xml";
//                            //检查目录是否存在
//                            if (!Directory.Exists(@"D:\QSConfiguration\Account\" + account))
//                                Directory.CreateDirectory(@"D:\QSConfiguration\Account\" + account);
//                            //检查文件是否存在
//                            if (File.Exists(filename))
//                                File.Delete(filename);
//                            doc.Save(filename);
//                            //更新该账户的合约与手续费
//                            //_clearcentre[account].UpdateMasterSecurity();
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "上传账户合约信息成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端上传合约信息错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                #endregion

//                #region 查询历史统计信息
//                //管理端请求下载某个交易账户的合约信息
//                case MessageTypes.MGRDOWNLOADCOMMISSIONTABLE:
//                    {
//                        try
//                        {
//                            debug("管理端请求下载账户合约" + msg);
//                            //获得该账户的xml信息
//                            //string so = XMLTransportHelper.XML2String(SecurityTracker.getXMLDoc(msg));
//                            //将费率信息转发到对应的管理端
//                            //CacheMessage(msg + "@" + GZip.Compress(so), MessageTypes.MGRACCOUNTCOMMISSIONREP, source);
//                            //CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "下载账户合约信息成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求下载账户合约错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                //请求每日账户汇总信息
//                case MessageTypes.REDAILYSUMMARYREQ:
//                    {
//                        try
//                        {
//                            debug("管理端查询账户每日汇总信息", QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(':');
//                            string account = p[0];
//                            //DateTime start = _clearcentre[account].RaceEntryTime;
//                            //if (p.Length >= 2)
//                                //start = DateTime.Parse(p[1]);

//                            //if (_report != null)
//                            //{
//                            //    DailySummaryList dl = _report.GenDailySummaryList(account, start);
//                            //    CacheMessage(dl.ToString(), MessageTypes.REDAILYSUMMARYREP, source);
//                            //}
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端查询账户每日汇总错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                #endregion

//                #region 查新账户的相关信息
//                //请求查询账户的profile信息
//                case MessageTypes.MGRREQACCOUNTPROFILE:
//                    {
//                        try
//                        {
//                            debug("管理端查询账户Profile信息:" + msg, QSEnumDebugLevel.INFO);
//                            string account = msg;
//                            int userid = (_clearcentre as ClearCentre).GetAccountUserID(account);
//                            if (userid <= 0) return 0;
//                            //debug("userid:" + userid.ToString(),QSEnumDebugLevel.MUST);
//                            //if(GetUserProfileEvent!=null)
//                            //{
//                            //    IProfile pf = GetUserProfileEvent(account, userid);

//                            //    //debug("get profile:" + Profile.Serialize(pf), QSEnumDebugLevel.INFO);

//                            //    //CacheMessage(Profile.Serialize(pf), MessageTypes.MGRREQACCOUNTPROFILERESPONSE, source);
//                            //}
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "查询注册信息成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求查询账户注册信息错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                //查询账户raceinfo信息
//                case MessageTypes.MGRREQRACEINFO:
//                    {
//                        try
//                        {
//                            debug("管理端查询账户Raceinfo信息:" + msg, QSEnumDebugLevel.INFO);
//                            string account = msg;
//                            //if (GetRaceInfoEvent != null)
//                            //{
//                            //    IRaceInfo ri = GetRaceInfoEvent(account);//_racecentre.getAccountRaceInfo(account);
//                            //    if (ri == null) return 0;
//                            //    string re = RaceInfo.Serialize(ri);
//                            //    debug("get raceinfo:" + re, QSEnumDebugLevel.INFO);
//                            //    CacheMessage(re, MessageTypes.RACEINFORESPONSE, source);
//                            //}
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "查询比赛信息成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求查询raceinfo错误:" + ex.ToString(),QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                //查询账户FinServiceInfo信息
//                case MessageTypes.MGRFINSERVICEREQ:
//                    {
//                        try
//                        {
//                            debug("管理端查询账户FinService信息:" + msg, QSEnumDebugLevel.INFO);
//                            string account = msg;
//                            //if (GetFinServiceInfoEvent != null)
//                            {
//                                //IFinServiceInfo fsinfo = GetFinServiceInfoEvent(account);//_racecentre.getAccountRaceInfo(account);
//                                //if (fsinfo == null) return 0;
//                                //string re = FinServiceInfo.Serialize(fsinfo);
//                                //debug("get finserviceinfo:" + re, QSEnumDebugLevel.INFO);
//                                //CacheMessage(re, MessageTypes.MGRFINSERVICEREP, source);
//                            }  
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "查询配资信息成功"), MessageTypes.SYSMESSAGE,source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理端请求查询info错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;
//                //查询账户财务数据
//                case MessageTypes.QRYACCOUNTINFO:
//                    {
//                        try
//                        {
//                            debug("管理服务器请求查询账户财务信息: " + msg, QSEnumDebugLevel.INFO);
//                            IAccount acc = _clearcentre[msg];
//                            if (acc == null) return 0;
//                            IAccountInfo a = ObjectInfoHelper.GenAccountInfo(acc);
//                            CacheMessage(AccountInfo.Serialize(a), MessageTypes.QRYACCOUNTINFORESPONSE, source);
//                            CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, "查询帐户财务信息成功"), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器请求查询账户财务信息出错 " + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                        return 0;
//                    }
//                case MessageTypes.MGRINSERTTRADE:
//                    {
//                        try
//                        {
//                            debug("管理服务器请求插入委托: " + msg, QSEnumDebugLevel.INFO);
//                            Trade fill = TradeImpl.Deserialize(msg);
//                            Order o = new MarketOrder(fill.symbol,fill.side,fill.UnsignedSize);
                            
//                            o.Account = fill.Account;
//                            o.date = fill.xdate;
//                            //委托比成交的时间早1秒
//                            o.time = Util.ToTLTime(Util.ToDateTime(fill.xdate, fill.xtime) - new TimeSpan(0, 0, 1));
//                            o.Status = QSEnumOrderStatus.Filled;
//                            o.Broker = fill.Broker;
//                            long ordid = _srv.futs_InsertOrderManual(o);
//                            fill.id = ordid;

//                            Thread.Sleep(100);
//                            _srv.futs_InsertTradeManual(fill);

                            
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("管理服务器请求插入委托 " + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                        return 0;
                        
//                    }
//                #endregion

//                #region 帐户配资操作
//                case MessageTypes.MGRINACTIVEFINSERVICE:
//                    {
//                        try
//                        {
//                            debug("管理服务器尝试冻结配资服务:" + msg, QSEnumDebugLevel.INFO);

//                            string remsg="";
//                            if (InActiveFinServiceEvent != null)
//                            {
//                                if (InActiveFinServiceEvent(msg, out remsg))
//                                {
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }
//                                else
//                                {
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }

                               
//                            }
//                            else
//                            {
//                                remsg = "服务端不支持该操作";
//                                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED,remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                            }
                            

//                        }
//                        catch (Exception ex)
//                        {
//                            debug("冻结配资服务错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;

//                case MessageTypes.MGRACTIVEFINSERVICE:
//                    {
//                        try
//                        {
//                            debug("管理服务器尝试激活配资服务:" + msg, QSEnumDebugLevel.INFO);

//                            string remsg="";
//                            if (ActiveFinServiceEvent != null)
//                            {
//                                if (ActiveFinServiceEvent(msg, out remsg))
//                                {
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }
//                                else
//                                {
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }

                                
//                            }
//                            else
//                            {
//                                remsg = "服务端不支持该操作";
//                                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED,remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                            }
                            

//                        }
//                        catch (Exception ex)
//                        {
//                            debug("激活配资服务错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;

//                case MessageTypes.MGRUPDATEFINSERVICE:
//                     {
//                        try
//                        {
//                            debug("管理服务器尝试更新配资服务:" + msg, QSEnumDebugLevel.INFO);
//                            string[] p = msg.Split(',');
//                            string account = p[0];

//                            decimal amount = 0;
//                            decimal.TryParse(p[1], out amount);

//                            QSEnumFinServiceType type = (QSEnumFinServiceType)Enum.Parse(typeof(QSEnumFinServiceType), p[2]);
                            
//                            decimal discount =1;
//                            decimal.TryParse(p[3], out discount);

//                            int agentcode =0000;
//                            int.TryParse(p[4], out agentcode);

//                            string remsg="";
//                            if (UpdateFinServiceEvent != null)
//                            {
//                                string o = string.Empty;
//                                if (UpdateFinServiceEvent(account, amount, type, discount, agentcode,out remsg))
//                                {
//                                    remsg = "更新服务成功";
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONSUCCESS, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }
//                                else
//                                {
//                                    remsg =  "更新服务失败" + o;
//                                    CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED, remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                                }
//                            }
//                            else
//                            {
//                                remsg = "服务端不支持该操作";
//                                CacheMessage(QSMessageHelper.Message(QSEnumSysMessageType.MGROPERATIONFAILED,remsg), MessageTypes.SYSMESSAGE, source);//发送客户端提示信息
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            debug("更新配资服务错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                        }
//                    }
//                    return 0;

//                #endregion
//                default:
//                    return 0;
//            }

//        }


//        #region 管理端发送上来的 委托与取消
//        /// <summary>
//        /// 取消委托请求
//        /// </summary>
//        /// <param name="val"></param>
//        void tl_newOrderCancelRequest(long val)
//        {
//            try
//            {
//                if (SendOrderCancelEvent != null)
//                    SendOrderCancelEvent(val);
//            }
//            catch (Exception ex)
//            {
//                debug("send order error" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /// <summary>
//        /// 委托报单请求
//        /// </summary>
//        /// <param name="o"></param>
//        void tl_newSendOrderRequest(Order o)
//        {
//            try
//            {
//                if (SendOrderEvent != null)
//                    SendOrderEvent(o);
//            }
//            catch (Exception ex)
//            {
//                debug("send order error" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        #endregion


//        /// <summary>
//        /// 请求功能列表
//        /// </summary>
//        /// <returns></returns>
//        MessageTypes[] tl_newFeatureRequest()
//        {
//            List<MessageTypes> f = new List<MessageTypes>();
//            f.Add(MessageTypes.REGISTERSTOCK);
//            f.Add(MessageTypes.TICKNOTIFY);
//            f.Add(MessageTypes.LIVEDATA);
//            f.Add(MessageTypes.BARRESPONSE);
//            f.Add(MessageTypes.BARREQUEST);
//            f.Add(MessageTypes.SIMTRADING);
//            f.Add(MessageTypes.SENDORDER);
//            f.Add(MessageTypes.SENDORDERLIMIT);
//            f.Add(MessageTypes.SENDORDERMARKET);
//            f.Add(MessageTypes.SENDORDERSTOP);
//            f.Add(MessageTypes.ORDERNOTIFY);
//            f.Add(MessageTypes.ORDERCANCELREQUEST);
//            f.Add(MessageTypes.ORDERCANCELRESPONSE);
//            f.Add(MessageTypes.EXECUTENOTIFY);

//            return f.ToArray();
//        }
//        #endregion

//        #region 账户实时财务状态推送线程,定时的将管理端观察的账户数据生成 实时财务数据 写入 消息缓存，由消息缓存对外发送


//        [TaskAttr("采集帐户信息",3,"定时采集帐户信息用于向管理端进行推送")]
//        public void Task_CollectAccountInfo()
//        {
//            try
//            {
//                foreach (CustInfoEx cst in customerExInfoMap.Values)
//                {
//                    //便利所有订阅账户列表
//                    foreach (string account in cst.WatchAccounts)
//                    {
//                        IAccount acc = _clearcentre[account];
//                        if (acc == null) continue;

//                        _liteinfocache.Write(new AccountInfoLiteSource(ObjectInfoHelper.GenAccountInfoLite(acc), cst.Source));

//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                debug("帐户信息采集出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }
//        /*
//        bool infogo = false;
//        Thread infothread = null;

//        void StartInfoPump()
//        {
//            if (infogo) return;
//            infogo = true;
//            infothread = new Thread(infoproc);
//            infothread.IsBackground = true;
//            infothread.Name = "MGR Info collection Thread";
//            infothread.Start();
//            ThreadTracker.Register(infothread);
//        }

//        void StopInfoPump()
//        {
//            if (!infogo) return;
//            infogo = false;
//            infothread.Abort();
//            infothread = null;
//        }

//        void collectAccountInfo()
//        {
//            foreach (CustInfoEx cst in customerExInfoMap.Values)
//            {
//                //便利所有订阅账户列表
//                foreach (string account in cst.WatchAccounts)
//                {
//                    IAccount acc = _clearcentre[account];
//                    if (acc == null) continue;

//                    _liteinfocache.Write(new AccountInfoLiteSource(ObjectInfoHelper.GenAccountInfoLite(acc), cst.Source));

//                }
//            }
//        }
       
//        void infoproc()
//        {
//            while (infogo)
//            {
//                try
//                {
//                    //采集账户的当前交易状态信息
//                    collectAccountInfo();
                    
//                }
//                catch (Exception ex)
//                {
//                    debug("账户列表财务状态生成出错" + ex.ToString(), QSEnumDebugLevel.ERROR);
//                }
//                Thread.Sleep(2000);
//            }
//        }
//        **/
//        #endregion

//        #region 消息发送总线 向管理端发送对应的消息
//        /// <summary>
//        /// 消息对外发送线程,由于ZeroMQ并不是线程安全的，因此我们不能运行多个线程去操作一个socket,当我们对外发送的时候统一调用了
//        /// TLSend _trans.Send(data, clientID)
//        /// 1.TradingServer形成的交易信息,我们需要通过MgrServer转发到对应的客户端TradingSrv->MgrServer->TLSend
//        /// 2.客户端注册操作 以及回补相关账户交易信息时，也会形成MgrServer对外的信息发送Client->MgServer->TLSend
//        /// 这样Mgr其实有多个线程通过MgrServer对外发送信息,因此我们需要在这里建立信息缓存，将多个线程需要转发的消息一并放入
//        /// 对应的缓存中，然后由单独的线程不间断的将这些缓存的信息统一发送出去
//        /// </summary>
//        void StartMessageOut()
//        {

//            if (_readgo) return;
//            _readgo = true;
//            messageoutthread = new Thread(messageout);
//            messageoutthread.IsBackground = true;
//            messageoutthread.Name = "MGR Server Message SendOut Thread";
//            messageoutthread.Start();
//            ThreadTracker.Register(messageoutthread);

//        }

//        void StopMessageRouter()
//        {
            
//            if (!_readgo) return;
//            ThreadTracker.Unregister(messageoutthread);
//            _readgo = false;
//            int mainwait = 0;
//            while (messageoutthread.IsAlive && mainwait < 10)
//            {
//                Thread.Sleep(1000);
//                mainwait++;
//            }
//            messageoutthread.Abort();
//            messageoutthread = null;
//        }
//        const int buffize = 5000;
//        RingBuffer<Order> _ocache = new RingBuffer<Order>(buffize);//委托缓存
//        RingBuffer<long> _ccache = new RingBuffer<long>(buffize);//取消缓存
//        RingBuffer<Trade> _fcache = new RingBuffer<Trade>(buffize);//成交缓存

//        //RingBuffer<IHealthInfo> _healthcache = new RingBuffer<IHealthInfo>(buffize);//服务器状态信息缓存
//        RingBuffer<SessionInfo> _sessioncache = new RingBuffer<SessionInfo>(buffize);//客户端登入 登出缓存
//        RingBuffer<IAccount> _accountchangecache = new RingBuffer<IAccount>(buffize);//帐户变动缓存
//        //RingBuffer<IRaceInfo> _raceinfocache = new RingBuffer<IRaceInfo>(buffize);//客户端比赛信息缓存

//        //管理端请求账户列表,服务端将该管理所管理的账户一起缓存到_ascache然后统一对外发送 管理端收到账户信息然后再向服务端进行请求交易信息
//        RingBuffer<AccountSource> _ascache = new RingBuffer<AccountSource>(buffize);

//        //管理端在需要查看某个账户的交易记录时,通过请求恢复该交易账户交易信息完成
//        RingBuffer<AccountSource> _resumecache = new RingBuffer<AccountSource>(buffize);

//        //管理端订阅账户动态更新信息缓存 
//        RingBuffer<AccountInfoLiteSource> _liteinfocache = new RingBuffer<AccountInfoLiteSource>(buffize);


//        RingBuffer<TrdMessage> _msgcache = new RingBuffer<TrdMessage>(buffize);//其他类别的消息放入到该缓存

//        void CacheMessage(string msg, MessageTypes type, string address)
//        {
//            _msgcache.Write(new TrdMessage(msg, type, address));
//        }

//        //关于交易信息转发,交易信息转发时,我们需要区分是实时发生的交易信息转发还是请求回补的信息转发。
//        //实时交易信息通过客户端权限检查自动将交易信息发送到所有有权,而回补信息则是针对不同的管理端进行的回补请求,若统一由tl.neworder转发会造成不同的管理端之间信息重复接收
//        bool _readgo = false;
//        Thread messageoutthread;

//        /// <summary>
//        /// 当没有任何回补信息的时候 我们才可以发送实时信息
//        /// </summary>
//        /// <returns></returns>
//        bool noresumeinfo()
//        {
//            return !_ascache.hasItems && !_resumecache.hasItems;//!_oscache.hasItems && !_tscache.hasItems && !_cscache.hasItems;
//        }
//        bool noaccountresumeinfo()
//        {
//            return !_ascache.hasItems;
//        }
//        /// <summary>
//        /// 所有需要转发到客户端的消息均通过缓存进行，这样避免了多个线程同时操作一个ZeroMQ socket
//        /// 
//        /// </summary>
//        void messageout()
//        {
//            while (_readgo)
//            {
//                try
//                {
//                    #region 发送交易账户列表 以及 回补某个交易账户的交易信息
//                    //发送账户列表信息
//                    while (_ascache.hasItems)
//                    { 
//                        AccountSource a = _ascache.Read();
//                        string msg = AccountBase.Series(a.Account);
//                        debug("send account:" + msg, QSEnumDebugLevel.MUST);
//                        tl.TLSend(msg, MessageTypes.MGRREQACCOUNTSRESPONSE,a.Source);
//                        //这里需要知道账户是否登入 如果登入需要发送登入信息
//                        //如果登入则转发登入信息,
//                        ClientTrackerInfo info=null;
//                        if (_riskcentre.Islogin(a.Account.ID,out info))
//                        { 
//                            _sessioncache.Write(new SessionInfo(a.Account.ID, true,info.IPAddress));
//                        }
//                        Thread.Sleep(10);
//                    }
//                    //发送账户交易信息回补
//                    while (_resumecache.hasItems)
//                    {
//                        AccountSource a = _resumecache.Read();
//                        string account = a.Account.ID;
//                        //昨日持仓数据
//                        foreach (Position pos in _clearcentre.getPositionHold(account))
//                        {
//                            tl.TLSend(PositionImpl.Serialize(pos), MessageTypes.POSITIONRESPONSE, a.Source);
//                        }
//                        //今日委托数据
//                        foreach (Order o in _clearcentre.getOrders(account))
//                        {
//                            tl.TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, a.Source);
//                        }
//                        //今日成交数据
//                        foreach (Trade f in _clearcentre.getTrades(account))
//                        {
//                            tl.TLSend(TradeImpl.Serialize(f), MessageTypes.EXECUTENOTIFY, a.Source);
//                        }
//                        //今日取消数据
//                        foreach (long l in _clearcentre.getCancels(account))
//                        {
//                            tl.TLSend(l.ToString(), MessageTypes.ORDERCANCELRESPONSE, a.Source);
//                        }
//                        //告知管理端 回补结束
//                        tl.TLSend(" ", MessageTypes.RESUMEFINISH,a.Source);
//                    }
//                    #endregion

//                    #region 转发管理端选中账户的交易信息
//                    //转发委托
//                    while (_ocache.hasItems && noresumeinfo())
//                    {
//                        Order o = _ocache.Read();
//                        foreach (CustInfoEx cst in customerExInfoMap.Values)
//                        {
//                            if(cst.NeedPushTradingInfo(o.Account))
//                                tl.TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, cst.Source);
//                        }
                        
//                    }
//                    //转发成交
//                    while (_fcache.hasItems && !_ocache.hasItems && noresumeinfo())
//                    {
//                        Trade f = _fcache.Read();
//                        foreach (CustInfoEx cst in customerExInfoMap.Values)
//                        {
//                            if (cst.NeedPushTradingInfo(f.Account))
//                                tl.TLSend(TradeImpl.Serialize(f), MessageTypes.EXECUTENOTIFY, cst.Source);
//                        }
                      
//                    }
//                    //转发取消
//                    while (_ccache.hasItems && !_ocache.hasItems && noresumeinfo())
//                    {
//                        Order o = _clearcentre.SentOrder(_ccache.Read());
//                        foreach (CustInfoEx cst in customerExInfoMap.Values)
//                        {
//                            if (cst.NeedPushTradingInfo(o.Account))
//                                tl.TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, cst.Source);
//                        }
//                    }
//                    #endregion


//                    //发送账户实时财务信息(当日交易状态)
//                    while (_liteinfocache.hasItems)
//                    {
//                        AccountInfoLiteSource s = _liteinfocache.Read();
//                        tl.TLSend(AccountInfoLite.Serialize(s.AccInfo),MessageTypes.MGRACCOUNTINFOLITE,s.Source);
//                        Thread.Sleep(20);
//                    }
//                    //发送服务器状态信息
//                    //while (_healthcache.hasItems)
//                    //{
//                    //    tl.newHealth(_healthcache.Read());
//                    //}
//                    //发送交易客户端登入 注销信息
//                    while (_sessioncache.hasItems && noresumeinfo())
//                    { 
//                        SessionInfo s = _sessioncache.Read();
//                        tl.newSessionUpdate(s.Account, s.LoggedIn, s.IPAddress);
//                    }
//                    while (_accountchangecache.hasItems && noresumeinfo())
//                    {
//                        tl.newAccountSettingChanged(_accountchangecache.Read());
//                    }
//                    //发送比赛信息
//                    //while (_raceinfocache.hasItems && noresumeinfo())
//                    //{
//                    //    IRaceInfo ri = _raceinfocache.Read();
//                    //    tl.newRaceInfo(ri);
//                    //}
//                    //发送其他类型的信息
//                    while (_msgcache.hasItems)
//                    {
//                        TrdMessage m = _msgcache.Read();
//                        debug("发送消息: 类型:" + m.Type.ToString() + " 发送消息:" + m.Message + " clientID:" + m.ClientID,QSEnumDebugLevel.MUST);
//                        tl.TLSend(m.Message, m.Type, m.ClientID);
//                    }
//                    Thread.Sleep(100);

//                }
//                catch (Exception ex)
//                {
//                    debug(ex.ToString());
//                }
//            }
            
//        }


//        #region 转发委托 成交 取消(交易信息)以及其他相关消息到管理客户端
//        /// <summary>
//        /// MgrServer转发Tick数据
//        /// </summary>
//        /// <param name="k"></param>
//        public void newTick(Tick k)
//        {
//            //直接通过服务端的Tick publisher转发Tick数据
//            tl.newTick(k);

//        }
//        /// <summary>
//        /// MgrServer转发委托信息
//        /// </summary>
//        /// <param name="o"></param>
//        public void newOrder(Order o)
//        {
//            debug("Management转发委托:" + o.ToString(), QSEnumDebugLevel.INFO);
//            _ocache.Write(new OrderImpl(o));
//        }
//        /// <summary>
//        /// MgrServer转发取消信息
//        /// </summary>
//        /// <param name="oid"></param>
//        public void newCancel(long oid)
//        {
//            debug("Management转发取消:" + oid.ToString(), QSEnumDebugLevel.INFO);
//            _ccache.Write(oid);
//            //tl.newCancel()
//        }
//        /// <summary>
//        /// MgrServer转发成交信息
//        /// </summary>
//        /// <param name="f"></param>
//        public void newTrade(Trade f)
//        {
//            debug("Management转发成交:" + f.ToString(), QSEnumDebugLevel.INFO);
//            _fcache.Write(new TradeImpl(f));

//        }
//        /// <summary>
//        /// MgrServer转发服务器状态信息
//        /// </summary>
//        /// <param name="h"></param>
//        //public void newHealth(IHealthInfo h)
//        //{
//            //debug("Management发送服务器状态:");
//        //    _healthcache.Write(h);
//        //}

//        /// <summary>
//        /// MgrServer转发交易客户端登入注销信息
//        /// 这里主要是发送客户端的地址信息
//        /// </summary>
//        /// <param name="accId"></param>
//        /// <param name="online"></param>
//        /// <param name="clientID"></param>
//        public void newSessionUpdate(string accid, bool online, IClientInfo info)
//        {
//            debug("Management转发登入:" + accid + " @" + info.Address + "|" + info.IPAddress + " " + (online ? "登入" : "注销"), QSEnumDebugLevel.INFO);
//            _sessioncache.Write(new SessionInfo(accid,online,info.IPAddress));
//        }
//        /// <summary>
//        /// MgrServer转发客户端比赛信息
//        /// </summary>
//        /// <param name="acc"></param>
//        public void newRaceInfo(IAccount acc)
//        {
//            debug("Management转发比赛信息:" + acc.ID, QSEnumDebugLevel.INFO);
//            //if (GetRaceInfoEvent != null)
//            //{
//            //    IRaceInfo ri = GetRaceInfoEvent(acc.ID);
//            //    if (ri == null) return;
//            //    _raceinfocache.Write(ri);
//            //}
//        }

//        /// <summary>
//        /// MgrServer转发交易账户设置变动更新
//        /// </summary>
//        /// <param name="acc"></param>
//        public void newAccountChanged(IAccount acc)
//        {
//            debug("Management转发帐户变动信息:" + acc.ID, QSEnumDebugLevel.INFO);
//            {
//                _accountchangecache.Write(acc);
//            }
//        }


//        #endregion

//        #endregion




//        #region 加载委托检查以及账户检查规则集
//        //加载风控规则
//        private void loadOrderRuleSet()
//        {
//            dicOrderRule = new Dictionary<string, Type>();
//            foreach (Type t in PluginHelper.LoadOrderRule())
//            {
//                //得到RuleSet的名称与描述
//                string rsname = (string)t.InvokeMember("Name",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                string rsdescription = (string)t.InvokeMember("Description",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                dicOrderRule.Add(t.FullName, t);
//            }
//        }

//        //加载账户规则模板
//        private void loadAccountRuleSet()
//        {
//            dicAccountRule = new Dictionary<string, Type>();
//            foreach (Type t in PluginHelper.LoadAccountRule())
//            {
//                //得到RuleSet的名称与描述
//                string rsname = (string)t.InvokeMember("Name",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                string rsdescription = (string)t.InvokeMember("Description",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                dicAccountRule.Add(t.FullName, t);
//            }
//        }
//        #endregion

//    }

///*关于管理端改进的构思
// * 原来的设计是统一获取账户信息,然后将这些账户所有的交易信息传输到管理端
// * 账户数目少时,运行没有问题。当账户数目上升后，这个方式就有弊端，每次连接需要传输当日所有交易数据，造成数据容易由于各种原因
// * 缺失。
// * 
// * 1.只传输账户信息，不集中传输交易信息
// * 2.账户监控列表 按监控的账户集合 统一向服务端订阅 账户实时信息，那么服务端记录该列表(20个)，服务端不间断的计算该列表内的账户
// * 最新信息然后发送到管理端。
// * 3.当管理端需要查看某个具体账户的交易记录时,双击该账户 则向服务端请求恢复该交易账户的当日交易记录
// * 
// * 
// * 服务端与客户端之间的账户类消息
// * 1.账户主体消息AccountBase，用从服务端获得账户基本信息,然后生成对应的Account实体
// * 2.RaceInfo 比赛信息，传递了账户比赛相关的信息,用于查看账户当前比赛状态,晋级 淘汰 差额等
// * 3.sessionInof 账户登入 注销 信息,用于动态的更新当前账户的连接信息
// * 4.AccountInfo 在进行账户查询时候 传递的账户消息,用于提供账户全面的财务信息
// * 5.
// * 
// * 
// * 
// * */
//    //定义了管理端请求数据集合
//    public class CustInfoEx
//    {

//        //管理端ID
//        public string Source;
//        //管理端的当前观察账户列表,保存了需要向管理端推送当前动态信息的账户列表
//        public ThreadSafeList<string> WatchAccounts=new ThreadSafeList<string>();
//        //保存了管理端当前需要推送实时交易信息的帐号,任何时刻管理端只接受若干个账户财务信息更新，以及某个账户的交易记录
//        string _selectacc = string.Empty;
//        public string SelectedAccount{get{return _selectacc;} set{_selectacc = value;}}

//        public CustInfoEx(string source)
//        {
//            Source = source;
//        }
//        /// <summary>
//        /// 当前状态是否接受某个账户的交易信息,管理端会选中某个交易帐号进行查看,则服务端只会讲该账户的交易信息推送到管理端
//        /// </summary>
//        /// <param name="accound"></param>
//        /// <returns></returns>
//        public bool NeedPushTradingInfo(string account)
//        { 
//            //如果提供的帐号 或者 设定当前选择的帐号为空或null 则不推送该交易信息
//            if(string.IsNullOrEmpty(account) || string.IsNullOrEmpty(SelectedAccount)) return false;
//            //选中的帐号与我们当前比较的帐号 相同,则我们推送该信息
//            if (account.Equals(this.SelectedAccount)) return true;

//            return false;
        
//        }
//        //账户观察列表是用,分割的一个字符串
//        /// <summary>
//        /// 观察一个账户列表,用于推送实时的权益数据
//        /// </summary>
//        /// <param name="msg"></param>
//        public void Watch(string msg)
//        {
//            WatchAccounts.Clear();
//            string[] accounts = msg.Split(',');

//            foreach (string account in accounts)
//            {
//                WatchAccounts.Add(account);
//            }
           
//        }
//        /// <summary>
//        /// 选中某个账户 用于回补该账户的交易记录
//        /// </summary>
//        /// <param name="account"></param>
//        public void Resume(string account)
//        {
//            _selectacc = account;
        
//        }

//    }

    
//    /// <summary>
//    /// 记录是哪个管理端请求了该账户
//    /// </summary>
//    public struct AccountSource
//    {
//        public IAccount Account;
//        public string Source;
//        public AccountSource(IAccount acc, string source)
//        {
//            Account = acc;
//            Source = source;
//        }
//    }

//    public struct AccountInfoLiteSource
//    {
//        public IAccountInfoLite AccInfo;
//        public string Source;
//        public AccountInfoLiteSource(IAccountInfoLite info, string source)
//        {
//            AccInfo = info;
//            Source = source;
//        }

//    }

//}
