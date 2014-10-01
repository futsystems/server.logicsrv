using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;
using System.Data;

namespace Lottoqq.Race
{
    public delegate void DDAccountEntryRaceDel(IAccount account,QSEnumDDRaceType type);

    [ContribAttr("DDianRace", "比赛模块", "比赛模块实现了模拟选拔比赛的逻辑,用于接收客户端报名,重生,以及计算排名,进行实盘晋级等操作")]
    public class DDianRaceCentre : ContribSrvObject, IContrib
    {

        /// <summary>
        /// 某个交易账户进入某个Race事件,用于触发风控规则修改/Race_positiontransaction修改/其他事宜
        /// </summary>
        public event DDAccountEntryRaceDel AccountEntryRaceEvent;
        /// <summary>
        /// 比赛状态发生变化
        /// 报名/淘汰/晋级会触发该事件
        /// </summary>
        public event IAccountChangedDel AccountRaceStatusChanged;

        /// <summary>
        /// 对账户进行实时的账户检查比如权益亏损强平并冻结等
        /// </summary>
        public event IAccountCheckDel AccountCheckEvent;

        ConnectionPoll<mysqlRace> conn;


        public DDianRaceCentre()
            : base("DDianRace")
        {

        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            TLCtxHelper.Debug("RaceCentre start loading....");
            //0.获得全局ctx对象
            //_clearcentre = TLCtxHelper.Ctx.ClearCentre;

            //1.加载配置文件
            ConfigFile cfg = ConfigFile.GetConfigFile("contribcfg/race.cfg");

            //2.生成数据库连接对象
            conn = new ConnectionPoll<mysqlRace>(cfg["DBAddress"].AsString(), cfg["DBUser"].AsString(), cfg["DBPass"].AsString(), cfg["DBName"].AsString(), cfg["DBPort"].AsInt());

            //3.从数据库恢复比赛Session数据
            LoadRaceSessionFromMysql();


            RestoreAccount(TLCtxHelper.CmdAccount.Accounts);

        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }


        /// <summary>
        /// 获得某个ID标识的race session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Race this[string id]
        {
            get
            {
                Race r;
                if (raceMAP.TryGetValue(id, out r))
                    return r;
                else
                    return null;

            }
        }

        Dictionary<string, Race> raceMAP = new Dictionary<string, Race>();
        bool HaveRace(string raceid)
        {
            Race r;
            return (raceMAP.TryGetValue(raceid, out r));
        }



        /// <summary>
        /// 从数据库加载比赛
        /// </summary>
        public void LoadRaceSessionFromMysql(string race_id = null)
        {
            debug(PROGRAME + ":从数据库恢复比赛Session数据....",QSEnumDebugLevel.INFO);
            DataSet tradeset;
            mysqlRace db = conn.mysqlDB;
            if (race_id == null)
                tradeset = db.getRaceSessions();
            else
                tradeset = db.getRaceSessions(race_id);
            conn.Return(db);

            DataTable dt = tradeset.Tables["race_session"];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                DateTime starttime = Convert.ToDateTime(dr["starttime"]);
                int entrynum = Convert.ToInt16(dr["entry_num"]);
                int eliminatenum = Convert.ToInt16(dr["eliminate_num"]);
                int promotnum = Convert.ToInt16(dr["promot_num"]);

                string raceid = Convert.ToString(dr["race_id"]);
                DateTime beginsigntime = Convert.ToDateTime(dr["begin_sign_time"]);
                DateTime endsigntime = Convert.ToDateTime(dr["end_sign_time"]);
                string comment = Convert.ToString(dr["comment"]);
                //QSEnumRaceType racetype = (QSEnumRaceType)Enum.Parse(typeof(QSEnumRaceType), Convert.ToString(dr["race_type"]));
                QSEnumDDRaceType racetype = (QSEnumDDRaceType)Enum.Parse(typeof(QSEnumDDRaceType), Convert.ToString(dr["race_type"]));

                Race race = new Race(raceid, racetype, entrynum, eliminatenum, promotnum);// new Race(raceid, racetype, entrynum, eliminatenum, promotnum, starttime, beginsigntime, endsigntime);
                //将该race session加入内存
                race.SendDebugEvent += new DebugDelegate(msgdebug);
                race.PromotAccountEvent += new PromotAccountDel(race_PromotAccountEvent);//晋级交易账户
                race.EliminateAccountEvent += new EliminateAccountDel(race_EliminateAccountEvent);//淘汰交易账户
                race.SignAccountEvent += new EntryAccountDel(race_SignAccountEvent);//报名参加预赛
                race.EntryAccountEvent += new EntryAccountDel(race_EntryAccountEvent);//有新交易账户加入某个比赛

                //将比赛数据缓存到内存
                if (!HaveRace(race.RaceID))
                    raceMAP.Add(race.RaceID, race);

            }
            debug(PROGRAME + ":数据库恢复比赛数据:" + raceMAP.Count.ToString() + "条",QSEnumDebugLevel.INFO);
            
        }

        /// <summary>
        /// 有新的交易账户加入到某个比赛
        /// 此时该帐户已经更新了对应的RaceID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="nextstatus"></param>
        void race_EntryAccountEvent(IAccount account, QSEnumAccountRaceStatus nextstatus)
        {
            //1.更新比赛对对应的人数数据
            Race old = this[account.RaceID];
            mysqlRace db = conn.mysqlDB;
            db.UpdateRaceInfo(old.EntryNum, old.EliminateNum, old.PromotNum, old.RaceID);
            conn.Return(db);
            //2.对外触发某个账户进入某个比赛的时间
            Race r = this[account.RaceID];//注意在晋级的过程中，以及将比赛ID绑定到账户,然后再调用Race.entryAccount来加入该比赛的
            if (AccountEntryRaceEvent != null)
            {
                try
                {
                    AccountEntryRaceEvent(account, r.RaceType);
                }
                catch (Exception ex)
                {
                    debug(PROGRAME + ":对外触发账户加入比赛事件出错:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 某个比赛接收某个账户的报名请求,只有预赛接收报名
        /// </summary>
        /// <param name="account"></param>
        /// <param name="nextstatus"></param>
        void race_SignAccountEvent(IAccount account, QSEnumAccountRaceStatus nextstatus)
        {
            //1.从目的状态得到对应的Race,如果为null则置race_id为0(查找对应的RaceSession)
            Race r = this["DDPreRace"];//报名需要得到最新初赛
            string race_id = r == null ? "0" : r.RaceID;
            AccountRaceStatusChange change = new AccountRaceStatusChange(DateTime.Now, account.ID, account.RaceID, account.RaceStatus, race_id, nextstatus);
            //2.记录账户比赛状态日志以及修改账户所属比赛(mysql)
            ChangeAccountRaceStatus(change);
            //3.修改对应账户的相关设置(内存)
            account.RaceID = race_id;
            account.RaceStatus = nextstatus;
            account.RaceEntryTime = change.DateTime;
            account.Reset();//重置该账户后,账户不被冻结

            //4.如果目的比赛存在,则我们让该账户加入该比赛
            if (r != null)
            {
                //1.加入该比赛
                r.EntryAccount(account);
                //2.初始化账户资金
                TLCtxHelper.CmdAccountCritical.ResetEquity(account.ID, r.StartEquity);
            }
            if (AccountRaceStatusChanged != null)
                AccountRaceStatusChanged(account);
        }

        /// <summary>
        /// 某个比赛触发淘汰账户事件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="nextstatus"></param>
        void race_EliminateAccountEvent(IAccount account, QSEnumAccountRaceStatus nextstatus)
        {

            //1.从目的状态得到对应的Race,如果为null则置race_id为0(查找对应的RaceSession)
            Race r = GetRaceViaAccountRaceStatus(nextstatus);
            string race_id = r == null ? "0" : r.RaceID;
            AccountRaceStatusChange change = new AccountRaceStatusChange(DateTime.Now, account.ID, account.RaceID, account.RaceStatus, race_id, nextstatus);
            //2.记录账户比赛状态日志以及修改账户所属比赛(mysql)
            ChangeAccountRaceStatus(change);
            //3.更新旧比赛对对应的人数数据
            Race old = this[account.RaceID];
            /*
            //复赛淘汰
            if (account.RaceStatus == QSEnumAccountRaceStatus.INSEMIRACE)
                _eliminate_semirace++;
            //初赛淘汰
            if (account.RaceStatus == QSEnumAccountRaceStatus.INPRERACE)
                _eliminate_prerace++;
             * **/
            //更新旧比赛的人数数据
            mysqlRace db = conn.mysqlDB;
            db.UpdateRaceInfo(old.EntryNum, old.EliminateNum, old.PromotNum, old.RaceID);
            conn.Return(db);
            //将选手从就比赛中退出
            old.ExitAccount(account);
            //4.修改对应账户的相关设置(内存)
            account.RaceID = race_id;
            account.RaceStatus = nextstatus;
            account.RaceEntryTime = change.DateTime;
            account.Reset();//重置账户,否则账户的折算权益不会进行更新
            //5.如果目的比赛存在,则我们让该账户加入该比赛
            if (r != null)
            {
                //1.加入该比赛
                r.EntryAccount(account);
                //2.初始化账户资金
                TLCtxHelper.CmdAccountCritical.ResetEquity(account.ID, r.StartEquity);
            }
            if (AccountRaceStatusChanged != null)
                AccountRaceStatusChanged(account);
        }

        /// <summary>
        /// 某个比赛触发晋级账户事件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="nextstatus"></param>
        void race_PromotAccountEvent(IAccount account, QSEnumAccountRaceStatus nextstatus)
        {
            //if (nextstatus == QSEnumAccountRaceStatus.INSEMIRACE)
            //    _promot2semirace++;

            //1.从目的状态得到对应的Race,如果为null则置race_id为0(查找对应的RaceSession)
            Race r = GetRaceViaAccountRaceStatus(nextstatus);
            string race_id = r == null ? "0" : r.RaceID;
            AccountRaceStatusChange change = new AccountRaceStatusChange(DateTime.Now, account.ID, account.RaceID, account.RaceStatus, race_id, nextstatus);
            //2.记录账户比赛状态日志以及修改账户所属比赛(mysql)
            ChangeAccountRaceStatus(change);
            //3.更新旧比赛对对应的人数数据
            Race old = this[account.RaceID];
            //更新旧比赛的人数数据
            mysqlRace db = conn.mysqlDB;
            db.UpdateRaceInfo(old.EntryNum, old.EliminateNum, old.PromotNum, old.RaceID);
            conn.Return(db);
            //将选手从就比赛中退出
            old.ExitAccount(account);
            //4.修改对应账户的相关设置(内存)
            account.RaceID = race_id;
            account.RaceStatus = nextstatus;
            account.RaceEntryTime = change.DateTime;
            account.Reset();
            //4.如果目的比赛存在,则我们让该账户加入该比赛(从一个比赛进入另外一个比赛 有一些固定的事务需要进行操作)
            if (r != null)
            {
                //1.加入该比赛
                r.EntryAccount(account);
                //2.初始化账户资金
                TLCtxHelper.CmdAccountCritical.ResetEquity(account.ID, r.StartEquity);
            }
            if (AccountRaceStatusChanged != null)
                AccountRaceStatusChanged(account);

        }


        /// <summary>
        /// 通过accountracestatus得到对应的比赛
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Race GetRaceViaAccountRaceStatus(QSEnumAccountRaceStatus status)
        {
            switch (status)
            {
                case QSEnumAccountRaceStatus.DDELIMINATE:
                    return null;
                case QSEnumAccountRaceStatus.INREALSTABLE_Check:
                    return null;
                case QSEnumAccountRaceStatus.INREALSTABLE:
                    return this["DDRealStable"];
                case QSEnumAccountRaceStatus.INREALUndulate_Check:
                    return null;
                case QSEnumAccountRaceStatus.INREALUndulate:
                    return this["DDRealUndulate"];
                default:
                    return null;
            }
        }

        /// <summary>
        /// 改变某个账户的比赛状态,记录状态改变日志,更新账户数据库为新的状态
        /// </summary>
        void ChangeAccountRaceStatus(AccountRaceStatusChange change)
        {
            //1.将比赛状态改变插入到数据库
            debug(PROGRAME + ":将账户比赛状态变动插入到数据库记录");
            mysqlRace db = conn.mysqlDB;
            db.InsertRaceStatusChange(change.AccountID, DateTime.Now, change.SourceRaceID, change.SourceStatus, change.DestRaceID, change.DestStatus);
            //2.更新账户的比赛状态记录比赛ID 以及当前状态 以及RaceID
            db.UpdateAccountRaceStatus(change.AccountID, change.DestRaceID, change.DestStatus, change.DateTime);
            conn.Return(db);
            //3.原始Race注销该账户

            //4.目的Race注册该账户

        }


        /// <summary>
        /// 将账户恢复到对应的比赛中
        /// </summary>
        /// <param name="account"></param>
        private void RestoreAccountToRaceSession(IAccount account)
        {
            if (account.RaceID == "0") return;
            string raceid = account.RaceID;
            if (HaveRace(raceid))
            {
                this[raceid].RestoreAccount(account);
                //account.StartEquity = this[raceid].StartEquity;//每组比赛的默认初始金额绑定给该账户 在清算中心加载账户的时候统一绑定模拟初始金额
                //比赛队账户初始金额的初始化在restoreAccount中完成
            }
        }
        /// <summary>
        /// 将交易账户数组加载到对应的Race Session中区
        /// </summary>
        /// <param name="accountarray"></param>
        private void RestoreAccount(IAccount[] accountarray)
        {
            debug(PROGRAME + " :加载交易账户到对应的比赛中去....");
            foreach (IAccount acc in accountarray)
            {
                RestoreAccountToRaceSession(acc);
            }

        }


    }
}
