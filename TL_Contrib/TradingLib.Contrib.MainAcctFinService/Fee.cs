using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.MainAcctFinService
{

    public class Fee:FeeSetting
    {


        /// <summary>
        /// 生成手计费项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Fee CreateCommissionFee(string account, decimal amount)
        {
            Fee f = new Fee();
            f.Account = account;
            f.Amount = amount;
            f.Collected = false;
            f.DateTime = Util.ToTLDateTime();
            f.FeeType = QSEnumFeeType.CommissionFee;
            f.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            f.FeeStatus = QSEnumFeeStatus.Charged;
            f.Error = string.Empty;
            return f;
        }

        /// <summary>
        /// 生成服务费计费项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Fee CreateServiceFee(string account, decimal amount)
        {
            Fee f = new Fee();
            f.Account = account;
            f.Amount = amount;
            f.Collected = false;
            f.DateTime = Util.ToTLDateTime();
            f.FeeType = QSEnumFeeType.FinServiceFee;
            f.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            f.FeeStatus = QSEnumFeeStatus.Charged;
            f.Error = string.Empty;
            return f;
        }


        

        public override string ToString()
        {
            return string.Format("{0} Charge Account:{1} Amount:{2} Type:{3} Collected:{4}", this.Settleday, this.Account, this.Amount, this.FeeType, this.Collected);
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}", this.ID, this.Account, Util.GetEnumDescription(this.FeeType), this.Amount.ToFormatStr());
            }
        }
    }
}
