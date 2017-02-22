using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.APIService
{
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
    }
}
