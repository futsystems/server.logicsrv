using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 针对一组账户进行实时统计
    /// </summary>
    public class GeneralStatistic : AccountsSet
    {
        //public GeneralStatistic(ClearCentreBase cc)
        //    : base(cc)
        //{
        //
        //
        //}

        /// <summary>
        /// 获得某个类别帐户的统计数据
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static GeneralStatistic GetFinStatForSim(IClearCentreBase c, QSEnumAccountCategory cat = QSEnumAccountCategory.DEALER)
        {
            GeneralStatistic s = new GeneralStatistic();
            IEnumerable<IAccount> list = 
            from acc in c.Accounts
                where acc.Category == cat
            select acc;

            s.SetAccounts(list.ToArray());
            return s;
        }


        public int NumTotalAccount { get { return this.Count; } }
        public int NumOrders
        {
            get
            {
                int n = 0;

                foreach (IAccount a in this)
                {
                    n += a.Orders.Count();
                }
                return n;
            }
        }

        public int NumTrades
        {
            get
            {
                int n = 0;
                foreach (IAccount a in this)
                {
                    n += a.Trades.Count();
                }
                return n;
            }
        }
        public decimal SumBuyPower
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.GetFundAvabile();
                }
                return d;
            }
        }
        public decimal SumEquity
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.NowEquity;
                }
                return d;
            }
        }
        public decimal SumRealizedPL
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.RealizedPL;
                }
                return d;

            }

        }

        public decimal SumUnRealizedPL
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.UnRealizedPL;
                }
                return d;

            }

        }

        public decimal SumCommission
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.Commission;
                }
                return d;

            }
        }

        public decimal SumNetProfit
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.Profit;
                }
                return d;

            }
        }

        public decimal SumMargin
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += a.GetFundUsed();// Margin;
                }
                return d;

            }
        }

        public decimal SumFrozenMargin
        {
            get
            {
                decimal d = 0;
                foreach (IAccount a in this)
                {
                    d += 0;// a.ForzenMargin;
                }
                return d;

            }

        }

        public int NumPositions
        {
            get
            {
                int n = 0;

                foreach (IAccount a in this)
                {
                    foreach (Position p in a.Positions)
                        if (!p.isFlat)
                            n++;
                }
                return n;
            }
        }
        /*
        public void Display()
        {
            debug("-------------账户集常规统计-----------------------------");
            debug("账户数目:" + NumTotalAccount.ToString());
            debug("总委托:" + NumOrders.ToString());
            debug("总成交:" + NumTrades.ToString());
            debug("总仓数:" + NumPositions.ToString());
            debug("--财务统计--");
            debug("总权益:" + decDisp(SumEquity));
            debug("总平仓盈亏:" + decDisp(SumRealizedPL));
            debug("总浮动盈亏:" + decDisp(SumRealizedPL));
            debug("总手续费:" + decDisp(SumRealizedPL));
            debug("总保证金占用:" + decDisp(SumMargin));
            debug("总冻结保证金:" + decDisp(SumFrozenMargin));
            debug("总利润:" + decDisp(SumNetProfit));
            debug("-------------------------------------------------------");

        }**/

    }

}
