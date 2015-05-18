using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;

namespace TradingLib.Contrib.Race
{
    public partial class RaceCentre
    {
        #region race api service
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "QryRaceStatus", "QryRaceStatus - 查询比赛状态", "查询比赛状态", QSEnumArgParseType.Json)]
        public object CTE_QryRaceStatus(string request)
        {
            JsonData args = JsonMapper.ToObject(request);
            string acct = args["Account"].ToString();

            IAccount account = TLCtxHelper.CmdAccount[acct];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            RaceService rs = Tracker.RaceServiceTracker[account.ID];
            return new
            {
                Account = rs.Account.ID,
                RaceID = rs.RaceID,
                RaceStatus = Util.GetEnumDescription(rs.RaceStatus),
                EntryTime = Util.ToDateTime(rs.EntryTime).ToString(),
                ExamineEquity = rs.ExamineEquity,
                IsAvabile = rs.IsAvabile,
                InRace = IsInRace(rs.RaceStatus),
            };
        }

        bool IsInRace(QSEnumAccountRaceStatus status)
        {
            switch (status)
            {
                case QSEnumAccountRaceStatus.ELIMINATE: return false;
                case QSEnumAccountRaceStatus.NORACE: return false;
                default:
                    return true;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "SignRace", "SignRace - 报名比赛", "报名比赛", QSEnumArgParseType.Json)]
        public object CTE_SignRace(string request)
        {
            JsonData args = JsonMapper.ToObject(request);
            string acct = args["Account"].ToString();

            IAccount account = TLCtxHelper.CmdAccount[acct];
            if (account == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }
            //报名参加比赛
            SignRaceService(account);

            RaceService rs = Tracker.RaceServiceTracker[account.ID];
            return new
            {
                Account = rs.Account.ID,
                RaceID = rs.RaceID,
                RaceStatus = Util.GetEnumDescription(rs.RaceStatus),
                EntryTime = Util.ToDateTime(rs.EntryTime).ToString(),
                ExamineEquity = rs.ExamineEquity,
                IsAvabile = rs.IsAvabile,
                InRace = IsInRace(rs.RaceStatus),
            };
        }


        

        #endregion

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ExamineRace", "ExamineRace - examine race", "执行考核比赛", QSEnumArgParseType.Json)]
        public void CTE_ExamineRace(ISession session)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            ExamineRace();

            session.OperationSuccess("执行考核比赛成功");

        }


        /// <summary>
        /// 查询比赛列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRaceList", "QryRaceList - query race list", "查询比赛列表", QSEnumArgParseType.Json)]
        public void CTE_QryRaceList(ISession session)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }
            RaceSetting[] list = ORM.MRace.SelectRaceSettings().ToArray();
            session.ReplyMgr(list);

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "OpenNewRace", "OpenNewRace - open new prepare race", "新开比赛", QSEnumArgParseType.Json)]
        public void CTE_OpenNewRace(ISession session)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            OpenNewPrerace();

            session.OperationSuccess("新开比赛成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "PromptAccount", "PromptAccount - prompt account", "晋级交易帐户")]
        public void CTE_PromptAccount(ISession session, string account)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            PromptAccount(account);

            session.OperationSuccess(string.Format("晋级帐户:{0}成功", account));
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "EliminateAccount", "EliminateAccount - eliminate account", "淘汰交易帐户")]
        public void CTE_EliminateAccount(ISession session, string account)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            EliminateAccount(account);

            session.OperationSuccess(string.Format("淘汰帐户:{0}成功", account));
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SignRaceAccount", "SignRaceAccount - sign account", "交易帐户报名参赛")]
        public void CTE_SignRaceAccount(ISession session, string account)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            IAccount acct = TLCtxHelper.CmdAccount[account];
            if (acct == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }


            SignRaceService(acct);
                

            session.OperationSuccess(string.Format("帐户:{0}报名成功", account));
        }




        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryRaceService", "QryRaceService - qry raceservice", "查询比赛服务", QSEnumArgParseType.Json)]
        public void CTE_QryRaceService(ISession session,string args)
        {
            Manager mgr = session.GetManager();
            if (!mgr.IsRoot())
            {
                throw new FutsRspError("无权进行该操作");
            }

            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(args);

            var account = data["account"].ToString();

            //var racetype = Enum.Parse(typeof(QSEnumRaceType), data["race_type"].ToString());
            IEnumerable<RaceServiceSetting> settings =  null;

            if (!string.IsNullOrEmpty(account)) //如果指定了交易帐户 则查询该帐户的比赛服务
            {

                if (TLCtxHelper.CmdAccount[account] == null)
                {
                    throw new FutsRspError(string.Format("交易帐户不存在"));
                }

                RaceService tmp = Tracker.RaceServiceTracker[account];


               
                RaceServiceSetting rs = ORM.MRace.SelectRaceServiceSetting(account);
                if (rs == null)
                {
                    throw new FutsRspError("比赛服务不存在");
                }
                List<RaceServiceSetting> list = new List<RaceServiceSetting>();
                list.Add(rs);
                settings = list;
            }
            else
            {
                settings = ORM.MRace.SelectRaceServiceSettings();
            }
            
            
            if (data["race_status"] != null)
            {
                var racestatus = (QSEnumAccountRaceStatus)Enum.Parse(typeof(QSEnumAccountRaceStatus), data["race_status"].ToString());
                settings = settings.Where(rs => rs.RaceStatus == racestatus);
            }

            RaceServiceSetting[] services = settings.ToArray();

            for (int i = 0; i < services.Length; i++)
            {
                session.ReplyMgr(services[i], i == services.Length);
            }
        }


    }
}
