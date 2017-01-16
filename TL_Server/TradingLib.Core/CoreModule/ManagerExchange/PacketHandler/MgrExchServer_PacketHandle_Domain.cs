//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 分区
    /// </summary>
    public partial class MgrExchServer
    {
        /// <summary>
        /// 查询分区
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDomain", "QryDomain - query domain", "查询域")]
        public void CTE_QryDomain(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.Domain.Super && manager.IsRoot())
            {
                DomainImpl [] domains= BasicTracker.DomainTracker.Domains.ToArray();
                session.ReplyMgr(domains);
            }
        }

        /// <summary>
        /// 查询分区管理员信息
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDomainRootLoginInfo", "QryDomainRootLoginInfo - query domain", "查询分区管理员信息")]
        public void CTE_QryManagerLoginInfo(ISession session,int domainid)
        {
            Manager manager = session.GetManager();
            if (manager.Domain.Super && manager.IsRoot())
            {
                Domain domain = BasicTracker.DomainTracker[domainid];

                Manager mgr = domain.GetRootManager();

                Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                logininfo.LoginID = mgr.Login;
                logininfo.Pass = ORM.MManager.GetManagerPass(mgr.Login);
                session.ReplyMgr(logininfo);
            }
        }

        /// <summary>
        /// 更新或添加分区
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateDomain", "UpdateDomain - update domain", "查询域", QSEnumArgParseType.Json)]
        public void CTE_UpdateDomain(ISession session, string json)
        {
            Manager manager = session.GetManager();
            //只有超级域的管理员
            if (manager.Domain.Super && manager.IsRoot())
            {

                DomainImpl domain = json.DeserializeObject<DomainImpl>();
                bool isadd = domain.ID == 0;

                if (isadd && BasicTracker.DomainTracker.Domains.Any(d => d.Dedicated))
                {
                    throw new FutsRspError("独立部署不允许添加多个分区");
                }

                BasicTracker.DomainTracker.UpdateDomain(domain);


                //如果是新增分区 则需要自动同步品种和合约数据
                if (isadd)
                {
                    Domain superdomain = BasicTracker.DomainTracker.SuperDomain;
                    Domain addeddomain = BasicTracker.DomainTracker[domain.ID];//通过刚才添加的id获得对应的分区对象
                    logger.Debug("superdomain:" + superdomain.ID.ToString() + " adddomain:" + addeddomain.ID.ToString());
                    if (superdomain != null && addeddomain != null)
                    {
                        //添加品种
                        foreach (SecurityFamilyImpl sec in superdomain.GetSecurityFamilies())
                        {
                            addeddomain.SyncSecurity(sec);
                        }

                        //添加合约
                        foreach (SymbolImpl sym in superdomain.GetSymbols())
                        {
                            addeddomain.SyncSymbol(sym);
                        }

                        //添加汇率数据
                        addeddomain.CreateExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday);
                    }
                    
                }

                //如果是新增domain则需要增加Manager
                if (isadd)
                {
                    Manager toadd = new Manager();
                    if (!domain.Dedicated)
                    {
                        toadd.Login = string.Format("admin-{0}", domain.ID);
                    }
                    else
                    {
                        toadd.Login = string.Format("admin");
                    }
                    toadd.Mobile = domain.Mobile;
                    toadd.Name = domain.LinkMan;
                    toadd.QQ = domain.QQ;
                    toadd.Type = QSEnumManagerType.ROOT;
                    toadd.AccLimit = domain.AccLimit;
                    toadd.AgentLimit = domain.AgentLimit;
                    toadd.Active = true;//新增domain时添加的Manger为激活状态 否则无法登入
                        
                    //设定域ID
                    toadd.domain_id = domain.ID;
                    //更新管理员信息
                    BasicTracker.ManagerTracker.UpdateManager(toadd);

                }
                else
                {
                    Manager mgr = domain.GetRootManager();
                    if (mgr == null)
                    {
                        logger.Warn("Domain:" + domain.Name + " 没有对应的Root Manager");
                        
                        Manager toadd = new Manager();
                        toadd.Login = string.Format("admin-{0}", domain.ID);
                        toadd.Mobile = domain.Mobile;
                        toadd.Name = domain.LinkMan;
                        toadd.QQ = domain.QQ;
                        toadd.Type = QSEnumManagerType.ROOT;
                        toadd.AccLimit = domain.AccLimit;
                        toadd.Active = true;//新增domain时添加的Manger为激活状态 否则无法登入

                        //设定域ID
                        toadd.domain_id = domain.ID;
                        //更新管理员信息
                        BasicTracker.ManagerTracker.UpdateManager(toadd);
                    }
                    else
                    {
                        //将域的信息更新到对应的Root Manager上
                        mgr.Mobile = domain.Mobile;
                        mgr.Name = domain.LinkMan;
                        mgr.QQ = domain.QQ;
                        mgr.AccLimit = domain.AccLimit;
                        mgr.AgentLimit = domain.AgentLimit;

                        BasicTracker.ManagerTracker.UpdateManager(mgr);
                    }
                        
                }

                session.NotifyMgr("NotifyDomain", BasicTracker.DomainTracker[domain.ID]);
                session.RspMessage("更新域信息成功");
            }
        }
    }
}
