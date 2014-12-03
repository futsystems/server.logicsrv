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

                target.Module_Agent = domain.Module_Agent;
                target.Module_FinService = domain.Module_FinService;
                target.Module_PayOnline = domain.Module_PayOnline;
                target.Router_Live = domain.Router_Live;
                target.Router_Sim = domain.Router_Sim;

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

                target.Module_Agent = domain.Module_Agent;
                target.Module_FinService = domain.Module_FinService;
                target.Module_PayOnline = domain.Module_PayOnline;
                target.Router_Live = domain.Router_Live;
                target.Router_Sim = domain.Router_Sim;

                ORM.MDomain.InsertDomain(target);
                domain.ID = target.ID;
                domainmap.TryAdd(target.ID, target);
            }
        }
    }
}
