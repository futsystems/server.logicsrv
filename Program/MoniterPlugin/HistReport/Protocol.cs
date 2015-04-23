using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Protocol
{
    /// <summary>
    /// 分区统计
    /// </summary>
    public class DomainStatistic
    {

        /// <summary>
        /// 分区ID
        /// </summary>
        public int Domain_ID { get; set; }
        /// <summary>
        /// 交易帐户数目
        /// </summary>
        public int AccNumTotal { get; set; }


        /// <summary>
        /// 登入交易帐户
        /// </summary>
        public int AccNumRegisted { get; set; }

        /// <summary>
        /// 最大交易帐户登入数
        /// </summary>
        public int MaxAccNumRegisted { get; set; }

        /// <summary>
        /// 发生交易的交易帐户
        /// </summary>
        public int AccNumTraded { get; set; }



        /// <summary>
        /// 所有保证金数量
        /// </summary>
        public decimal MarginTotal { get; set; }

        /// <summary>
        /// 最大保证金占用
        /// </summary>
        public decimal MaxMarginTotal { get; set; }

        /// <summary>
        /// 多头持仓数量
        /// </summary>
        public int LongPositionHold { get; set; }

        /// <summary>
        /// 最大多头持仓
        /// </summary>
        public decimal MaxLongPositionHold { get; set; }

        /// <summary>
        /// 空头持仓数量
        /// </summary>
        public int ShortPositionHold { get; set; }


        /// <summary>
        /// 最大空头持仓
        /// </summary>
        public int MaxShortPositionHold { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal RealizedPL { get; set; }

        /// <summary>
        /// 持仓盈亏
        /// </summary>
        public decimal UnRealizedPL { get; set; }

        /// <summary>
        /// 总权益
        /// </summary>
        public decimal Equity { get; set; }

        /// <summary>
        /// 总信用额度
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// 总入金
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// 总入金 
        /// </summary>
        public decimal CashOut { get; set; }
    }


    public class SummaryAccount
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 累计入金
        /// </summary>
        public decimal CashIn { get; set; }


        /// <summary>
        /// 累计出金
        /// </summary>
        public decimal CashOut { get; set; }


        /// <summary>
        /// 统计项目
        /// </summary>
        public SummaryAccountItem[] Items { get; set; }
    }
    public class SummaryAccountItem
    {
        public SummaryAccountItem()
        {
            this.Account = string.Empty;
            this.SecCode = string.Empty;
            this.RealizedPL = 0M;
            this.Commission = 0M;
            this.Volume = 0;
        }


        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 交易品种代码
        /// </summary>
        public string SecCode { get; set; }

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal RealizedPL { get; set; }


        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 交易量
        /// </summary>
        public int Volume { get; set; }


    }

    public class SummaryViaSec
    {
        public SummaryViaSec()
        {
            this.Sec_Code = "";
            this.Total_Size = 0;
            this.Total_Commission = 0;
            this.Total_Profit = 0;
            this.Manager_ID = 0;
        }

        /// <summary>
        /// 管理域ID
        /// </summary>
        public int Manager_ID { get; set; }

        /// <summary>
        /// 品种代码
        /// </summary>
        public string Sec_Code { get; set; }

        /// <summary>
        /// 总成交量
        /// </summary>
        public int Total_Size { get; set; }


        /// <summary>
        /// 总佣金
        /// </summary>
        public decimal Total_Commission { get; set; }

        /// <summary>
        /// 总利润
        /// </summary>
        public decimal Total_Profit { get; set; }
    }
}
