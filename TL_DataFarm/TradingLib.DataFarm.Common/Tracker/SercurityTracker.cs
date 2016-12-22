using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 品种管理器
    /// </summary>
    public class DBSecurityTracker
    {

        Dictionary<string, SecurityFamilyImpl> seccodemap = new Dictionary<string, SecurityFamilyImpl>();
        Dictionary<int, SecurityFamilyImpl> idxcodemap = new Dictionary<int, SecurityFamilyImpl>();

        int _domianId = 1;
        public DBSecurityTracker(int domianId=1)
        {
            _domianId = domianId;
            //从数据库加载品种数据
            foreach (SecurityFamilyImpl sec in ORM.MBasicInfo.SelectSecurity(_domianId))
            {
                //如果交易所不存在 则品种不加载
                if ( MDBasicTracker.ExchagneTracker[sec.exchange_fk] == null)
                {
                    continue;
                }
                seccodemap[sec.Code] = sec;
                idxcodemap[sec.ID] = sec;
            }


            //绑定底层证券
            foreach (SecurityFamilyImpl sec in seccodemap.Values)
            {
                sec.UnderLaying = this[sec.underlaying_fk];
                sec.MarketTime =MDBasicTracker.MarketTimeTracker[sec.mkttime_fk];
                sec.Exchange =MDBasicTracker.ExchagneTracker[sec.exchange_fk];
            }
        }

        public string GetSecurityName(string code)
        {
            SecurityFamilyImpl sec = null;
            if (seccodemap.TryGetValue(code, out sec))
            {
                return sec.Name;
            }
            return "未知";
        }

        public int GetMultiple(string code)
        {
            SecurityFamilyImpl sec = null;
            if (seccodemap.TryGetValue(code, out sec))
            {
                return sec.Multiple;
            }
            return 1;
        }

        /// <summary>
        /// 查找基于某个sec的所有证券品种
        /// 比如IO基于IF,则通过IF查找 基于IF的所有品种
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public SecurityFamilyImpl[] GetUnderlayedOn(SecurityFamily sec)
        {
            seccodemap.Values.Where(t => (t.UnderLaying == sec)).ToArray();
            return null;
        }

        /// <summary>
        /// 通过品种编码获得品种
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SecurityFamilyImpl this[string code]
        {
            get
            {
                SecurityFamilyImpl sec = null;
                if (seccodemap.TryGetValue(code, out sec))
                {
                    return sec;
                }
                return null;
            }
        }


        /// <summary>
        /// 通过数据库编号获得品种
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SecurityFamilyImpl this[int index]
        {
            get
            {
                SecurityFamilyImpl sec = null;
                if (idxcodemap.TryGetValue(index, out sec))
                {
                    return sec;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回所有品种簇
        /// </summary>
        public SecurityFamilyImpl[] Securities
        {
            get
            {
                return idxcodemap.Values.ToArray();
            }
        }

        /// <summary>
        /// 同步品种信息 将品种信息同步到本域
        /// </summary>
        /// <param name="sec"></param>
        public void SyncSecurity(SecurityFamilyImpl sec)
        {
            SecurityFamilyImpl target = null;
            if (seccodemap.TryGetValue(sec.Code, out target))//品种存在 更新品种
            {
                //内存实例更新
                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                //TLCtxHelper.Ctx.debug("mkttime_fk:" + sec.mkttime_fk.ToString() + " exchange_fk:" + sec.exchange_fk.ToString() + " underlay_fk:" + sec.underlaying_fk.ToString());
                target.exchange_fk = sec.exchange_fk;
                target.Exchange = MDBasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = MDBasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying =MDBasicTracker.SecurityTracker[target.underlaying_fk];

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;
                target.Domain_ID = _domianId;

                //同步已经存在的品种 不更新费率信息
                //target.EntryCommission = sec.EntryCommission;
                //target.ExitCommission = sec.ExitCommission;
                //target.Margin = sec.Margin;
                //target.ExtraMargin = sec.ExtraMargin;
                //target.MaintanceMargin = sec.MaintanceMargin;
                //target.Tradeable = sec.Tradeable;

                //数据库更新
                ORM.MBasicInfo.UpdateSecurity(target);
            }
            else//品种不存在 则添加品种
            {
                target = new SecurityFamilyImpl();
                target.Domain_ID = _domianId;

                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                target.exchange_fk = sec.exchange_fk;
                target.Exchange = MDBasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = MDBasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying =MDBasicTracker.SecurityTracker[target.underlaying_fk];

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;
                target.EntryCommission = sec.EntryCommission;
                target.ExitCommission = sec.ExitCommission;
                target.Margin = sec.Margin;
                target.ExtraMargin = sec.ExtraMargin;
                target.MaintanceMargin = sec.MaintanceMargin;
                target.Tradeable = sec.Tradeable;

                ORM.MBasicInfo.InsertSecurity(target);

                //将新增加的品种添加到内存中
                seccodemap[target.Code] = target;
                idxcodemap[target.ID] = target;
            }

        }
        public void UpdateSecurity(SecurityFamilyImpl sec)
        {
            SecurityFamilyImpl target = null;
            if (idxcodemap.TryGetValue(sec.ID, out target))//品种存在 更新品种 通过ID进行更新 用于更新品种Code
            {
                //内存实例更新
                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                //TLCtxHelper.Ctx.debug("mkttime_fk:" + sec.mkttime_fk.ToString() + " exchange_fk:" + sec.exchange_fk.ToString() + " underlay_fk:" + sec.underlaying_fk.ToString());
                target.exchange_fk = sec.exchange_fk;
                target.Exchange = MDBasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = MDBasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying =MDBasicTracker.SecurityTracker[target.underlaying_fk];

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;


                target.EntryCommission = sec.EntryCommission;
                target.ExitCommission = sec.ExitCommission;
                target.Margin = sec.Margin;
                target.ExtraMargin = sec.ExtraMargin;
                target.MaintanceMargin = sec.MaintanceMargin;
                target.Tradeable = sec.Tradeable;
                

                //数据库更新
                ORM.MBasicInfo.UpdateSecurity(target);
            }
            else//品种不存在 则添加品种
            {
                target = new SecurityFamilyImpl();
                target.Domain_ID = sec.Domain_ID;

                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                target.exchange_fk = sec.exchange_fk;
                target.Exchange = MDBasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = MDBasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = MDBasicTracker.SecurityTracker[target.underlaying_fk];

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;
                target.EntryCommission = sec.EntryCommission;
                target.ExitCommission = sec.ExitCommission;
                target.Margin = sec.Margin;
                target.ExtraMargin = sec.ExtraMargin;
                target.MaintanceMargin = sec.MaintanceMargin;
                target.Tradeable = sec.Tradeable;

                //数据库插入 并绑定数据库ID
                ORM.MBasicInfo.InsertSecurity(target);
                //将新增加的品种添加到内存中
                seccodemap[target.Code] = target;
                idxcodemap[target.ID] = target;

                sec.ID = target.ID;//将数据库ID传递给sec
            }
        }
    }
}
