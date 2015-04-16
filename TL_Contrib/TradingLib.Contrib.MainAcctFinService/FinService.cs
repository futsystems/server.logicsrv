using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.MainAcctFinService
{

    
    /// <summary>
    /// 配资服务
    /// 用于设定服务参数
    /// </summary>
    public class FinService : FinServiceSetting
    {

        public FinServiceSetting ToFinServiceSetting()
        {
            FinServiceSetting target = new FinServiceSetting();
            target.Account = this.Account;
            target.ChargeFreq = this.ChargeFreq;
            target.ChargeMethod = this.ChargeMethod;
            target.ChargeTime = this.ChargeTime;
            target.ChargeValue = this.ChargeValue;
            target.InterestType = this.InterestType;
            target.ServiceType = this.ServiceType;
            return target;
        }
        /// <summary>
        /// 交易帐户对象
        /// </summary>
        public IAccount oAccount { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        { 
        
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            IAccount acc = TLCtxHelper.ModuleAccountManager[this.Account];
            if (acc != null)
            {
                this.oAccount = acc;
            }
            else
            { 
                Util.Warn(string.Format("Account:{0} do not exist",this.Account));
            }

        }

        public bool IsValid
        {
            get
            {
                if (this.oAccount == null)
                    return false;
                return true;
            }
        }

        public string GetDescription()
        {
            //按利息收取
            if (this.ServiceType == QSEnumFinServiceType.Interest)
            {
                return string.Format("{0}以{1}{2}收取利息,金额:{3}", Util.GetEnumDescription(this.ChargeFreq), Util.FormatDecimal(this.ChargeValue), Util.GetEnumDescription(this.InterestType), Util.FormatDecimal(CalServiceFee()));
            }
            return "未设置";
        }
        /// <summary>
        /// 计算当日配资服务费
        /// </summary>
        /// <returns></returns>
        public decimal CalServiceFee()
        {
            //按利息
            if (this.ServiceType == QSEnumFinServiceType.Interest)
            {
                if (this.InterestType == QSEnumInterestType.ByPoint)
                {
                    return this.oAccount.Credit / 10000 * this.ChargeValue;
                }
                if (this.InterestType == QSEnumInterestType.ByPercent)
                {
                    return this.oAccount.Credit * this.ChargeValue / 100;
                }
                if (InterestType == QSEnumInterestType.ByMoney)
                {
                    return this.ChargeValue;
                }
            }
            //按盈利分红返回计费值
            if(this.ServiceType == QSEnumFinServiceType.Bonus)
            {
                if(this.oAccount.Profit <=0)
                {
                    return 0;
                }
                else
                {
                    return this.oAccount.Profit*this.ChargeValue/100;
                }
            }

            return 0;
        }
    }
}
