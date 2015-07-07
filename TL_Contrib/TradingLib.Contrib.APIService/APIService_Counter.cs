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
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "CreateCounter", "CreateCounter - 新建柜台实例", "在该部署环境中新建柜台实例", QSEnumArgParseType.Json)]
        public object CreateCounter(string request)
        {
            debug("got create counter request:" + request, QSEnumDebugLevel.INFO);
            JsonData args = JsonMapper.ToObject(request);

            DomainImpl domain = new DomainImpl();
            domain.Name = args["Company"].ToString();
            domain.LinkMan = args["LinkMan"].ToString();
            domain.QQ = "";
            domain.Mobile = "";
            domain.IsProduction = false;

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

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "QryCounter", "QryCounter - 查询柜台参数", "查询柜台参数", QSEnumArgParseType.Json)]
        public object QryCounter(string request)
        {

            debug("got qry counter request:" + request, QSEnumDebugLevel.INFO);
            JsonData args = JsonMapper.ToObject(request);
            //如果传入的参数不包含DomainID则无法更新柜台实例的参数
            if (!args.Keys.Contains("DomainID"))
            {

            }
            int domain_id = int.Parse(args["DomainID"].ToString());

            DomainImpl tmp = BasicTracker.DomainTracker[domain_id];

            return new
            {
                DomainID = domain_id,
                SubAccountNum = tmp.AccLimit,
                MainAccountNum = tmp.VendorLimit,
                AgentNum = tmp.AgentLimit,
                RouterGroupNum = tmp.RouterGroupLimit,
                ExpireDate = tmp.DateExpired,
                IsProd = tmp.IsProduction,
                DiscountNum = tmp.DiscountNum,

                ExLive = tmp.Router_Live,
                ExSIM = tmp.Router_Sim,
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

            //主帐户个数
            int mainacct_num = args.Keys.Contains("MainAccountNum") ? int.Parse(args["MainAccountNum"].ToString()) : tmp.VendorLimit;
            //分帐户个数
            int subacct_num = args.Keys.Contains("SubAccountNum") ? int.Parse(args["SubAccountNum"].ToString()) : tmp.AccLimit;
            //代理数量
            int agent_num = args.Keys.Contains("AgentNum") ? int.Parse(args["AgentNum"].ToString()) : tmp.AgentLimit;

            int rg_num = args.Keys.Contains("RouterGroupNum") ? int.Parse(args["RouterGroupNum"].ToString()) : tmp.RouterGroupLimit;
            //到期日期
            int expiredate = args.Keys.Contains("ExpireDate") ? int.Parse(args["ExpireDate"].ToString()) : tmp.DateExpired;
            //测试还是运营分区
            bool isprod = args.Keys.Contains("IsProd") ? bool.Parse(args["IsProd"].ToString()) : tmp.IsProduction;

            //优惠个数
            int discountnum = args.Keys.Contains("DiscountNum") ? int.Parse(args["DiscountNum"].ToString()) : tmp.DiscountNum;

            bool exlive = args.Keys.Contains("ExLive") ? bool.Parse(args["ExLive"].ToString()) : tmp.Router_Live;
            bool exsim = args.Keys.Contains("ExSIM") ? bool.Parse(args["ExSIM"].ToString()) : tmp.Router_Sim;

            //分区不为空则更新该分区
            if (tmp != null)
            {
                tmp.AccLimit = subacct_num;
                tmp.VendorLimit = mainacct_num;
                tmp.AgentLimit = agent_num;
                tmp.DateExpired = expiredate;
                tmp.Router_Live = exlive;
                tmp.Router_Sim = exsim;
                tmp.RouterGroupLimit = rg_num;
                tmp.IsProduction = isprod;
                tmp.DiscountNum = discountnum;
                tmp.RouterItemLimit = 1;
                tmp.ID = domain_id;

                BasicTracker.DomainTracker.UpdateDomain(tmp);

                //对应超级管理员不为空 则更新超级管理员
                Manager mgr = tmp.GetRootManager();
                if (mgr != null)
                {
                    mgr.AccLimit = tmp.AccLimit;
                    mgr.AgentLimit = tmp.AgentLimit;
                    BasicTracker.ManagerTracker.UpdateManager(mgr);
                }
            }

            return new
            {
                DomainID = domain_id,
                SubAccountNum = tmp.AccLimit,
                MainAccountNum = tmp.VendorLimit,
                AgentNum = tmp.AgentLimit,
                RouterGroupNum = tmp.RouterGroupLimit,
                ExpireDate = tmp.DateExpired,
                IsProd = tmp.IsProduction,
                DiscountNum = tmp.DiscountNum,
                ExLive = tmp.Router_Live,
                ExSIM = tmp.Router_Sim,
            };
        }
    }
}
