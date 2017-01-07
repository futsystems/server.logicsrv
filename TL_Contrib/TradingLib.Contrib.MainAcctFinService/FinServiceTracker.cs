using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.MainAcctFinService
{
    public class FinServiceTracker
    {
        ConcurrentDictionary<string, FinService> finserviceMap = new ConcurrentDictionary<string, FinService>();

        public FinServiceTracker()
        {
            foreach (var fs in ORM.MFinService.SelectFinServices())
            {
                //初始化配资服务
                fs.Init();
                if (!fs.IsValid)
                    continue;

                if (!finserviceMap.Keys.Contains(fs.Account))
                {
                    finserviceMap.TryAdd(fs.Account, fs);
                }
                else
                {
                    //Util.Warn(string.Format("Account:{0} already have finservice registed", fs.Account));
                }
            }
        }

        /// <summary>
        /// 通过交易帐户获得配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public FinService this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account))
                    return null;
                FinService target = null;
                if (finserviceMap.TryGetValue(account, out target))
                {
                    return target;
                }
                return null;
            }
        }


        /// <summary>
        /// 删除某个交易帐户的配资服务
        /// </summary>
        /// <param name="account"></param>
        public void DelFinService(string account)
        { 
            FinService target = null;
            finserviceMap.TryRemove(account, out target);
            ORM.MFinService.DeleteFinService(account);
        }

        /// <summary>
        /// 更新配资服务
        /// </summary>
        /// <param name="fs"></param>
        public void UpdateFinService(FinServiceSetting fs)
        {
            FinService target = null;
            if (finserviceMap.TryGetValue(fs.Account, out target))
            {
                target.ChargeFreq = fs.ChargeFreq;
                target.ChargeMethod = fs.ChargeMethod;
                target.ChargeTime = fs.ChargeTime;
                target.ChargeValue = fs.ChargeValue;
                target.InterestType = fs.InterestType;
                target.ServiceType = fs.ServiceType;

                ORM.MFinService.UpdateFinService(fs);
            }
            else
            {
                target = new FinService();
                target.Account = fs.Account;
                target.ChargeFreq = fs.ChargeFreq;
                target.ChargeMethod = fs.ChargeMethod;
                target.ChargeTime = fs.ChargeTime;
                target.ChargeValue = fs.ChargeValue;
                target.InterestType = fs.InterestType;
                target.ServiceType = fs.ServiceType;

                target.Init();

                if (target.IsValid)
                {
                    ORM.MFinService.InsertFinService(fs);
                    if (!finserviceMap.Keys.Contains(fs.Account))
                    {
                        finserviceMap.TryAdd(target.Account, target);
                    }
                    else
                    {
                        //Util.Warn(string.Format("Account:{0} already have finservice registed", target.Account));
                    }
                }
                
            }
        }

        /// <summary>
        /// 插入计费项目
        /// </summary>
        /// <param name="f"></param>
        public void InsertFee(Fee f)
        {
            ORM.MFee.InsertFee(f);
        }

        /// <summary>
        /// 获得某个交易日的所有计费记录
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public IEnumerable<Fee> GetFees(int settleday)
        {
            return ORM.MFee.SelectFees(settleday);
        }
        /// <summary>
        /// 设置收费已完成
        /// </summary>
        /// <param name="f"></param>
        public void FeeCollected(Fee f)
        {
            f.Collected = true;
            ORM.MFee.UpdateFeeCollectStatus(f);
        }

        /// <summary>
        /// 设置收费未完成
        /// </summary>
        /// <param name="f"></param>
        public void FeeUnCollected(Fee f)
        {
            f.Collected = false;
            ORM.MFee.UpdateFeeCollectStatus(f);
        }

        /// <summary>
        /// 更新计费状态
        /// </summary>
        /// <param name="f"></param>
        public void UpdateFeeStatus(Fee f,QSEnumFeeStatus status,string error="")
        {
            f.FeeStatus = status;
            f.Error = error;
            ORM.MFee.UpdateFeeStatus(f);
        }
        /// <summary>
        /// 通过ID获得对应的Fee对象
        /// </summary>
        /// <param name="id"></param>
        public Fee GetFee(int id)
        {
            return ORM.MFee.GetFee(id);
        }

        public FeeSetting GetFeeSetting(int id)
        {
            return ORM.MFee.GetFeeSetting(id);
        }

        /// <summary>
        /// 更新收费项目金额和备注
        /// 当收费项目处于收费状态则可以进行修改，
        /// 否则需要回滚该收费项目然后再进行修改
        /// </summary>
        /// <param name="fee"></param>
        public void UpdateFee(FeeSetting fee)
        {
            ORM.MFee.UpdateFee(fee);
        }
    }
}
