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
        /// <summary>
        /// 昨日权益
        /// </summary>
        public decimal LastEquity { get; set; }

        /// <summary>
        /// 当前权益 经过排查 commission并非线程安全
        /// </summary>
        public decimal NowEquity { get { return TotalLiquidation; } }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal RealizedPL { get { return this.CalFutRealizedPL() + this.CalOptRealizedPL() + this.CalInnovRealizedPL(); } }

        /// <summary>
        /// 浮动盈亏 
        /// </summary>
        public decimal UnRealizedPL { get { return this.CalFutUnRealizedPL() + this.CalOptPositionValue() - this.CalOptPositionCost() + this.CalInnovPositionValue() - this.CalInnovPositionCost(); } }

        /// <summary>
        /// 盯市盈亏
        /// </summary>
        public decimal SettleUnRealizedPL { get { return this.CalFutSettleUnRealizedPL() + this.CalOptSettlePositionValue() - this.CalOptPositionValue() + this.CalInnovSettlePositionValue() - this.CalInnovPositionValue(); } }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get { return this.CalFutCommission() + this.CalOptCommission() + this.CalInnovCommission(); } }

        /// <summary>
        /// 净利润
        /// </summary>
        public decimal Profit { get { return RealizedPL + UnRealizedPL - Commission; } }

        decimal _cashin = 0;
        /// <summary>
        /// 本次结算周期(本日入金)
        /// </summary>
        public decimal CashIn { get { return _cashin; } set { _cashin = value; } }

        decimal _cashout = 0;
        /// <summary>
        /// 本次结算周期(本日出金)
        /// </summary>
        public decimal CashOut { get { return _cashout; } set { _cashout = value; } }

        /// <summary>
        /// 保证金占用
        /// 期货保证金占用 期权持仓成本 异化保证金
        /// </summary>
        public decimal Margin { get { return this.CalFutMargin() + this.CalOptPositionCost() + this.CalInnovMargin(); } }

        /// <summary>
        /// 保证金冻结
        /// 期货保证金占用 期权资金占用 异化保证金占用
        /// </summary>
        public decimal MarginFrozen { get { return this.CalFutMarginFrozen() + this.CalOptMoneyFrozen() + this.CalInnovMarginFrozen(); } }

        /// <summary>
        /// 总占用资金
        /// </summary>
        public decimal MoneyUsed{get{ return this.CalFutMoneyUsed() + this.CalOptMoneyUsed() + this.CalInnovMoneyUsed();}}

        /// <summary>
        /// 总净值
        /// </summary>
        public decimal TotalLiquidation { get { return LastEquity + this.CalFutLiquidation() + this.CalOptLiquidation() + this.CalInnovLiquidation() + CashIn - CashOut; } }//帐户总净值
        
        /// <summary>
        /// 总可用资金
        /// 常规计算的可用资金 + 扩展模块提供的配资资金
        /// </summary>
        public decimal AvabileFunds { get { return TotalLiquidation - MoneyUsed + TLCtxHelper.ExContribEvent.GetFinAmmountAvabile(this.ID); } }//帐户总可用资金
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
    }
}
