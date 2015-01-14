using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 帐户常规检查
    /// </summary>
    public partial class AccountBase
    {
        /// <summary>
        /// 检查帐户是否可以交易某个合约,在帐户是否可以交易合约逻辑部分
        /// 我们可以按照合约的种类去查询对应的服务是否支持该合约。
        /// 具体的检查逻辑可以在扩展模块的服务对象中去实现
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool CanTakeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            bool re = true;
            if (symbol.SecurityType == SecurityType.INNOV)
            {
                msg = "合约无法交易";
                re = false;
            }
            return re;

            
        }

        /// <summary>
        /// 检查某个帐户是否可以执行某个委托Order
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual bool CanFundTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            //如果是平仓委托 则直接返回
            if (!o.IsEntryPosition) return true;
            //获得对应方向的持仓
            Position pos = GetPosition(o.Symbol, o.PositionSide);

            //获得某个帐户交易某个合约的可用资金
            decimal avabile = GetFundAvabile(o.oSymbol);

            //可用资金大于需求资金则可以接受该委托
            decimal required = CalOrderFundRequired(o);
            //Util.Debug("[CanFundTakeOrder Check] Fundavabile:" + avabile.ToString() + " Required:" + required);
            if (required > avabile)
            {
                msg = "资金不足";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查询帐户当前某合约的可开仓数量
        /// 有些可开逻辑是按照对应服务的相关逻辑进行计算可开数量
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual int CanOpenSize(Symbol symbol)
        {

            decimal price = TLCtxHelper.CmdUtils.GetAvabilePrice(symbol.Symbol);

            decimal fundperlot = Calc.CalFundRequired(symbol, price, 1);

            decimal avabilefund = GetFundAvabile(symbol);

            Util.Debug("QryCanOpenSize Fundavablie:" + avabilefund.ToString() + " Symbol:" + symbol.Symbol + " Price:" + price.ToString() + " Fundperlot:" + fundperlot.ToString());
            return (int)(avabilefund/fundperlot);
        }
    }
}
