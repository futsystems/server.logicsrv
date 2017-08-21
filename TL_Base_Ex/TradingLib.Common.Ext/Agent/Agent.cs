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
        public decimal CommissionCost { get { return splitlist.Where(o => !o.Settled).Where(split => !split.Settled).Sum(split => split.CommissionCost * this.GetExchangeRate(split.Settleday,split.Currency)); } }

        /// <summary>
        /// 手续费收入
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CommissionIncome { get { return splitlist.Where(o => !o.Settled).Where(split => !split.Settled).Sum(split => split.CommissionIncome * this.GetExchangeRate(split.Settleday, split.Currency)); } }

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
        public decimal CashIn { get { return cashtranslsit.Where(o=>!o.Settled).Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 未结算出金
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CashOut { get { return cashtranslsit.Where(o => !o.Settled).Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.OwnEquity).Sum(tx => tx.Amount); } }



        /// <summary>
        /// 优先资金 入金
        /// </summary>
        /// 
        [Newtonsoft.Json.JsonIgnore]
        public decimal CreditCashIn { get { return cashtranslsit.Where(o => !o.Settled).Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.Deposit && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }

        /// <summary>
        /// 优先资金出金
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public decimal CreditCashOut { get { return cashtranslsit.Where(o => !o.Settled).Where(tx => !tx.Settled && tx.TxnType == QSEnumCashOperation.WithDraw && tx.EquityType == QSEnumEquityType.CreditEquity).Sum(tx => tx.Amount); } }



        /// <summary>
        /// 结算
        /// </summary>
        public void Settle(int settleday)
        { 
            //管理结算账户普通代理与自营代理统一进行结算 结算格式与投资者账户结算相同，具体代理相关统计信息 存入统计表避免对原有数据结构进行修改
            AccountSettlement settlement = new AccountSettlementImpl();
            settlement.Account = this.Account;
            settlement.Settleday = settleday;
            settlement.LastEquity = this.LastEquity;
            settlement.LastCredit = this.LastCredit;


            //自盈代理
            if (this.AgentType == EnumAgentType.SelfOperated && _manger != null)
            {
                IEnumerable<IAccount> acclist = _manger.GetVisibleAccount();
                AccountSettlement accsettle = null;
                foreach (var acc in acclist)
                {
                    accsettle = TLCtxHelper.ModuleSettleCentre.GetInvestSettlement(acc.ID);
                    if (accsettle == null)
                    {
                        continue;
                    }
                    //累加代理下面所有交易账户的平仓盈亏与盯市浮动盈亏
                    settlement.PositionProfitByDate += accsettle.PositionProfitByDate * this.GetExchangeRate(settleday, acc.Currency);
                    settlement.CloseProfitByDate += accsettle.CloseProfitByDate * this.GetExchangeRate(settleday, acc.Currency);
                }

                //自营代理手续费 为分拆的手续费成本
                settlement.Commission = this.CommissionCost;
            }
            else
            { 
                //普通代理手续费差 以入金的方式入账
                if(this.CommissionCost !=0 || this.CommissionIncome !=0)
                {
                    decimal commissionProfit = this.CommissionIncome - this.CommissionCost;
                    if (commissionProfit != 0)
                    {
                        CashTransaction txn = new CashTransactionImpl();
                        txn.Account = this.Account;
                        txn.EquityType = QSEnumEquityType.OwnEquity;
                        txn.Amount = Math.Abs(commissionProfit);
                        txn.TxnType = commissionProfit > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;
                        txn.Operator = "SETTLE";
                        txn.TxnRef = string.Format("{0}-{1}-{2}-{3}", "SETTLE", this.Account, settleday, "COMISSION");
                        txn.Comment = string.Format("手续费差[{0}]", settleday);

                        //生成唯一序列号
                        TLCtxHelper.ModuleAgentManager.AssignTxnID(txn);
                        this.LoadCashTrans(txn);
                        TLCtxHelper.ModuleDataRepository.NewAgentCashTransactioin(txn);
                    }
                }
            }

            settlement.CashIn = this.CashIn;
            settlement.CashOut = this.CashOut;
            settlement.CreditCashIn = this.CreditCashIn;
            settlement.CreditCashOut = this.CreditCashOut;



            //结算权益 = 昨日权益 + 入金 - 出金 + 平仓盈亏 - 浮动盈亏 - 资产买入额 + 资产卖出额 - 手续费
            settlement.EquitySettled = settlement.LastEquity + settlement.CashIn - settlement.CashOut + settlement.CloseProfitByDate + settlement.PositionProfitByDate - settlement.AssetBuyAmount + settlement.AssetSellAmount - settlement.Commission;
            settlement.CreditSettled = settlement.LastCredit + settlement.CreditCashIn - settlement.CreditCashOut;

            ORM.MAgentSettlement.InsertAgentSettlement(settlement);


            //标注交易所结算记录
            foreach (var split in splitlist)
            {
                split.Settled = true;
                //TLCtxHelper.ModuleDataRepository.MarkExchangeSettlementSettled(settle);
            }

            //标注出入金记录 已结算
            foreach (var txn in cashtranslsit)
            {
                txn.Settled = true;//标注已结算
                //TLCtxHelper.ModuleDataRepository.MarkCashTransactionSettled(txn);统一在结算完毕后执行Mark
            }

            //设置昨日权益等信息
            this.LastEquity = settlement.EquitySettled;
            this.LastCredit = settlement.CreditSettled;
        }


        public void Reset()
        {
            this.cashtranslsit.Clear();
            this.splitlist.Clear();
        }


        #endregion
        [Newtonsoft.Json.JsonIgnore]
        public decimal SubStaticEquity
        {
            get
            {
                if (_manger == null) return 0;

                //直客静态权益
                decimal custStaticEquity = _manger.GetDirectAccounts().Sum(ac => ac.StaticEquity * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday,ac.Currency));

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentStaticEquity = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentStaticEquity += mgr.AgentAccount.SubStaticEquity * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
                    }
                    else
                    {
                        agentStaticEquity += mgr.AgentAccount.StaticEquity * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
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
                return this.LastEquity + this.CashIn - this.CashOut;
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
                return this.LastCredit + this.CreditCashIn - this.CreditCashOut;
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
                    return list.Sum(acc => acc.RealizedPL* this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday,acc.Currency));
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
                    return list.Sum(acc => acc.UnRealizedPL * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));
                }
                return 0;
            }
        }


        #region 下级客户信息汇总
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustMargin 
        {

            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.Margin * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));
            }
        
        }//占用保证金
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustForzenMargin
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.MarginFrozen  *this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));
            }
        }//冻结保证金
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustRealizedPL 
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.RealizedPL * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public decimal CustUnRealizedPL 
        {
            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.UnRealizedPL * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));
            }
        
        }//浮动盈亏
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustCashIn 
        {
            get
            {
                if (_manger == null) return 0;

                //直客手续费
                decimal custCashIn = _manger.GetDirectAccounts().Sum(acc => acc.CashIn * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentCashIn = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentCashIn += mgr.AgentAccount.CustCashIn * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
                    }
                    else
                    {
                        agentCashIn += mgr.AgentAccount.CashIn * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
                    }
                }

                return custCashIn + agentCashIn;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustCashOut 
        {
            get
            {
                if (_manger == null) return 0;

                //直客手续费
                decimal custCashOut = _manger.GetDirectAccounts().Sum(acc => acc.CashOut * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, acc.Currency));

                IEnumerable<Manager> directAgents = _manger.GetDirectAgents();

                decimal agentCashOut = 0;
                //如果是自盈代理 则直接累加该代理的静态权益
                //如果是普通代理 则直接累加该代理的下级静态权益
                foreach (var mgr in directAgents)
                {
                    if (mgr.AgentAccount == null) continue;
                    if (mgr.AgentAccount.AgentType == EnumAgentType.Normal)
                    {
                        agentCashOut += mgr.AgentAccount.CustCashOut * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
                    }
                    else
                    {
                        agentCashOut += mgr.AgentAccount.CustCashOut * this.GetExchangeRate(TLCtxHelper.ModuleSettleCentre.Tradingday, mgr.AgentAccount.Currency);
                    }
                }

                return custCashOut + agentCashOut;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public int CustLongPositionSize
        {

            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.PositionsLong.Sum(pos=>pos.UnsignedSize));
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public int CustShortPositionSize
        {

            get
            {
                if (_manger == null) return 0;
                return _manger.GetVisibleAccount().Sum(acc => acc.PositionsShort.Sum(pos => pos.UnsignedSize));
            }

        }


        [Newtonsoft.Json.JsonIgnore]
        public decimal CustCreditCashIn { get { return 0; } }
        [Newtonsoft.Json.JsonIgnore]
        public decimal CustCreditCashOut { get { return 0; } }

        #endregion


    }

}
