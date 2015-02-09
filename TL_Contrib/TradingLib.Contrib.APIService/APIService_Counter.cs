using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;

namespace TradingLib.Contrib.APIService
{
    public partial class APIServiceBundle
    {


        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "CreateCounter", "CreateCounter - 新建柜台实例", "在该部署环境中新建柜台实例")]
        public object CreateCounter(string request)
        {
            debug("got create counter request....:" + request,QSEnumDebugLevel.INFO);
            DomainImpl domain = new DomainImpl();
            BasicTracker.DomainTracker.UpdateDomain(domain);

            Manager toadd = new Manager();
            toadd.Login = string.Format("root-{0}", domain.ID);
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
            debug("domain created id:" + domain.ID);

            return new { DomainID = domain.ID };
            
        }
    }
}
