using System;
using System.Collections.Generic;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 将财务计算,账户交易信息,以及账户操作等接口集合成IAccountClearCentre用于调用相关函数进行操作
	/// 将Account对象需要的功能暴露给Account对象
    /// </summary>
    public interface IAccountClearCentre : IFianceFutBase, IFinanceOptBase,IFinanceINNOVBase,IAccOperation
    {

        decimal CalOrderFundRequired(Order o,decimal defaultvalue);//计算冻结保证金
        //decimal ObverseProfit { get; }//计算参赛以来的折算收益
        //decimal FinAmmountAvabile { get; }//有效配资额度
		//decimal FinAmmountTotal { get; }//总配资额度


        #region 委托与持仓管理器
        //object OrderTracker { get; }
        //object PositionTracker { get; }
        #endregion

        //#region 合约信息
        //void UpdateMasterSecurity();//更新该账户的合约品种信息
        //Security getMasterSecurity(string symbol);//通过合约代码获得主合约信息
       // #endregion


        /// <summary>
        /// 查询可开仓数量
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //int CanOpenSize(string symbol);

        /// <summary>
        /// 获得某个合约的当前市场价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string symbol);


    }
}
