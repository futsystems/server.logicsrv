using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.Race
{
    [ContribAttr(RaceCentre.ContribName, "比赛服务", "用于提供交易帐户的比赛服务")]
    public partial class RaceCentre : ContribSrvObject, IContrib
    {
        const string ContribName = "RaceCentre";

        public RaceCentre()
            : base(RaceCentre.ContribName)
        { 

        
        }

        ConfigDB _cfgdb;
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(RaceCentre.ContribName);

            if (!_cfgdb.HaveConfig("Commission_L1"))
            {
                _cfgdb.UpdateConfig("Commission_L1", QSEnumCfgType.Decimal, 10, "L1手续费加收比例");
            }


            RaceSetting tmp = new RaceSetting();
            tmp.EliminateNum = 0;
            tmp.EntryNum = 0;
            tmp.PromotNum = 0;
            tmp.StartTime = 0;
            tmp.BeginSignTime = 0;
            tmp.EndSignTime = 0;

            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.PRERACE))
            { 
                tmp.RaceID = "PRERACE";
                tmp.RaceType= QSEnumRaceType.PRERACE;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }

            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.SEMIRACE))
            {
                tmp.RaceID = "SEMIRACE";
                tmp.RaceType = QSEnumRaceType.SEMIRACE;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }

            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.REAL1))
            {
                tmp.RaceID = "REAL1";
                tmp.RaceType = QSEnumRaceType.REAL1;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }

            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.REAL2))
            {
                tmp.RaceID = "REAL2";
                tmp.RaceType = QSEnumRaceType.REAL2;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }
            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.REAL3))
            {
                tmp.RaceID = "REAL3";
                tmp.RaceType = QSEnumRaceType.REAL3;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }
            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.REAL4))
            {
                tmp.RaceID = "REAL4";
                tmp.RaceType = QSEnumRaceType.REAL4;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }
            if (!Tracker.RaceTracker.HaveAnyRace(QSEnumRaceType.REAL5))
            {
                tmp.RaceID = "REAL5";
                tmp.RaceType = QSEnumRaceType.REAL5;
                Tracker.RaceTracker.UpdateRaceSetting(tmp);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            //用户创建时 添加秘籍服务
            //TLCtxHelper.EventAccount.AccountAddEvent -= new AccountIdDel(OnAccountAdded);

            //有成交时 如果该成交是秘籍服务产生的成交 则进行手续费调整
            //TLCtxHelper.ExContribEvent.AdjustCommissionEvent -= new AdjustCommissionDel(OnAdjustCommissionEvent);
            base.Dispose();
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("启动比赛服务", QSEnumDebugLevel.INFO);

            foreach (var race in Tracker.RaceTracker.Races)
            {
                WireRaceEvent(race);
            }

            foreach (var rs in Tracker.RaceServiceTracker.RaceServices)
            {
                //初始化比赛服务
                rs.InitRaceService();

                Race race = Tracker.RaceTracker[rs.RaceID];
                if (race != null)
                {
                    //将比赛服务对象添加到对应的比赛中去
                    race.RestoreAccount(rs);
                }
            }
          

        }

        void WireRaceEvent(Race race)
        {
            //遍历所有比赛对象然后绑定对应的事件
            race.PromotAccountEvent += new Action<RaceService, QSEnumAccountRaceStatus>(race_PromotAccountEvent);
            race.EliminateAccountEvent += new Action<RaceService, QSEnumAccountRaceStatus>(race_EliminateAccountEvent);
            race.SignAccountEvent += new Action<RaceService, QSEnumAccountRaceStatus>(race_SignAccountEvent);
            race.EntryAccountEvent += new Action<RaceService, QSEnumAccountRaceStatus>(race_EntryAccountEvent);
        }

        /// <summary>
        /// 交易帐户进入某个比赛
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void race_EntryAccountEvent(RaceService arg1, QSEnumAccountRaceStatus arg2)
        {
            //当有比赛服务加入到比赛时更新该比赛的当前数量信息
            Race race = Tracker.RaceTracker[arg1.RaceID];
            ORM.MRace.UpdateRace(race);
        }


        /// <summary>
        /// 交易帐户报名参加某个比赛
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void race_SignAccountEvent(RaceService arg1, QSEnumAccountRaceStatus arg2)
        {
            //通过下个比赛状态获得对应的比赛对象
            Race r = Tracker.RaceTracker.LatestPrerace;
            string race_id = r == null ? "" : r.RaceID;

            RaceStatusChange change = new RaceStatusChange(Util.ToTLDateTime(), arg1.Acct, arg1.RaceID, arg1.RaceStatus, race_id, arg2);
            
            //记录帐户比赛变动
            ORM.MRace.InsertRaceChangeLog(change);

            //更新比赛服务
            arg1.RaceID = race_id;
            arg1.RaceStatus = arg2;
            arg1.EntryTime = change.DateTime;
            arg1.EntrySettleday = TLCtxHelper.CmdSettleCentre.NextTradingday;//当前报名结算日为下一个结算日(这个下一个结算日是按上个结算日推断出来的，然后当前结算日根据假期和时间来推断出当前结算日)
            
            ORM.MRace.UpdateRaceService(arg1);

            //新注册比赛后 要冻结该比赛服务
            arg1.InActive();

            //目的比赛不为空则加入比赛 并调整资金等其他事宜
            if (r != null)
            {
                //加入比赛
                r.EntryAccount(arg1);
                //重置比赛资金
                ResetAccountEquity(arg1.Account, r.StartEquity);
            }
        }

        /// <summary>
        /// 交易帐户被淘汰
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void race_EliminateAccountEvent(RaceService arg1, QSEnumAccountRaceStatus arg2)
        {
            //通过下个比赛状态获得对应的比赛对象
            Race r = Tracker.RaceTracker.GetRaceViaRaceStatus(arg2);
            string race_id = r == null ? "" : r.RaceID;

            RaceStatusChange change = new RaceStatusChange(Util.ToTLDateTime(), arg1.Acct, arg1.RaceID, arg1.RaceStatus, race_id, arg2);
            
            //记录帐户比赛变动
            ORM.MRace.InsertRaceChangeLog(change);

            //更新旧比赛对对应的人数数据
            Race old = Tracker.RaceTracker[arg1.RaceID];
            ORM.MRace.UpdateRace(old);

            //更新比赛服务
            arg1.RaceID = race_id;
            arg1.RaceStatus = arg2;
            arg1.EntryTime = change.DateTime;
            ORM.MRace.UpdateRaceService(arg1);

            //将选手从旧比赛中退出
            old.ExitAccount(arg1);

            //目的比赛不为空则加入比赛 并调整资金等其他事宜
            if (r != null)
            {
                //加入比赛 比如从REAL2降级到复赛等情况
                r.EntryAccount(arg1);
                //重置比赛资金
                ResetAccountEquity(arg1.Account, r.StartEquity);
            }


        }

        /// <summary>
        /// 比赛触发交易帐户晋级
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void race_PromotAccountEvent(RaceService arg1, QSEnumAccountRaceStatus arg2)
        {
            //通过下个比赛状态获得对应的比赛对象
            Race r = Tracker.RaceTracker.GetRaceViaRaceStatus(arg2);
            string race_id = r == null ? "" : r.RaceID;

            RaceStatusChange change = new RaceStatusChange(Util.ToTLDateTime(), arg1.Acct, arg1.RaceID, arg1.RaceStatus, race_id, arg2);

            //记录帐户比赛变动
            ORM.MRace.InsertRaceChangeLog(change);

            //更新旧比赛对对应的人数数据
            Race old = Tracker.RaceTracker[arg1.RaceID];
            ORM.MRace.UpdateRace(old);

            //更新比赛服务
            arg1.RaceID = race_id;
            arg1.RaceStatus = arg2;
            arg1.EntryTime = change.DateTime;
            ORM.MRace.UpdateRaceService(arg1);

            //将选手从旧比赛中退出
            old.ExitAccount(arg1);

            if (r != null)
            {
                //加入比赛
                r.EntryAccount(arg1);
                //重置比赛资金
                ResetAccountEquity(arg1.Account, r.StartEquity);

                arg1.InActive();
            }
        }


        /// <summary>
        /// 重置交易帐户资金到某值
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        void ResetAccountEquity(IAccount account, decimal amount)
        {
            decimal nowequity = account.NowEquity;
            decimal diff = amount - nowequity;
            TLCtxHelper.CmdAuthCashOperation.CashOperation(account.ID, diff, QSEnumEquityType.OwnEquity, "", "比赛重置");

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }



    }
}
