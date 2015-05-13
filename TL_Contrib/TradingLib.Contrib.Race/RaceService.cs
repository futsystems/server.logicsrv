using System;
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
        /// 比赛状态
        /// </summary>
        public QSEnumAccountRaceStatus RaceStatus { get; set; }
    }


    /// <summary>
    /// 交易帐户比赛服务
    /// </summary>
    public class RaceService : RaceServiceSetting,IAccountService
    {
        public bool IsAvabile { get; set; }


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
            msg = string.Empty;
            return true;
        }

        public CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            return null;
        }



    }
}
