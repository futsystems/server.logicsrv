using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.MySql;
using TradingLib.API;
using TradingLib.Common;
using System.Data;

namespace TradingLib.Contrib
{
    /// <summary>
    /// 定义了比赛中心,比赛中心管理了所有的比赛,以及对比赛对应函数的调用以及数据库资源的统一调用
    /// </summary>
    [ContribAttr("Race","比赛模块","比赛模块扩展系统的比赛业务,记录每个帐号的参赛过程,执行比赛考核,进行晋级或者淘汰操作,同时计算比赛统计数据")]
    public  partial class RaceCentre : ContribSrvObject, IContrib
    {
        /// <summary>
        /// 某个交易账户进入某个Race事件,用于触发风控规则修改/Race_positiontransaction修改/其他事宜
        /// </summary>
        public event AccountEntryRaceDel AccountEntryRaceEvent;
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
        //IClearCentreSrv _clearcentre;
        /// <summary>
        /// 比赛中心构造函数
        /// 外围服务模块不用传入特定的参数
        /// 1.系统交易内核对象通过全局ctx传入
        /// 2.服务模块自身的设置通过对应的配置文件来载入
        /// 通过这2种参数导入,使得外围服务模块具有很强的独立性,外围模块与核心模块的交互均通过对应的接口进行
        /// 接口协商好后只要保证在接口范围内功能稳定可用则就可以保证该业务模块加入后系统是稳定可用的。
        /// 通过这种方式将核心系统压缩至最小状态,将可以拓展成外围业务模块的统一通过外围模块扩展的方式来加入功能。
        /// </summary>
        public RaceCentre():base("RaceCentre")
        {
            //0.从全局ctx获得核心对象的引用
            //_clearcentre = TLCtxHelper.Ctx.ClearCentre;

            //ConfigFile cfg = ConfigFile.GetConfigFile("race.cfg");
            //conn = new ConnectionPoll<mysqlRace>(cfg["DBAddress"].AsString(), cfg["DBUser"].AsString(), cfg["DBPass"].AsString(), cfg["DBName"].AsString(), cfg["DBPort"].AsInt());

            //1.从数据库恢复比赛Session数据
            //LoadRaceSessionFromMysql();
            //2.将所有的账户按照账户设置分配到不同的比赛Session队列中
            //RestoreAccount(_clearcentre.Accounts);

            //向任务中心注册定时任务
            //TaskCentre.RegisterTask(new TaskProc("复赛比赛帐户风控", new TimeSpan(0, 0, 2), Task_CheckAccountSEMIRACE));
            //TaskCentre.RegisterTask(new TaskProc("实盘帐户风控", new TimeSpan(0, 0, 1), Task_CheckAccountREAL));

            
        }

        #region IContrib 接口实现
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

            //4.将所有的账户按照账户设置分配到不同的比赛Session队列中

            RestoreAccount(TLCtxHelper.CmdAccount.Accounts);


            //5.将本地事件绑定到核心服务
            //当某个账户进入一个某个比赛的时候 调用风控中心修改 该账户的风控规则
            //this.AccountEntryRaceEvent += new AccountEntryRaceDel(TLCtxHelper.Ctx.RiskCentre.OnAccountEntryRace);//处理
            //当某个账户比赛状态发生变化的时候 通知客户端更新比赛状态
            //this.AccountRaceStatusChanged += new IAccountChangedDel(TLCtxHelper.Ctx.MessageExchange.newRaceInfo);//处理
            //客户端请求报名的时候 调用比赛中心报名函数
            //TLCtxHelper.Ctx.MessageExchange.SignupPreraceEvent += new IAccountSignForPreraceDel(this.Signup);//处理
            //tradingserver发送比赛状态的时候需要获取比赛信息 调用比赛中心获取比赛信息
            //TLCtxHelper.Ctx.MessageExchange.GetRaceInfoEvent += new GetRaceInfoDel(this.GetAccountRaceInfo);//获得账户raceinfo//处理
            
            //比赛组件内部的实时风控需要调用风控中心的checkaccount,风控中心只处理 配资客户 配资客户在账户加载时就已经在清算中心有所区分
            //this.AccountCheckEvent += new IAccountCheckDel(TLCtxHelper.Ctx.RiskCentre.CheckAccount);
            //比赛中心与基本组件的交叉调用
            //账户raceinfo信息更新 1.将比赛状态转发给交易客户端 2.将比赛状态转发给管理端
            //this.AccountRaceStatusChanged += new IAccountChangedDel(TLCtxHelper.Ctx.MessageMgr.newRaceInfo);
            //管理服务端请求比赛信息 
            //TLCtxHelper.Ctx.MessageMgr.GetRaceInfoEvent += new GetRaceInfoDel(this.GetAccountRaceInfo);

            //TLCtxHelper.Ctx.MessageExchange.EventAccLoginSuccess += new AccountIdDel(MessageExchange_EventAccLoginSuccess);
            
        }

        void MessageExchange_EventAccLoginSuccess(string account)
        {
            //交易帐号登入成功后,在回报登入成功的消息过程中,我们向客户端发送比赛状态信息
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        { 
            
        }
        /// <summary>
        /// 运行
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


        #region ContribCommand 外部暴露的命令
        [ContribCommandAttr(QSEnumCommandSource.MessageExchange, "racedemo", "racedemo - check the function is called", "we will use this arch do more complex stuff")]
        [MethodArgument("帐号XYZ",QSEnumMethodArgumentType.String,3,"选手的交易帐号公司演示")]
        [MethodArgument("初始权益1101",QSEnumMethodArgumentType.Decimal,4,"报名参赛后,账户恢复的初始权益公式演示")]
        public void RaceFunction(ISession session, int num, string name, decimal va)
        {
            TLCtxHelper.Debug("racefunction is called");
            TLCtxHelper.Debug("sessionid:" + session.FrontID + "|" + session.SessionID);
            TLCtxHelper.Debug("int:" + num.ToString() + " string:" + name.ToString() + " decimal:" + va.ToString());
            
            //race demo
            
            
        }

       

        [ContribCommandAttr(QSEnumCommandSource.MessageExchange,"signup","signup - Signup Prerace","客户端报名参加比赛")]
        public void Signup(ISession session)
        {
            if (!session.IsLoggedIn) return;
            IAccount account = TLCtxHelper.CmdAccount[session.AccountID];

            if (account == null) return;
            string msg;
            bool re = Signup(account, out msg);
            if (re)
            {
                
            }
            else
            { 
                
            }
        }

        #endregion
        #endregion

        int _signupnum = 0;//报名人数
        int _promot2semirace = 0;//晋级复赛
        int _eliminate_semirace = 0;//复赛淘汰
        int _eliminate_prerace = 0;//初赛淘汰

        

        /// <summary>
        /// 返回最近的初赛,用于接受交易客户端的注册
        /// </summary>
        /// <returns></returns>
        Race LatestPrerace
        {
            get {
                Race latest =null;
                foreach (Race r in raceMAP.Values)
                {
                    if (r.Type != QSEnumRaceType.PRERACE) continue;
                    if(latest == null || latest.StartTime < r.StartTime)
                        latest = r;
                }
                return latest;
            }
        
        }

        
        /// <summary>
        /// 选手报名
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Signup(IAccount acc,out string msg)
        {
            debug(PROGRAME + ":选手:" + acc.ID + "报名参加预赛..");
            msg = string.Empty;
            if (acc.RaceID != "0")
            {
                Race race = GetRaceViaAccount(acc);
                if (race != null)
                    msg = "已参加比赛:" + race.Title;
                return false;//已经在比赛中的Account不能报名参加初赛
            }

            if (LatestPrerace == null || !LatestPrerace.IsValidForSign)
            {
                msg = "没有可以报名的比赛";
                return false;
            }
            
            //报名后冻结交易账户,禁止交易一天，次日进行交易 4月2日报名,在计算折算收益的时候 从4月3号的结算信息开始计算
            //acc.Reset();//报名时重置账户,用于从新计算折算数据
            TLCtxHelper.CmdAccount.InactiveAccount(acc.ID);//报名当天冻结账户交易

            mysqlRace db = conn.mysqlDB;
            db.DelAccountPR(acc.ID);//报名当时清空选手原来的比赛PR数据,用于重新计算选手的交易指标
            conn.Return(db);

            LatestPrerace.Sigup(acc);
            _signupnum++;
            return true;
        }
        /// <summary>
        /// 遍历所有比赛运行比赛考核程序进行检查账户
        /// </summary>
        public void Task_CheckRace()
        {
            if (!TLCtxHelper.CmdClearCentre.IsTradeDay) return;//非交易日不用做比赛检查
            debug(PROGRAME +":进行比赛考核...",QSEnumDebugLevel.INFO);
            foreach (Race r in raceMAP.Values)
            {
                r.CheckAccount();
            }
            debug(PROGRAME + ":比赛考核完毕===================================================",QSEnumDebugLevel.INFO);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-----------------------比赛考核完毕--------------------------------");
            sb.AppendLine("当日报名:" + _signupnum.ToString());
            sb.AppendLine("晋级复赛:" + _promot2semirace.ToString());
            sb.AppendLine("初赛淘汰:" + _eliminate_prerace.ToString());
            sb.AppendLine("复赛淘汰:" + _eliminate_semirace.ToString());
            Notify("比赛考核[" + DateTime.Now.ToShortDateString() + "]", sb.ToString());
            _signupnum = 0;
            _promot2semirace = 0;
            _eliminate_semirace = 0;
            _eliminate_prerace = 0;
            Notify("考核比赛", "考核比赛");
        }


        /// <summary>
        /// 新开初赛
        /// </summary>
        public void OpenNewPrerace()
        {
            if (LatestPrerace == null || !LatestPrerace.IsValidForSign)
            {
                DateTime start = DateTime.Now;
                string raceid = "PRERACE-" + start.ToString("yyyyMMdd");
                mysqlRace db = conn.mysqlDB;
                db.NewPrerace(raceid,start,20);
                conn.Return(db);

                try
                {
                    LoadRaceSessionFromMysql(raceid);
                }
                catch (Exception ex)
                {
                    debug("错误:" + ex.ToString());
                }
            }
            else
            {
                debug(PROGRAME + ":当前有初赛可以报名不用新开初赛");
            }
        }

        QSEnumAccountRaceStatus nextRaceStatus(IAccount acc)
        {
            switch (acc.RaceStatus)
            { 
                case QSEnumAccountRaceStatus.INPRERACE:
                    return QSEnumAccountRaceStatus.INSEMIRACE;
                case QSEnumAccountRaceStatus.INSEMIRACE:
                    return QSEnumAccountRaceStatus.INREAL1;
                case QSEnumAccountRaceStatus.INREAL1:
                    return QSEnumAccountRaceStatus.INREAL2;
                case QSEnumAccountRaceStatus.INREAL2:
                    return QSEnumAccountRaceStatus.INREAL3;
                case QSEnumAccountRaceStatus.INREAL3:
                    return QSEnumAccountRaceStatus.INREAL4;
                case QSEnumAccountRaceStatus.INREAL4:
                    return QSEnumAccountRaceStatus.INREAL5;
                case QSEnumAccountRaceStatus.INREAL5:
                    return QSEnumAccountRaceStatus.TOP;
                default:
                    return QSEnumAccountRaceStatus.NORACE;
            }
        }
        /// <summary>
        /// 手工晋级某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void PromptAccount(IAccount acc)
        {
            debug(PROGRAME + ":手工晋级账户:" + acc.ID);
            Race r = GetRaceViaAccount(acc);
            if (r == null) return;
            QSEnumAccountRaceStatus rs = nextRaceStatus(acc);
            if(rs == QSEnumAccountRaceStatus.NORACE) return;
            r.PromotAccount(acc, rs);

        }

        /// <summary>
        /// 手工淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void EliminateAccount(IAccount acc)
        {
            debug(PROGRAME + ":手工淘汰账户:" + acc.ID);
            Race r = GetRaceViaAccount(acc);
            if (r == null) return;
            r.EliminateAccount(acc, QSEnumAccountRaceStatus.ELIMINATE);
        
        }
        /// <summary>
        /// 获得某个账户的比赛信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public IRaceInfo GetAccountRaceInfo(string account)
        {
            try
            {
                IAccount acc = TLCtxHelper.CmdAccount[account];
                RaceInfo ri = new RaceInfo();
                ri.AccountID = acc.ID;
                ri.EntryTime = acc.RaceEntryTime;
                ri.RaceID = acc.RaceID;
                ri.RaceStatus = acc.RaceStatus;
                ri.ObverseProfit = acc.ObverseProfit;
                ri.NowEquity = acc.NowEquity;
                Race r = GetRaceViaAccount(acc);
                if (r == null) return ri;
                ri.StartEquity = r.StartEquity;
                ri.PromptEquity = r.PromptEquity;
                ri.ElimiateEquity = r.EliminateEquity;
                return ri;
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":get account raceinfo error:" + ex.ToString());
                return null;
            }
        }
        Race GetRaceViaAccount(IAccount acc)
        {
            if (HaveRace(acc.RaceID))
                return this[acc.RaceID];
            else
                return null;
        }
        /// <summary>
        /// 获得某个ID标识的race session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Race this[string id] { get { 
            Race r;
            if (raceMAP.TryGetValue(id, out r))
                return r;
            else
                return null;
            
        } }
        Dictionary<string, Race> raceMAP = new Dictionary<string, Race>();
        bool HaveRace(string raceid)
        {
            Race r;
            return (raceMAP.TryGetValue(raceid, out r));
        }
        /// <summary>
        /// 从数据库加载Race Session
        /// </summary>
        private void LoadRaceSessionFromMysql(string race_id=null)
        {
            debug(PROGRAME +":从数据库恢复比赛Session数据....");
            DataSet tradeset ;
            mysqlRace db = conn.mysqlDB;
            if(race_id == null)
                tradeset= db.getRaceSessions();
            else
                tradeset = db.getRaceSessions(race_id);
            conn.Return(db);

            DataTable dt = tradeset.Tables["race_session"];
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                DateTime starttime = Convert.ToDateTime(dr["starttime"]);
                int entrynum = Convert.ToInt16(dr["entry_num"]);
                int eliminatenum= Convert.ToInt16(dr["eliminate_num"]);
                int promotnum= Convert.ToInt16(dr["promot_num"]);

                string raceid = Convert.ToString(dr["race_id"]);
                DateTime beginsigntime = Convert.ToDateTime(dr["begin_sign_time"]);
                DateTime endsigntime = Convert.ToDateTime(dr["end_sign_time"]);
                string comment = Convert.ToString(dr["comment"]);
                QSEnumRaceType racetype= (QSEnumRaceType)Enum.Parse(typeof(QSEnumRaceType),Convert.ToString(dr["race_type"]));

                Race race = new Race(raceid, racetype, entrynum, eliminatenum, promotnum, starttime, beginsigntime, endsigntime);
                //将该race session加入内存
                race.SendDebugEvent +=new DebugDelegate(msgdebug);
                race.PromotAccountEvent += new PromotAccountDel(race_PromotAccountEvent);//晋级交易账户
                race.EliminateAccountEvent += new EliminateAccountDel(race_EliminateAccountEvent);//淘汰交易账户
                race.SignAccountEvent += new EntryAccountDel(race_SignAccountEvent);//报名参加预赛
                race.EntryAccountEvent += new EntryAccountDel(race_EntryAccountEvent);//有新交易账户加入某个比赛

                //将比赛数据缓存到内存
                if (!HaveRace(race.RaceID))
                    raceMAP.Add(race.RaceID, race);
                
            }
            debug(PROGRAME +":数据库恢复比赛数据:" +  raceMAP.Count.ToString()+ "条");
        }

        /// <summary>
        /// 有新的交易账户加入到某个比赛
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
                    AccountEntryRaceEvent(account, r.Type);
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
            Race r = LatestPrerace;//报名需要得到最新初赛
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
            string race_id = r==null?"0":r.RaceID;
            AccountRaceStatusChange change = new AccountRaceStatusChange(DateTime.Now, account.ID, account.RaceID, account.RaceStatus, race_id, nextstatus);
            //2.记录账户比赛状态日志以及修改账户所属比赛(mysql)
            ChangeAccountRaceStatus(change);
            //3.更新旧比赛对对应的人数数据
            Race old = this[account.RaceID];
            //复赛淘汰
            if (account.RaceStatus == QSEnumAccountRaceStatus.INSEMIRACE)
                _eliminate_semirace++;
            //初赛淘汰
            if (account.RaceStatus == QSEnumAccountRaceStatus.INPRERACE)
                _eliminate_prerace++;
            //更新旧比赛的人数数据
            mysqlRace db = conn.mysqlDB;
            db.UpdateRaceInfo(old.EntryNum,old.EliminateNum,old.PromotNum,old.RaceID);
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
            if (nextstatus == QSEnumAccountRaceStatus.INSEMIRACE)
                _promot2semirace++;

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
        /// 返回比赛管理中心所有的比赛信息
        /// </summary>
        public List<Race> RaceList {
            get {
                return raceMAP.Values.ToList<Race>();
            }
        }

        public List<IRaceStatistic> RaceStatList
        {
            
            get
            {
                List<IRaceStatistic> l = new List<IRaceStatistic>();
                foreach (Race r in raceMAP.Values)
                {
                    l.Add(r.StatisticInfo);
                }
                return l;
            }
        }
        public void Display()
        {
            debug("------------------------比赛列表-------------------------------");
            foreach (Race r in raceMAP.Values)
            {
                debug(r.ToString());
            }
            debug("--------------------------------------------------------------");
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
                case QSEnumAccountRaceStatus.ELIMINATE:
                    return null;
                case QSEnumAccountRaceStatus.INSEMIRACE:
                    return this["SEMIRACE"];
                case QSEnumAccountRaceStatus.INREAL1:
                    return this["REAL1"];
                case QSEnumAccountRaceStatus.INREAL2:
                    return this["REAL2"];
                case QSEnumAccountRaceStatus.INREAL3:
                    return this["REAL3"];
                case QSEnumAccountRaceStatus.INREAL4:
                    return this["REAL4"];
                case QSEnumAccountRaceStatus.INREAL5:
                    return this["REAL5"];
                default:
                    return null;
            }
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
            debug(PROGRAME +" :加载交易账户到对应的比赛中去....");
            foreach (IAccount acc in accountarray)
            {
                RestoreAccountToRaceSession(acc);
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


        #region  比赛统计信息
        /// <summary>
        /// 清空比赛统计信息
        /// </summary>
        public void Task_ClearRaceStatistic()
        {
            if (!TLCtxHelper.CmdClearCentre.IsTradeDay) return;//非结算日不进行排名计算

            debug("清空赛统计数据", QSEnumDebugLevel.INFO);
            mysqlRace db = conn.mysqlDB;
            db.ClearRaceStatistics();
            conn.Return(db);
            debug("清空比赛统计数据完毕", QSEnumDebugLevel.INFO);
        }
        /// <summary>
        /// 生成比赛统计数据
        /// 0.清空排行榜数据表
        /// 1.利用数据库view 从Race_postiontransactoin中生成比赛统计数据 交易回合/盈利次数/手续费/累计盈利/累计亏损/等基本统计信息
        /// 2.遍历所有统计信息 并结合当前Account状态 将组合成的信息插入到数据表中
        /// </summary>
        public void Task_GenRaceStatistic()
        {

            if (!TLCtxHelper.CmdClearCentre.IsTradeDay) return;//非结算日不进行排名计算
            try
            {
                debug("生成比赛统计数据", QSEnumDebugLevel.INFO);
                mysqlRace db = conn.mysqlDB;
                DataSet statistics = db.GetRaceStatistics();
                //清空比赛排名数据 由数据维护进程进行统一处理，交易服务程序只负责将数据写入
                //db.ClearRaceStatistics();

                DataTable dt = statistics.Tables["racestatistics"];
                List<long> clist = new List<long>();
                //遍历每一行数据库统计信息 并结合当前Account状态 生成我们需要的数据并插入到数据库
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    string account = Convert.ToString(dr["account"]);//账户
                    int pr_num = 0;//总交易次数
                    Int32.TryParse(dr["num"].ToString(), out pr_num);

                    int winnum = 0;// Convert.ToInt16(dr["win"]);//盈利次数
                    Int32.TryParse(dr["win"].ToString(), out winnum);

                    int lossnum = pr_num - winnum;

                    decimal grossprofit = 0;
                    decimal.TryParse(dr["gain"].ToString(), out grossprofit);//累积盈利

                    decimal grossloss = 0;
                    decimal.TryParse(dr["loss"].ToString(), out grossloss);//累积损失

                    decimal avg_profit = winnum > 0 ? grossprofit / ((decimal)winnum) : 0;
                    decimal avg_loss = lossnum > 0 ? grossloss / ((decimal)lossnum) : 0;

                    decimal perform = 0;
                    decimal.TryParse(dr["totalperformance"].ToString(), out perform);

                    decimal entryperform = 0;
                    decimal.TryParse(dr["entrypeformance"].ToString(), out entryperform);

                    decimal exitperform = 0;
                    decimal.TryParse(dr["exitperformance"].ToString(), out exitperform);

                    int secends = 0;
                    Int32.TryParse(dr["totalsecends"].ToString(), out secends);

                    int _prnum = 0;
                    Int32.TryParse(dr["prnum"].ToString(), out _prnum);
                    decimal minutes = _prnum > 0 ? secends / _prnum / 60 : 0;

                    //判断当前清算中心是否包含该帐户，如果不包含则不记录该数据，如果包含则记录该数据
                    IAccount acc = TLCtxHelper.CmdAccount[account];
                    if (acc == null) continue;//如果账户不存在则直接返回
                    if (acc.RaceStatus == QSEnumAccountRaceStatus.ELIMINATE || acc.RaceStatus == QSEnumAccountRaceStatus.NORACE)
                    {
                        db.DelAccountPR(acc.ID);//费比赛帐户删除该帐户在race_positiontransaction中的数据
                        continue;//非比赛选手不统计
                    }

                    decimal nowequity = acc.NowEquity;//当前权益
                    decimal obverse_equity = acc.ObverseProfit;//折算权益

                    int winday = 0;//盈利日
                    int successivewinday = 0;//连盈天数

                    int lossday = 0;//亏损日
                    int successivelossday = 0;//连亏天数

                    decimal avg_postransperday = (winday + lossday) > 0 ? (decimal)pr_num / (decimal)(winday + lossday) : 0;
                    decimal commission = 0;//累计手续费

                    int race_day = (int)(DateTime.Now - acc.RaceEntryTime).TotalDays;

                    decimal winpercent = (pr_num > 0 ? (decimal)winnum / (decimal)pr_num : 0);
                    decimal profitfactor = avg_loss != 0 ? avg_profit / Math.Abs(avg_loss) : 0;

                    //从数据库获得该选手参赛以来的结算数据,手续费，净利等信息
                    DataSet pd = db.ReTotalDaily(acc.ID, acc.RaceEntryTime, DateTime.Now);//获得参赛以来的每日盈亏
                    DataTable settletable = pd.Tables["settlement"];
                    RaceUtils.StaDayReport(settletable, out winday, out lossday, out successivewinday, out successivelossday, out commission);

                    //将以上计算的比赛统计插入到数据库,用于网站获取数据,这样网站显示数据就不用每次都进行计算,加快网站的处理速度
                    db.InsertRaceStatistics(account, acc.RaceID, acc.RaceStatus.ToString(), acc.RaceEntryTime, race_day,
                        acc.NowEquity, acc.ObverseProfit, commission, pr_num, winnum, lossnum, avg_profit, avg_loss,
                        winday, successivewinday, lossday, successivelossday,
                        avg_postransperday, minutes, perform, entryperform, exitperform, winpercent, profitfactor);
                }



                conn.Return(db);
                Notify("生成比赛统计数据", "生成比赛统计数据");
            }
            catch (Exception ex)
            {
                debug("生成比赛统计数据错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        #endregion

        #region 风控检查



        void checkAccount(IAccount a)
        {
            if (AccountCheckEvent != null)
                AccountCheckEvent(a);
        
        }
        /// <summary>
        /// 检查复赛账户
        /// </summary>
        void Task_CheckAccountSEMIRACE()
        {
            try
            {
                if (this["SEMIRACE"] != null)
                {
                    foreach (IAccount a in this["SEMIRACE"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("RaceCentre CheckAccountSemirace Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        /// <summary>
        /// 实盘账户盘中实时检查
        /// </summary>
        void Task_CheckAccountREAL()
        {
            try
            {
                if (this["REAL1"] != null)
                {
                    foreach (IAccount a in this["REAL1"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
                if (this["REAL2"] != null)
                {
                    foreach (IAccount a in this["REAL2"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
                if (this["REAL3"]!=null)
                {
                    foreach (IAccount a in this["REAL3"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
                if (this["REAL4"] != null)
                {
                    foreach (IAccount a in this["REAL4"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
                if (this["REAL5"] != null)
                {
                    foreach (IAccount a in this["REAL5"].Contestants)
                    {
                        checkAccount(a);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("RaceCentre CheckAccountReal Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        #endregion
    }
}
