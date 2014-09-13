using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易帐户对象
    /// </summary>
    public partial class AccountBase : IAccount
    {
        


        public IAccountClearCentre ClearCentre { get;set;}
        public IAccountRiskCentre RiskCentre {get;set;}

        //#region 构造函数
        //public AccountBase() 
        //{ }
        public AccountBase(string AccountID) { _id = AccountID; }
        
        /*
        public AccountBase(string AccountID, string Description) { _id = AccountID; _desc = Description; }
        public AccountBase(string AccountID, string accType, decimal lastequity,string agentcode="0000")
            : this(AccountID)
        {
            _lastequity = lastequity;
            _ordroutetype = (QSEnumOrderTransferType)Enum.Parse(typeof(QSEnumOrderTransferType), accType);
            _agentcode = agentcode;

        }**/
        //#endregion


        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool isValid { get { return (ID != null) && (ID != ""); } }
        bool _execute = true;
        bool _notify = true;
        /// <summary>
        /// 是否进行交易回报
        /// </summary>
        public bool Notify { get { return _notify; } set { _notify = value; } }
        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool Execute { get { return _execute; } set { _execute = value; } }

        int _userid = 0;
        /// <summary>
        /// 与交易帐号所绑定的全局UserID
        /// </summary>
        public int UserID { get { return _userid; } set { _userid = value; } }


        string _id = "";
        /// <summary>
        /// 账户ID
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get { return _id; } }
        string _desc;
        /// <summary>
        /// 账户描述
        /// </summary>
        /// <value>The desc.</value>
        public string Desc { get { return _desc; } set { _desc = value; } }

        public override string ToString()
        {
            return ID;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            AccountBase o = (AccountBase)obj;
            return base.Equals(o);
        }
        public bool Equals(AccountBase a)
        {
            return this._id.Equals(a.ID);
        }

        bool _intraday = true;
        /// <summary>
        /// 是否是日内交易
        /// </summary>
        public bool IntraDay { get { return _intraday; } set { _intraday = value; } }

        /// <summary>
        /// 账户类别 交易员 配资客户
        /// </summary>
        public QSEnumAccountCategory Category { get; set; }

        /// <summary>
        /// 硬件地址
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 最近结算确认日期
        /// </summary>
        public long SettlementConfirmTimeStamp { get; set; }
        

        #region 比赛所用字段
        /// <summary>
        /// 记录账户的建立时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 比赛周期ID,记录该账户处于的比赛界数。每个月一个比赛周期,每个月未计入比赛的选手可以申请加入该界比赛
        /// ？是否可以考虑取消
        /// </summary>
        public string RaceID { get; set; }

        /// <summary>
        /// 进入当前比赛状态的时间,用于计算进入该比赛状态以来的盈亏,达到晋级标准 晋级并记录晋级时间。淘汰则
        /// </summary>
        public DateTime RaceEntryTime { get; set; }
        /// <summary>
        /// 当前账户的比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus RaceStatus { get; set; }
        //当某个账户有一条race状态改变时,进行日志记录account,初始状态,目的状态
        #endregion


        private QSEnumOrderTransferType _ordroutetype = QSEnumOrderTransferType.SIM;
        public QSEnumOrderTransferType OrderRouteType { get { return _ordroutetype; } set { _ordroutetype = value; } }

        #region 该账户当日交易信息
        public bool AnyPosition { get { return ClearCentre.AnyPosition; } }//是否有持仓
        /// <summary>
        /// 获得账户当前持仓
        /// </summary>
        public Position[] Positions
        {
            get
            {
                return ClearCentre.Positions;
            }
        }

        public Order[] Ordres { get { return ClearCentre.Ordres; } }//获得当日所有委托
        public Trade[] Trades { get { return ClearCentre.Trades; } }//获得当日所有成交
        public long[] Cancels { get { return ClearCentre.Cancels; } }//获得当日所有取消
        public Position[] PositionsHold { get { return ClearCentre.PositionsHold; } }
        public Position getPosition(string symbol)//获得某个symbol的持仓信息
        {
            return ClearCentre.getPosition(symbol);
        }


        #endregion


        #region 账户财务参数

        private DateTime _settletime = new DateTime(1977, 1, 1, 1, 1, 1);
        /// <summary>
        /// 上次结算日
        /// </summary>
        public DateTime SettleDateTime
        {
            get { return _settletime; }
            set { _settletime = value; }
        }


        //昨日权益 从数据库读取
        
        
        //当前动态权益 动态计算
        //账户当前权益 = 上期权益+账户内仓位未平仓利润 + 平仓利润 +手续费


       
        //当前有效配资额度
        //public decimal FinAmmountAvabile { get { return _cc.FinAmmountAvabile; } }
        //设定的总配资额度
        //public decimal FinAmmountTotal { get { return _cc.FinAmmountTotal; } }




        // 购买力,用于获得账户的购买力,从而检查资金使用情况
        //public decimal BuyPower { get { return Cash * this.BuyMultiplier; } }//当前购买力为 当前可用资金*购买乘数

        int _buymultipler = 1;
        /// <summary>
        /// 购买乘数,普通账户购买乘数为1,配资账户购买乘数为配资比例.计算保证金的时候用购买乘数来计算购买力
        /// </summary>
        public int BuyMultiplier { get { return _buymultipler; } set { _buymultipler = value; } }



        //decimal _obverseprofit = 0;
        //bool _obversecaled = false;
        ///// <summary>
        ///// 折算收益,防止多次计算导致负载过高,每天只需要计算一次折算收益，当日交易完毕后再进行计算
        ///// </summary>
        //public decimal ObverseProfit
        //{
        //    get
        //    {
        //        if (!_obversecaled)//当账户没有计算过折算权益,则我们计算一次折算权益。如果计算过则直接返回
        //        {
        //            _obverseprofit = _cc.ObverseProfit;
        //            _obversecaled = true;
        //        }
        //        return _obverseprofit;
        //    }
        //}

        //decimal _startEquity = -1;
        ///// <summary>
        ///// 该数据由比赛中心进行委托,将初始权益绑定给账户
        ///// </summary>
        //public decimal StartEquity { get { return _startEquity; } set { _startEquity = value; } }



        #endregion






        //#region 配资服务部分

        ////IFinService _fs = null;
        ////public IFinService FinService { get { return _fs; } set { _fs = value; } }

        //#endregion

        //#region 居间代理字段

        //string _agentcode = "";
        //public string AgentCode { get { return _agentcode; }  set { _agentcode = value; } }

        //string _agentsubtoken = "";
        //public string AgentSubToken { get { return _agentsubtoken; } set { _agentsubtoken = value; } }
        //#endregion






        /// <summary>
        /// 重置账户状态,用于每日造成开盘时,重置数据 
        /// </summary>
        public void Reset()
        {
            //this.Execute = true;//将被冻结的交易账户打开交易权限 当报名后 清算中心冻结账户 在报名流程中还会reset账户,用于重新计算折算权益 这里不能打开执行
            //重置折算收益计算标识
            //_obversecaled = false;//用于次日重新计算折算权益

            //清空账户附加的规则 用于重新加载帐户规则
            ClearAccountCheck();
            ClearOrderCheck();
            _rulitemloaded = false;
        }


        public string DisplayString
        {
            get
            {
                string re = "ID:" + this.ID + " 昨日权益:" + this.LastEquity.ToString() + " 当前权益:" + this.NowEquity.ToString() + " 总委托:" + this.Ordres.Length.ToString() + " 总成交:" + this.Trades.Length.ToString() + " 比赛状态:" + this.RaceStatus.ToString();
                return re;

            }
        }

        public string DisplayString2
        {
            get
            {
                string re = "FM:" + FutMarginUsed.ToString() + " FMF:" + FutMarginFrozen.ToString() + " FR:" + FutRealizedPL.ToString() + " FU:" + FutUnRealizedPL.ToString() + " FC:" + FutCommission.ToString()  + Environment.NewLine+ " OCost:" + OptPositionCost.ToString() + " OV:" + OptPositionValue.ToString() + " OR:" + OptRealizedPL.ToString() + " OC:" + OptCommission.ToString() + Environment.NewLine;
                string re1 = "FCash:" + FutCash.ToString() + " FL:" + FutLiquidation.ToString() + " FMU:" + FutMoneyUsed.ToString() + Environment.NewLine;
                string re2 = "OCash:" + OptCash.ToString() + " OMV:" + OptMarketValue.ToString() + " OL:" + OptLiquidation.ToString() + " OMU:" + OptMoneyUsed.ToString() + Environment.NewLine;
                string re3 = "TL:" + TotalLiquidation.ToString() + " AF:" + AvabileFunds.ToString()+Environment.NewLine;

                return re + re1 + re2 + re3;
            }
            
        }



        public static string Series(IAccount a)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(a.ID);//交易帐户编号
            sb.Append(d);
            sb.Append(a.OrderRouteType.ToString());//路由类别
            sb.Append(d);
            sb.Append(a.LastEquity.ToString());//上期权益
            sb.Append(d);
            sb.Append(a.SettleDateTime.ToString());//结算时间
            sb.Append(d);
            sb.Append(a.Execute.ToString());//是否允许交易
            sb.Append(d);
            sb.Append(a.CashIn.ToString());//入金
            sb.Append(d);
            sb.Append(a.CashOut.ToString());//出金
            sb.Append(d);
            sb.Append(a.CreatedTime.ToString());//账户建立时间;
            sb.Append(d);
            sb.Append("");//账户当前raceID;
            sb.Append(d);
            sb.Append("");//账户当前RaceStatus;
            sb.Append(d);
            sb.Append("");//加入比赛时间
            sb.Append(d);
            sb.Append(a.IntraDay.ToString());//是否是日内
            sb.Append(d);
            sb.Append(a.Category.ToString());//类别 交易员 配资客户
            //sb.Append(d);
            //sb.Append(a.AgentCode.ToString());//代理编码
            //sb.Append(d);
            //sb.Append(a.AgentSubToken.ToString());//代理介绍人
            return sb.ToString();
        }
        public static IAccount Deseries(string msg)
        {
            string[] p = msg.Split(',');
            string acc = p[0];
            QSEnumOrderTransferType ortype = (QSEnumOrderTransferType)Enum.Parse(typeof(QSEnumOrderTransferType), p[1]);
            decimal lasequity = decimal.Parse(p[2]);
            DateTime settletime = DateTime.Parse(p[3]);
            bool execute = bool.Parse(p[4]);
            decimal cashin = decimal.Parse(p[5]);
            decimal cashout = decimal.Parse(p[6]);
            DateTime createdtime = Convert.ToDateTime(p[7]);
            string raceid = Convert.ToString(p[8]);
            QSEnumAccountRaceStatus racestatus = (QSEnumAccountRaceStatus)Enum.Parse(typeof(QSEnumAccountRaceStatus), p[9]);
            DateTime raceentrytime = Convert.ToDateTime(p[10]);
            bool intraday = Convert.ToBoolean(p[11]);
            QSEnumAccountCategory ca = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), p[12]);

            string agentcode = p[13];
            string agentsubtoken = p[14];
            
            AccountBase a = new AccountBase(acc);
            a.OrderRouteType = ortype;
            a.LastEquity = lasequity;
            a.SettleDateTime = settletime;
            a.Execute = execute;
            a.CashIn = cashin;
            a.CashOut = cashout;
            a.CreatedTime = createdtime;

            a.RaceID = raceid;
            a.RaceStatus = racestatus;
            a.RaceEntryTime = raceentrytime;

            a.IntraDay = intraday;
            a.Category = ca;
            //a.AgentCode = agentcode;
            //a.AgentSubToken = agentsubtoken;
            return a;

        }

        #region AccountBase静态函数 用于生成Account对应的相关数据或者统计

        /// <summary>
        /// 为交易帐号生成结算数据
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Settlement GenSettle(IAccount account)
        {
            if (account.ID.Equals("9580001"))
            {
                int x = 0;
            }
            Settlement settle = new SettlementImpl();
            settle.Account = account.ID;
            settle.CashIn = account.CashIn;
            settle.CashOut = account.CashOut;
            settle.Commission = account.Commission;
            settle.Confirmed = false;
            settle.LastEqutiy = account.LastEquity;
            settle.RealizedPL = account.RealizedPL;
            settle.UnRealizedPL = account.SettleUnRealizedPL;
            settle.NowEquity = settle.LastEqutiy + settle.RealizedPL + settle.UnRealizedPL - settle.Commission + settle.CashIn - settle.CashOut;
            
            //指定交易日期
            settle.SettleDay = Util.ToTLDate();
            settle.SettleTime = Util.ToTLTime();
            return settle;
        }
        #endregion
    }

    

}
