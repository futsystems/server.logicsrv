using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    public class DropDinpayment:Drop
    {

        public string InputChartset { get; set; }

        public string InterfaceVersion { get; set; }

        public string MarchantCode { get; set; }

        public string NotifyUrl { get; set; }

        public string OrderAmount { get; set; }

        public string OrderNo { get; set; }

        public string OrderTime { get; set; }

        //public string SignType { get; set; }

        public string ProductCode { get; set; }

        public string ProductDesc { get; set; }

        public string ProductName { get; set; }

        public string ProductNum { get; set; }

        public string ReturnUrl { get; set; }

        public string ServiceType { get; set; }

        public string ShowRrl { get; set; }

        public string ExtendParam { get; set; }

        public string ExtraReturnParam { get; set; }

        public string BankCode { get; set; }

        public string ClientIP { get; set; }

        public string ClientIPCheck { get; set; }

        public string RedoFlag { get; set; }

        public string PayType { get; set; }

        public string Sign { get; set; }

        public string SignType { get; set; }

        public string PayUrl { get; set; }
        /// <summary>
        /// 交易账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }



    }
}
