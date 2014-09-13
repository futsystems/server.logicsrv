using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.API
{
    /*
    public interface IFinServiceCentre :IDebug,IMail
    {
        
        /// <summary>
        /// 通过account id获得IAccount对象
        /// </summary>
        //event FindAccountDel FindAccountEvent;
        /// <summary>
        /// 查询清算中心当天是否是交易日,交易日扣费非交易日不扣费
        /// </summary>
        //event IsTradingDayDel IsTradingDayEvent;
        /// <summary>
        /// 配资中心的扣费事件 用于通知清算中心从帐户中出金以达到扣费的目的
        /// </summary>
        //event ChargeFinFeeDel ChargeFinFeeEvent;
        event IAccountCheckDel AccountCheckEvent;


        /// <summary>
        /// 添加配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        /// <param name="type"></param>
        /// <param name="discount"></param>
        /// <param name="agent"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool AddFinService(string account, decimal ammount, QSEnumFinServiceType type, decimal discount, string agent, out string msg);
        /// <summary>
        /// 修改配资服务激活状态
        /// </summary>
        /// <param name="account"></param>
        /// <param name="active"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool UpdateFinServiceActive(string account, bool active, out string msg);

        /// <summary>
        /// 更新配资服务类别
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="issync"></param>
        /// <returns></returns>
        bool UpdateFinServiceType(string account, QSEnumFinServiceType type, out string msg, bool issync = false);


        /// <summary>
        /// 更新配资服务额度
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        /// <param name="msg"></param>
        /// <param name="issync"></param>
        /// <returns></returns>
        bool UpdateFinServiceAmmount(string account, decimal ammount, out string msg, bool issync = false);


        /// <summary>
        /// 更新配资服务代理人
        /// </summary>
        /// <param name="account"></param>
        /// <param name="agentcode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool UpdateFinServiceAgent(string account, int agentcode, out string msg);

        /// <summary>
        /// 更新配资服务折扣
        /// </summary>
        /// <param name="account"></param>
        /// <param name="discount"></param>
        /// <param name="msg"></param>
        /// <param name="issync"></param>
        /// <returns></returns>
        bool UpdateFinServiceDiscount(string account, decimal discount, out string msg, bool issync = false);

        /// <summary>
        /// 修正配资服务额度,亏损过半自动降低配资额度
        /// </summary>
        /// <param name="account"></param>
        /// <param name="iswithdraw"></param>
        void TrimFinService(string account, bool iswithdraw = true);


        /// <summary>
        /// 验证出金
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool ValidWithdraw(string account, decimal ammount, out string msg);

        /// <summary>
        /// 查询配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool QryFinService(string account, out string msg);

        /// <summary>
        /// 获得某个帐户的可用配资额度
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        decimal GetFinAmmountAvabile(string account);

        /// <summary>
        /// 获得某个帐户的总计配资额度
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        decimal GetFinAmmountTotal(string account);

        /// <summary>
        /// 调整某个帐户的交易手续费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="size"></param>
        /// <param name="commission"></param>
        /// <returns></returns>
        decimal AdjestCommission(Trade fill, IPositionRound positionround);

        /// <summary>
        /// 单独加载某个帐户的配资服务
        /// </summary>
        /// <param name="accid"></param>
        void LoadFinServices(string accid = null);//

        /// <summary>
        /// 配资中心处理某个positionround关闭
        /// </summary>
        /// <param name="pr"></param>
        void OnPositionRoundClosed(IPositionRound pr);

        /// <summary>
        /// 当有配资本帐号创建时,调用绑定默认的配资服务
        /// </summary>
        /// <param name="account"></param>
        void OnLoaneeAccountCreated(string account);
        /// <summary>
        /// 重置配资服务
        /// </summary>
        void Reset();//

        /// <summary>
        /// 结算配资服务
        /// </summary>
        void SettleFinServices();

        /// <summary>
        /// 获得某个帐户的配资本服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        IFinService this[string account] { get; }

        /// <summary>
        /// 启动
        /// </summary>
        //void Start();
        /// <summary>
        /// 停止
        /// </summary>
        //void Stop();
        /// <summary>
        /// 获得所有配资帐户的统计
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        IFinStatistic GetFinStatForTotal(IClearCentreBase cc);
        IFinStatistic GetFinStatForSIM(IClearCentreBase cc);
        IFinStatistic GetFinStatForLIVE(IClearCentreBase cc);
        IFinSummary GetFinSummary(IClearCentreBase cc);
    }**/
}
