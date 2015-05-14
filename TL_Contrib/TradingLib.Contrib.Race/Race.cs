using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.Race
{
    public class RaceSetting
    {
        /// <summary>
        /// 比赛session标识
        /// </summary>
        public string RaceID { get; set; }

        /// <summary>
        /// 比赛级别
        /// </summary>
        public QSEnumRaceType RaceType { get; set; }

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

        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 报名开始时间
        /// </summary>
        public long BeginSignTime { get; set; }

        /// <summary>
        /// 报名结束时间
        /// </summary>
        public long EndSignTime { get; set; }

    }


    /// <summary>
    /// 定义了比赛,比赛有多个初赛,一个复赛,实盘各个级别的Race只有一个,不断的有新的选手晋级进入或者淘汰出去
    /// </summary>
    public class Race:RaceSetting
    {

        public event Action<RaceService, QSEnumAccountRaceStatus> PromotAccountEvent;//晋级事件
        public event Action<RaceService, QSEnumAccountRaceStatus> EliminateAccountEvent;//淘汰事件
        public event Action<RaceService, QSEnumAccountRaceStatus> SignAccountEvent;//报名参赛事件
        public event Action<RaceService, QSEnumAccountRaceStatus> EntryAccountEvent;//新选手加入事件

        public string Title
        {
            get
            {
                bool sim = (this.RaceType == QSEnumRaceType.PRERACE || this.RaceType == QSEnumRaceType.SEMIRACE);
                string title = (sim == true ? "模拟-" : "实盘-");
                title = title + Util.GetEnumDescription(this.RaceType);
                return title;
            }
        }


        /// <summary>
        /// 查看该race是否接受自注注册
        /// </summary>
        public bool IsValidForSign
        {
            get
            {
                long now = Util.ToTLDateTime();
                return (this.RaceType == QSEnumRaceType.PRERACE && now > this.BeginSignTime && now < this.EndSignTime);
            }
        }
        /// <summary>
        /// 返回该比赛的初始资金
        /// </summary>
        public decimal StartEquity
        {
            get
            {
                return RaceRule.StartEquity(this.RaceType);
            }
            set { }
        }

        /// <summary>
        /// 晋级权益
        /// </summary>
        public decimal PromptEquity
        {
            get
            {
                return RaceRule.PromptEquity(this.RaceType);
            }
        }

        /// <summary>
        /// 淘汰权益
        /// </summary>
        public decimal EliminateEquity
        {
            get { return RaceRule.EliminateEquity(this.RaceType); }
        }
       

        public Race()
        {

        }
        /// <summary>
        /// 返回当前在赛选手
        /// </summary>
        public List<RaceService> Contestants
        {
            get
            {
                return raceAccountMap.Values.ToList();
            }
        }

        /// <summary>
        /// 返回当前在赛中的人数
        /// </summary>
        public int ContestantsNum
        {

            get { return raceAccountMap.Count; }
            set { }
        }



        //记录比赛中的Account
        private Dictionary<string, RaceService> raceAccountMap = new Dictionary<string, RaceService>();

        /// <summary>
        /// 判断该比赛中是否有某个account
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        bool HaveAccount(string accountid)
        {
            return raceAccountMap.Keys.Contains(accountid);
        }

        /// <summary>
        /// 将某个账户插入到对应的racesession,用于从数据库加载数据初始化时 将账户分配到不同的RaceSession
        /// RestoreAccount不触发新选手加入事件
        /// </summary>
        /// <param name="account"></param>
        public void RestoreAccount(RaceService rs)
        {
            if (rs.RaceID != this.RaceID) return;//如果账户的raceID与本race id不符合 则拒绝加入该账户
            if (!HaveAccount(rs.Acct))
            {
                raceAccountMap.Add(rs.Acct, rs);
            }

        }



        /// <summary>
        /// 晋级某个账户 
        /// </summary>
        /// <param name="acc"></param>
        public void PromotAccount(RaceService rs, QSEnumAccountRaceStatus next)
        {
            Util.Debug(Title + ":晋级账户:" + rs.Account + next.ToString());
            this.PromotNum++;
            if (PromotAccountEvent != null)
                PromotAccountEvent(rs, next);
        }

        /// <summary>
        /// 淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void EliminateAccount(RaceService rs, QSEnumAccountRaceStatus next)
        {
            Util.Debug(Title + ":淘汰账户:" + rs.Account + next.ToString(),QSEnumDebugLevel.INFO);
            this.EliminateNum++;
            if (EliminateAccountEvent != null)
                EliminateAccountEvent(rs, next);
        }


        /// <summary>
        /// 报名参加该届比赛,只有预赛可以自主报名,其他比赛均在大赛规则中自动进行
        /// </summary>
        /// <param name="acount"></param>
        public void Sigup(RaceService rs)
        {

            if (this.RaceType != QSEnumRaceType.PRERACE)
                return;
            Util.Debug(Title + ":注册账户:" + rs.Account, QSEnumDebugLevel.INFO);
            if (SignAccountEvent != null)
                SignAccountEvent(rs, QSEnumAccountRaceStatus.INPRERACE);//通过对外触发注册事件,由racecentre调用对应的race的entryAccount来触发计数器
        }

        /// <summary>
        /// 加入某个账户进入比赛,所有的选手都是通过racecentre调用不同的race的entryaccount加入到比赛中去的
        /// 实际比赛中的选手通过账户个数统计,EntryNum统计的是从开始到现在加入到比赛的选手个数 = 淘汰 +晋级 +保留 的选手
        /// </summary>
        /// <param name="acc"></param>
        public void EntryAccount(RaceService rs)
        {
            this.EntryNum++;
            //从数据库恢复账户与从EntryAccount的区别在于对比赛人数的计数
            this.RestoreAccount(rs);
            if (EntryAccountEvent != null)
                EntryAccountEvent(rs, QSEnumAccountRaceStatus.NORACE);//这里的账户进入比赛的时间只是用于记录数据库信息，不做操作。nextstatus没有作用

        }

        /// <summary>
        /// 某个选手退出该比赛,淘汰  晋级都会产生，那么系统在下一次账户检查中就不会检查该账户，而是被其他比赛规则进行检查了
        /// </summary>
        /// <param name="acc"></param>
        public void ExitAccount(RaceService rs)
        {
            if (HaveAccount(rs.Acct))
                raceAccountMap.Remove(rs.Acct);
        }

        



        public override string ToString()
        {
            string acclist = string.Empty;
            foreach (IAccount a in this.Contestants)
            {
                acclist = acclist + a.ID + ",";
            }
            return "比赛[" + Title + RaceID + "]" + " 总选手:" + EntryNum.ToString() + " 在赛选手:" + this.ContestantsNum.ToString() + " [" + acclist + "]" + " 淘汰:" + EliminateNum.ToString() + " 晋级:" + PromotNum.ToString();
        }

        /// <summary>
        /// 每日定期检查所有参赛账户,用于判断账户是保留 晋级 还是淘汰
        /// </summary>
        public void CheckAccount()
        {
            //遍历所有的选手进行账户检查
            Util.Debug(Title + ":检查参赛账户权益...",QSEnumDebugLevel.INFO);
            foreach (var a in Contestants)
            {
                CheckAccount(a);
            }
        }

        /// <summary>
        /// 1.检查某个账户 判断是晋级还是淘汰(晋级淘汰标准)
        /// 2.根据晋级淘汰目的地规则，判断将该账户晋级或者淘汰到哪个状态
        /// </summary>
        /// <param name="account"></param>
        void CheckAccount(RaceService  rs)
        {
            QSEnumRaceCheckResult r;
            //根据Race的类型选择不同的判断规则来判断当前账户是 保留,淘汰,还是晋级
            decimal exequity = 0;
            switch (RaceType)
            {
                case QSEnumRaceType.PRERACE:
                    r = RaceRule.PRERACECheck(rs.Account,out exequity);
                    break;
                case QSEnumRaceType.SEMIRACE:
                    r = RaceRule.SEMIRACECheck(rs.Account, out exequity);
                    break;
                case QSEnumRaceType.REAL1:
                    r = RaceRule.REAL1Check(rs.Account, out exequity);
                    break;
                case QSEnumRaceType.REAL2:
                    r = RaceRule.REAL2Check(rs.Account, out exequity);
                    break;
                case QSEnumRaceType.REAL3:
                    r = RaceRule.REAL3Check(rs.Account, out exequity);
                    break;
                case QSEnumRaceType.REAL4:
                    r = RaceRule.REAL4Check(rs.Account, out exequity);
                    break;
                case QSEnumRaceType.REAL5:
                    r = RaceRule.REAL5Check(rs.Account, out exequity);
                    break;
                default:
                    r = QSEnumRaceCheckResult.STAY;
                    break;
            }

            //更新比赛服务的考核信息
            rs.ExamineTime = Util.ToTLDateTime();
            rs.ExamineEquity = exequity;

            //更新考核信息
            ORM.MRace.UpdateRaceServiceExamine(rs);

            //如果比赛处于冻结状态检查报名时间和当前时间
            if (!rs.IsAvabile)
            {
                if (DateTime.Now.Subtract(Util.ToDateTime(rs.EntryTime)).TotalDays > 1)
                {
                    rs.Active();//激活配资服务
                }

            }

            //debug("检查结果:" +r.ToString());
            //得到检查结果后,调用racerule来判断我们需要将该账户推送到哪个状态
            if (r == QSEnumRaceCheckResult.STAY) return;//如果检查该账户得到保留结果,则直接返回不用修改账户Race状态
            QSEnumAccountRaceStatus nextstatus = RaceRule.GetRaceStatus(this.RaceType, r);
            if (nextstatus == QSEnumAccountRaceStatus.TOP) return;//达到顶级比赛状态的选手直接返回

            //根据淘汰或者晋级的状态 将该账户淘汰 或者晋级
            if (r == QSEnumRaceCheckResult.PROMOT)
            {
                this.PromotAccount(rs, nextstatus);
                Util.Debug("比赛[" + Title + "]" + " 晋级选手:" + rs.Account + " 到:" + nextstatus.ToString(), QSEnumDebugLevel.INFO);
            }
            if (r == QSEnumRaceCheckResult.ELIMINATE)
            {
                this.EliminateAccount(rs, nextstatus);
                Util.Debug("比赛[" + Title + "]" + " 淘汰选手:" + rs.Account + " 到:" + nextstatus.ToString(), QSEnumDebugLevel.INFO);
            }

            
        }
    }


    //记录了某个账户的比赛状态改变
    public class RaceStatusChange
    {
        public RaceStatusChange(long datetime, string account, string sraceid, QSEnumAccountRaceStatus source, string draceid, QSEnumAccountRaceStatus dest)
        {
            this.DateTime = datetime;
            this.Account = account;
            this.SrcRaceID = sraceid;
            this.SrcStatus = source;
            this.DestRaceID = draceid;
            this.DestStatus = dest;

        }

        /// <summary>
        /// 状态改变时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 起始比赛ID
        /// </summary>
        public string SrcRaceID { get; set; }

        /// <summary>
        /// 起始比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus SrcStatus { get; set; }

        /// <summary>
        /// 目的比赛ID
        /// </summary>
        public string DestRaceID { get; set; }
    
        /// <summary>
        /// 目的比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus DestStatus { get; set; }


    }
}
