using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class CashOperationRequest
    {
        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 网关类别
        /// </summary>
        public string GateWay { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 出入金请求处理备注
        /// </summary>
        public string ProcessComment { get; set; }

        public static string Serialize(CashOperationRequest info)
        {
            if (info == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(info.Account);
            sb.Append(d);
            sb.Append(info.Amount);
            sb.Append(d);
            sb.Append(info.GateWay);
            sb.Append(d);
            sb.Append(info.RefID);
            sb.Append(d);
            sb.Append(info.Args);
            sb.Append(d);
            sb.Append(info.ProcessComment);
            return sb.ToString();
        }

        public static CashOperationRequest Deserialize(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;
            string[] rec = content.Split(',');
            CashOperationRequest info = new CashOperationRequest();
            info.Account = rec[0];
            info.Amount = decimal.Parse(rec[1]);
            info.GateWay = rec[2];
            info.RefID = rec[3];
            info.Args = rec[4];
            info.ProcessComment = rec[5];
            return info;
        }
    }
}
