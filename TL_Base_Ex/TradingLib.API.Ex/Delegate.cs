using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.API
{

    //ansycserver->tlserver由消息中继调用的TLServer handlemessge事件
    public delegate long HandleTLMessageDel(MessageTypes type, string msg, string front, string address);
    //tlserver->messagerouter/messagemgr(tradingserver)
    public delegate long UnknownMessageDelegateSession(MessageTypes t, string msg, ISession session);

   

    //通过帐户编号返回账户实例
    public delegate IAccount FindAccountDel(string account);
    public delegate void IAccountDel(IAccount account);

    //清算中心查询position
    public delegate Position FindPosition(string account,string symbol);
    //平掉某个持仓的委托
    public delegate void FlatPostionDel(Position pos,QSEnumOrderSource source,string comment);

    //交易服务获得委托编号
    public delegate void AssignOrderIDDel(ref Order o);

    //配资中心使用的委托
    public delegate decimal AccountFinAmmountDel(string account);//查找某个账户的配资额度

    public delegate void ChargeFinFeeDel(string account, decimal fee);//配资中心的扣费出金委托 用于调用清算中心进行出金操作
    public delegate bool IsTradingDayDel();//查询当前是否是交易日

    //public delegate void RaceStatsticDel(IRaceStatistic rs);
    public delegate void IAccountChangedDel(IAccount account);//账户发生变动委托
    public delegate void IAccountLoadedDel(IAccount account);//得到某个账户的回调
    public delegate void IAccountCheckDel(IAccount account);//针对某个账户做检查时进行的回调

    public delegate List<string> GetRaceStatisticStrListDel();//获得所有比赛状态

    //通过登入账户ID反向查找客户端ID
    public delegate string LoginID2ClientIDDel(string loginid);

    //缓存消息然后用线程安全的方式发送出去
    public delegate void CacheMessageDel(string msg, MessageTypes type, string address);


    //通过交易所代码找到系统配置中对应的交易通道
    public delegate IBroker LookupBroker(string exchange);
    public delegate IBroker LookupSimBroker();
    public delegate IDataFeed LookupDataFeed(string exchange);


    //webchannel 相关事件委托
    public delegate string WebTaskDel(string param);
    public delegate string WebMessageDel(string param,bool istnetstring);
    public delegate string AddAccountDel(string user_id, string pass);
    public delegate bool ValidWithdrawDel(string account, decimal ammount, out string msg);
    public delegate bool AccountParamDel(string account, out string msg);
    public delegate void TrimFinServiceDel(string account);

    //当帐户状态发生变化
    public delegate void AccountSettingChangedDel(IAccount account);

    //Connector
    public delegate IBroker FindBrokerDel(string fullanme);
    public delegate IDataFeed FindDataFeedDel(string fullname);

}
