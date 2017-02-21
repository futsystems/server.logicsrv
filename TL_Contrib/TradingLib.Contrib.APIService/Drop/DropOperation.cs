using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropCashOperation:Drop
    {
        CashOperation _op = null;
        public DropCashOperation(CashOperation op)
        {
            _op = op;
        }

        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get { return _op.Account; } }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get { return string.Format("{0}[{1}]", _op.Amount.ToFormatStr(), _op.Amount.ToChineseStr()); } }

        /// <summary>
        /// 编号
        /// </summary>
        public string Ref { get { return _op.Ref; } }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get { return Util.GetEnumDescription(_op.OperationType); } }
    }
}
