using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib
{
    /// <summary>
    /// 定义了比赛,比赛有多个初赛,一个复赛,实盘各个级别的Race只有一个,不断的有新的选手晋级进入或者淘汰出去
    /// </summary>
    public class Race : BaseSrvObject, IRaceStatistic
    {
        public event PromotAccountDel PromotAccountEvent;//晋级事件
        public event EliminateAccountDel EliminateAccountEvent;//淘汰事件
        public event EntryAccountDel SignAccountEvent;//报名参赛事件
        public event EntryAccountDel EntryAccountEvent;//新选手加入事件

        DateTime _starttime;
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime StartTime { get { return _starttime; } set{_starttime = value;}}
        string _id;
        /// <summary>
        /// 比赛session标识
        /// </summary>
        public string RaceID { get { return _id; } set { _id = value; } }

        public string Title {
            get {
                bool sim = (this.Type == QSEnumRaceType.PRERACE || this.Type == QSEnumRaceType.SEMIRACE);
                string title = (sim == true ? "模拟:" : "实盘:");
                title = title + LibUtil.GetEnumDescription(this.Type);
                return title;
            }
        }
        QSEnumRaceType _type;
        /// <summary>
        /// 比赛类型
        /// </summary>
        public QSEnumRaceType Type{get{return _type;}set{_type = value;}}
        /// <summary>
        /// 报名开始时间
        /// </summary>
        public DateTime BeginSingUpTime { get; set; }
        /// <summary>
        /// 报名结束时间
        /// </summary>
        public DateTime EndSingUpTime { get; set; }
        /// <summary>
        /// 查看该race是否接受自注注册
        /// </summary>
        public bool IsValidForSign{
            get
            {
                debug("it is go to here");
                debug("start:" + BeginSingUpTime.ToString());
                debug("end:" + EndSingUpTime.ToShortDateString());
                debug("now:" + DateTime.Now.ToString());
                debug("type:" + Type.ToString());
                return (this.Type == QSEnumRaceType.PRERACE && DateTime.Now > BeginSingUpTime && DateTime.Now < EndSingUpTime);
            }
        }
        /// <summary>
        /// 返回该比赛的初始资金
        /// </summary>
        public decimal StartEquity {
            get
            {
                return RaceRule.StartEquity(this.Type);
            }
            set { }
        }
        /// <summary>
        /// 晋级权益
        /// </summary>
        public decimal PromptEquity {
            get {
                return RaceRule.PromptEquity(this.Type);
            }
        }
        /// <summary>
        /// 淘汰权益
        /// </summary>
        public decimal EliminateEquity {
            get { return RaceRule.EliminateEquity(this.Type); }
        }

        /// <summary>
        /// 参赛总人数(包含淘汰与晋级)
        /// </summary>
        public int EntryNum { get; set; }
        /// <summary>
        /// 淘汰人数
        /// </summary>
        public int EliminateNum { get; set; }
        /// <summary>
        /// 晋级人数
        /// </summary>
        public int PromotNum { get; set; }


        public RaceStatistic StatisticInfo {
            get
            {
                RaceStatistic r = new RaceStatistic();
                r.RaceID = this.RaceID;
                r.Type = this.Type;
                r.StartTime = this.StartTime;
                r.BeginSingUpTime = this.BeginSingUpTime;
                r.EndSingUpTime = this.EndSingUpTime;
                r.StartEquity = this.StartEquity;
                r.EntryNum = this.EntryNum;
                r.EliminateNum = this.EliminateNum;
                r.PromotNum = this.PromotNum;
                r.ContestantsNum = this.ContestantsNum;

                return r;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">比赛标识</param>
        /// <param name="type">比赛类别</param>
        /// <param name="entrynum">参赛人数</param>
        /// <param name="eliminatenum">淘汰人数</param>
        /// <param name="promotnum">晋级人数</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="beginsigntime">开始报名时间</param>
        /// <param name="endsigntime">结束报名时间</param>
        public Race(string id,QSEnumRaceType type,int entrynum,int eliminatenum,int promotnum,DateTime starttime,DateTime beginsigntime,DateTime endsigntime):base("Race")
        {
            RaceID = id;
            Type = type;
            EntryNum = entrynum;
            EliminateNum = eliminatenum;
            PromotNum = promotnum;
            StartTime = starttime;
            BeginSingUpTime = beginsigntime;
            EndSingUpTime = endsigntime;
        }
        /// <summary>
        /// 返回当前在赛选手
        /// </summary>
        public List<IAccount> Contestants { 
            get {
                return raceAccountMap.Values.ToList();
            }
        }

        /// <summary>
        /// 返回当前在赛中的人数
        /// </summary>
        public int ContestantsNum{
        
            get{return raceAccountMap.Count;}
            set { }
        }
        


        //记录比赛中的Account
        private Dictionary<string, IAccount> raceAccountMap = new Dictionary<string, IAccount>();
        /// <summary>
        /// 判断该比赛中是否有某个account
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        bool HaveAccount(string accountid)
        {
            IAccount acc;
            return raceAccountMap.TryGetValue(accountid,out acc);
        }
        /// <summary>
        /// 将某个账户插入到对应的racesession,用于从数据库加载数据初始化时 将账户分配到不同的RaceSession
        /// RestoreAccount不触发新选手加入事件
        /// </summary>
        /// <param name="account"></param>
        public void RestoreAccount(IAccount account)
        { 
            if(account.RaceID != this.RaceID) return;//如果账户的raceID与本race id不符合 则拒绝加入该账户
            if (!HaveAccount(account.ID))
            {
                raceAccountMap.Add(account.ID, account);
                account.StartEquity = this.StartEquity;
            }
            
        }
        /// <summary>
        /// 将某个账户从该racesession中删除
        /// </summary>
        /// <param name="account"></param>
        public void DelAccount(IAccount account)
        {
            if(HaveAccount(account.ID))
                raceAccountMap.Remove(account.ID);
        }

        /// <summary>
        /// 晋级某个账户 
        /// </summary>
        /// <param name="acc"></param>
        public void PromotAccount(IAccount acc,QSEnumAccountRaceStatus next)
        {
            debug(Title+":晋级账户:"+acc.ToString() +next.ToString());
            this.PromotNum ++;
            if (PromotAccountEvent != null)
                PromotAccountEvent(acc, next);
        }
        /// <summary>
        /// 淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void EliminateAccount(IAccount acc,QSEnumAccountRaceStatus next)
        {
            debug(Title + ":淘汰账户:" + acc.ToString() + next.ToString());
            this.EliminateNum ++;
            if (EliminateAccountEvent != null)
                EliminateAccountEvent(acc, next);
        }

        /// <summary>
        /// 加入某个账户进入比赛,所有的选手都是通过racecentre调用不同的race的entryaccount加入到比赛中去的
        /// 实际比赛中的选手通过账户个数统计,EntryNum统计的是从开始到现在加入到比赛的选手个数 = 淘汰 +晋级 +保留 的选手
        /// </summary>
        /// <param name="acc"></param>
        public void EntryAccount(IAccount acc)
        {
            
            this.EntryNum ++;
            //从数据库恢复账户与从EntryAccount的区别在于对比赛人数的计数
            this.RestoreAccount(acc);
            if (EntryAccountEvent != null)
                EntryAccountEvent(acc,QSEnumAccountRaceStatus.NORACE);//这里的账户进入比赛的时间只是用于记录数据库信息，不做操作。nextstatus没有作用

        }
        /// <summary>
        /// 某个选手退出该比赛,淘汰  晋级都会产生，那么系统在下一次账户检查中就不会检查该账户，而是被其他比赛规则进行检查了
        /// </summary>
        /// <param name="acc"></param>
        public void ExitAccount(IAccount acc)
        {
            if (HaveAccount(acc.ID))
                raceAccountMap.Remove(acc.ID);
        }

        /// <summary>
        /// 报名参加该届比赛,只有预赛可以自主报名,其他比赛均在大赛规则中自动进行
        /// </summary>
        /// <param name="acount"></param>
        public void Sigup(IAccount account)
        {
            
            if (this.Type != QSEnumRaceType.PRERACE)
                return;
            debug(Title + ":注册账户:" + account.ToString());
            if (SignAccountEvent != null)
                SignAccountEvent(account, QSEnumAccountRaceStatus.INPRERACE);//通过对外触发注册事件,由racecentre调用对应的race的entryAccount来触发计数器
        }

        public override string ToString()
        {
            string acclist=string.Empty;
            foreach(IAccount a in this.Contestants)
            {
                acclist = acclist + a.ID+",";
            }
            return "比赛[" + Title +RaceID +"]" + " 总选手:" + EntryNum.ToString() + " 在赛选手:" + this.ContestantsNum.ToString() + " [" + acclist + "]" + " 淘汰:" + EliminateNum.ToString() + " 晋级:" + PromotNum.ToString();
        }
        /// <summary>
        /// 每日定期检查所有参赛账户,用于判断账户是保留 晋级 还是淘汰
        /// </summary>
        public void CheckAccount()
        { 
            //遍历所有的选手进行账户检查
            debug(Title + ":检查参赛账户权益...");
            foreach(IAccount a in Contestants)
            {
                CheckAccount(a);
            }
        }

        /// <summary>
        /// 1.检查某个账户 判断是晋级还是淘汰(晋级淘汰标准)
        /// 2.根据晋级淘汰目的地规则，判断将该账户晋级或者淘汰到哪个状态
        /// </summary>
        /// <param name="account"></param>
        void CheckAccount(IAccount account)
        {
            QSEnumRaceCheckResult  r;

            //根据Race的类型选择不同的判断规则来判断当前账户是 保留,淘汰,还是晋级
            switch (Type)
            {
                case QSEnumRaceType.PRERACE:
                    r = RaceRule.PRERACECheck(account);
                    break;
                case QSEnumRaceType.SEMIRACE:
                    r = RaceRule.SEMIRACECheck(account);
                    break;
                case QSEnumRaceType.REAL1:
                    r = RaceRule.REAL1Check(account);
                    break;
                case QSEnumRaceType.REAL2:
                    r = RaceRule.REAL2Check(account);
                    break;
                case QSEnumRaceType.REAL3:
                    r = RaceRule.REAL3Check(account);
                    break;
                case QSEnumRaceType.REAL4:
                    r = RaceRule.REAL4Check(account);
                    break;
                case QSEnumRaceType.REAL5:
                    r = RaceRule.REAL5Check(account);
                    break;
                default:
                    r= QSEnumRaceCheckResult.STAY;
                    break;
            }
            //debug("检查结果:" +r.ToString());
            //得到检查结果后,调用racerule来判断我们需要将该账户推送到哪个状态
            if(r == QSEnumRaceCheckResult.STAY) return;//如果检查该账户得到保留结果,则直接返回不用修改账户Race状态
            QSEnumAccountRaceStatus nextstatus = RaceRule.GetRaceStatus(this.Type, r);
            if (nextstatus == QSEnumAccountRaceStatus.TOP) return;//达到顶级比赛状态的选手直接返回
            //根据淘汰或者晋级的状态 将该账户淘汰 或者晋级
            if (r == QSEnumRaceCheckResult.PROMOT)
            {
                
                this.PromotAccount(account, nextstatus);
                debug("比赛[" + Title + "]"+" 晋级选手:"+account.ID+" 到:"+nextstatus.ToString());
            }
            if(r == QSEnumRaceCheckResult.ELIMINATE)
            {
                this.EliminateAccount(account,nextstatus);
                debug("比赛[" + Title + "]" + " 淘汰选手:" + account.ID + " 到:" + nextstatus.ToString());
            }
        }
    }

    //记录了某个账户的比赛状态改变
    public struct AccountRaceStatusChange
    {
        public AccountRaceStatusChange(DateTime time, string accid, string sraceid, QSEnumAccountRaceStatus source, string draceid, QSEnumAccountRaceStatus dest)
        {
            _time = time;
            _accid = accid;
            _srcid = sraceid;
            _srcstatus = source;
            _destid = draceid;
            _destsoruce = dest;
        }
        DateTime _time;
        /// <summary>
        /// 状态改变时间
        /// </summary>
        public DateTime DateTime { get { return _time; } set { _time = value; } }
        string _accid;
        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccountID { get { return _accid; } set { _accid = value; } }
        string _srcid;
        /// <summary>
        /// 起始比赛ID
        /// </summary>
        public string SourceRaceID { get { return _srcid; } set { _srcid = value; } }
        QSEnumAccountRaceStatus _srcstatus;
        /// <summary>
        /// 起始比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus SourceStatus { get { return _srcstatus; } set { _srcstatus = value; } }

        string _destid;
        /// <summary>
        /// 目的比赛ID
        /// </summary>
        public string DestRaceID { get { return _destid; } set { _destid = value; } }
        QSEnumAccountRaceStatus _destsoruce;
        /// <summary>
        /// 目的比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus DestStatus { get { return _destsoruce; } set { _destsoruce = value; } }
    }


}
