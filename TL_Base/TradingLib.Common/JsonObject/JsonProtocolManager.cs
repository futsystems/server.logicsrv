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


        public int bank_id {get;set;}
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


    public class JsonWrapperAgentPaymentInfo
    {
        /// <summary>
        /// 主域编号
        /// </summary>
        public int BaseMGRFK { get; set; }

        /// <summary>
        /// 代理姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代理手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 代理QQ号码
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 银行帐户信息
        /// </summary>
        public JsonWrapperBankAccount BankAccount { get; set; }
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
        /// 资金流入 入金
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 资金流出 出金
        /// </summary>
        public decimal CashOut { get; set; }


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
    /// 交易帐户对应的银行帐户信息
    /// </summary>
    public class JsonWrapperAccountBankAC : JsonObjectBase
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }


        /// <summary>
        /// 银行帐户
        /// </summary>
        public string BankAC { get; set; }

        /// <summary>
        /// 开户分行
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 代理信息
        /// </summary>
        public string AgentInfo { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }
    }
    /// <summary>
    /// 收款银行
    /// </summary>
    public class JsonWrapperReceivableAccount : JsonObjectBase
    {
        /// <summary>
        /// 收款银行ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string Bank_AC { get; set; }

        /// <summary>
        /// 分行地址
        /// </summary>
        public string Branch { get; set; }
    }


    /// <summary>
    /// 出入金操作
    /// </summary>
    public class JsonWrapperCashOperation : JsonObjectBase
    {
        /// <summary>
        /// 主域ID
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

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


        /// <summary>
        /// 出入金操作来源
        /// </summary>
        public QSEnumCashOPSource Source { get; set; }


        /// <summary>
        /// 收款银行ID
        /// 0表示第三方支付
        /// </summary>
        public string  RecvInfo { get; set; }
    }

    public class JsonWrapperCasnTrans
    {
        public int ID { get; set; }
        /// <summary>
        /// 管理主域ID
        /// </summary>
        public int mgr_fk { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

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


    /// <summary>
    /// 管理端界面权限
    /// 用于控制管理端的界面元素的现实
    /// </summary>
    public class UIAccess : JsonObjectBase
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 界面权限名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 头部导航 系统管理
        /// </summary>
        public bool nav_system { get; set; }


        /// <summary>
        /// 系统管理-开启清算中心
        /// </summary>
        public bool nav_system_ccopen { get; set; }

        /// <summary>
        /// 系统管理-关闭清算中心
        /// </summary>
        public bool nav_system_ccclose { get; set; }


        /// <summary>
        /// 系统管理-路由列表
        /// </summary>
        public bool nav_system_router { get; set; }

        /// <summary>
        /// 系统管理-内核状态
        /// </summary>
        public bool nav_system_corestatus { get; set; }

        /// <summary>
        /// 基础数据
        /// </summary>
        public bool nav_basic { get; set; }

        /// <summary>
        /// 基础数据-交易时间段
        /// </summary>
        public bool nav_basic_mktime { get; set; }

        /// <summary>
        /// 基础数据-交易所
        /// </summary>
        public bool nav_basic_exchange { get; set; }

        /// <summary>
        /// 基础数据-品种管理
        /// </summary>
        public bool nav_basic_security { get; set; }

        /// <summary>
        /// 基础数据-合约管理
        /// </summary>
        public bool nav_basic_symbol { get; set; }

        /// <summary>
        /// 柜员管理
        /// </summary>
        public bool nav_manager { get; set; }

        /// <summary>
        /// 柜员管理-柜员列表
        /// </summary>
        public bool nav_manager_management { get; set; }

        /// <summary>
        /// 柜员管理-资费管理
        /// </summary>
        public bool nav_manager_feeconfig { get; set; }

        /// <summary>
        /// 财务管理
        /// </summary>
        public bool nav_finance { get; set; }

        /// <summary>
        /// 财务管理-财务中心
        /// </summary>
        public bool nav_finance_fincentre { get; set; }

        /// <summary>
        /// 财务管理-在线支付
        /// </summary>
        public bool nav_finance_payonline { get; set; }

        /// <summary>
        /// 财务管理-出纳管理
        /// </summary>
        public bool nav_finance_cashercentre { get; set; }

        /// <summary>
        /// 记录与报表
        /// </summary>
        public bool nav_report { get; set; }

        /// <summary>
        /// 记录报表-记录查询
        /// </summary>
        public bool nav_report_acchistinfo { get; set; }

        /// <summary>
        /// 记录报表-出入金查询
        /// </summary>
        public bool nav_report_acccashtrans { get; set; }

        /// <summary>
        /// 记录报表-结算单
        /// </summary>
        public bool nav_report_accsettlement { get; set; }


        /// <summary>
        /// 记录报表-代理出入金
        /// </summary>
        public bool nav_report_agentcashtrans { get; set; }


        /// <summary>
        /// 记录报表-代理结算
        /// </summary>
        public bool nav_report_agentsettlement { get; set; }

        /// <summary>
        /// 代理分润
        /// </summary>
        public bool nav_report_agentreport { get; set; }
    }
}
