using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        /// <summary>
        /// 昨日权益
        /// </summary>
        public decimal LastEquity { get; set; }

        /// <summary>
        /// 昨日优先资金
        /// </summary>
        public decimal LastCredit { get; set; }

        /// <summary>
        /// 当前权益 经过排查 commission并非线程安全
        /// </summary>
        public decimal NowEquity { get { return TotalLiquidation; } }

        /// <summary>
        /// 平仓盈亏
        /// 交易所结算盯市盈亏 计入 平仓盈亏中
        /// </summary>
        public decimal RealizedPL { get { return this.PendingSettleCloseProfitByDate + this.PendingSettlePositionProfitByDate + this.CalFutRealizedPL() + this.CalOptRealizedPL() + this.CalInnovRealizedPL(); } }

        /// <summary>
        /// 浮动盈亏 
        /// </summary>
        public decimal UnRealizedPL { get { return  + this.CalFutUnRealizedPL() + this.CalOptPositionValue() - this.CalOptPositionCost() + this.CalInnovPositionValue() - this.CalInnovPositionCost(); } }

        /// <summary>
        /// 盯市盈亏
        /// </summary>
        public decimal SettleUnRealizedPL { get { return this.CalFutSettleUnRealizedPL() + this.CalOptSettlePositionValue() - this.CalOptPositionValue() + this.CalInnovSettlePositionValue() - this.CalInnovPositionValue(); } }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get { return this.PendingSettleCommission + this.CalFutCommission() + this.CalOptCommission() + this.CalInnovCommission(); } }

        /// <summary>
        /// 净利润
        /// </summary>
        public decimal Profit { get { return RealizedPL + UnRealizedPL - Commission; } }

        ThreadSafeList<CashTransaction> cashtranslsit = new ThreadSafeList<CashTransaction>();
        /// <summary>
        /// 未结算入金
        /// </summary>
        public decimal CashIn { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 未结算出金
        /// </summary>
        public decimal CashOut { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }


        
        /// <summary>
        /// 优先资金 入金
        /// </summary>
        public decimal CreditCashIn { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 优先资金出金
        /// </summary>
        public decimal CreditCashOut { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }


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
        public decimal AvabileFunds { get { return TotalLiquidation - MoneyUsed + this.Credit + GetAvabileAdjustment(); } }//帐户总可用资金

        /// <summary>
        /// 获得可用资金期货部分调整
        /// 取决于交易参数中浮盈是否可以开仓
        /// </summary>
        /// <returns></returns>
        decimal GetAvabileAdjustment()
        {
            decimal futunpl = this.CalFutUnRealizedPL();
            decimal futclosepl = this.CalFutRealizedPL();
            if (futunpl <= 0) return 0;//如果当前处于浮亏状态，则可用资金调整为0，按正常算法计算可用资金
            //如果浮动盈亏大于0 则按照设置来判断浮盈是否可以开仓
            if (this.GetParamIncludePositionProfit())
            {
                return -1 * futunpl;
            }
            if (this.GetParamIncludeCloseProfit())
            {
                return -1 * futclosepl;
            }
            return 0;
        }

        /// <summary>
        /// 帐户信用额度
        /// </summary>
        //public decimal Credit { get { return TLCtxHelper.ExContribEvent.GetFinAmmountAvabile(this.ID); } }

        /// <summary>
        /// 当前优先资金 = 昨日优先资金 + 今日优先资入金 - 今日优先资金出金
        /// </summary>
        public decimal Credit { get { return LastCredit + CreditCashIn - CreditCashOut + TLCtxHelper.ExContribEvent.GetFinAmmountAvabile(this.ID); } }

        /// <summary>
        /// 帐户资金操作
        /// </summary>
        /// <param name="txn"></param>
        public void CashTrans(CashTransaction txn)
        {
            cashtranslsit.Add(txn);
        }

        ///// <summary>
        ///// 入金
        ///// </summary>
        ///// <param name="amount"></param>
        //public void Deposit(decimal amount)
        //{
        //    amount = Math.Abs(amount);
        //    _cashin += amount;
        //}

        ///// <summary>
        ///// 出金
        ///// 入金,出金均是绝对值,用于记录金额
        ///// </summary>
        ///// <param name="amount"></param>
        //public void Withdraw(decimal amount)
        //{
        //    amount = Math.Abs(amount);
        //    _cashout += amount;
        //}

        ///// <summary>
        ///// 优先资入金
        ///// </summary>
        ///// <param name="amount"></param>
        //public void CreditDeposit(decimal amount)
        //{
        //    amount = Math.Abs(amount);
        //    _creditcashin += amount;
        //}

        ///// <summary>
        ///// 优先资金出金
        ///// </summary>
        ///// <param name="amount"></param>
        //public void CreditWithdraw(decimal amount)
        //{
        //    amount = Math.Abs(amount);
        //    _creditcashout += amount;
        //}
    }
}
