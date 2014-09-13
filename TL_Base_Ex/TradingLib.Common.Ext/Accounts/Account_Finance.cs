using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {

        #region 【IFinanceTotal】
        decimal _lastequity;
        /// <summary>
        /// 昨日权益
        /// </summary>
        public decimal LastEquity { get { return _lastequity; } set { _lastequity = value; } }

        /// <summary>
        /// 当前权益 经过排查 commission并非线程安全
        /// </summary>
        public decimal NowEquity { get { return TotalLiquidation; } }

        //当日平仓盈亏 
        public decimal RealizedPL { get { return FutRealizedPL + OptRealizedPL + InnovRealizedPL; } }

        //当日浮动盈亏 
        public decimal UnRealizedPL { get { return FutUnRealizedPL + OptMarketValue - OptPositionCost + InnovMarketValue - InnovPositionCost; } }

        /// <summary>
        /// 盯市盈亏
        /// </summary>
        public decimal SettleUnRealizedPL { get { return FutSettleUnRealizedPL + OptSettlePositionValue - OptPositionCost + InnovSettlePositionValue - InnovPositionCost; } }

        //当日手续费 通过成交委托来计算手续费
        public decimal Commission { get { return FutCommission + OptCommission + InnovCommission; } }

        //当日净利
        public decimal Profit { get { return RealizedPL + UnRealizedPL - Commission; } }

        decimal _cashin = 0;
        /// <summary>
        /// 本次结算周期(本日入金)
        /// </summary>
        public decimal CashIn { get { return _cashin; } set { _cashin = value; } }

        decimal _cashout = 0;
        /// <summary>
        /// 本次结算周期(本地出金)
        /// </summary>
        public decimal CashOut { get { return _cashout; } set { _cashout = value; } }
        /// <summary>
        /// 总占用资金
        /// </summary>
        public decimal MoneyUsed { 
            
            get 
            {
                    return FutMoneyUsed + OptMoneyUsed + InnovMoneyUsed;
            } 
        
        }
        /// <summary>
        /// 保证金占用
        /// 期货保证金占用 期权持仓成本 异化保证金
        /// </summary>
        public decimal Margin { get { return this.FutMarginUsed + this.OptPositionCost + this.InnovMargin; } }

        /// <summary>
        /// 保证金冻结
        /// 期货保证金占用 期权资金占用 异化保证金占用
        /// </summary>
        public decimal MarginFrozen { get { return this.FutMarginFrozen + this.OptMoneyFrozen + this.InnovMarginFrozen; } }


        /// <summary>
        /// 总净值
        /// </summary>
        public decimal TotalLiquidation { get { return LastEquity + FutLiquidation + OptLiquidation + InnovLiquidation + CashIn - CashOut; } }//帐户总净值
        /// <summary>
        /// 总可用资金
        /// </summary>
        public decimal AvabileFunds { get { return TotalLiquidation - FutMoneyUsed - OptMoneyUsed - InnovMoneyUsed; } }//帐户总可用资金
        #endregion


        /// <summary>
        /// 入金
        /// </summary>
        /// <param name="amount"></param>
        public void Deposit(decimal amount)
        {
            amount = Math.Abs(amount);
            _cashin += amount;
        }

        /// <summary>
        /// 出金
        /// 入金,出金均是绝对值,用于记录金额
        /// </summary>
        /// <param name="amount"></param>
        public void Withdraw(decimal amount)
        {
            amount = Math.Abs(amount);
            _cashout += amount;
        }

        /*
         * 关于 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */
        #region 【IFinanceFut/OPT/Innov】

        public decimal FutMarginUsed { get { return ClearCentre.FutMarginUsed; } }//期货占用保证金
        public decimal FutMarginFrozen { get { return ClearCentre.FutMarginFrozen; } }//期货冻结保证金
        public decimal FutRealizedPL { get { return ClearCentre.FutRealizedPL; } }//期货平仓盈亏
        public decimal FutUnRealizedPL { get { return ClearCentre.FutUnRealizedPL; } }//期货浮动盈亏
        public decimal FutCommission { get { return ClearCentre.FutCommission; } }//期货交易手续费

        public decimal FutCash { get { return FutRealizedPL + FutUnRealizedPL - FutCommission; } }//期货交易现金
        public decimal FutLiquidation { get { return FutCash; } }//期货总净值
        public decimal FutMoneyUsed { 
            get 
            {
                try
                {
                    //TLCtxHelper.Ctx.debug("get fut moneyused called");
                    return FutMarginUsed + FutMarginFrozen;
                }
                catch (Exception ex)
                {
                    TLCtxHelper.Ctx.debug("error2:" + ex.ToString());
                }
                return 0;
            
            }
        
        }//期货资金占用
        public decimal FutAvabileFunds {  get { return AvabileFunds + 0 /*加入期货配资提供的融资额度*/; } }

        /// <summary>
        /// 期货盯市盈亏
        /// </summary>
        public decimal FutSettleUnRealizedPL { get { return ClearCentre.FutSettleUnRealizedPL; } }//期货结算时盯市盈亏



        public decimal OptPositionCost { get { return ClearCentre.OptPositionCost; } }//期权持仓成本
        public decimal OptPositionValue { get { return ClearCentre.OptPositionValue; } }//期权持仓市值
        public decimal OptRealizedPL { get { return ClearCentre.OptRealizedPL; } }//期权平仓盈亏
        public decimal OptCommission { get { return ClearCentre.OptCommission; } }//期权交易手续费
        public decimal OptMoneyFrozen { get { return ClearCentre.OptMoneyFrozen; } }//期权资金冻结


        public decimal OptCash { get { return OptRealizedPL - OptCommission - OptPositionCost; } }//期权交易现金
        public decimal OptMarketValue { get { return OptPositionValue; } }//期权总市值
        public decimal OptLiquidation { get { return OptMarketValue + OptCash; } }//期权总净值
        public decimal OptMoneyUsed { get { return OptPositionCost * 1 + OptMoneyFrozen/*期权交易保证金比例*/; } }//期权资金占用
        public decimal OptAvabileFunds { get {return AvabileFunds + 0 /*加入系统秘籍提供的融资额度*/;  } }

        public decimal OptSettlePositionValue { get { return ClearCentre.OptSettlePositionValue; } }//期货结算持仓市值

        public decimal InnovPositionCost { get { return ClearCentre.InnovPositionCost; } }
        public decimal InnovPositionValue { get { return ClearCentre.InnovPositionValue; } }
        public decimal InnovCommission { get { return ClearCentre.InnovCommission; } }
        public decimal InnovRealizedPL { get { return ClearCentre.InnovRealizedPL; } }
        public decimal InnovMargin { get { return ClearCentre.InnovMargin; } }
        public decimal InnovMarginFrozen { get { return ClearCentre.InnovMarginFrozen; } }//异化合约保证金


        public decimal InnovCash { get { return InnovRealizedPL - InnovCommission - InnovPositionCost; } }//异化合约现金流
        public decimal InnovMarketValue { get { return InnovPositionValue; } }//异化合约市值
        public decimal InnovLiquidation { get { return InnovMarketValue + InnovCash; } }//异化合约净值
        public decimal InnovMoneyUsed { get { return ClearCentre.InnovMargin + ClearCentre.InnovMarginFrozen; } }//异化合约资金占用
        public decimal InnovAvabileFunds {  get { return AvabileFunds + 0;  } }//异化合约可用资金

        public decimal InnovSettlePositionValue { get { return ClearCentre.InnovSettlePositionValue; } }
        #endregion
    }
}
