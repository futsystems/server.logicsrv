using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.MainAcctFinService
{

    public partial class MainAcctFinServiceCentre
    {


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateFinService", "UpdateFinService - update finservice", "更新交易帐户配资服务", QSEnumArgParseType.Json)]
        public void CTE_UpdateFinService(ISession session, string reqeust)
        {
            logger.Info(string.Format("管理员:{0} 更新配资服务:{1}", session.GetManager().Login,reqeust));
            Manager manager = session.GetManager();

            var fs = TradingLib.Mixins.Json.JsonMapper.ToObject<FinServiceSetting>(reqeust);

            FinGlobal.FinServiceTracker.UpdateFinService(fs);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelFinService", "DelFinService - del finservice", "删除交易帐户配资服务", QSEnumArgParseType.Json)]
        public void CTE_DelFinService(ISession session, string reqeust)
        {
            logger.Info(string.Format("管理员:{0} 删除配资服务:{1}", session.GetManager().Login,reqeust));

            Manager manager = session.GetManager();
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(reqeust);

            var account = data["account"].ToString();


            FinGlobal.FinServiceTracker.DelFinService(account);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryFinService", "QryFinService - qry finservice", "查询交易帐户配资服务", QSEnumArgParseType.Json)]
        public void CTE_QryFinService(ISession session,string request)
        {
            logger.Info(string.Format("管理员:{0} 查询配资服务:{1}", session.GetManager().Login, request));
            Manager manager = session.GetManager();
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(request);

            var account = data["account"].ToString();

            FinService fs = FinGlobal.FinServiceTracker[account];
            if (fs == null)
            {
                session.ReplyMgr(null);
            }
            else
            {
                session.ReplyMgr(fs.ToFinServiceSetting());
            }
        }

        /// <summary>
        /// 手工进行收费
        /// 这里需要指定收费方式
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ManualCollectFee", "ManualCollectFee - collect by manual", "手工进行确认收费", QSEnumArgParseType.Json)]
        public void CTE_ManualCollectFee(ISession session, string request)
        {
            logger.Info(string.Format("管理员:{0} 手工收费{1}", session.GetManager().Login,request));
            Manager manager = session.GetManager();

            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(request);

            var feeid = int.Parse(data["id"].ToString());
            var method = Util.ParseEnum<QSEnumChargeMethod>(data["charge_method"].ToString());

            

            Fee fee = FinGlobal.FinServiceTracker.GetFee(feeid);
            if(fee == null)
            {
                throw new FutsRspError(string.Format("收费记录:{0}不存在",feeid));
            }

            IAccount account = TLCtxHelper.ModuleAccountManager[fee.Account];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(account))
            {
                throw new FutsRspError("管理员无权执行该操作");
            }


            if (fee.Collected)
            {
                throw new FutsRspError(string.Format("收费记录:{0}已经收取", feeid));
            }

            CollectFee(fee, method);

            //通知
            FeeSetting notify = FinGlobal.FinServiceTracker.GetFeeSetting(feeid);
            TLCtxHelper.ModuleMgrExchange.Notify(ContribName, "NotifyFee", notify, account.GetNotifyPredicate());
            
            session.OperationSuccess("执行收费操作成功");
        }

        /// <summary>
        /// 查询当日收费记录
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryTodayFee", "QryTodayFee - qry today fee", "查询当日收费记录", QSEnumArgParseType.Json)]
        public void CTE_QryFinService(ISession session)
        {
            logger.Info(string.Format("管理员:{0} 查询收费记录", session.GetManager().Login));
            Manager manager = session.GetManager();


            Fee[] feelist = ORM.MFee.SelectFees(TLCtxHelper.ModuleSettleCentre.Tradingday).ToArray();

            for (int i = 0; i < feelist.Length; i++)
            {
                session.ReplyMgr(feelist[i], i != feelist.Length - 1);
            }
        }

        /// <summary>
        /// 更新收费项目
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "RollBackFee", "RollBackFee - rollback fee", "回滚收费操作", QSEnumArgParseType.Json)]
        public void CTE_RollBackFee(ISession session, string request)
        {
            logger.Info(string.Format("管理员:{0} 回滚收费项目:{1}", session.GetManager().Login, request));
            Manager manager = session.GetManager();
            FeeSetting f = TradingLib.Mixins.Json.JsonMapper.ToObject<FeeSetting>(request);
            IAccount account = TLCtxHelper.ModuleAccountManager[f.Account];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(account))
            {
                throw new FutsRspError("管理员无权执行该操作");
            }
            Fee target = FinGlobal.FinServiceTracker.GetFee(f.ID);
            if (!target.Collected)
            {
                throw new FutsRspError("未扣费收费项目无法回滚");
            }

            //执行回滚
            RollbackFee(target);

            //通知
            FeeSetting notify = FinGlobal.FinServiceTracker.GetFeeSetting(f.ID);
            TLCtxHelper.ModuleMgrExchange.Notify(ContribName, "NotifyFee", notify, account.GetNotifyPredicate());
            session.OperationSuccess("回滚收费项目成功");

        }

        /// <summary>
        /// 更新收费项目
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateFee", "UpdateFee - update fee", "更新收费项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateFee(ISession session,string request)
        {
            logger.Info(string.Format("管理员:{0} 更新收费项目:{1}", session.GetManager().Login,request));
            Manager manager = session.GetManager();

            FeeSetting f = TradingLib.Mixins.Json.JsonMapper.ToObject<FeeSetting>(request);

            IAccount account = TLCtxHelper.ModuleAccountManager[f.Account];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(account))
            {
                throw new FutsRspError("管理员无权执行该操作");
            }
            

            if (f != null)
            {
                FinGlobal.FinServiceTracker.UpdateFee(f);
            }
            FeeSetting target = FinGlobal.FinServiceTracker.GetFeeSetting(f.ID);

            TLCtxHelper.ModuleMgrExchange.Notify(ContribName,"NotifyFee", target, account.GetNotifyPredicate());
            session.OperationSuccess("更新收费项目成功");
        }

        /// <summary>
        /// 更新收费项目
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryStatus", "QryStatus - qry status", "查询状态", QSEnumArgParseType.Json)]
        public void CTE_UpdateFee(ISession session)
        {
            logger.Info(string.Format("管理员:{0} 查询状态", session.GetManager().Login));
            Manager manager = session.GetManager();

            session.ReplyMgr(_status);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ManualForceChargeServiceFee", "ManualForceChargeServiceFee - manual force charge", "手工强制执行收费", QSEnumArgParseType.Json)]
        public void CTE_ManualForceChargeServiceFee(ISession session)
        {
            logger.Info(string.Format("管理员:{0} 查询状态", session.GetManager().Login));
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday)    
            {
                throw new FutsRspError("非交易日，无法执行收费");
            }

            //执行收费操作
            _status.ChargeServiceBeforeTimeSpan = true;
            ChargeServiceFee(QSEnumChargeTime.BeforeTimeSpan);

            _status.ChargeServiceAfterTimeSpan = true;
            ChargeServiceFee(QSEnumChargeTime.AfterTimeSpan);

            //执行扣费操作
            CollectFee();
            session.ReplyMgr(_status);
            session.OperationSuccess("手工生成服务费收费记录完成");
        }



        [CoreCommandAttr(QSEnumCommandSource.CLI, "deferred", "deferred - 执行收费", "执行收费")]
        public void CTE_chargefee(int x)
        {
            //ChargeServiceFee(account);


            Deferred df = new Deferred(deferredcall, null);
            df.OnSuccess(cbsuccess)
                .OnError(cberror);

            df.Run();

        }

        object[] deferredcall(object[] objs)
        {
            logger.Info("deferred runing....");
            int i =0;
            while (i < 10)
            {
                logger.Info("sleep .....:" + i.ToString());
                Util.sleep(100);
                i++;
            }
            //throw new Exception("deferred call error");
            return new object[] { "18001", "accountinfo" };
            

        }

        void cbsuccess(IDeferredResult d)
        {

            logger.Info("deferred success account:"+d.Result.GetValue(0).ToString());
        }

        void cberror(IDeferredResult d)
        {
            logger.Info("deferred error");
        }

        #region 定时任务
        /// <summary>
        /// 白天开盘前5分钟执行计费操作
        /// </summary>
        [CoreCommandAttr(QSEnumCommandSource.CLI, "chargesf0", "chargesf0 - 执行计费", "执行计费")]
        [TaskAttr("开盘前执行计费操作",8,55,0, "开盘前执行计费操作")]
        public void CTE_ChargeServiceFeeBeforeTimeSpan()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday)
                return;
            _status.ChargeServiceBeforeTimeSpan = true;
            //执行计费操作
            ChargeServiceFee(QSEnumChargeTime.BeforeTimeSpan);

            //执行收费操作
            CollectFee();
        }

        /// <summary>
        /// 白天收盘后2分中执行计费操作
        /// </summary>
        [CoreCommandAttr(QSEnumCommandSource.CLI, "fsqrybroker", "fsqrybroker - 查询主帐户信息", "查询主帐户信息")]
        [TaskAttr("收盘后查询主帐户", 15, 16, 0, "收盘后查询主帐户")]
        public void CTE_QryBrokerAccountInfo()
        {

            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday)
                return;
            _status.QryAfterTimeSpane = true;
            QryBrokerAccountInfo();
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "fschargecf", "fschargecf - 征收手续费", "征收手续费")]
        [TaskAttr("收盘后收取手续费", 15, 18, 0, "收盘后收取手续费")]
        public void CTE_ChargeCommissionFee()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday)
                return;
            _status.ChargeCommissionAfterTimeSpan = true;
            ChargeCommissionFee();

            //执行收费操作
            CollectFee();
        }


        /// <summary>
        /// 白天收盘后5分中执行计费操作
        /// </summary>
        [CoreCommandAttr(QSEnumCommandSource.CLI, "chargesf1", "chargesf1 - 执行计费", "执行计费")]
        [TaskAttr("收盘后收取服务费", 15, 20, 0, "收盘后收取服务费")]
        public void CTE_ChargeServiceFeeAfterTimeSpan()
        {
            if (!TLCtxHelper.ModuleSettleCentre.IsTradingday)
                return;
            _status.ChargeServiceAfterTimeSpan = true;
            //执行计费操作
            ChargeServiceFee(QSEnumChargeTime.AfterTimeSpan);

            //执行收费操作
            CollectFee();
        }

        [TaskAttr("定时进行出入金超时检查",1,0, "定时进行出入金超时检查")]
        public void CTE_BrokerTransTimeOutCheck()
        {
            FinGlobal.BrokerTransTracker.CheckBrokerTransTimeOut();
        }

        #endregion

    }
}
