﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;



namespace TradingLib.Common
{
    public class SecurityTracker
    {
        Dictionary<int, DBSecurityTracker> domainsecboltracker = new Dictionary<int, DBSecurityTracker>();
        public SecurityTracker()
        {
            //加载所有Domain的合约数据
            foreach (Domain domain in BasicTracker.DomainTracker.Domains)
            {
                if (!domainsecboltracker.Keys.Contains(domain.ID))
                {
                    domainsecboltracker.Add(domain.ID, new DBSecurityTracker(domain));
                }
            }
        }


        internal DBSecurityTracker this[int domain_id]
        {
            get
            {
                DBSecurityTracker tracker = null;
                if (domainsecboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker;
                }
                //如果当前内存中不存在DBSercurityTracker且该Domain存在则添加
                Domain domain = BasicTracker.DomainTracker[domain_id];
                if (domain != null)
                {
                    domainsecboltracker[domain_id] = new DBSecurityTracker(domain);
                    return domainsecboltracker[domain_id];
                }
                return null;
            }
        }
        internal SecurityFamilyImpl this[int domain_id, string code]
        {
            get
            {   DBSecurityTracker tracker = null;
                if (domainsecboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker[code];
                }
                return null;
            }
        }
        internal SecurityFamilyImpl this[int domain_id, int idx]
        {
            get
            {
                DBSecurityTracker tracker = null;
                if (domainsecboltracker.TryGetValue(domain_id, out tracker))
                {
                    return tracker[idx];
                }
                return null;
            }
        }

        internal void UpdateSecurity(SecurityFamilyImpl sec,bool updateall = true)
        {
            DBSecurityTracker tracker = null;
            if (!domainsecboltracker.TryGetValue(sec.Domain_ID, out tracker))
            {
                domainsecboltracker.Add(sec.ID, new DBSecurityTracker(BasicTracker.DomainTracker[sec.Domain_ID]));
            }
            domainsecboltracker[sec.Domain_ID].UpdateSecurity(sec,updateall);

        }

        internal void SyncSecurity(Domain domain,SecurityFamilyImpl sec)
        {
            DBSecurityTracker tracker = null;
            if (!domainsecboltracker.TryGetValue(domain.ID, out tracker))
            {
                domainsecboltracker.Add(sec.ID, new DBSecurityTracker(BasicTracker.DomainTracker[domain.ID]));
            }
            domainsecboltracker[domain.ID].SyncSecurity(sec);

        }


    }
    /// <summary>
    /// 品种管理器
    /// </summary>
    public class DBSecurityTracker
    {

        Dictionary<string, SecurityFamilyImpl> seccodemap = new Dictionary<string, SecurityFamilyImpl>();
        Dictionary<int, SecurityFamilyImpl> idxcodemap = new Dictionary<int, SecurityFamilyImpl>();

        Domain _doamin = null;
        public DBSecurityTracker(Domain domain)
        {
            _doamin = domain;
            //从数据库加载品种数据
            foreach (SecurityFamilyImpl sec in ORM.MBasicInfo.SelectSecurity(_doamin.ID))
            {
                seccodemap[sec.Code] = sec;
                idxcodemap[sec.ID] = sec;
            }

            
            //绑定底层证券
            foreach (SecurityFamilyImpl sec in seccodemap.Values)
            {
                sec.UnderLaying = this[sec.underlaying_fk] ;
                sec.MarketTime = BasicTracker.MarketTimeTracker[sec.mkttime_fk];
                sec.Exchange = BasicTracker.ExchagneTracker[sec.exchange_fk];
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
                target.Exchange = BasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = BasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = BasicTracker.SecurityTracker[target.Domain_ID, target.underlaying_fk];

                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;
                target.Domain_ID = _doamin.ID;

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
                target.Domain_ID = _doamin.ID;

                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                target.exchange_fk = sec.exchange_fk;
                target.Exchange = BasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = BasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = BasicTracker.SecurityTracker[target.Domain_ID, target.underlaying_fk];

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
        public void UpdateSecurity(SecurityFamilyImpl sec,bool updateall = true)
        {
            SecurityFamilyImpl target = null;
            if (idxcodemap.TryGetValue(sec.ID, out target))//品种存在 更新品种 通过ID更新 品种会存在修改code的情况
            {
                //内存实例更新
                target.Code = sec.Code;
                target.Name = sec.Name;
                target.Currency = sec.Currency;
                target.Type = sec.Type;

                //TLCtxHelper.Ctx.debug("mkttime_fk:" + sec.mkttime_fk.ToString() + " exchange_fk:" + sec.exchange_fk.ToString() + " underlay_fk:" + sec.underlaying_fk.ToString());
                target.exchange_fk = sec.exchange_fk;
                target.Exchange = BasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = BasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = BasicTracker.SecurityTracker[target.Domain_ID, target.underlaying_fk];

                
                target.Multiple = sec.Multiple;
                target.PriceTick = sec.PriceTick;

                if (updateall)
                {
                    target.EntryCommission = sec.EntryCommission;
                    target.ExitCommission = sec.ExitCommission;
                    target.Margin = sec.Margin;
                    target.ExtraMargin = sec.ExtraMargin;
                    target.MaintanceMargin = sec.MaintanceMargin;
                    target.Tradeable = sec.Tradeable;
                }
 
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
                target.Exchange = BasicTracker.ExchagneTracker[target.exchange_fk];

                target.mkttime_fk = sec.mkttime_fk;
                target.MarketTime = BasicTracker.MarketTimeTracker[target.mkttime_fk];

                target.underlaying_fk = sec.underlaying_fk;
                target.UnderLaying = BasicTracker.SecurityTracker[target.Domain_ID,target.underlaying_fk];

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