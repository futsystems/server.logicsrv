using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 定义了针对某个账户的交易信息操作接口
    /// </summary>
    public interface IAccountTradingInfo
    {
        /// <summary>
        /// 查询某个账户是否有持仓
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool AnyPosition(string id);

        //#region 帐户的持仓与委托管理器
        ///// <summary>
        ///// 获得某交易账户的仓位管理器
        ///// </summary>
        ///// <param name="AccountID"></param>
        ///// <returns></returns>
        //Object getPositionTracker(string AccountID);

        ///// <summary>
        ///// 获得某个交易账户的委托管理器
        ///// </summary>
        ///// <param name="AccountID"></param>
        ///// <returns></returns>
        //Object getOrderTracker(string AccountID);
        //#endregion

        #region 获得当日交易记录 account 为null返回所有
        /// <summary>
        /// 获得昨日持仓数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        Position[] getPositionHold(string accountID);//获得昨日持仓数据
        /// <summary>
        /// 获得当前所有持仓数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        Position[] getPositions(string accountID);//获得所有仓位
        /// <summary>
        /// 获得当前所有委托
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        Order[] getOrders(string accountID);//获得所有委托
        /// <summary>
        /// 获得当前所有成交
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        Trade[] getTrades(string accountID);//获得所有成交
        /// <summary>
        /// 获得当前所有取消
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        long[] getCancels(string accountID);//获得所有取消
        #endregion


        /// <summary>
        /// 通过order.account order.symbol来获得清算中心account下的symbol持仓
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        Position getPosition(string account, string symbol,bool side);


        Position getPosition(string account, string symbol);
        /// <summary>
        /// 获得某个委托下对应账户与合约持仓的反向 未平仓合约 用于CTP检查持仓状态
        /// 主要用于检查可平持仓
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //int getUnfilledSizeExceptStop(Order o);

        /// <summary>
        /// 获得某个账户 某个合约 隔夜持仓数据 0 为无持仓
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        int getPositionHoldSize(string account, string symbol);

        /// <summary>
        /// 获得某个委托下对应账户与合约 方向与合约方向一致的未成交委托，用于撤销以前的平仓委托(不包含当前委托o)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        long[] getPendingOrders(Order o);

        /// <summary>
        /// 获得某个账户 某个合约 某个方向的所有待成交委托
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        long[] getPendingOrders(string account, string symbol, bool side);
    }
}
