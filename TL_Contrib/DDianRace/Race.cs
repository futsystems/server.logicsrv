using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    public class Race:BaseSrvObject
    {
        public event PromotAccountDel PromotAccountEvent;//晋级事件
        public event EliminateAccountDel EliminateAccountEvent;//淘汰事件
        public event EntryAccountDel SignAccountEvent;//报名参赛事件
        public event EntryAccountDel EntryAccountEvent;//新选手加入事件


        string _id = "";
        /// <summary>
        /// 比赛标识
        /// </summary>
        public string RaceID { get { return _id; } set { _id = value; } }


        QSEnumDDRaceType _racetype;
        /// <summary>
        /// 比赛类别
        /// </summary>
        public QSEnumDDRaceType RaceType { get { return _racetype; } set { _racetype = value; } }


        /// <summary>
        /// 比赛标题
        /// </summary>
        public string Title { get {

            return LibUtil.GetEnumDescription(this.RaceType);
        } }

        /// <summary>
        /// 是否允许报名参赛
        /// </summary>
        public bool IsValidForSign
        {
            get
            {
                if (this.RaceType == QSEnumDDRaceType.RaceSim)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 比赛初始资金
        /// 如果比赛初始资金为0，则表明该赛组是按照具体的帐户进行动态分配资金
        /// </summary>
        public decimal StartEquity
        {
            get
            {
                return RaceRule.StartEquity(this.RaceType);
            }
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


        public Race(string id, QSEnumDDRaceType type, int entryNum, int eliminateNum, int promotNum):
            base("Race:"+id)
        {
            RaceID = id;
            RaceType = type;
            EntryNum = entryNum;
            EliminateNum = entryNum;
            PromotNum = promotNum;
        }



        /// <summary>
        /// 返回当前在赛选手
        /// </summary>
        public List<IAccount> Contestants
        {
            get
            {
                return raceAccountMap.Values.ToList();
            }
        }



        //记录比赛中的Account
        private Dictionary<string, IAccount> raceAccountMap = new Dictionary<string, IAccount>();
        /// <summary>
        /// 返回当前在赛中的人数
        /// </summary>
        public int ContestantsNum
        {
            get { return raceAccountMap.Count; }
        }


        /// <summary>
        /// 判断该比赛中是否有某个account
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        bool HaveAccount(string accountid)
        {
            IAccount acc;
            return raceAccountMap.TryGetValue(accountid, out acc);
        }
        /// <summary>
        /// 将某个账户插入到对应的racesession,用于从数据库加载数据初始化时 将账户分配到不同的RaceSession
        /// RestoreAccount不触发新选手加入事件
        /// </summary>
        /// <param name="account"></param>
        public void RestoreAccount(IAccount account)
        {
            if (account.RaceID != this.RaceID) return;//如果账户的raceID与本race id不符合 则拒绝加入该账户 每个帐户有绑定的RaceID
            if (!HaveAccount(account.ID))
            {
                raceAccountMap.Add(account.ID, account);
                //account.StartEquity = this.StartEquity;
            }

        }
        /// <summary>
        /// 将某个账户从该racesession中删除
        /// </summary>
        /// <param name="account"></param>
        public void DelAccount(IAccount account)
        {
            if (HaveAccount(account.ID))
                raceAccountMap.Remove(account.ID);
        }

        /// <summary>
        /// 晋级某个账户 
        /// </summary>
        /// <param name="acc"></param>
        public void PromotAccount(IAccount acc, QSEnumAccountRaceStatus next)
        {
            debug(Title + ":晋级账户:" + acc.ToString() + next.ToString());

            //只有模拟比赛可以晋级
            if (this.RaceType == QSEnumDDRaceType.RaceSim)
            {
                this.PromotNum++;
                if (PromotAccountEvent != null)
                    PromotAccountEvent(acc, next);
            }
        }
        /// <summary>
        /// 淘汰某个账户
        /// </summary>
        /// <param name="acc"></param>
        public void EliminateAccount(IAccount acc, QSEnumAccountRaceStatus next)
        {
            debug(Title + ":淘汰账户:" + acc.ToString() + next.ToString());

            //模拟比赛不能淘汰帐户
            if (this.RaceType != QSEnumDDRaceType.RaceSim)
            {
                this.EliminateNum++;
                if (EliminateAccountEvent != null)
                    EliminateAccountEvent(acc, next);
            }
        }

        /// <summary>
        /// 加入某个账户进入比赛,所有的选手都是通过racecentre调用不同的race的entryaccount加入到比赛中去的
        /// 实际比赛中的选手通过账户个数统计,EntryNum统计的是从开始到现在加入到比赛的选手个数 = 淘汰 +晋级 +保留 的选手
        /// </summary>
        /// <param name="acc"></param>
        public void EntryAccount(IAccount acc)
        {

            this.EntryNum++;
            //从数据库恢复账户与从EntryAccount的区别在于对比赛人数的计数
            this.RestoreAccount(acc);
            if (EntryAccountEvent != null)
                EntryAccountEvent(acc, QSEnumAccountRaceStatus.NORACE);//这里的账户进入比赛的时间只是用于记录数据库信息，不做操作。nextstatus没有作用

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

            if (this.RaceType != QSEnumDDRaceType.RaceSim)
                return;
            debug(Title + ":注册账户:" + account.ToString());
            if (SignAccountEvent != null)
                SignAccountEvent(account, QSEnumAccountRaceStatus.INPRERACE);//通过对外触发注册事件,由racecentre调用对应的race的entryAccount来触发计数器
        }


    }


    /// <summary>
    /// 记录了某个帐户的比赛状态改变
    /// 在某个时间 某个帐户 从某个比赛状态 进入到 下一个比赛状态，如果有比赛编号则记录比赛编号
    /// </summary>
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
