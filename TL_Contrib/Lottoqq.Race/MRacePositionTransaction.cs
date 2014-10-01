using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Lottoqq.Race;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MRacePositionTransaction:MBase
    {
        /// <summary>
        /// 插入持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        public static int InsertPositionTransaction(IPositionRound pr)
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "INSERT INTO lottoqq_race_postransactions (account,symbol,security,type,multiple,entrytime,entrysize,entryprice,entrycommission,exittime,exitsize,exitprice,exitcommission,highest,lowest,size,hold,side,wl,totalpoints,profit,commission,netprofit) values (@account,@symbol,@security,@type,@multiple,@entrytime,@entrysize,@entryprice,@entrycommission,@exittime,@exitsize,@exitprice,@exitcommission,@highest,@lowest,@size,@hold,@side,@wl,@totalpoints,@profit,@commission,@netprofit)";
                int row = db.Connection.Execute(query, new { account = pr.Account, symbol = pr.Symbol, security = pr.Security, type = pr.Type, multiple = pr.Multiple, entrytime = pr.EntryTime, entrysize = pr.EntrySize, entryprice = pr.EntryPrice, entrycommission = pr.EntryCommission, exittime = pr.ExitTime, exitsize = pr.ExitSize, exitprice = pr.ExitPrice, exitcommission = pr.ExitCommission, highest = pr.Highest, lowest = pr.Lowest, size = pr.Size, hold = pr.HoldSize, side = pr.Side, wl = pr.WL, totalpoints = pr.TotalPoints, profit = pr.Profit, commission = pr.Commissoin, netprofit = pr.NetProfit });
                return row;
            }
        }

        /// <summary>
        /// 清空某个交易帐号或者所有交易帐号的比赛交易回合记录
        /// </summary>
        /// <param name="account"></param>
        public static void ClearPositionRound(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string basequery = "DELETE FROM lottoqq_race_postransactions";
                string query = string.IsNullOrEmpty(account) ? basequery : (basequery + " WHERE account=" + account);
                int row = db.Connection.Execute(query, null);
            }
        }


    }
}
