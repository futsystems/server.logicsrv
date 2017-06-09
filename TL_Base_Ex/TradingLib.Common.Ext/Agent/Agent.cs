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

        [Newtonsoft.Json.JsonIgnore]
        public Manager Manager { get { return _manger; } }

        /// <summary>
        /// 手续费成本
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CommissionCost { get { return splitlist.Where(split => !split.Settled).Sum(split => split.CommissionCost); } }

        /// <summary>
        /// 手续费收入
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
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
        [Newtonsoft.Json.JsonIgnore]
        public decimal CashIn { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 未结算出金
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CashOut { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }



        /// <summary>
        /// 优先资金 入金
        /// </summary>
        /// 
        [Newtonsoft.Json.JsonIgnore]
        public decimal CreditCashIn { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 优先资金出金
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CreditCashOut { get { return cashtranslsit.Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }

        #endregion
        [Newtonsoft.Json.JsonIgnore]
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

        [Newtonsoft.Json.JsonIgnore]
        public decimal StaticEquity
        {
            get
            {
                return this.LastCredit + this.CashIn - this.CashOut;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
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
        [Newtonsoft.Json.JsonIgnore]
        public decimal NowCredit
        {
            get
            {
                return this.LastEquity + this.CreditCashIn - this.CreditCashOut;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public decimal RealizedPL
        {
            get
            {
                if (_manger == null) return 0;
                //普通代理平仓盈亏为0
                if (this.AgentType == EnumAgentType.Normal)
                {
                    return 0;
                }
                if (this.AgentType == EnumAgentType.SelfOperated)
                {
                    List<IAccount> list = _manger.GetVisibleAccount().ToList();
                    return list.Sum(acc => acc.RealizedPL);
                }
                return 0;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public decimal UnRealizedPL
        {
            get
            {
                if (_manger == null) return 0;
                if (this.AgentType == EnumAgentType.Normal)
                {
                    return 0;
                }
                if (this.AgentType == EnumAgentType.SelfOperated)
                {
                    List<IAccount> list = _manger.GetVisibleAccount().ToList();
                    return list.Sum(acc => acc.UnRealizedPL);
                }
                return 0;
            }
        }


        #region 下级客户信息汇总

        public decimal CustMargin 
        {

            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(ac => ac.Margin);
            }
        
        }//占用保证金

        public decimal CustForzenMargin
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(ac => ac.MarginFrozen);
            }
        }//冻结保证金

        public decimal CustRealizedPL 
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(ac => ac.RealizedPL);
            }
        }


        public decimal CustUnRealizedPL 
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(ac => ac.UnRealizedPL);
            }
        
        }//浮动盈亏

        public decimal CustCashIn 
        {
            get
            {
                if (_manger == null) return 0;

                //直客手续费
                decimal custCashIn = _manger.GetDirectAccounts().Sum(ac => ac.CashIn);

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentCashIn = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentCashIn += mgr.AgentAccount.CustCashIn;
                    }
                    else
                    {
                        agentCashIn += mgr.AgentAccount.CashIn;
                    }
                }

                return custCashIn + agentCashIn;
            }
        }

        public decimal CustCashOut 
        {
            get
            {
                if (_manger == null) return 0;

                //直客手续费
                decimal custCashOut = _manger.GetDirectAccounts().Sum(ac => ac.CashOut);

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentCashOut = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentCashOut += mgr.AgentAccount.CustCashOut;
                    }
                    else
                    {
                        agentCashOut += mgr.AgentAccount.CustCashOut;
                    }
                }

                return custCashOut + agentCashOut;
            }
        }

        public decimal CustCreditCashIn { get { return 0; } }

        public decimal CustCreditCashOut { get { return 0; } }

        #endregion


    }

}
