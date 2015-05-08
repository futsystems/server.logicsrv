using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.Race
{
    /// <summary>
    /// 交易帐户比赛服务
    /// </summary>
    public class RaceService
    {
        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 交易帐户对象
        /// </summary>
        public IAccount oAccount { get; set; }

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

        /// <summary>
        /// 绑定交易帐户对象
        /// </summary>
        public void InitRaceService()
        {
            this.oAccount = TLCtxHelper.CmdAccount[this.Account];
        }
    }
}
