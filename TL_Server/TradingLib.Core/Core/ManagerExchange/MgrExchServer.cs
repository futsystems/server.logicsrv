using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class MgrExchServer : BaseSrvObject, ICore, IMessageMgr
    {
        const string CoreName = "MgrExchServer";
        public string CoreId { get { return this.PROGRAME; } }


        TLServer_MgrExch tl;

        MsgExchServer exchsrv;
        ClearCentre clearcentre;
        RiskCentre riskcentre;
        ConfigDB _cfgdb;

        ConcurrentDictionary<string, CustInfoEx> customerExInfoMap = null;


        public MgrExchServer(MsgExchServer srv,ClearCentre c,RiskCentre r)
            :base(MgrExchServer.CoreName)
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

            //tl.NumWorkers = 1;
            //tl.EnableTPTracker = false;

            tl.CachePacketEvent += new IPacketDelegate(CachePacket);
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newLoginRequest += new LoginRequestDel<MgrClientInfo>(tl_newLoginRequest);
            tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
            tl.newOrderActionRequest += new OrderActionDelegate(tl_newOrderActionRequest);
            tl.newPacketRequest += new MgrPacketRequestDel(tl_newPacketRequest);
            tl.GetLocationsViaAccountEvent += new LocationsViaAccountDel(tl_GetLocationsViaAccountEvent);

            tl.ClientRegistedEvent += new ClientInfoDelegate<MgrClientInfo>(tl_ClientRegistedEvent);
            tl.ClientUnregistedEvent += new ClientInfoDelegate<MgrClientInfo>(tl_ClientUnregistedEvent);

            exchsrv = srv;
            clearcentre = c;
            riskcentre = r;

            customerExInfoMap = new ConcurrentDictionary<string, CustInfoEx>();






            //启动消息服务
            StartMessageRouter();
        }

        

        

       
        


        bool _valid = false;
        public void Start()
        {
            //StartMessageOut();

            debug("##########启动 Manager Server###################",QSEnumDebugLevel.INFO);
            try
            {
                tl.Start();
                tl.RestoreSession();
            }
            catch (Exception ex)
            {
                _valid = false;
                return;
            }
            _valid = true;
            if (_valid)
            {
                debug("Trading Server Starting success");
            }
            else
                debug("Trading Server Starting failed.");
        }
        public void Stop()
        {
            //StopMessageRouter();

            debug("#########停止 Manager Server ##########", QSEnumDebugLevel.INFO);
            if (tl != null && tl.IsLive)
            {
                tl.Stop();
            }
            debug("Manger server stopped....");
        }

        


    }
}



/*关于管理端改进的构思
 * 原来的设计是统一获取账户信息,然后将这些账户所有的交易信息传输到管理端
 * 账户数目少时,运行没有问题。当账户数目上升后，这个方式就有弊端，每次连接需要传输当日所有交易数据，造成数据容易由于各种原因
 * 缺失。
 * 
 * 1.只传输账户信息，不集中传输交易信息
 * 2.账户监控列表 按监控的账户集合 统一向服务端订阅 账户实时信息，那么服务端记录该列表(20个)，服务端不间断的计算该列表内的账户
 * 最新信息然后发送到管理端。
 * 3.当管理端需要查看某个具体账户的交易记录时,双击该账户 则向服务端请求恢复该交易账户的当日交易记录
 * 
 * 
 * 服务端与客户端之间的账户类消息
 * 1.账户主体消息AccountBase，用从服务端获得账户基本信息,然后生成对应的Account实体
 * 2.RaceInfo 比赛信息，传递了账户比赛相关的信息,用于查看账户当前比赛状态,晋级 淘汰 差额等
 * 3.sessionInof 账户登入 注销 信息,用于动态的更新当前账户的连接信息
 * 4.AccountInfo 在进行账户查询时候 传递的账户消息,用于提供账户全面的财务信息
 * 5.
 * 
 * 
 * 
 * */
//定义了管理端请求数据集合
public class CustInfoEx
{

    //管理端ID
    public ILocation Location {get;set;}
    //管理端的当前观察账户列表,保存了需要向管理端推送当前动态信息的账户列表
    ThreadSafeList<string> WatchAccounts = new ThreadSafeList<string>();
    public ThreadSafeList<string> WathAccountList { get { return this.WatchAccounts; } }
    //保存了管理端当前需要推送实时交易信息的帐号,任何时刻管理端只接受若干个账户财务信息更新，以及某个账户的交易记录
    string _selectacc = string.Empty;
    public string SelectedAccount { get { return _selectacc; } set { _selectacc = value; } }

    public CustInfoEx(ILocation location)
    {
        Location = location;
    }

    /// <summary>
    /// 当前状态是否接受某个账户的交易信息,管理端会选中某个交易帐号进行查看,则服务端只会讲该账户的交易信息推送到管理端
    /// </summary>
    /// <param name="accound"></param>
    /// <returns></returns>
    public bool NeedPushTradingInfo(string account)
    {
        //如果提供的帐号 或者 设定当前选择的帐号为空或null 则不推送该交易信息
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(SelectedAccount)) return false;
        //选中的帐号与我们当前比较的帐号 相同,则我们推送该信息
        if (account.Equals(this.SelectedAccount)) return true;

        return false;

    }
    //账户观察列表是用,分割的一个字符串
    /// <summary>
    /// 观察一个账户列表,用于推送实时的权益数据
    /// </summary>
    /// <param name="msg"></param>
    public void Watch(List<string> accountlist)
    {
        WatchAccounts.Clear();
        foreach (string account in accountlist)
        {
            WatchAccounts.Add(account);
        }

    }
    /// <summary>
    /// 选中某个账户 用于回补该账户的交易记录
    /// </summary>
    /// <param name="account"></param>
    public void Selected(string account)
    {
        _selectacc = account;
    }

}


/// <summary>
/// 记录是哪个管理端请求了该账户
/// </summary>
public struct AccountSource
{
    public IAccount Account;
    public string Source;
    public AccountSource(IAccount acc, string source)
    {
        Account = acc;
        Source = source;
    }
}

public struct AccountInfoLiteSource
{
    public IAccountInfoLite AccInfo;
    public string Source;
    public AccountInfoLiteSource(IAccountInfoLite info, string source)
    {
        AccInfo = info;
        Source = source;
    }

}
