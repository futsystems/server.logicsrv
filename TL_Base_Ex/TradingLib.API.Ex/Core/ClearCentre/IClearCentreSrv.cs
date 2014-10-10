using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.API
{
    /// <summary>
    /// 服务端的IClearCentreSrv
    /// </summary>
    public interface IClearCentreSrv : IClearCentreBase, IBrokerTradingInfo
    {

        /// <summary>
        /// 添加交易帐号
        /// </summary>
        event AccountIdDel AccountAddEvent;

        /// <summary>
        /// 删除交易帐号
        /// </summary>
        event AccountIdDel AccountDelEvent;

        /// <summary>
        /// 激活交易帐号
        /// </summary>
        event AccountIdDel AccountActiveEvent;

        /// <summary>
        /// 冻结交易帐号
        /// </summary>
        event AccountIdDel AccountInActiveEvent;

        /// <summary>
        /// 交易帐户加载到内存
        /// 在交易帐户初始化时,我们触发该事件，
        /// 通过该事件我们将相关功能组件的适配器绑定到交易帐户，从而实现交易帐户直接调用相关功能
        /// 比如 获得清算中心对应该帐户的数据
        /// 风控中心的强平操作等
        /// </summary>
        //event IAccountDel AccountCachedEvent;

        /// <summary>
        /// 帐户修改事件
        /// </summary>
        event AccountSettingChangedDel AccountChangedEvent;

        /// <summary>
        /// 对外传递带手续费的成交信息
        /// </summary>
        event FillDelegate GotCommissionFill;

        /// <summary>
        /// 调整手续费事件,对外触发手续费调整事件,用于相关逻辑进行手续费调整
        /// </summary>
        event AdjustCommissionDel AdjustCommissionEvent;


        /// <summary>
        /// 持仓回合关闭事件
        /// </summary>
        event PositionRoundClosedDel PositionRoundClosedEvent;//交易回合生成事件

        

        /// <summary>
        /// 从数据库恢复当日交易数据/交易数据每日结算，因此恢复当前交易状态只需要恢复当日数据即可
        /// </summary>
        void RestoreFromMysql();

        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="user_id"></param>
        /// <param name="pass"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool AddAccount(out string account, string user_id, string setaccount,string pass, QSEnumAccountCategory type,int mgr_fk);

        /// <summary>
        /// 验证某交易账户
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        bool VaildAccount(string ac, string pass);

        /// <summary>
        /// 查询某个交易账户 可开symbol多少手
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //int QryCanOpenPosition(string acc, string symbol);

       

        
        /// <summary>
        /// 安全出入金操作,主要用于web端的交互
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="ammount"></param>
        /// <param name="comment"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CashOperationSafe(string accid, decimal ammount, string comment, out string msg);


        IEnumerable<Position> GetPositions(string account);

    }
}
