using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IGotAccount
    {
        void gotAccount(IAccount acc);
    }

    /// <summary>
    /// 底层帐户接口
    /// 集成了财务数据,交易信息,风控检查,帐户操作等几大功能接口
    /// IFinanceTotal:交易帐户总财务数据接口
    /// IFinanceFut:交易帐户期货财务数据接口
    /// IFinanceOpt:交易帐户期权财务数据接口
    /// IFinacneINNOV:交易帐户异化财务数据接口
    /// IAccCal:交易帐户计算类接口
    /// IAccTradingInfo:交易帐户交易信息类接口
    /// IGeneralCehck:交易帐户通用检查,是否可以交易某合约,资金是否够交易某委托等
    /// IAccRiskCheck:交易帐户风控规则检查,添加删除委托风控规则或帐户风控规则
    /// IAccOperation:交易帐户操作接口
    /// </summary>
    public interface IAccount : IFinanceTotal, IAccCal, IAccTradingInfo, IAccOperation, IGeneralCheck, IRiskRule, IAccConfig, IAccountIndicator
    {

        #region 交易帐号服务类相关操作
        /// <summary>
        /// 绑定交易帐户服务
        /// </summary>
        /// <param name="service"></param>
        /// <param name="force"></param>
        void BindService(IAccountService service, bool force = true);

        /// <summary>
        /// 解绑帐户服务
        /// </summary>
        /// <param name="service"></param>
        void UnBindService(IAccountService service);

        /// <summary>
        /// 获得某个交易帐户服务
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        bool GetService(string sn, out IAccountService service);
        #endregion

        /// <summary>
        /// 交易帐号对应的数据库全局ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 是否可以进行交易
        /// </summary>
        bool Execute { get; set; }

        /// <summary>
        /// 是否处于警告状态
        /// </summary>
        bool IsWarn { get; set; }



        /// <summary>
        /// 是否日内交易
        /// </summary>
        bool IntraDay { get; set; }

        /// <summary>
        /// 账户委托转发通道类型
        /// </summary>
        QSEnumOrderTransferType OrderRouteType { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        QSEnumAccountCategory Category { get; set; }


        /// <summary>
        /// 货币类别
        /// </summary>
        CurrencyType Currency { get; set; }

        #region 投资者信息
        /// <summary>
        /// MAC地址 用于标注客户端硬件
        /// </summary>
        string MAC { get; set; }

        /// <summary>
        /// 帐户Name 用于储存帐户名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 帐户
        /// </summary>
        string Broker { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        int BankID { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        string BankAC { get; set; }
        #endregion


        /// <summary>
        /// 账户建立时间
        /// </summary>
        DateTime CreatedTime { get; set; }

        /// <summary>
        /// 上次结算日
        /// </summary>
        DateTime SettleDateTime { get; set; }

        /// <summary>
        /// 确认结算日期
        /// </summary>
        long SettlementConfirmTimeStamp { get; set; }

       

        /// <summary>
        /// 帐号隶属于哪个管理员
        /// 可以属于超级管理员Root
        /// 或者属于代理Agent
        /// 在用于添加时候就自动进行了绑定
        /// </summary>
        int Mgr_fk { get; set; }


        /// <summary>
        /// 路由组ID 用于将某个帐户绑定到某个路由组上面,然后这组用户下单就会下单路由组内的成交接口上
        /// </summary>
        int RG_FK { get; set; }

        #region 模板编号
        /// <summary>
        /// 手续费模板ID
        /// </summary>
        int Commission_ID { get; set; }

        /// <summary>
        /// 保证金模板ID
        /// </summary>
        int Margin_ID { get; set; }

        /// <summary>
        /// 交易参数模板ID
        /// </summary>
        int ExStrategy_ID { get; set; }

        #endregion


        /// <summary>
        /// 帐户加载交易所结算记录
        /// </summary>
        /// <param name="settle"></param>
        void LoadExchangeSettlement(ExchangeSettlement settle);


        /// <summary>
        /// 执行某个交易所 某个交易日的结算
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="settleday"></param>
        void SettleExchange(IExchange exchange, int settleday);

        #region 出入金操作
        /// <summary>
        /// 入金接口
        /// </summary>
        /// <param name="amount"></param>
        void Deposit(decimal amount);

        /// <summary>
        /// 出金接口
        /// </summary>
        /// <param name="amount"></param>
        void Withdraw(decimal amount);

        /// <summary>
        /// 信用入金
        /// </summary>
        /// <param name="amount"></param>
        void CreditDeposit(decimal amount);

        /// <summary>
        /// 信用出金
        /// </summary>
        /// <param name="amount"></param>
        void CreditWithdraw(decimal amount);
        #endregion

        /// <summary>
        /// 域ID
        /// </summary>
        Domain Domain { get;}

        /// <summary>
        /// 该帐号所绑定的全局UserID
        /// </summary>
        int UserID { get; set; }


        #region 交易回话类

        /// <summary>
        /// 是否登入
        /// </summary>
        bool IsLogin { get; set; }

        /// <summary>
        /// 回话信息 
        /// </summary>
        string SessionInfo { get; set; }

        #endregion


        /// <summary>
        /// 重置账户状态,用于每日造成开盘时,重置数据 
        /// </summary>
        void Reset();

        /// <summary>
        /// 删除标志，如果已经删除 则管理端不显示，下次启动不会加载
        /// </summary>
        bool Deleted { get; set; }


        /// <summary>
        /// 获得帐户交易通知
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetNotice();
        

    }


}
