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

   

    //更新登入信息
    //public delegate void LoginInfoDel(string loginID, bool isloggedin, IClientInfo clientInfo);//更新客户端登入信息
    //以IClient为参数的委托 包含登入客户端 注销客户端
    //public delegate void ClientParamDel(IClientInfo c);

    //当有持仓回合记录关闭时触发该事件
    public delegate void PositionRoundClosedDel(IPositionRound pr,Position pos);

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
    //清算中心手续费调整委托 用于调整某个成交的手续费
    public delegate decimal AdjustCommissionDel(Trade fill, IPositionRound positionround);
    public delegate void ChargeFinFeeDel(string account, decimal fee);//配资中心的扣费出金委托 用于调用清算中心进行出金操作
    public delegate bool IsTradingDayDel();//查询当前是否是交易日
    //public delegate void IFinStatisticDel(IFinStatistic f);//获得配资账户总体的一个统计信息
    //public delegate IFinServiceInfo GetFinServiceInfoDel(string acc);//获得账户的配资信息
    //public delegate IFinService GetFinServiceDel(string acc);//获得账户的配资服务
    


    //Race中用到的委托
    public delegate void PromotAccountDel(IAccount account, QSEnumAccountRaceStatus nextstatus);
    public delegate void EliminateAccountDel(IAccount account, QSEnumAccountRaceStatus nextstatus);
    public delegate void EntryAccountDel(IAccount account, QSEnumAccountRaceStatus nextstatus);
    public delegate void AccountEntryRaceDel(IAccount account, QSEnumRaceType type);//某个账户进入某个类别的比赛
    public delegate bool IAccountSignForPreraceDel(IAccount a, out string msg);//某个账户请求报名预赛

    //public delegate IRaceInfo GetRaceInfoDel(string acc);//获取某个账户的比赛信息
    
    //public delegate void RaceStatsticDel(IRaceStatistic rs);
    public delegate void IAccountChangedDel(IAccount account);//账户发生变动委托
    public delegate void IAccountLoadedDel(IAccount account);//得到某个账户的回调
    public delegate void IAccountCheckDel(IAccount account);//针对某个账户做检查时进行的回调

    public delegate List<string> GetRaceStatisticStrListDel();//获得所有比赛状态
    //public delegate DataSet FindCustomerDataSet(string customer);//查找customer对应的数据集


    //public delegate void SymPosStatisticAddDel(ISymbolPositionStatistic sps);//当新增加一个合约仓位统计时触发


    //以Ihelath为参数的委托
    //public delegate void HealthParamDel(IHealthInfo h);
    //public delegate IHealthInfo HealthResoultDel();
    //public delegate void WebStatisticParamDel(IWebStatistic w);
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
    public delegate bool UpdateFinServiceDel(string account, decimal ammount, QSEnumFinServiceType type, decimal discount, int agentcode, out string msg);
    public delegate bool AddFinServiceDel(string account, decimal ammount, QSEnumFinServiceType type, string agent, decimal discount, out string msg);
    public delegate string AddAccountDel(string user_id, string pass);
    public delegate bool ValidWithdrawDel(string account, decimal ammount, out string msg);
    public delegate bool AccountParamDel(string account, out string msg);
    public delegate void TrimFinServiceDel(string account);

    //清算中心
    //清算中心状态发生变化
    public delegate void ClearCentreStatusDel(QSEnumClearCentreStatus status);
    //当帐户状态发生变化
    public delegate void AccountSettingChangedDel(IAccount account);

    //Connector
    public delegate IBroker FindBrokerDel(string fullanme);
    public delegate IDataFeed FindDataFeedDel(string fullname);

}
