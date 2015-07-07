using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.Race
{
    public partial class RaceCentre
    {
        [TaskAttr("比赛帐户风控检查",2,0, "比赛风控风控检查")]
        public void TaskAccountCheck()
        {
            if (TLCtxHelper.CmdSettleCentre.IsTradingday)
            {
                foreach (var rs in Tracker.RaceServiceTracker.RaceServices)
                {
                    
                    if (rs.IsAvabile && rs.Account != null)
                    {
                        if (rs.RaceStatus == QSEnumAccountRaceStatus.INPRERACE)
                        {

                            if (rs.Account.Profit < 0 && Math.Abs(rs.Account.Profit) >= 30000)
                            {
                                //如果该帐户仍然处于可交易状态 则冻结帐户 同时强平持仓 这样就避免多次冻结和强平操作
                                if (rs.Account.Execute)
                                {
                                    rs.Account.InactiveAccount();
                                    rs.Account.FlatPosition(QSEnumOrderSource.RISKCENTRE, "比赛强平");
                                }
                            }
                        }

                        else
                        {
                            if (rs.Account.Profit < 0 && Math.Abs(rs.Account.Profit) >= 20000)
                            {
                                //如果该帐户仍然处于可交易状态 则冻结帐户 同时强平持仓 这样就避免多次冻结和强平操作
                                if (rs.Account.Execute)
                                {
                                    rs.Account.InactiveAccount();
                                    rs.Account.FlatPosition(QSEnumOrderSource.RISKCENTRE, "比赛强平");
                                }
                            }
                        }
                        
                    }
                }
            }
        
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "raceprompt", "prompt - prompt account", "手工晋级某个交易帐号")]
        public void CTE_Prompt(string account)
        {
            PromptAccount(account);
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "racecal", "racecal - cal account", "计算帐户参赛以来的折算收益")]
        public string CTE_RaceCal(string account)
        {
            RaceService rs = Tracker.RaceServiceTracker[account];

            string re =  string.Format("参赛时间:{0} 折算收益:{1}", rs.EntryTime, ProfitCal.CalObverseProfit(rs));
            debug(re, QSEnumDebugLevel.DEBUG);

            return re;
        }


        [CoreCommandAttr(QSEnumCommandSource.CLI, "raceeliminate", "eliminate - eliminate account", "手工淘汰某个交易帐号")]
        public void CTE_Eliminate(string account)
        {
            EliminateAccount(account);
        }



        [CoreCommandAttr(QSEnumCommandSource.CLI, "newrace", "newrace - open new race", "开设新的比赛赛季")]
        public string CTE_OpenNewPrerace()
        {
            try
            {
                OpenNewPrerace();
                return "新开比赛成功";
            }
            catch (FutsRspError ex)
            {
                return ex.ErrorMessage;
            }
        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "signrace", "signrace - sign up race", "报名参加比赛")]
        public string CTE_SignRaceService(string account)
        {
            IAccount acct = TLCtxHelper.CmdAccount[account];
            if (acct == null)
            {
                return "交易帐户不存在";
            }

            try
            {
                SignRaceService(acct);
                return "报名参赛成功";
            }
            catch(FutsRspError ex)
            {
                return ex.ErrorMessage;
            }

        }

        [CoreCommandAttr(QSEnumCommandSource.CLI, "examinerace", "examinerace - examine race", "考核比赛")]
        public void CTE_ExamineRace()
        {
            ExamineRace();

        }


        /// <summary>
        /// 手工晋级某个账户
        /// </summary>
        /// <param name="acc"></param>
        void PromptAccount(string account)
        {
            debug(PROGRAME + ":手工晋级账户:" + account);
            RaceService rs = Tracker.RaceServiceTracker[account];
            if (rs == null)
            {
                throw new FutsRspError("比赛服务不存在或无法创建");
            }

            Race r = Tracker.RaceTracker[rs.RaceID];
            if (r == null) return;
            QSEnumAccountRaceStatus next = RaceRule.NextRacePromptStatus(rs.RaceStatus);
            if (next == QSEnumAccountRaceStatus.NORACE) return;
            r.PromotAccount(rs, next);


        }

        /// <summary>
        /// 手工淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void EliminateAccount(string account)
        {
            debug(PROGRAME + ":手工淘汰账户:" + account);
            RaceService rs = Tracker.RaceServiceTracker[account];
            if (rs == null)
            {
                throw new FutsRspError("比赛服务不存在或无法创建");
            }

            Race r = Tracker.RaceTracker[rs.RaceID];
            if (r == null) return;
            r.EliminateAccount(rs, QSEnumAccountRaceStatus.ELIMINATE);

        }




        /// <summary>
        /// 考核比赛
        /// 遍历所有比赛然后对参与该比赛的交易帐户按规则考核
        /// </summary>
        void ExamineRace()
        {
            debug(PROGRAME + ":进行比赛考核...", QSEnumDebugLevel.INFO);
            foreach (Race r in Tracker.RaceTracker.Races)
            {
                r.CheckAccount();
            }
        }

        /// <summary>
        /// 某个交易帐户报名参加比赛
        /// </summary>
        /// <param name="account"></param>
        void SignRaceService(IAccount account)
        {
            RaceService rs = Tracker.RaceServiceTracker[account.ID];

            if (rs.RaceStatus != QSEnumAccountRaceStatus.NORACE && rs.RaceStatus!= QSEnumAccountRaceStatus.ELIMINATE)
            {
                throw new FutsRspError("当前状态不能报名参赛");
            }

            Race latestRace = Tracker.RaceTracker.LatestPrerace;
            if (latestRace == null || !latestRace.IsValidForSign)
            {
                throw new FutsRspError("没有可报名的比赛");
            }

            latestRace.Sigup(rs);

        }

        /// <summary>
        /// 新开初赛
        /// </summary>
        void OpenNewPrerace()
        {
            if (Tracker.RaceTracker.LatestPrerace == null || !Tracker.RaceTracker.LatestPrerace.IsValidForSign)
            {
                DateTime start = DateTime.Now;
                string raceid = "PRERACE-" + start.ToString("yyyyMMdd");

                RaceSetting tmp = new RaceSetting();
                tmp.EliminateNum = 0;
                tmp.EntryNum = 0;
                tmp.PromotNum = 0;
                
                DateTime dt = DateTime.Now;

                DateTime startMonth = dt.AddDays(1 - dt.Day); //本月月初
                DateTime endMonth = startMonth.AddMonths(1).AddDays(-1); //本月月末

                tmp.StartTime = Util.ToTLDateTime(dt);
                tmp.BeginSignTime = Util.ToTLDateTime(dt);
                tmp.EndSignTime = Util.ToTLDateTime(endMonth);
                tmp.RaceID = raceid;

                if (Tracker.RaceTracker[raceid] == null)
                {
                    Tracker.RaceTracker.UpdateRaceSetting(tmp);
                }
                if (Tracker.RaceTracker[raceid] != null)
                {
                    WireRaceEvent(Tracker.RaceTracker[raceid]);
                }
                
            }
            else
            {
                throw new FutsRspError("当前有接受报名的初赛");
            }
        }
    }
}
