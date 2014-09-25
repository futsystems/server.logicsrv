using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace Lottoqq.Race
{
    /// <summary>
    /// 比赛状态
    /// </summary>
    public enum QSEnumRaceStatus
    { 
        //未参加比赛
        NORACE,
        //在比赛中
        INRACEE,
        //冻结
        BLOCK,
    }

    public class RaceService:IAccountService
    {

        public string SN { get { return "RaceService"; } }
        /// <summary>
        /// 数据库全局编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 比赛服务所绑定的帐号
        /// Account是在后期进行绑定
        /// </summary>
        IAccount _account = null;
        public IAccount Account { get { return _account; } set { _account = value; } }


        /// <summary>
        /// 比赛状态
        /// </summary>
        public QSEnumRaceStatus Status { get; set; }

        /// <summary>
        /// 参赛时间
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime SettleDay { get; set; }


        /// <summary>
        /// 昨日期货权益
        /// </summary>
        public decimal LastFutEquity { get; set; }

        /// <summary>
        /// 昨日期权权益
        /// </summary>
        public decimal LastOptEquity { get; set; }

        /// <summary>
        /// 昨日秘籍权益
        /// </summary>
        public decimal LastMJEquity { get; set; }


        public int CanOpenSize(Symbol symbol)
        {
            return 0;
        }

        public bool CanTradeSymbol(Symbol symbol, out string message)
        {
            message = "";
            return true;
        }

        public bool CanTakeOrder(Order o, out string message)
        {
            message = "";
            return true;
        }

        /// <summary>
        /// 获得某个合约的可用资金
        /// 比赛服务需要按照比赛规则给定具体的可用资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            if (symbol.SecurityType == SecurityType.FUT)
                return FutAvabileFund;//昨日权益 + 当前总流动性 - 资金占用
            if (symbol.SecurityType == SecurityType.OPT)
                return OptAvabileFund;
            if (symbol.SecurityType == SecurityType.INNOV)
                return MJAvabileFund;
            else
                return 0;
        }



        public decimal FutAvabileFund
        {
            get
            {
                return this.LastFutEquity + _account.CalFutLiquidation() - _account.CalFutMoneyUsed();
            }
        }

        public decimal OptAvabileFund
        {
            get
            {
                return this.LastOptEquity + _account.CalOptLiquidation() - _account.CalOptMoneyUsed();
            }
        }
        public decimal MJAvabileFund
        {
            get
            {
                return this.LastMJEquity + _account.CalInnovLiquidation() - _account.CalOptMoneyUsed();
            }
        }

        /// <summary>
        /// 累计参赛天数
        /// </summary>
        public int EntrDays
        {
            get
            {
                return (int)DateTime.Now.Subtract(EntryTime).TotalDays;
            }
        }

        /// <summary>
        /// 当天是否已经结算过
        /// </summary>
        public bool IsSettled
        {
            get
            {
                return LibUtil.isToday(SettleDay);
            }
        }



        public override string ToString()
        {
            return "Account:" + Account.ID + " Status:" + Status.ToString() + " EntryTime:" + EntryTime.ToString() + " SettleDay:" + SettleDay.ToString() + " LastFutEquity:" + LastFutEquity.ToString() + " Ava:" + FutAvabileFund.ToString() + " LastOptEquity:" + LastOptEquity.ToString() + " Ava:" + OptAvabileFund.ToString() + " LastMJEquity:" + LastMJEquity.ToString() + " Ava:" + MJAvabileFund.ToString();
        }


        /// <summary>
        /// 是否在比赛状态中
        /// 如果在比赛状态,则会记录其名下的成交回合
        /// 同时在清算中心中做委托或者保证金检查时也需要检查比赛状态
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (Status == QSEnumRaceStatus.INRACEE)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 当前服务是否可用
        /// </summary>
        public bool IsAvabile
        {
            get
            {
                if (Status == QSEnumRaceStatus.INRACEE)
                    return true;
                return false;
            }
        }

        #region 服务查询和设置
        /// <summary>
        /// 查询服务状态和参数
        /// </summary>
        /// <returns></returns>
        public string QryService()
        {
            return "";
        }

        /// <summary>
        /// 设置服务状态和参数
        /// </summary>
        /// <param name="cfg"></param>
        public void SetService(string cfg)
        {

        }

        #endregion

    }
}
