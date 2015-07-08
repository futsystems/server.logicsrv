﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.Race
{
    public class RaceServiceSetting
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Acct { get; set; }

        /// <summary>
        /// 比赛编号
        /// </summary>
        public string RaceID { get; set; }

        /// <summary>
        /// 参赛时间
        /// </summary>
        public long EntryTime { get; set; }

        /// <summary>
        /// 参赛结算日 在哪个结算日参赛
        /// </summary>
        public int EntrySettleday { get; set; }

        /// <summary>
        /// 比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus RaceStatus { get; set; }

        /// <summary>
        /// 考核时间
        /// </summary>
        public long ExamineTime { get; set; }


        /// <summary>
        /// 考核权益
        /// 初赛或者复赛会进行盈利折算
        /// </summary>
        public decimal ExamineEquity { get; set; }


        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsAvabile { get; set; }


    }


    /// <summary>
    /// 交易帐户比赛服务
    /// </summary>
    public class RaceService : RaceServiceSetting,IAccountService
    {
        /// <summary>
        /// 交易帐户对象
        /// </summary>
        public IAccount Account { get; set; }

        /// <summary>
        /// 绑定交易帐户对象
        /// </summary>
        public void InitRaceService()
        {
            
            this.Account = TLCtxHelper.CmdAccount[this.Acct];
            if (this.Account != null)
            {
                this.Account.BindService(this);
            }
        }

        public string SN { get { return "RaceService"; } }

        public decimal GetFundAvabile(Symbol symbol)
        {
            return 0;
        }

        public int CanOpenSize(Symbol symbol, bool side, QSEnumOffsetFlag flag)
        {
            return 0;
        }

        public bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            return true;
        }

        public bool CanTakeOrder(Order o, out string msg)
        {
            if (!this.IsAvabile)
            {
                msg = "比赛服务未激活,报名后需等待一个工作日";
                return false;
            }
            if (o.IsEntryPosition)
            {
                if (Account.Commission >= 4000)
                {
                    msg = "手续费超过限制:4000";
                    return false;
                }
            }
            msg = string.Empty;
            return true;
        }

        public CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            return null;
        }


        /// <summary>
        /// 激活服务
        /// </summary>
        public void Active()
        {
            this.IsAvabile = true;
            ORM.MRace.UpdateRaceServiceActive(this);
        }

        /// <summary>
        /// 冻结服务
        /// </summary>
        public void InActive()
        {
            this.IsAvabile = false;
            ORM.MRace.UpdateRaceServiceActive(this);
        }


        public IEnumerable<string> GetNotice()
        {
            List<string> notice = new List<string>();
            notice.Add(string.Format("比赛编号:{0}", this.RaceID) + System.Environment.NewLine);
            notice.Add(string.Format("比赛状态:{0}", Util.GetEnumDescription(this.RaceStatus)) + System.Environment.NewLine);
            notice.Add(string.Format("考核权益:{0}", Util.FormatDecimal(this.ExamineEquity)) + System.Environment.NewLine);
            notice.Add(string.Format("比赛激活:{0}", this.IsAvabile ? "激活" : "冻结") + System.Environment.NewLine);

            return notice;
        }
    }
}