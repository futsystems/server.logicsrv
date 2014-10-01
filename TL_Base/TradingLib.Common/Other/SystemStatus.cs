using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class SystemStatus
    {
        public SystemStatus()
        {
            this.LastSettleday = 0;
            this.CurrentTradingday = 0;
            this.NextTradingday = 0;
            this.IsSettleNormal = true;
            this.IsClearCentreOpen = true;
            this.TotalAccountNum = 0;

        }
        /// <summary>
        /// 上个结算日
        /// </summary>
        public int LastSettleday { get; set; }
        /// <summary>
        /// 当前交易日
        /// </summary>
        public int CurrentTradingday { get; set; }
        /// <summary>
        /// 按结算日计算的下个交易日
        /// </summary>
        public int NextTradingday { get; set; }
        /// <summary>
        /// 当前是否是交易
        /// </summary>
        public bool IsTradingday { get; set; }
        /// <summary>
        /// 结算系统是否正常
        /// </summary>
        public bool IsSettleNormal { get; set; }

        /// <summary>
        /// 清算中心是否开启
        /// </summary>
        public bool IsClearCentreOpen { get; set; }

        /// <summary>
        /// 账户总数
        /// </summary>
        public int TotalAccountNum { get; set; }

        /// <summary>
        /// 是否检查开市时间
        /// </summary>
        public bool MarketOpenCheck { get; set; }

        /// <summary>
        /// 是否运行在开发模式
        /// </summary>
        public bool IsDevMode { get; set; }
        public  string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.LastSettleday.ToString());
            sb.Append(d);
            sb.Append(this.CurrentTradingday.ToString());
            sb.Append(d);
            sb.Append(this.NextTradingday.ToString());
            sb.Append(d);
            sb.Append(this.IsTradingday.ToString());
            sb.Append(d);
            sb.Append(this.IsSettleNormal.ToString());
            sb.Append(d);
            sb.Append(this.IsClearCentreOpen.ToString());
            sb.Append(d);
            sb.Append(this.TotalAccountNum.ToString());
            sb.Append(d);
            sb.Append(this.MarketOpenCheck.ToString());
            sb.Append(d);
            sb.Append(this.IsDevMode.ToString());
            return sb.ToString();
        }

        public  void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.LastSettleday = int.Parse(rec[0]);
            this.CurrentTradingday = int.Parse(rec[1]);
            this.NextTradingday = int.Parse(rec[2]);
            this.IsTradingday = bool.Parse(rec[3]);
            this.IsSettleNormal = bool.Parse(rec[4]);
            this.IsClearCentreOpen = bool.Parse(rec[5]);
            this.TotalAccountNum = int.Parse(rec[6]);
            this.MarketOpenCheck = bool.Parse(rec[7]);
            this.IsDevMode = bool.Parse(rec[8]);
        }
    }
}
