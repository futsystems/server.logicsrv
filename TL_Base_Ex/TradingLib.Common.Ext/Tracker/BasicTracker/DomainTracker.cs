using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class DomainTracker
    {
        ConcurrentDictionary<int, DomainImpl> domainmap = new ConcurrentDictionary<int, DomainImpl>();

        public DomainTracker()
        {
            //加载所有Domain
            foreach (DomainImpl domain in ORM.MDomain.SelectDomains())
            {
                domainmap.TryAdd(domain.ID, domain);
            }
        }

        DomainImpl _superdomian = null;
        /// <summary>
        /// 返回超级域 
        /// </summary>
        public DomainImpl SuperDomain
        {
            get
            {
                if(_superdomian == null)
                    _superdomian = domainmap.Values.Where(d => (d.Super)).FirstOrDefault();
                return _superdomian;
            }
            
        }
        /// <summary>
        /// 按DomainID返回对应的域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DomainImpl this[int id]
        {
            get
            {
                DomainImpl domain = null;
                if (domainmap.TryGetValue(id,out domain))
                {
                    return domain;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回所有域
        /// </summary>
        public IEnumerable<DomainImpl> Domains
        {
            get
            {
                return domainmap.Values;
            }
        }


        /// <summary>
        /// 更新合约同步源
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="vendorid"></param>
        public void UpdateSyncVendor(DomainImpl domain, int vendorid)
        {
            domain.CFG_SyncVendor_ID = vendorid;
            ORM.MDomain.UpdateSyncVendor(domain);
        }

        public void UpdateDomain(DomainImpl domain)
        {
            DomainImpl target = null;
            if (domainmap.TryGetValue(domain.ID, out target))//更新
            {
                target.LinkMan = domain.LinkMan;
                target.Mobile = domain.Mobile;
                target.Name = domain.Name;
                target.Email = domain.Email;
                target.QQ = domain.QQ;

                //target.Super = domain.Super;
                target.DateExpired = domain.DateExpired;

                target.AccLimit = domain.AccLimit;
                target.RouterGroupLimit = domain.RouterGroupLimit;
                target.RouterItemLimit = domain.RouterItemLimit;
                target.InterfaceList = domain.InterfaceList;
                target.FinSPList = domain.FinSPList;

                target.Module_Agent = domain.Module_Agent;
                target.Module_SubAgent = domain.Module_SubAgent;

                target.Module_FinService = domain.Module_FinService;
                target.Module_PayOnline = domain.Module_PayOnline;
                target.Module_Slip = domain.Module_Slip;
                target.Router_Live = domain.Router_Live;
                target.Router_Sim = domain.Router_Sim;
                target.VendorLimit = domain.VendorLimit;
                target.Switch_Router = domain.Switch_Router;

                target.AgentLimit = domain.AgentLimit;

                target.IsProduction = domain.IsProduction;
                target.DiscountNum = domain.DiscountNum;
                target.Dedicated = domain.Dedicated;

                target.Cfg_GrossPosition = domain.Cfg_GrossPosition;
                target.Cfg_MaxMarginSide = domain.Cfg_MaxMarginSide;
                ORM.MDomain.UpdateDomain(target);
                
            }
            else
            {
                target = new DomainImpl();
                target.LinkMan = domain.LinkMan;
                target.Mobile = domain.Mobile;
                target.Name = domain.Name;
                target.Email = domain.Email;
                target.QQ = domain.QQ;

                //target.Super = domain.Super;
                target.DateExpired = domain.DateExpired;

                target.AccLimit = domain.AccLimit;
                target.RouterGroupLimit = domain.RouterGroupLimit;
                target.RouterItemLimit = domain.RouterItemLimit;
                target.InterfaceList = domain.InterfaceList;
                target.FinSPList = domain.FinSPList;
                target.Module_Agent = domain.Module_Agent;
                target.Module_SubAgent = domain.Module_SubAgent;
                target.Module_FinService = domain.Module_FinService;
                target.Module_PayOnline = domain.Module_PayOnline;
                target.Module_Slip = domain.Module_Slip;

                target.Router_Live = domain.Router_Live;
                target.Router_Sim = domain.Router_Sim;
                target.VendorLimit = domain.VendorLimit;
                target.Switch_Router = domain.Switch_Router;

                target.AgentLimit = domain.AgentLimit;
                target.IsProduction = domain.IsProduction;
                target.DiscountNum = domain.DiscountNum;
                target.Dedicated = domain.Dedicated;

                target.Cfg_GrossPosition = domain.Cfg_GrossPosition;
                target.Cfg_MaxMarginSide = domain.Cfg_MaxMarginSide;

                ORM.MDomain.InsertDomain(target);
                domain.ID = target.ID;
                domainmap.TryAdd(target.ID, target);
            }
        }
    }
}
