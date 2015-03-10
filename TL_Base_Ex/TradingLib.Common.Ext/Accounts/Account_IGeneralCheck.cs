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
        public virtual int CanOpenSize(Symbol symbol,bool side,QSEnumOffsetFlag flag)
        {
            //未启用单向大边
            if (!this.GetArgsSideMargin())
            {
                decimal price = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Symbol);

                decimal fundperlot = this.CalOrderMarginFrozen(symbol, 1);

                decimal avabilefund = GetFundAvabile(symbol);

                Util.Debug("QryCanOpenSize Fundavablie:" + avabilefund.ToString() + " Symbol:" + symbol.Symbol + " Price:" + price.ToString() + " Fundperlot:" + fundperlot.ToString());
                return (int)(avabilefund / fundperlot);
            }
            else
            {
                decimal fundperlot = this.CalOrderMarginFrozen(symbol, 1);

                decimal avabilefund = GetFundAvabile(symbol);

                int canfronzensize = (int)(avabilefund / fundperlot);//通过当前可用资金 和 每手 占用资金来估算可开数量
                //Util.Debug(string.Format("QryCanOpenSize[MarginSizde] fund:{0} perlot:{1}",avabilefund,fundperlot), QSEnumDebugLevel.INFO);
                //单向大边情况下，大边就是该数值，如果是小边则是大边数量 + 可开数量 - 小边数量
                MarginSet ms = this.CalFutMarginSet().Where(t => t.Code.Equals(symbol.SecurityFamily.Code)).FirstOrDefault();
                if (ms == null)
                {
                    return canfronzensize;
                }
                else
                {
                    Util.Debug(string.Format("QryCanOpenSize[MarginSizde] symbol:{0} side:{1} bigside:{2} bighold:{3} smalhold:{4} netfronzen:{5}  bigpending:{6} smallpending:{7} | fund:{8} prelot:{9}", symbol.Symbol, side, ms.MarginSide, ms.BigHoldSize, ms.SmallHoldSize, ms.NetFronzenSize, ms.BigPendingOpenSize, ms.SmallPendingOpenSize,avabilefund,fundperlot), QSEnumDebugLevel.ERROR);
                    //如果查询大边可开数量
                    if (side == ms.MarginSide)
                    {
                        //大边可开数量 = 净冻结数量 + 可用资金可开数量 - 大便待开数量
                        return ms.NetFronzenSize + canfronzensize - ms.BigPendingOpenSize;
                    }
                    else
                    {
                        //大边持仓 - 小边持仓 + 净冻结数量 + 可用资金可开仓数量 - 小边待开仓数量
                        return ms.BigHoldSize - ms.SmallHoldSize + ms.NetFronzenSize + canfronzensize - ms.SmallPendingOpenSize;
                    }
                    
                }

            }
        }
    }
}
