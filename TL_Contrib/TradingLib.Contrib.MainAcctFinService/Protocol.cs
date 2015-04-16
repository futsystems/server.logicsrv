using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.MainAcctFinService
{
    public class FeeSetting
    {

        public FeeSetting()
        {
            this.ID = 0;
            this.Settleday = 0;
            this.Account = string.Empty;
            this.Amount = 0;
            this.FeeType = QSEnumFeeType.CommissionFee;
            this.DateTime = 0;
            this.Collected = false;
            this.Comment = string.Empty;
        }

        public int ID { get; set; }
        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 费用类别
        /// </summary>
        public QSEnumFeeType FeeType { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 是否已经收取
        /// </summary>
        public bool Collected { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 收费方式
        /// </summary>
        public QSEnumChargeMethod ChargeMethod { get; set; }

        /// <summary>
        /// 收费时间
        /// </summary>
        public QSEnumChargeTime ChargeTime { get; set; }


        /// <summary>
        /// 收费状态
        /// </summary>
        public QSEnumFeeStatus FeeStatus { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
        


    public class FinServiceSetting
    {

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 服务方式
        /// </summary>
        public QSEnumFinServiceType ServiceType { get; set; }

        /// <summary>
        /// 计费周期
        /// </summary>
        public QSEnumChargeFreq ChargeFreq { get; set; }

        /// <summary>
        /// 计息方式
        /// </summary>
        public QSEnumInterestType InterestType { get; set; }

        /// <summary>
        /// 收费值
        /// </summary>
        public decimal ChargeValue { get; set; }

        /// <summary>
        /// 收费时间
        /// </summary>
        public QSEnumChargeTime ChargeTime { get; set; }

        /// <summary>
        /// 收费方式
        /// </summary>
        public QSEnumChargeMethod ChargeMethod { get; set; }



    }


    public class FinServiceCentreStatus
    {

        public void Reset()
        {
            this.ChargeCommissionAfterTimeSpan = false;
            this.ChargeServiceAfterTimeSpan = false;
            this.ChargeServiceBeforeTimeSpan = false;
            this.QryAfterTimeSpane = false;
        }
        public bool ChargeServiceBeforeTimeSpan { get; set; }


        public bool QryAfterTimeSpane { get; set; }
        public bool ChargeCommissionAfterTimeSpan { get; set; }
        public bool ChargeServiceAfterTimeSpan { get; set; }
    }

}
