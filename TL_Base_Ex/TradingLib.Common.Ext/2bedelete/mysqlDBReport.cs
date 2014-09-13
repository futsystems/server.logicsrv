//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using System.Data;
//using MySql.Data.MySqlClient;
//namespace TradingLib.MySql
//{
    
//    /// <summary>
//    /// 数据库连接 用于获取交易报告
//    /// </summary>
//     public class mysqlDBReport:mysqlDBBase
//    {


//         //获得账户的每日交易汇总
//         public DataSet ReTotalDaily(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("SELECT account,settleday,realizedpl,unrealizedpl,commission,realizedpl+unrealizedpl-commission as netprofit FROM settlement where account='{0}' and settleday>='{1}' and settleday<='{2}';",account,start.ToString(),end.ToString());
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "settlement");
//             return retSet;
//         }

//         public DataSet ReSide_WL(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("select `viewprofit`.`account` AS `account`,`viewprofit`.`side` AS `side`,`viewprofit`.`wl` AS `wl`,sum(`viewprofit`.`profit`) AS `profit`,sum(`viewprofit`.`commission`) AS `commission`,sum(`viewprofit`.`netprofit`) AS `netprofit` from (select `v`.`account` AS `account`,`v`.`symbol` AS `symbol`,`v`.`security` AS `security`,`v`.`side` AS `side`,(case when ((((`v`.`size` * `v`.`points`) * `v`.`multiple`) - `v`.`commission`) > 0) then 1 else -(1) end) AS `wl`,`v`.`size` AS `size`,`v`.`entrytime` AS `entrytime`,`v`.`entryprice` AS `entryprice`,`v`.`exittime` AS `exittime`,`v`.`exitprice` AS `exitprice`,`v`.`points` AS `points`,(`v`.`size` * `v`.`points`) AS `totalpoints`,((`v`.`size` * `v`.`points`) * `v`.`multiple`) AS `profit`,(((`v`.`size` * `v`.`points`) * `v`.`multiple`) - `v`.`commission`) AS `netprofit`,`v`.`commission` AS `commission` from (select `postransactions`.`account` AS `account`,`postransactions`.`symbol` AS `symbol`,`postransactions`.`security` AS `security`,`postransactions`.`multiple` AS `multiple`,sign(`postransactions`.`entrysize`) AS `side`,abs(`postransactions`.`entrysize`) AS `size`,`postransactions`.`entrytime` AS `entrytime`,`postransactions`.`entryprice` AS `entryprice`,`postransactions`.`exittime` AS `exittime`,`postransactions`.`exitprice` AS `exitprice`,((sign(`postransactions`.`entrysize`) * -(1)) * (`postransactions`.`entryprice` - `postransactions`.`exitprice`)) AS `points`,(abs(`postransactions`.`entrycommission`) + abs(`postransactions`.`exitcommission`)) AS `commission` from `postransactions` where (`postransactions`.`hold` = 0 and `postransactions`.`account`='{0}' and `postransactions`.`entrytime`>='{1}' and `postransactions`.`entrytime`<='{2}' ) ) AS `v`) as `viewprofit` group by `viewprofit`.`account`,`viewprofit`.`side`,`viewprofit`.`wl`;", account, start.ToString(), end.ToString());

//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "viewprofit");
//             return retSet;
//         }

//         /// <summary>
//         /// 保存每日的运营数据
//         /// </summary>
//         /// <param name="day"></param>
//         /// <param name="finfee"></param>
//         /// <param name="commission_in"></param>
//         /// <param name="commission_out"></param>
//         /// <param name="deposit"></param>
//         /// <param name="withdraw"></param>
//         /// <returns></returns>
//         public bool SaveOPReport(DateTime day,decimal finfee,decimal commission_in ,decimal commission_out,decimal deposit,decimal withdraw,decimal finammount,decimal finammountinterest,decimal finammountbonus,decimal marginused)
//         {
//             this.SqlReady();
//             //if (IsOPReportSaved(day))
//             DeleteOPReport(day);
//             string sql = String.Format("Insert into operationreport (`day`,`finfee`,`commission_in`,`commission_out`,`deposit`,`withdraw`,`finammount_total`,`finammount_interest`,`finammount_bonus`,`marginused`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", day.ToString(), finfee.ToString(), commission_in.ToString(), commission_out.ToString(), deposit.ToString(), withdraw.ToString(),finammount.ToString(),finammountinterest.ToString(),finammountbonus.ToString(),marginused.ToString());
//             cmd.CommandText = sql;
//             return (cmd.ExecuteNonQuery() > 0);
//         }

//         /// <summary>
//         /// 记录某个账户的登入登出记录
//         /// </summary>
//         public bool SaveSession(string account, string ipaddress, string hardware, string frontid, string login_flag,string apit_ype,string api_version)
//         {
//             this.SqlReady();
//             string sql = String.Format("Insert into risk_session (`account`,`datetime`,`ipaddress`,`hardware`,`frontid`,`login_flag`,`API_type`,`API_version`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",account,DateTime.Now.ToString(),ipaddress,hardware,frontid,login_flag,apit_ype,api_version);
//             cmd.CommandText = sql;
//             return (cmd.ExecuteNonQuery() > 0);
//         }
//         /// <summary>
//         /// 删除当日数据,保证数据唯一性
//         /// </summary>
//         /// <param name="day"></param>
//         /// <returns></returns>
//         public bool DeleteOPReport(DateTime day)
//         {
//             this.SqlReady();
//             string sql = String.Format("delete from operationreport where `day` ='{0}'", day.ToString("yyyy-MM-dd"));
//             cmd.CommandText = sql;
//             return (cmd.ExecuteNonQuery() > 0);
//         }
//        /*
//        public bool IsOPReportSaved(DateTime day)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT * FROM operationreport where `day` ='{0}'", day.ToString("yyyy-MM-dd"));
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                if (myReader.GetString("day") == day.ToString("yyyy-MM-dd"))
//                    return true;
//                else
//                    return false;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//            finally
//            {
//                myReader.Close();
//            }
//        }**/

//        #region 交易记录查询
//         const string ACCOUNT="账户";
//         const string DATE="日期";
//         const string REALIZEDPL="平仓盈亏";
//         const string UNREALIZEDPL="持仓盈亏";
//         const string COMMISSION="手续费";
//         const string DEPOSIT="入金";
//         const string WITHDRAW="出尽";
//         const string LASTEQUITY ="昨日权益";
//         const string SETTLEEQUITY="结算权益";
//         public DataTable GetSettelment(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("select account,settleday,FORMAT(realizedpl,2),FORMAT(unrealizedpl,2),FORMAT(commission,2),FORMAT(cashin,2),FORMAT(cashout,2),FORMAT(lastequity,2),FORMAT(nowequity,2) from settlement where account='{0}' and settleday >='{1}' and settleday <='{2}'", account, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "settlement");
//             DataTable tb = retSet.Tables["settlement"];
//             tb.Columns[0].ColumnName = ACCOUNT;
//             tb.Columns[1].ColumnName = DATE;
//             tb.Columns[2].ColumnName = REALIZEDPL;
//             tb.Columns[3].ColumnName = UNREALIZEDPL;
//             tb.Columns[4].ColumnName = COMMISSION;
//             tb.Columns[5].ColumnName = DEPOSIT;
//             tb.Columns[6].ColumnName = WITHDRAW;
//             tb.Columns[7].ColumnName = LASTEQUITY;
//             tb.Columns[8].ColumnName = SETTLEEQUITY;
//             return tb;
//         }
//         const string ORDERID = "编号";
//         const string SIDE = "买/卖";
//         const string SYMBOL = "合约";
//         const string SIZE = "数量";
//         const string PRICE = "限价";
//         const string STOP = "追价";
//         const string TIME = "时间";
//         const string TIF = "TIF";
//         const string BROKER = "Broker";
//         const string BROKERKEY = "Brokerkey";
//         const string STATUS = "状态";


//         public DataTable GetOrders(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("select ordid,date,time,side,symbol,size,price,stop,tif,broker,brokerkey,status from log_orders where account='{0}' and date >='{1}' and date <='{2}'", account, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "settlement");
//             DataTable tb = retSet.Tables["settlement"];
//             tb.Columns[0].ColumnName = ORDERID;
//             tb.Columns[1].ColumnName = DATE;
//             tb.Columns[2].ColumnName = TIME;
//             tb.Columns[3].ColumnName = SIDE;
//             tb.Columns[4].ColumnName = SYMBOL;
//             tb.Columns[5].ColumnName = SIZE;
//             tb.Columns[6].ColumnName = PRICE;
//             tb.Columns[7].ColumnName = STOP;
//             tb.Columns[8].ColumnName = TIF;
//             tb.Columns[9].ColumnName = BROKER;
//             tb.Columns[10].ColumnName = BROKERKEY;
//             tb.Columns[11].ColumnName = STATUS;
//             return tb;
//         }
//         const string XSIZE = "数量";
//         const string XSIDE = "方向";
//         const string XPRICE = "价格";
//         const string POSOPERATION = "操作";

//         public DataTable GetTrades(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("select ordid,xdate,xtime,xsize,symbol,xsize,xprice,broker,brokerkey,posoperation from log_trades where account='{0}' and xdate >='{1}' and xdate <='{2}'", account, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "settlement");
//             DataTable tb = retSet.Tables["settlement"];
//             tb.Columns[0].ColumnName = ORDERID;
//             tb.Columns[1].ColumnName = DATE;
//             tb.Columns[2].ColumnName = TIME;
//             tb.Columns[3].ColumnName = XSIDE;
//             tb.Columns[4].ColumnName = SYMBOL;
//             tb.Columns[5].ColumnName = XSIZE;
//             tb.Columns[6].ColumnName = XPRICE;
//             tb.Columns[7].ColumnName = BROKER;
//             tb.Columns[8].ColumnName = BROKERKEY;
//             tb.Columns[9].ColumnName = POSOPERATION;

//             return tb;
//         }

//         const string DATETIME = "时间";
//         const string AMMOUNT = "金额";
//         const string CASHSIDE = "操作";
//         const string COMMENT = "标注";
//         public DataTable GetCash(string account, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("select account,datetime,ammount,ammount,comment from transactions where account='{0}' and datetime >='{1}' and datetime <='{2}'", account, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "cash");
//             DataTable tb = retSet.Tables["cash"];
//             tb.Columns[0].ColumnName = ACCOUNT;
//             tb.Columns[1].ColumnName = DATETIME;
//             tb.Columns[2].ColumnName = CASHSIDE;
//             tb.Columns[3].ColumnName = AMMOUNT;
//             tb.Columns[4].ColumnName = COMMENT;
//             return tb;           
//         }

//        #endregion

//        #region 固定保证金配资报表
//         const string AGENTCODE = "代理编号";
//         //const string ACCOUNT = "交易帐号";
//         const string AGENTTOKEN = "介绍人标识";
//         //const string SIZE = "数量";
//         //const string SYMBOL = "合约";
//         const string ENTRYTIME = "开仓时间";
//         const string ENTRYPRICE = "开仓价格";
//         const string EXITTIME = "平仓时间";
//         const string EXITPRICE = "平仓价格";

//         const string TOTALPOINTS = "盈亏点数";
//         const string PROFIT = "盈亏";
//         const string ACCOUNTFEE = "客户收费";
//         //const string COMMISSION = "交易手续费";
//         const string SERVICEFEE = "平台服务费";
//         const string AGENTFEE = "代理成本";
//         const string PLEDGE = "代理押金";
//         const string AGENTREVENUE = "代理当日收入";

//         public DataTable GetAgentPRDetails(string agentcode, DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("SELECT account,agent_token,size,symbol,entrytime,entryprice,exittime,exitprice,totalpoints,profit,account_fee,commission,service_fee,agent_fee,agent_pledge,agent_revenue FROM finservices_pr_fj where agentcode='{0}' and entrytime >='{1}' and entrytime <='{2}'", agentcode, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "agentprdetails");
//             DataTable tb = retSet.Tables["agentprdetails"];
//             tb.Columns[0].ColumnName = ACCOUNT;
//             tb.Columns[1].ColumnName = AGENTTOKEN;
//             tb.Columns[2].ColumnName = SIZE;
//             tb.Columns[3].ColumnName = SYMBOL;
//             tb.Columns[4].ColumnName = ENTRYTIME;
//             tb.Columns[5].ColumnName = ENTRYPRICE;
//             tb.Columns[6].ColumnName = EXITTIME;
//             tb.Columns[7].ColumnName = EXITPRICE;
//             tb.Columns[8].ColumnName = TOTALPOINTS;
//             tb.Columns[9].ColumnName = PROFIT;
//             tb.Columns[10].ColumnName = ACCOUNTFEE;
//             tb.Columns[11].ColumnName = COMMISSION;
//             tb.Columns[12].ColumnName = SERVICEFEE;
//             tb.Columns[13].ColumnName = AGENTFEE;
//             tb.Columns[14].ColumnName = PLEDGE;
//             tb.Columns[15].ColumnName = AGENTREVENUE;
//             return tb;
//         }

//         public DataTable GetAgentPRSummary(DateTime start, DateTime end)
//         {
//             this.SqlReady();
//             string sql = String.Format("SELECT agentcode,sum(size) as totalsize,symbol,sum(totalpoints) as totalpoints,sum(profit) as totalprofit,sum(account_fee) totalaccountfee,sum(commission) as totalcommission,sum(service_fee) as totalservicefee,sum(agent_fee) as toallagentfee,sum(agent_pledge) as totalpledge,sum(agent_revenue) as totalrevenue FROM finservices_pr_fj  where entrytime >='{0}' and entrytime <='{1}' group by agentcode",start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
//             // 创建一个适配器
//             MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//             // 创建DataSet，用于存储数据.
//             DataSet retSet = new DataSet();
//             // 执行查询，并将数据导入DataSet.
//             adapter.Fill(retSet, "agentprsummary");
//             DataTable tb = retSet.Tables["agentprsummary"];
//             tb.Columns[0].ColumnName = AGENTTOKEN;
//             tb.Columns[1].ColumnName = "累计手数";
//             tb.Columns[2].ColumnName = SYMBOL;
//             tb.Columns[3].ColumnName = "盈亏点数";
//             tb.Columns[4].ColumnName = "累计盈亏";
//             tb.Columns[5].ColumnName = "累计收费";
//             tb.Columns[6].ColumnName = "累计手续费";
//             tb.Columns[7].ColumnName = "累计服务费";
//             tb.Columns[8].ColumnName = "代理费用合计";
//             tb.Columns[9].ColumnName = "累计押金";
//             tb.Columns[10].ColumnName = "代理累计收入";
//             return tb;
//         }
//        #endregion
//    }
//}
