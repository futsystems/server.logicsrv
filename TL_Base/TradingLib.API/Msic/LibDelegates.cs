using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.API
{
    /*
     * 定义lib中所使用的委托类型
     */
    public delegate void HandleTLMessageClient(MessageTypes type, string msg);//客户端处理消息委托

    public delegate void IPacketDelegate(IPacket packet);
    
 
    //public delegate void AddResponseDel(Response r);//添加Response策略委托
    public delegate bool RiskCheckOrderDel(Order o, out string msg,bool inter);//委托检查委托
    public delegate void RuleAddedDel(IRule rs);//增加一条风险规则委托
    //public delegate void AccountRuleAddDel(IAccountCheck rs);//增加一条账户检查规则委托

    //public delegate void BasketDel(Basket b);//参数为basket的事件,注册合约等
    //public delegate void SecurityBaseDel(Security sec);//合约控件 品种选择发生变化时候的委托

    //用于针对某个Order返回一些关于Order的消息
    public delegate void OrderMessageDel(Order o,string msg);
    //用于客户端弹出冒泡提示框
    //public delegate void PopMessageDel(QSEnumMessageLevel type, string _title, string _msg);

    //运行 停止某个策略配置实例
    //public delegate void StartResponseDel(Response r);
    //public delegate void StopResponseDel(Response r);
    //public delegate void SwitchPositionCheckDel(IPositionCheck check, bool run);//停止或者运行某个positioncheck
    

    //与服务端连接建立与断开委托
    public delegate void ConnectDel();//连接建立委托
    public delegate void DisconnectDel();//连接断开委托
    public delegate void ServerUpDel();//服务端可用委托
    public delegate void ServerDownDel();//服务端不可用委托
    public delegate void DataPubConnectDel();//Tick数据连接成功
    public delegate void DataPubDisconnectDel();//Tick数据连接成功

    //帐户以及帐户信息委托
    public delegate bool ServerLoginDel(string server,string user,string pass);//提供服务器地址,请求建立连接并登入系统

    /// <summary>
    /// 外部用户中心认证,通过login pass向外部授权服务进行认证,如果认证通过则返回0,并返回全局uid,认证失败则返回错误代码,并同时返回messsage
    /// 0:验证成功
    /// 1:验证函数未绑定/没有绑定到实际的验证函数
    /// 100:用户不存在
    /// 102:验证失败
    /// </summary>
    /// <param name="login">uid/email/mobile</param>
    /// <param name="pass">相对应的password</param>
    /// <param name="uid">认证通过会返回uid 用于交易系统在交易帐户表中查找uid</param>
    /// <param name="message">认证失败会返回对应的message</param>
    /// <returns>返回0表示认证通过,返回数值不为0则为错误代码</returns>
    //public delegate int AuthUserDel(string login,string pass,out int uid,out string message);//统一中心认证委托,通过提供uid/email/mobile 以及对应的pass进行认证 通过返回代码来判断认证状态 并返回认证过程中的状态消息

    //public delegate bool LoginRequestDel(string loginid, string pass,ref ILoginResponse response);//请求账户验证委托 传递loginreponse对象 然后根据业务层的相关结果对其进行更新
    //public delegate void LoginResponseDel(bool resoult,string account);//得到登入回报委托 成功或者失败 同时传入服务端返回的交易帐号
    

    public delegate void IAccountInfoDel(IAccountInfo accinfo);//以帐户信息为参数的调用比如显示或者处理等
    public delegate void IAccountInfoLiteDel(IAccountInfoLite info);
    public delegate void AccountIdDel(string account);//以帐户为参数传递事件 某个帐户成功登入,登入失败
    
    public delegate void ChangeAccountPassDel(string oldpass, string newpass);//修改帐户密码
 
    //以Connecter为参数的函数调用 connecter在基础api中 用于客户自定义connecter
    public delegate void IConnecterParamDel(string tocken);

    //获得某个symbol的tick委托
    public delegate Tick GetSymbolTickDel(string symbol);

    //查看账户可开合约数量
    public delegate void QryCanOpenPosition(string symbol);
    public delegate int QryCanOpenPositionLocalDel(string symbol);

    public delegate Order FindOrderDel(long oid);
    //public delegate Security FindSecurity(string symbol);//通过symbol找到对应的security
    public delegate Symbol Str2SymbolDel(string symbol);//通过symbol找到对应的symbol对象

    //objectinfo del
    //public delegate void IRaceInfoDel(IRaceInfo ri);//客户端获得比赛状态信息
    //public delegate void IFinServiceInfoDel(IFinServiceInfo fs);//获得配资信息
    

    //ihealth信息
    //public delegate IProfile GetUserProfileDel(string account,int userid);
    public delegate void EmailDel(IEmail email);

    //日志
    public delegate void LogDelegate(string objname, string msg, QSEnumDebugLevel level);


    //#region chart中使用到的委托 
    //public delegate void BarDelegate(Bar bar);
    
    //#endregion


    //日志项目委托
    public delegate void ILogItemDel(ILogItem log);

}
