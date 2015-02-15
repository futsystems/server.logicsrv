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


        /// <summary>
        /// 创建柜台实例
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "CreateCounter", "CreateCounter - 新建柜台实例", "在该部署环境中新建柜台实例",QSEnumArgParseType.Json)]
        public object CreateCounter(string request)
        {
            debug("got create counter request:" + request,QSEnumDebugLevel.INFO);
            JsonData args = JsonMapper.ToObject(request);

            DomainImpl domain = new DomainImpl();
            domain.Name = args["Company"].ToString();
            domain.LinkMan = args["LinkMan"].ToString();
            domain.QQ = "";
            domain.Mobile = "";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "RenewCounter", "RenewCounter - 续费柜台", "续费柜台", QSEnumArgParseType.Json)]
        public object RenewCounter(string request)
        {
            debug("got renew counter request:" + request, QSEnumDebugLevel.INFO);
            JsonData args = JsonMapper.ToObject(request);
            int domain_id = int.Parse(args["DomainID"].ToString());
            int expiredate = int.Parse(args["ExpireDate"].ToString());

            DomainImpl tmp = BasicTracker.DomainTracker[domain_id];
            if (tmp != null)
            {
                tmp.DateExpired = expiredate;
                BasicTracker.DomainTracker.UpdateDomain(tmp);
            }

            return new
            {
                DomainID = domain_id,
                ExpireDate = tmp.DateExpired,
            };
        }

        /// <summary>
        /// 更新柜台参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "UpdateCounter", "UpdateCounter - 更新柜台参数", "更新柜台参数", QSEnumArgParseType.Json)]
        public object UpdateCounter(string request)
        {
            debug("got update counter request:" + request, QSEnumDebugLevel.INFO);
            JsonData args = JsonMapper.ToObject(request);
            //如果传入的参数不包含DomainID则无法更新柜台实例的参数
            if (!args.Keys.Contains("DomainID"))
            { 
                
            }
            int domain_id = int.Parse(args["DomainID"].ToString());

            DomainImpl tmp = BasicTracker.DomainTracker[domain_id];

            int mainacct_num = args.Keys.Contains("MainAccountNum")?int.Parse(args["MainAccountNum"].ToString()):tmp.VendorLimit;
            int subacct_num = args.Keys.Contains("SubAccountNum")?int.Parse(args["SubAccountNum"].ToString()):tmp.AccLimit;

            int rg_num = args.Keys.Contains("RouterGroupNum") ? int.Parse(args["RouterGroupNum"].ToString()) : tmp.RouterGroupLimit;
            int expiredate = args.Keys.Contains("ExpireDate")?int.Parse(args["ExpireDate"].ToString()):tmp.DateExpired;

            bool exlive = args.Keys.Contains("ExLive") ? bool.Parse(args["ExLive"].ToString()):tmp.Router_Live;
            bool exsim = args.Keys.Contains("ExSIM") ? bool.Parse(args["ExSIM"].ToString()):tmp.Router_Sim;
            if (tmp != null)
            {
                tmp.AccLimit = subacct_num;
                tmp.VendorLimit = mainacct_num;
                tmp.DateExpired = expiredate;
                tmp.Router_Live = exlive;
                tmp.Router_Sim = exsim;
                tmp.RouterGroupLimit = rg_num;
                tmp.RouterItemLimit = 1;
                tmp.ID = domain_id;

                BasicTracker.DomainTracker.UpdateDomain(tmp);
            }

            return new 
            { 
                DomainID = domain_id,
                SubAccountNum =tmp.AccLimit,
                MainAccountNum = tmp.VendorLimit,
                RouterGroupNum = tmp.RouterGroupLimit,
                ExpireDate =tmp.DateExpired,
                ExLive = tmp.Router_Live,
                ExSIM = tmp.Router_Sim,
            };
        }
    }
}
