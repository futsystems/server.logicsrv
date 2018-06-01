using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class MgrExchServer : BaseSrvObject, IModuleMgrExchange
    {
        const string CoreName = "MgrExchServer";
        public string CoreId { get { return this.PROGRAME; } }


        TLServer_MgrExch tl;
        ConfigDB _cfgdb;

        ConcurrentDictionary<string, MgrClientInfoEx> customerExInfoMap = new ConcurrentDictionary<string, MgrClientInfoEx>();

        public MgrExchServer()
            : base(MgrExchServer.CoreName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(MgrExchServer.CoreName);
            if (!_cfgdb.HaveConfig("TLServerIP"))
            {
                _cfgdb.UpdateConfig("TLServerIP", QSEnumCfgType.String, "*", "TLServer_Moniter监听IP地址");
            }
            if (!_cfgdb.HaveConfig("TLPort"))
            {
                _cfgdb.UpdateConfig("TLPort", QSEnumCfgType.Int, 6670, "TLServer_Moniter监听Base端口");
            }

            tl = new TLServer_MgrExch(_cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), false);
            tl.NumWorkers = 1;

            tl.CachePacketEvent += new IPacketDelegate(CachePacket);
            tl.NewPacketRequest += new Action<ISession, IPacket, Manager>(OnPacketRequest);
            tl.QryNotifyLocationsViaAccount += new Func<string, ILocation[]>(QryNotifyLocationsViaAccount);

            tl.ClientRegistedEvent += new Action<MgrClientInfo>(OnClientRegistedEvent);
            tl.ClientUnregistedEvent += new Action<MgrClientInfo>(OnClientUnregistedEvent);


            //初始化通知
            InitNotifySection();

            //启动消息服务
            StartMessageRouter();

            //订阅交易信息
            TLCtxHelper.EventIndicator.GotTickEvent += new TickDelegate(this.OnTick);
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(this.OnTrade);
            TLCtxHelper.EventIndicator.GotOrderEvent += new OrderDelegate(this.OnOrder);
            TLCtxHelper.EventIndicator.GotOrderErrorEvent += new OrderErrorDelegate(this.OnOrderError);

            //订阅帐户变动信息
            TLCtxHelper.EventAccount.AccountChangeEvent += new Action<IAccount>(this.OnAccountChanged);
            TLCtxHelper.EventAccount.AccountAddEvent += new Action<IAccount>(this.OnAccountAdded);
            TLCtxHelper.EventAccount.AccountDelEvent += new Action<IAccount>(this.OnAccountDeleted);

            TLCtxHelper.EventSession.ClientSessionEvent += new Action<TrdClientInfo, bool>(OnSessionEvent);  


        }

        MgrClientInfoEx GetCustInfoEx(ISession session)
        {
            MgrClientInfoEx target = null;
            if (customerExInfoMap.TryGetValue(session.Location.ClientID,out target))
            {
                return target;
            }
            return null;
        }


        void OnClientUnregistedEvent(MgrClientInfo client)
        {
            //logger.Info("unregisted");
            MgrClientInfoEx o = null;
            customerExInfoMap.TryRemove(client.Location.ClientID, out o);
        }

        void OnClientRegistedEvent(MgrClientInfo client)
        {
            //logger.Info("registed");
            customerExInfoMap[client.Location.ClientID] = new MgrClientInfoEx(client);
        }

        /// <summary>
        /// 查询具有查看某个帐户account权限的Manager地址
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        ILocation[] QryNotifyLocationsViaAccount(string account)
        {
            return customerExInfoMap.Values.Where(ex => ex.RightAccessAccount(account)).Select(ex2 => ex2.Location).ToArray();
        }


        /// <summary>
        /// 查看Root权限的管理段地址
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        ILocation[] QryNotifyLocationsForRoot()
        {
            return customerExInfoMap.Values.Where(ex=>ex.Manager!=null).Where(ex => ex.Manager.IsRoot()).Select(ex2 => ex2.Location).ToArray();
        }






        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            tl.Dispose();
            tl = null;

            StopMessageRouter();
        }






        bool _valid = false;
        public void Start()
        {
            //StartMessageOut();
            Util.StartStatus(this.PROGRAME);
            //debug("##########启动 Manager Server###################", QSEnumDebugLevel.INFO);
            try
            {
                tl.Start();
            }
            catch (Exception ex)
            {
                _valid = false;
                return;
            }
            _valid = true;
            if (_valid)
            {
                logger.Info("Trading Server Starting success");
            }
            else
                logger.Info("Trading Server Starting failed.");
        }
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            if (tl != null && tl.IsLive)
            {
                tl.Stop();
            }
            //_pusherSrv.Stop();
            logger.Info("Manger server stopped....");
        }




    }



    /// <summary>
    /// 维护管理端对象相关数据
    /// 1.记录管理端观察账户列表
    /// 2.记录管理端选中账户
    /// </summary>
    public class MgrClientInfoEx
    {

        /// <summary>
        /// 管理端当前位置
        /// </summary>
        public ILocation Location { get { return _clientInfo.Location; } }

        /// <summary>
        /// 对应的Manager对象 如果管理端没有登入 则Manager为空
        /// </summary>
        public Manager Manager { get { return _clientInfo.Manager; } }


        /// <summary>
        /// 管理端的当前观察账户列表,保存了需要向管理端推送当前动态信息的账户列表
        /// </summary>
        ThreadSafeList<IAccount> WatchAccounts = new ThreadSafeList<IAccount>();

        public IEnumerable<IAccount> WathAccountList { get { return this.WatchAccounts; } }

        ThreadSafeList<IAgent> watchAgents = new ThreadSafeList<IAgent>();
        public IEnumerable<IAgent> WatchAgentList { get { return watchAgents; } }


        /// <summary>
        /// 保存了管理端当前需要推送实时交易信息的帐号,任何时刻管理端只接受若干个账户财务信息更新，以及某个账户的交易记录
        /// </summary>
        public IAccount AccountSelected { get; private set; }

        MgrClientInfo _clientInfo;
        public MgrClientInfoEx(MgrClientInfo clientInfo)
        {
            _clientInfo = clientInfo;
            this.AccountSelected = null;
        }

        /// <summary>
        /// 该client cutinfoex是否有权限访问帐户
        /// 判定管理员是否有权操作某账户时 如果以账户ID作为参数
        /// 当账户删除事件中 通过TLCtxHelper.ModuleAccountManager 则查询不到被删账户了
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool RightAccessAccount(string account)
        {
            if (this.Manager == null) return false;//没有管理端绑定 则返回false
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null) return false;
            return this.Manager.RightAccessAccount(acc);
        }

        


        /// <summary>
        /// 当前状态是否接受某个账户的交易信息,管理端会选中某个交易帐号进行查看,则服务端只会讲该账户的交易信息推送到管理端
        /// </summary>
        /// <param name="accound"></param>
        /// <returns></returns>
        public bool NeedPushTradingInfo(string account)
        {
            //如果提供的帐号 或者 设定当前选择的帐号为空或null 则不推送该交易信息
            if (string.IsNullOrEmpty(account) || this.AccountSelected == null) return false;
            //选中的帐号与我们当前比较的帐号 相同,则我们推送该信息
            if (this.AccountSelected.ID == account) return true;
            return false;
        }


        /// <summary>
        /// 观察一个账户列表,用于推送实时的权益数据
        /// </summary>
        /// <param name="msg"></param>
        public void Watch(IEnumerable<string> accountlist)
        {
            WatchAccounts.Clear();
            foreach (string account in accountlist)
            {
                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                if (acc == null) continue;//交易帐户不存在 
                if (!this.Manager.RightAccessAccount(acc)) continue;//无权查看交易帐户 不添加
                WatchAccounts.Add(acc);
            }
        }

        public void WatchAgents(IEnumerable<string> agentlist)
        {
            watchAgents.Clear();
            foreach (string account in agentlist)
            {
                var agent = BasicTracker.AgentTracker[account];
                if (agent == null) continue;
                Manager mgr = BasicTracker.ManagerTracker[agent.Account];
                if(mgr == null) continue;
                if (!this.Manager.RightAccessManager(mgr)) continue;
                watchAgents.Add(agent);
            }
        }


        #region 注册观察通道列表 用于发送实时通道状态
        ThreadSafeList<IBroker> _watchBrokers = new ThreadSafeList<IBroker>();
        public ThreadSafeList<IBroker> WatchBrokers { get { return this._watchBrokers; } }

        public void RegVendor(IBroker broker)
        {
            if(!_watchBrokers.Any(b=>b.Token.Equals(broker.Token)))
                _watchBrokers.Add(broker);
        }

        public void UnregVendor(IBroker broker)
        { 
            if(_watchBrokers.Any(b=>b.Token.Equals(broker.Token)))
                _watchBrokers.Remove(broker);
        }

        public void ClearVendor()
        {
            _watchBrokers.Clear();
        }
        #endregion


        /// <summary>
        /// 选中某个账户 用于回补该账户的交易记录
        /// </summary>
        /// <param name="account"></param>
        public void Selected(IAccount account)
        {
            if (!this.Manager.RightAccessAccount(account)) return;//无权查看交易帐户
            this.AccountSelected = account;
        }


        public string ToString(bool full)
        {
            string domain = "null";
            string client = "null";
            string id = "null";
            if (_clientInfo == null)
            {
                domain = "null";
                client = "null";
            }
            else
            {
                client = _clientInfo.Location.ClientID;
                if (_clientInfo.Manager == null)
                {
                    domain = "mgr null";
                    id = "mgr null";
                }
                else
                {
                    domain = _clientInfo.Manager.domain_id.ToString();
                    id = _clientInfo.LoginID;
                }
            }

            string info = string.Format("ClientInfo:{0} Domain:{1} ID:{2}", client, domain, id);

            if (full)
            {
                return info + "-" + string.Join(",", WathAccountList.Select(a => a.ID).ToArray());
            }
            else
            {
                return info;
            }
        }
        public override string ToString()
        {
            return ToString(false);
        }
    }


}