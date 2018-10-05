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
    public partial class MgrExchServer
    {

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryContractBank", "QryContractBank - query bank", "查询银行列表")]
        public void CTE_QryBank(ISession session)
        {
            ContractBank[] splist = BasicTracker.ContractBankTracker.Banks.ToArray();
            session.ReplyMgr(splist);
        }

        /// <summary>
        /// 查询收款银行列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryReceiveableBank", "QryReceiveableBank - query QryReceiveableBank", "查询收款银行银行列表")]
        public void CTE_QryReceiveableBank(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager != null)
            {
                JsonWrapperReceivableAccount[] splist = manager.Domain.GetRecvBanks().ToArray();
                session.ReplyMgr(splist);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateReceiveableBank", "UpdateReceiveableBank - update  ReceiveableBank", "更新收款银行银行列表", QSEnumArgParseType.Json)]
        public void CTE_UpdateReceiveableBank(ISession session, string json)
        {

            Manager manager = session.GetManager();
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权添加收款银行信息");
            }

            JsonWrapperReceivableAccount bank = json.DeserializeObject<JsonWrapperReceivableAccount>();
            manager.Domain.UpdateRecvBanks(bank);

            session.RspMessage("更新收款银行信息成功");
            //通知银行信息变更
            session.NotifyMgr("NotifyRecvBank", manager.Domain.GetRecvBank(bank.ID), manager.Domain.GetRootLocations());
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySystemStatus", "QrySystemStatus - query system status", "查询系统状态")]
        public void CTE_QrySystemStatus(ISession session)
        {

            Manager manger = session.GetManager();
            SystemStatus status = new SystemStatus();

            status.StartUpTime = TLCtxHelper.StartUpTime;
            status.LastSettleday = TLCtxHelper.ModuleSettleCentre.LastSettleday;
            status.Tradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            status.NextSettleTime = TLCtxHelper.ModuleSettleCentre.NextSettleTime;
            status.IsSettleNormal = true;// TLCtxHelper.ModuleSettleCentre.IsNormal;
            status.IsClearCentreLive = TLCtxHelper.ModuleClearCentre.IsLive;// TLCtxHelper.ModuleClearCentre.Status;
            status.SettleMode = TLCtxHelper.ModuleSettleCentre.SettleMode;
            status.UnsettledAcctOrderNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledAcctOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
            status.UnsettledBrokerOrderNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledBrokerOrderNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
            status.UnsettledExchangeSettlementNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledExchangeSettlementNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);
            status.UnsettledAcctTradeNumOfPreSettleday = ORM.MTradingInfo.GetUnsettledAcctTradeNum(TLCtxHelper.ModuleSettleCentre.LastSettleday);

            status.TotalOrderNum = ORM.MTradingInfo.GetInterdayOrderNum(TLCtxHelper.ModuleSettleCentre.Tradingday);
            status.TotalTradeNum = ORM.MTradingInfo.GetInterdayTradeNum(TLCtxHelper.ModuleSettleCentre.Tradingday);
            status.TotalAccountNum = manger.Domain.Super ? TLCtxHelper.ModuleAccountManager.Accounts.Count() : manger.GetAccounts().Count();
            session.ReplyMgr(status);

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ExitSystem", "ExitSystem - exit system for reboot", "关闭系统")]
        public void CTE_ExitSystem(ISession session)
        {
            System.Environment.Exit(0);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CleanData", "CleanData - clean data", "删除历史数据", QSEnumArgParseType.Json)]
        public void CTE_DelAccount(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var date = req["date"].ToString();
            Manager manager = session.GetManager();
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权执行数据清理操作");
            }

            DateTime to = Util.ToDateTime(int.Parse(date), 0);
            DateTime now = Util.ToDateTime(TLCtxHelper.ModuleSettleCentre.LastSettleday, 0);

            if (now.Subtract(to).TotalDays < 7)
            {
                throw new FutsRspError("最少保留1周交易数据");
            }

            //队列中删除
            System.Threading.ThreadPool.QueueUserWorkItem(o =>
            {
                ORM.MTradingInfo.CleanData(int.Parse(date));
                session.RspMessage("历史数据清理成功");
            });
        }

        /// <summary>
        /// 在某些情况下会出现当日委托 成交 交易所结算 出入金等数据没有正常结算，导致下一个交易日出现多余数据的情况
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CleanOTE", "CleanOTE - clean ote", "清除结算异常数据", QSEnumArgParseType.Json)]
        public void CTE_DelAccount(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权执行清理结算异常操作");
            }

            
            //队列中删除
            System.Threading.ThreadPool.QueueUserWorkItem(o =>
            {
                ORM.MTradingInfo.CleanOTE();
                session.RspMessage("结算异常数据清理成功");
            });
        }




        
        




    }
}
