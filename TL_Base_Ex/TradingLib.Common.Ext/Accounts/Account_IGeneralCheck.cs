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
        /// 账户下开仓委托 进行账户资金充足性检查
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool CheckEquityAdequacy(Order o, out string msg)
        {
            msg = string.Empty;
            //如果是平仓委托 则直接返回
            if (!o.IsEntryPosition) return true;

            //可用资金大于需求资金则可以接受该委托
            decimal requiredfund = this.CalOrderFundRequired(o);
            //Util.Debug("[CanFundTakeOrder Check] Fundavabile:" + avabile.ToString() + " Required:" + required);
            if (requiredfund > this.AvabileFunds)
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
        public int CheckMaxOrderSize(Symbol symbol, bool side, QSEnumOffsetFlag flag)
        {
            switch (symbol.SecurityType)
            {
                case SecurityType.FUT:
                    {
                        //未启用单向大边
                        if (!this.GetParamSideMargin())
                        {
                            if (flag == QSEnumOffsetFlag.OPEN)
                            {
                                decimal price = TLCtxHelper.ModuleDataRouter.GetAvabilePrice(symbol.Exchange, symbol.Symbol);

                                decimal fundperlot = this.CalOrderMarginFrozen(symbol, 1) * this.GetExchangeRate(symbol.SecurityFamily);

                                //decimal avabilefund = GetFundAvabile(symbol);
                                return (int)(this.AvabileFunds / fundperlot);
                            }

                            return 0;
                        }
                        else
                        {
                            decimal fundperlot = this.CalOrderMarginFrozen(symbol, 1) * this.GetExchangeRate(symbol.SecurityFamily);

                            decimal avabilefund = this.AvabileFunds;

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

                                logger.Error(string.Format("QryCanOpenSize[MarginSizde] symbol:{0} side:{1} bigside:{2} bighold:{3} smalhold:{4} netfronzen:{5}  bigpending:{6} smallpending:{7} | fund:{8} prelot:{9}", symbol.Symbol, side, ms.MarginSide, ms.BigHoldSize, ms.SmallHoldSize, ms.NetFronzenSize, ms.BigPendingOpenSize, ms.SmallPendingOpenSize, avabilefund, fundperlot));
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
                case SecurityType.STK:
                    {
                        //买入
                        if (side)
                        {
                            //decimal avabilefund = GetFundAvabile(symbol);
                            decimal fundperlot = this.CalOrderMarginFrozen(symbol, 1) * this.GetExchangeRate(symbol.SecurityFamily);
                            return (int)(this.AvabileFunds / fundperlot);
                        }
                        else//卖出
                        {
                            Position pos  = this.GetPosition(symbol.Symbol,true);
                            int pendingSize = this.GetPendingExitSize(symbol.Symbol, true);
                            int ydsize = pos.PositionDetailYdNew.Sum(p => p.Volume); //隔夜仓可平

                            return ydsize - pendingSize;
                        }
                    }
                default:
                    return 0;
            }

        }

        public virtual IEnumerable<string> GetNotice()
        {
            return new List<string>();
        }
    }
}
