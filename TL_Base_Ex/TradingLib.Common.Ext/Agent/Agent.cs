using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{


    public class AgentImpl : AgentSetting, IAgent
    {

        ThreadSafeList<CashTransaction> cashtranslsit = new ThreadSafeList<CashTransaction>();//当日出入金记录
        ThreadSafeList<AgentCommissionSplit> splitlist = new ThreadSafeList<AgentCommissionSplit>();//当日手续费拆分记录

        Manager _manger = null;
        public void BindManager(Manager manager)
        {
            manager.AgentAccount = this;
            _manger = manager;
        }

        public void LoadCommissionSplit(AgentCommissionSplit split)
        {
            splitlist.Add(split);
        }

        /// <summary>
        /// 手续费成本
        /// </summary>
        public decimal CommissionCost { get { return splitlist.Where(split => !split.Settled).Sum(split => split.CommissionCost); } }

        /// <summary>
        /// 手续费收入
        /// </summary>
        public decimal CommissionIncome { get { return splitlist.Where(split => !split.Settled).Sum(split => split.CommissionIncome); } }

        /// <summary>
        /// 出入金数据通过CashTransaction进行统计获得
        /// </summary>
        #region 出入金金额


        /// <summary>
        /// 帐户资金操作
        /// </summary>
        /// <param name="txn"></param>
        public void LoadCashTrans(CashTransaction txn)
        {
            cashtranslsit.Add(txn);
        }


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

        #endregion

        public decimal SubStaticEquity
        {
            get
            {
                if (_manger == null) return 0;

                //直客静态权益
                decimal custStaticEquity = _manger.GetDirectAccounts().Sum(ac => ac.StaticEquity);

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentStaticEquity = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentStaticEquity += mgr.AgentAccount.SubStaticEquity;
                    }
                    else
                    {
                        agentStaticEquity += mgr.AgentAccount.StaticEquity;
                    }
                }

                return custStaticEquity + agentStaticEquity;
            }
        }

        public decimal StaticEquity
        {
            get
            {
                return this.LastCredit + this.CashIn - this.CashOut;
            }
        }

        public decimal NowEquity
        {
            get
            {
                return this.StaticEquity + this.RealizedPL + this.UnRealizedPL - this.CommissionCost;
            }
        }

        /// <summary>
        /// 当前信用额度
        /// </summary>
        public decimal NowCredit
        {
            get
            {
                return this.LastEquity + this.CreditCashIn - this.CreditCashOut;
            }
        }

        public decimal RealizedPL
        {
            get
            {
                if (_manger == null) return 0;
                List<IAccount> list = _manger.GetVisibleAccount().ToList();
                return list.Sum(acc => acc.RealizedPL);
            }
        }

        public decimal UnRealizedPL
        {
            get
            {
                if (_manger == null) return 0;
                List<IAccount> list = _manger.GetVisibleAccount().ToList();
                return list.Sum(acc => acc.UnRealizedPL);
            }
        }



    }

}
