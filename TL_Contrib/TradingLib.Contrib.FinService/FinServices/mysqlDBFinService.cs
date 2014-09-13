using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;

namespace TradingLib.Contrib
{
    public class mysqlDBFinService : mysqlDBBase
    {

        /// <summary>
        /// 检查某个账户是否有对应的服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool HaveAccountFinService(string account)
        {

            this.SqlReady();
            string sql = String.Format("select * from finservices where `account` = '{0}'", account);
            // 创建一个适配器
            cmd.CommandText = sql;
            MySqlDataReader myReader = cmd.ExecuteReader();
            try
            {
                myReader.Read();
                if (myReader.GetString("account") == account) return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                myReader.Close();

            }

        }
        /// <summary>
        /// 插入融资服务到数据库
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ammount"></param>
        /// <param name="type"></param>
        /// <param name="discount"></param>
        /// <param name="agent"></param>
        /// <param name="active"></param>
        /// <param name="finserviceid"></param>
        /// <returns></returns>
        public bool InsertFinService(string account, decimal ammount, QSEnumFinServiceType type, decimal discount, string agent, bool active)
        {
            this.SqlReady();
            string sql = String.Format("Insert into `finservices` (`account`,`ammount`,`type`,`discount`,`agent`,`active`,`createtime`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", account, ammount.ToString(), type.ToString(), discount.ToString(), agent, active.ToString(), DateTime.Now.ToString());
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }
        /// <summary>
        /// 插入某个服务当天的费用记录
        /// 当天结算后，然后进行结算服务，将每个服务的费用结算后记录到数据库，然后从账户权益中扣除
        /// </summary>
        /// <param name="fingid"></param>
        /// <param name="account"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        public bool InsertFinFee(string account, decimal fee)
        {
            this.SqlReady();
            string sql = String.Format("Insert into `finservices_fee` (`date`,`fee`,`account`) values('{0}','{1}','{2}')", DateTime.Now.ToString(), fee.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }


        /// <summary>
        /// 更新某个融资服务的状态
        /// </summary>
        /// <param name="fingid"></param>
        /// <returns></returns>
        public bool UpdateFinServiceActive(string account, bool active)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE finservices SET active = '{0}'  WHERE account = '{1}'", active.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 更新某个融资服务的额度
        /// </summary>
        /// <param name="fingid"></param>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public bool UpdateFinServiceAmmount(string account, decimal ammount)
        {

            this.SqlReady();
            string sql = String.Format("UPDATE finservices SET ammount = '{0}'  WHERE account = '{1}'", ammount.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 更新配资服务的代理编码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="agentcode"></param>
        /// <returns></returns>
        public bool UpdateFinServiceAgent(string account, int agentcode)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE finservices SET agent = '{0}'  WHERE account = '{1}'", agentcode.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }
        /// <summary>
        /// 更新某个融资服务的计费类别
        /// </summary>
        /// <param name="fingid"></param>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public bool UpdateFinServiceType(string account, QSEnumFinServiceType type)
        {

            this.SqlReady();
            string sql = String.Format("UPDATE finservices SET type = '{0}'  WHERE account = '{1}'", type.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 更新配资服务的费用折扣
        /// </summary>
        /// <param name="fingid"></param>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public bool UpdateFinServiceDiscount(string account, decimal discount)
        {
            this.SqlReady();
            string sql = String.Format("UPDATE finservices SET discount = '{0}'  WHERE account = '{1}'", discount.ToString(), account);
            cmd.CommandText = sql;
            return (cmd.ExecuteNonQuery() > 0);
        }

        /// <summary>
        /// 检查某日是否已经扣费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool IsFeeCharged(string account, DateTime day)
        {
            this.SqlReady();
            string sql = String.Format("SELECT * FROM finservices_fee where date_format(`finservices_fee`.`date`,'%Y-%m-%d')='{0}' and account = '{1}'", day.ToString("yyyy-MM-dd"), account);
            cmd.CommandText = sql;
            MySqlDataReader myReader = cmd.ExecuteReader();
            try
            {
                myReader.Read();
                if (myReader.GetString("account") == account)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                myReader.Close();
            }
        }

        /// <summary>
        /// 获得所有赛季列表
        /// </summary>
        /// <returns></returns>
        public DataSet getFinServices(string account)
        {
            this.SqlReady();
            string sql = String.Format("select * from finservices where account = '{0}'", account);
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "finservices");
            return retSet;

        }

        /// <summary>
        /// 获得所有赛季列表
        /// </summary>
        /// <returns></returns>
        public DataSet getFinServices()
        {
            this.SqlReady();
            string sql = String.Format("select * from finservices");
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "finservices");
            return retSet;

        }

        public bool InsertPositionTransaction(PositionRoundForClear prc)
        {
            this.SqlReady();
            string sql = String.Format("Insert into finservices_pr_fj (`account`,`symbol`,`security`,`entrytime`,`entrysize`,`entryprice`,`exittime`,`exitsize`,`exitprice`,`size`,`totalpoints`,`multiple`,`points`,`profit`,`account_fee`,`agentcode`,`agent_fee`,`agent_pledge`,`agent_token`,`agent_revenue`,`commission`,`service_fee`,`wl`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')", prc.Account, prc.Symbol, prc.Security, prc.EntryTime, prc.EntrySize, prc.EntryPrice, prc.ExitTime, prc.ExitSize, prc.ExitPrice, prc.Size, prc.TotalPoints, prc.Multiple, prc.Points, prc.Profit, prc.AccountFee, prc.AgentCode, prc.AgentFee, prc.AgentPledge, prc.AgentSubToken, prc.AgentRevenue, prc.Commission, prc.ServiceFee,prc.WL.ToString());
            cmd.CommandText = sql;
            return ((cmd.ExecuteNonQuery() > 0));
        }

    }

}
