using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib
{
    /*
    public class FinSummary:GeneralStatistic,IFinSummary
    {
        public static IFinSummary GetFinSummary(IClearCentreBase c, FinServiceCentre f)
        {
            return new FinSummary(c, f);
        }
        IClearCentreBase _cc;//清算中心
        FinServiceCentre _fincentre;//配资中心

        public FinSummary(IClearCentreBase c, FinServiceCentre f)
        {
            _cc = c;
            _fincentre = f;
            List<IAccount> l = new List<IAccount>();
            foreach (IAccount acc in _cc.Accounts)
            {
                if (acc.Category == QSEnumAccountCategory.LOANEE)
                {
                    l.Add(acc);
                }
            }
            
            this.SetAccounts(l.ToArray());
        }
        /// <summary>
        /// 累计激活的账户个数
        /// </summary>
        public decimal NumActived { get { return _fincentre.NumActived; } }

        /// <summary>
        /// 配资费用总和
        /// </summary>
        public decimal SumFinFee { get { return _fincentre.TotalFee; } }

        /// <summary>
        /// 配资额度总和
        /// </summary>
        public decimal SumFinAmmount { get { return _fincentre.TotalFinAmmount; } }

        /// <summary>
        /// 全日收息
        /// </summary>
        public decimal SumFinAmmountIntereset { get { return _fincentre.TotalInterestAmmount; } }
        /// <summary>
        /// 分红
        /// </summary>
        public decimal SumFinAmmountBonus { get { return _fincentre.TotalBonusAmmount; } }
        /// <summary>
        /// 手续费加成
        /// </summary>
        public decimal SumFinAmmountCommission { get { return 0; } }
        /// <summary>
        /// 夜盘收息
        /// </summary>
        public decimal SumFinAmmountNight { get { return 0; } }
        /// <summary>
        /// 日盘收息
        /// </summary>
        public decimal SumFinAmmountDay { get { return 0; } }



        /// <summary>
        /// 配资累计使用融资额度
        /// </summary>
        public decimal SumMarginUsed { get { return _fincentre.MarginUsed; } }

        /// <summary>
        /// 累计手续费收入
        /// </summary>
        public decimal SumCommissionIn { get { return this.SumCommission; } }

        /// <summary>
        /// 累计入金
        /// </summary>
        public decimal SumDeposit { get {
            decimal r = 0;
            foreach (IAccount a in this)
            {
                r += a.CashIn;
            }
            return r;
        } }

        /// <summary>
        /// 累计出金
        /// </summary>
        public decimal SumWithdraw {

            get
            {
                decimal r = 0;
                foreach (IAccount a in this)
                {
                    r += a.CashOut;
                }
                return r;

            }
        }
    }***/
}
