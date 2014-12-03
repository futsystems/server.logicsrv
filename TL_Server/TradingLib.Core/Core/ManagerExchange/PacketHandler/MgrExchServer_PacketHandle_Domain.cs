﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryDomain", "QryDomain - query domain", "查询域")]
        public void CTE_QryDomain(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.Domain.Super && manager.RightRootDomain())
            {
                DomainImpl [] domains= BasicTracker.DomainTracker.Domains.ToArray();
               ;
               session.SendJsonReplyMgr(domains);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateDomain", "UpdateDomain - update domain", "查询域", true)]
        public void CTE_UpdateDomain(ISession session, string json)
        {
            try
            {
                Manager manager = session.GetManager();
                //只有超级域的管理员
                if (manager.Domain.Super && manager.RightRootDomain())
                {
                    DomainImpl domain = TradingLib.Mixins.LitJson.JsonMapper.ToObject<DomainImpl>(json);
                    bool isadd = domain.ID == 0;
                    BasicTracker.DomainTracker.UpdateDomain(domain);

                    //如果是新增domain则需要增加Manager
                    if (isadd)
                    {
                        Manager toadd = new Manager();
                        toadd.Login = string.Format("root-{0}", domain.ID);
                        toadd.Mobile = domain.Mobile;
                        toadd.Name = domain.LinkMan;
                        toadd.QQ = domain.QQ;
                        toadd.Type = QSEnumManagerType.ROOT;
                        toadd.AccLimit = domain.AccLimit;

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
                            Util.Debug("Domain:" + domain.Name + " 没有对应的Root Manager", QSEnumDebugLevel.WARNING);
                        }
                        else
                        {
                            //将域的信息更新到对应的Root Manager上
                            mgr.Mobile = domain.Mobile;
                            mgr.Name = domain.LinkMan;
                            mgr.QQ = domain.QQ;
                            mgr.AccLimit = domain.AccLimit;

                            BasicTracker.ManagerTracker.UpdateManager(mgr);
                        }
                        
                    }

                    session.NotifyMgr("NotifyDomain", BasicTracker.DomainTracker[domain.ID]);
                    session.OperationSuccess("更新域信息成功");
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        
        }
    }
}
