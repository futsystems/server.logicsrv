using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;


namespace Lottoqq.MJService
{
    public class MJServiceTracker
    {

        ConcurrentDictionary<int, MJService> idxmjmap = new ConcurrentDictionary<int, MJService>();
        ConcurrentDictionary<string, MJService> accountmjmap = new ConcurrentDictionary<string, MJService>();

        //GenericTracker<MJService> mjlist = new GenericTracker<MJService>();

        public MJServiceTracker()
        {
            //从数据库加载秘籍服务
            foreach (MJService mj in MMJService.SelectMJService())
            {
                try
                {
                    AddMJService(mj);

                }
                catch(Exception ex)
                {

                }
            }
        }


        /// <summary>
        /// 添加秘籍服务
        /// </summary>
        /// <param name="mj"></param>
        public void AddMJService(MJService mj)
        {
            if (mj.ID > 0)
            {
                //秘籍服务所绑定的帐号不存在
                if (mj.Account == null)
                {
                    throw new MJErrorAccountNull();
                }
                //如果未加载该帐户的秘籍服务 则进行加载并绑定
                if (!HaveMJService(mj.Account.ID))
                {
                    //Util.Debug("--------------为帐户:" + mj.Account.ID + " 添加秘籍服务-------------------");
                    accountmjmap.TryAdd(mj.Account.ID, mj);
                    idxmjmap.TryAdd(mj.ID, mj);

                    //将秘籍服务绑定到帐户
                    //mj.Account.MJService = mj;
                    mj.Account.BindService(mj);
                }
                else
                {
                    throw new MJErrorMJServiceLoaded();
                }

            }
            else
            {
                MMJService.InsertMJService(mj);
                this.AddMJService(mj);
            }
        }
        /// <summary>
        /// 获得所有秘籍服务数组
        /// </summary>
        public MJService[] MJServices { get { return idxmjmap.Values.ToArray(); } }
        /// <summary>
        /// 检查某个帐号是否设置了秘籍服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool HaveMJService(string account)
        {
            if (accountmjmap.Keys.Contains(account))
                return true;
            return false;
        }




        /// <summary>
        /// 通过交易帐号获得秘籍服务对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public MJService this[string account]
        {
            get
            {
                MJService mj = null;
                if (accountmjmap.TryGetValue(account, out mj))
                {
                    return mj;
                }
                return null;
            }
        }
        /// <summary>
        /// 更新秘籍收费类别
        /// </summary>
        /// <param name="mj"></param>
        /// <param name="type"></param>
        public void UpdateMJFeeType(MJService mj, QSEnumMJFeeType type)
        {
            mj.FeeType = type;
            MMJService.UpdateFeeType(mj);
            
        }
        /// <summary>
        /// 更新秘籍级别
        /// </summary>
        /// <param name="mj"></param>
        /// <param name="level"></param>
        public void UpdateMJLevel(MJService mj, QSEnumMJServiceLevel level)
        {
            mj.Level = level;
            MMJService.UpdateLevel(mj);
        }

        /// <summary>
        /// 更新秘籍过期时间
        /// </summary>
        /// <param name="mj"></param>
        /// <param name="time"></param>
        public void UpdateMJExpire(MJService mj, DateTime time)
        {
            mj.ExpiredDate = time;
            MMJService.UpdateExpiredDate(mj);
        }

    }
}
