using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.API
{
    /// <summary>
    /// 清算中心接口
    /// 帐户操作类接口
    /// 所有交易信息
    /// 成交侧交易信息
    /// 帐户认证与出入金操作
    /// </summary>
    public interface IClearCentreSrv  :IAccountOperation,ITotalAccountInfo, IBrokerTradingInfo, IAuthCashOperation
    {
        /// <summary>
        /// 请求获得某个symbol的Tick数据
        /// </summary>
        event GetSymbolTickDel newSymbolTickRequest;

        /// <summary>
        /// 获得某个合约的当前市场价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string symbol);


        /// <summary>
        /// 添加交易帐号
        /// </summary>
        event AccoundIDDel AccountAddEvent;

        /// <summary>
        /// 删除交易帐号
        /// </summary>
        event AccoundIDDel AccountDelEvent;

        /// <summary>
        /// 激活交易帐号
        /// </summary>
        event AccoundIDDel AccountActiveEvent;

        /// <summary>
        /// 冻结交易帐号
        /// </summary>
        event AccoundIDDel AccountInActiveEvent;


        /// <summary>
        /// 帐户修改事件
        /// </summary>
        event AccountSettingChangedDel AccountChangedEvent;

        /// <summary>
        /// 调整手续费事件,对外触发手续费调整事件,用于相关逻辑进行手续费调整
        /// </summary>
        //event AdjustCommissionDel AdjustCommissionEvent;

        /// <summary>
        /// 持仓回合关闭事件
        /// </summary>
        //event PositionRoundClosedDel PositionRoundClosedEvent;//交易回合生成事件

        

        /// <summary>
        /// 从数据库恢复当日交易数据/交易数据每日结算，因此恢复当前交易状态只需要恢复当日数据即可
        /// </summary>
        void RestoreFromMysql();
 
        /// <summary>
        /// 验证某交易账户
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        bool VaildAccount(string ac, string pass);

        /// <summary>
        /// 安全出入金操作,主要用于web端的交互
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="ammount"></param>
        /// <param name="comment"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        //bool CashOperationSafe(string accid, decimal ammount, string comment, out string msg);

        
    }
}
