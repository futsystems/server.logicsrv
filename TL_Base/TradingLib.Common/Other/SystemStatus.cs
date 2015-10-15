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
            this.StartUpTime = 0;
            this.LastSettleday = 0;
            this.Tradingday = 0;
            this.NextSettleTime = 0;
            this.IsSettleNormal = true;
            this.ClearCentreStatus = QSEnumClearCentreStatus.UNKNOWN;
            this.TotalAccountNum = 0;

        }

        /// <summary>
        /// 系统启动时间
        /// </summary>
        public long StartUpTime { get; set; }

        /// <summary>
        /// 上个结算日
        /// </summary>
        public int LastSettleday { get; set; }

        /// <summary>
        /// 当前交易日
        /// </summary>
        public int Tradingday { get; set; }

        /// <summary>
        /// 下次结算时间
        /// </summary>
        public long NextSettleTime { get; set; }

        /// <summary>
        /// 结算系统是否正常
        /// </summary>
        public bool IsSettleNormal { get; set; }

        /// <summary>
        /// 清算中心是否开启
        /// </summary>
        public QSEnumClearCentreStatus ClearCentreStatus { get; set; }

        /// <summary>
        /// 账户总数
        /// </summary>
        public int TotalAccountNum { get; set; }
        



        public  string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(this.StartUpTime);
            sb.Append(d);
            sb.Append(this.LastSettleday);
            sb.Append(d);
            sb.Append(this.Tradingday);
            sb.Append(d);
            sb.Append(this.NextSettleTime);
            sb.Append(d);
            sb.Append(this.IsSettleNormal);
            sb.Append(d);
            sb.Append(this.ClearCentreStatus.ToString());
            sb.Append(d);
            sb.Append(this.TotalAccountNum.ToString());
            return sb.ToString();
        }

        public  void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.StartUpTime = long.Parse(rec[0]);
            this.LastSettleday = int.Parse(rec[1]);
            this.Tradingday = int.Parse(rec[2]);
            this.NextSettleTime = long.Parse(rec[3]);
            this.IsSettleNormal = bool.Parse(rec[4]);
            this.ClearCentreStatus = (QSEnumClearCentreStatus)Enum.Parse(typeof(QSEnumClearCentreStatus), rec[5]);
            this.TotalAccountNum = int.Parse(rec[6]);
        }
    }
}
