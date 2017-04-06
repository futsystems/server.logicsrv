using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.APIService
{
    public enum EnumBusinessType
    { 
        /// <summary>
        /// 普通类别
        /// </summary>
        [Description("普通出入金")]
        Normal,

        /// <summary>
        /// 配资入金 资金会根据账户资金按杠杆比例自动调整优先资金
        /// </summary>
        [Description("配资入金")]
        LeverageDeposit,

        /// <summary>
        /// 减少配资 减少账户优先资金 用于降低风险
        /// </summary>
        [Description("减少配资")]
        CreditWithdraw
    }
    public class CashOperation
    {
        public CashOperation()
        {
            this.Status = QSEnumCashInOutStatus.PENDING;
        }
        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get;set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount {get;set;}

        /// <summary>
        /// 时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 出入金类别
        /// </summary>
        public QSEnumCashOperation OperationType { get; set; }

        /// <summary>
        /// 网关类别
        /// </summary>
        public QSEnumGateWayType GateWayType { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public QSEnumCashInOutStatus Status { get; set; }

        /// <summary>
        /// 单号引用
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 业务类别
        /// 普通 不执行优先资金操作
        /// 配资 执行优先资金操作
        /// </summary>
        public EnumBusinessType  BusinessType { get; set; }

        /// <summary>
        /// 分区编号
        /// </summary>
        public int Domain_ID { get; set; }


        /// <summary>
        /// 出入金操作生成对应的出入金记录
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static CashTransaction GenCashTransaction(CashOperation operation)
        {
            CashTransactionImpl txn = new CashTransactionImpl();
            txn.Account = operation.Account;
            txn.Amount = operation.Amount;
            txn.EquityType = QSEnumEquityType.OwnEquity;
            txn.TxnType = operation.OperationType;
            txn.TxnRef = operation.Ref;
            txn.DateTime = Util.ToTLDateTime();
            txn.Comment = "在线出入金";
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return txn;
        }

        public static CashTransaction GenCommissionTransaction(CashOperation operation)
        {
            CashTransactionImpl txn = new CashTransactionImpl();
            txn.Account = operation.Account;
            txn.Amount = 0;
            txn.EquityType = QSEnumEquityType.OwnEquity;
            txn.TxnType =  QSEnumCashOperation.WithDraw;
            txn.TxnRef = operation.Ref+"-Commission";
            txn.DateTime = Util.ToTLDateTime();
            txn.Comment = "手续费";
            txn.Operator = "System";
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return txn;
        }


        public static CashTransaction GenCreditTransaction(CashOperation operation,decimal amount)
        {
            CashTransactionImpl txn = new CashTransactionImpl();
            txn.Account = operation.Account;
            txn.Amount = Math.Abs(amount);

            txn.EquityType = QSEnumEquityType.CreditEquity;
            txn.TxnType = amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;
            txn.TxnRef = operation.Ref + "-Credit";
            txn.DateTime = Util.ToTLDateTime();
            txn.Comment = "优先资金调整";
            txn.Operator = "System";
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            return txn;
        }
    }
}
