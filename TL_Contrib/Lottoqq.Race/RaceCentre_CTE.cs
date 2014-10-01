using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace Lottoqq.Race
{
    public partial class RaceCentre
    {

        #region 客户端比赛交互
        
        /*
        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "entryrace",
            "entryrace - entryrace",
            "参加比赛")]**/
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "entryrace", "entryrace - entryrace", "参加比赛")]
        public string CTE_EntryRace(string account)
        {
            try
            {
                EntryRace(account);
                return ReplyHelper.Success_Generic;

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "entryrace",
            "entryrace - entryrace",
            "参加比赛")]
        public string CTE_EntryRace(ISession session)
        {
            try
            {
                EntryRace(session.AccountID);
                return ReplyHelper.Success_Generic;

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "exitrace", "exitrace - exitrace", "退出比赛")]
        public string CTE_ExitRace(string account)
        {
            try
            {
                ExitRace(account);
                return ReplyHelper.Success_Generic;

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }
        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "exitrace",
            "exitrace - exitrace",
            "退出比赛")]
        public string CTE_ExitRace(ISession session)
        {
            try
            {
                ExitRace(session.AccountID);
                return ReplyHelper.Success_Generic;

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }


        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "qryrace",
            "qryrace - qryrace",
            "查询比赛")]
        public void CTE_QryRaceService(ISession session)
        {
            try
            {
                string account = session.AccountID;
                IAccount acc = TLCtxHelper.CmdAccount[account];
                if (acc == null)
                {
                    //SendContribResponse(session, JsonReply.GenericError(ReplyType.AccountNotFound, "交易帐号不存在"));
                }

                RaceService rs = RaceHelper.RaceServiceTracker[account];
                if (rs == null)
                {
                    //SendContribResponse(session, JsonReply.GenericError(ReplyType.NoRaceService, "无比赛服务"));
                }
                else
                {
                    //SendContribResponse(session, new JsonWrapperRaceService(rs));
                }

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                //Send(session,JsonReply.GenericError(ReplyType.ServerSideError,"服务端异常"));
            }
        }
        #endregion

        #region cli web交互操作
        [ContribCommandAttr(QSEnumCommandSource.CLI, "addrs", "addrs - addrs", "为交易帐号添加比赛服务")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "addrs", "addrs - addrs", "为交易帐号添加比赛服务")]
        public string CTE_AddRaceService(string account)
        {
            try
            {
                if (RaceHelper.RaceServiceTracker.HaveRaceService(account))
                {
                    return JsonReply.GenericSuccess(ReplyType.Error,"帐户已经有比赛服务").ToJson();
                }
                AddRaceService(account);
                RaceService rs = RaceHelper.RaceServiceTracker[account];
                if (rs != null)
                {
                    return ReplyHelper.Success_Generic;
                }
                else
                {
                    return ReplyHelper.Error_ServerSide;
                }
            }
            catch (Exception ex)
            {
                debug("addrs error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "blockrace", "blockrace - blockrace", "冻结比赛")]
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "blockrace", "blockrace - blockrace", "冻结比赛")]
        public string CTE_BlockRace(string account)
        {
            try
            {
                BlockRace(account);
                return ReplyHelper.Success_Generic;

            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "settlerace", "settlerace - settlerace", "结算比赛")]
        public string CTE_SettleRaceService(string account)
        {
            try
            {
                IAccount acc = TLCtxHelper.CmdAccount[account];
                if (acc == null)
                {
                    throw new QSErrorAccountNotExist();
                }

                RaceService rs = RaceHelper.RaceServiceTracker[account];
                if (rs == null)
                {
                    throw new RaceServiceNotExit();
                }
                TradingLib.ORM.MRaceSettle.SettleRaceService(rs);
                return ReplyHelper.Success_Generic;
            }
            catch (Exception ex)
            {
                debug("entryrace error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.CLI, "demorace", "demorace - demorace", "比赛演示操作")]
        public void CTE_demo()
        {

            RankRace();
            
        }

        #endregion

        #region 定时任务
        /// <summary>
        /// 
        /// 系统帐户15:50执行结算
        /// 系统16:00执行重置
        /// 比赛服务需要在系统结算之前获得当日的交易统计比比如平仓盈亏,手续费等信息,用于迭加累计获得当前的数据
        /// 
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI, "settlerace", "settlerace - settlerace", "结算比赛")]
        [TaskAttr("比赛服务中心执行结算", 15, 40, 5, "比赛服务中心执行结算")]
        public void CTE_SettleRace()
        {
            try
            {
                Settle();
            }
            catch (Exception ex)
            {
                debug("race settle error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "statrace", "statrace - statrace", "比赛统计")]
        [TaskAttr("比赛服务中心执行比赛统计计算", 15, 50, 5, "比赛服务中心执行比赛统计计算")]
        public void CTE_GenRaceStatistic()
        {
            try
            {
                RankRace();
            }
            catch (Exception ex)
            {
                debug("race statistic error:" + ex.ToString(), QSEnumDebugLevel.INFO);
            }
        }
        #endregion


    }


    internal class JsonWrapperRaceService
    {
        RaceService _rs;
        public JsonWrapperRaceService(RaceService rs)
        {
            _rs = rs;
        }

        public string Account { get { return _rs.Account.ID; } }
        public DateTime EntryTime { get { return _rs.EntryTime; } }
        public DateTime SettleDay { get { return _rs.SettleDay; } }
        public string Status { get { return _rs.Status.ToString(); } }
        public decimal LastFutEquity { get { return _rs.LastFutEquity; } }
        public decimal LastOptEquity { get { return _rs.LastOptEquity; } }
        public decimal LastMJEquity { get { return _rs.LastMJEquity; } }

        public decimal FutAvabileFund { get { return _rs.FutAvabileFund; } }
        public decimal OptAvabileFund { get { return _rs.OptAvabileFund; } }
        public decimal MJAvabileFund { get { return _rs.MJAvabileFund; } }
    }
}
