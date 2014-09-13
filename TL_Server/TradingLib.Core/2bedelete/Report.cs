using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;


using System.Data;

namespace TradingLib.Contrib
{
    /*
    /// <summary>
    /// 用于获得交易账户的统计数据,管理端查询交易客户端的交易统计则调用该组件生成对应的数据回传给风控管理端
    /// 同时封装 客户端profile信息,本地服务器health信息,以及其他模块的report
    /// </summary>
    public class Report
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        ConnectionPoll<mysqlDBReport> conn;
        public Report(string _server,string _user,string _pass)
        {
            conn = new ConnectionPoll<mysqlDBReport>(_server, _user, _pass,CoreGlobal.DBName,CoreGlobal.DBPort);
        }

        public DailySummaryList  GenDailySummaryList(string account,DateTime start)
        {
            return GenDailySummaryList(account,start,DateTime.Now);
        }
        public DailySummaryList  GenDailySummaryList(string account, DateTime start, DateTime end)
        {
            DailySummaryList dl= new DailySummaryList();
            mysqlDBReport db = conn.mysqlDB;
            DataSet ds = db.ReTotalDaily(account,start,end);
            conn.Return(db);

            DataTable dt = ds.Tables["settlement"];
            List<long> clist = new List<long>();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DailySummary d = new DailySummary();
                DataRow dr = dt.Rows[i];
                d.Account = Convert.ToString(dr["account"]);
                d.DateTime = Convert.ToDateTime(dr["settleday"]);
                d.RealizedPL = Convert.ToDecimal(dr["realizedpl"]);
                d.UnRealizedPL = Convert.ToDecimal(dr["unrealizedpl"]);
                d.Commission = Convert.ToDecimal(dr["commission"]);

                dl.Add(d);
            }
            return dl;
        }

        public WLSideDIS GenWLSideDIS(string account, DateTime start, DateTime end)
        {
            WLSideDIS temp = new WLSideDIS();
            mysqlDBReport db = conn.mysqlDB;
            DataSet ds = db.ReSide_WL(account, start, end);
            conn.Return(db);

            DataTable dt = ds.Tables["viewprofit"];
            for (int i = 0; i < dt.Rows.Count; i++)
            { 
                DataRow dr = dt.Rows[i];
                int side = Convert.ToInt16(dr["side"]);
                int wl = Convert.ToInt16(dr["wl"]);
                decimal profit = Convert.ToDecimal(dr["profit"]);
                decimal commission = Convert.ToDecimal(dr["commission"]);
                decimal netprofit = Convert.ToDecimal(dr["netprofit"]);
            }

            return temp;
        }

        
    }
     * **/
}
