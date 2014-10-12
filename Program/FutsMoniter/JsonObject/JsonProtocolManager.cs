using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Mixins.JsonObject
{
    /// <summary>
    /// 银行
    /// </summary>
    public class JsonWrapperBank
    {
        /// <summary>
        /// 银行编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 代理 银行信息
    /// </summary>
    public class JsonWrapperBankAccount
    {
        /// <summary>
        /// 主域ID
        /// </summary>
        public int mgr_fk { get; set; }


        public int bank_id { get; set; }
        public JsonWrapperBank Bank { get; set; }



        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 银行帐户号码
        /// </summary>
        public string Bank_AC { get; set; }

        /// <summary>
        /// 开户支行信息
        /// </summary>
        public string Branch { get; set; }


    }


    /// <summary>
    /// 代理财务信息 精简
    /// </summary>
    public class JsonWrapperAgentFinanceInfoLite
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 当前权益信息
        /// </summary>
        public JsonWrapperAgentBalance Balance { get; set; }

        /// <summary>
        /// 待处理提现金额
        /// </summary>
        public decimal PendingDeposit { get; set; }

        /// <summary>
        /// 待处理充值金额
        /// </summary>
        public decimal PendingWithDraw { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 提现记录
        /// </summary>
        public decimal CashOut { get; set; }


    }

    /// <summary>
    /// 代理财务信息 全面
    /// </summary>
    public class JsonWrapperAgentFinanceInfo
    {

        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 银行帐户信息
        /// </summary>
        public JsonWrapperBankAccount BankAccount { get; set; }

        /// <summary>
        /// 当前权益信息
        /// </summary>
        public JsonWrapperAgentBalance Balance { get; set; }

        /// <summary>
        /// 待处理提现金额
        /// </summary>
        public decimal PendingDeposit { get; set; }

        /// <summary>
        /// 待处理充值金额
        /// </summary>
        public decimal PendingWithDraw { get; set; }


        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 提现记录
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// Balance对应的最近结算信息
        /// </summary>
        public JsonWrapperAgentSettle LastSettle { get; set; }


        /// <summary>
        /// 近期出入金操作
        /// </summary>
        public JsonWrapperCashOperation[] LatestCashOperations { get; set; }
    }

    /// <summary>
    /// 代理帐户余额记录
    /// </summary>
    public class JsonWrapperAgentBalance
    {

        /// <summary>
        /// 主域编号
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }
    }


    /// <summary>
    /// 代理结算记录
    /// </summary>
    public class JsonWrapperAgentSettle
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 直客收入
        /// </summary>
        public decimal Profit_Fee { get; set; }


        /// <summary>
        /// 代理佣金收入
        /// </summary>
        public decimal Profit_Commission { get; set; }


        /// <summary>
        /// 上日权益
        /// </summary>
        public decimal LastEquity { get; set; }


        /// <summary>
        /// 结算后权益
        /// </summary>
        public decimal NowEquity { get; set; }
    }


    /// <summary>
    /// 出入金操作
    /// </summary>
    public class JsonWrapperCashOperation
    {
        /// <summary>
        /// 主域ID
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 资金操作
        /// </summary>
        public QSEnumCashOperation Operation { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public QSEnumCashInOutStatus Status { get; set; }
    }

    public class JsonWrapperCasnTrans
    {
        /// <summary>
        /// 
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }


        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 金额 带有方向
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 对应流水
        /// </summary>
        public string TransRef { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }
    }
}
