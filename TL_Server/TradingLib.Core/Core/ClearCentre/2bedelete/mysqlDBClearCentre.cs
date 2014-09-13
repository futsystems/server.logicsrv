//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MySql.Data.MySqlClient;

//using TradingLib.API;
//using System.Data;


//namespace TradingLib.MySql
//{
//    /// <summary>
//    /// 清算中心使用的数据库,由于需要处理并发请求,客户认证,修改密码,修改账户属性等，
//    /// ClearCenter需要实现非阻塞 并发 处理请求
//    /// </summary>
//    public class mysqlDBClearCentre : mysqlDBBase
//    {

//        #region  从数据库恢复交易数据 
//        //从数据库恢复交易数据只有当服务器重新启动时候才予以调用
//        public DataSet getPositionSet()
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from positionhold");//where date >='{0}' and date <='{1}'", start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "positions");
//            return retSet;

//        }

//        public DataSet getPositionRoundSet()
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from postransactionsopened");
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "postransactionsopened");
//            return retSet;

//        }

//        //比较结算日期与当前日期,如果结算日期小于当前日期则返回结算日期为恢复数据日期
//        //如果当天有结算,则恢复当天数据.具体账户的交易信息由读取数据的时候进行比较来分辨时间
//        //将该时间间隔内的交易数据恢复到内存
//        DateTime restoreTime()
//        {
//            this.SqlReady();
//            DateTime settletime = getLastSettleTime();
//            DateTime start = settletime < DateTime.Today ? settletime : DateTime.Today;
//            return start;
//        }
//        //读取数据库操作
//        /// <summary>
//        /// 获得委托集合
//        /// </summary>
//        /// <returns></returns>
//        public DataSet getOrderSet()
//        {
//            return getOrderSet(restoreTime(), DateTime.Today);
//        }
//        public DataSet getOrderSet(DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from orders where date >='{0}' and date <='{1}'", start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "orders");
//            return retSet;
//        }
//        /// <summary>
//        /// 获得成交集合
//        /// </summary>
//        /// <returns></returns>
//        public DataSet getTradesSet()
//        {
//            return getTradeSet(restoreTime(), DateTime.Today);
//        }
//        public DataSet getTradeSet(DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from trades where xdate >='{0}' and xdate <='{1}'", start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "trades");
//            return retSet;
//        }
//        /// <summary>
//        /// 获得取消委托集合
//        /// </summary>
//        /// <returns></returns>
//        public DataSet getCancelSet()
//        {
//            return getCancelSet(restoreTime(), DateTime.Today);
//        }
//        public DataSet getCancelSet(DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from cancles where date >='{0}' and date <='{1}'", start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "cancles");
//            return retSet;
//        }
//        /*
//        //读取数据库操作
//        public DataSet getTradeSet()
//        {
//            string sql = "select * from trades";
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "trades");
//            return retSet;
//        }


//        //查询某个账户 某个段时间 某个symbol的委托记录
//        public DataSet getOrderSet(string account, int starttime, int endtime, string symbol)
//        {
//            string sql = "select * from orders";
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "orders");
//            return retSet;
//        }**/

//        #endregion

//        //每天15:20自动进行系统结算
//        #region 交易账户结算部分
//        //检查某个Account是否存在当天的结算记录,如果没有我们则可以对该交易账户进行结算
//        /// <summary>
//        /// 检查账户当天是否已经结算过了，当天结算过的账户不能再进行结算
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public bool IsAccountSettled(string account)
//        {
//            return IsAccountSettled(account, DateTime.Today);
//        }
//        /// <summary>
//        /// 检查账户是否结算过,搜索结算信息表,如果该日有结算信息,则结算过,没有则没有结算过
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="day"></param>
//        /// <returns></returns>
//        public bool IsAccountSettled(string account, DateTime day)
//        {
//            this.SqlReady();
//            string sql = String.Format("select account from settlement  where `account` = '{0}' and `settleday` = '{1}'", account, day.ToString());
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                if (myReader.GetString("account") == account)
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
//        }
//        /// <summary>
//        /// 结算某个交易账户
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <returns></returns>
//        public bool SettleAccount(IAccount acc)
//        {
//            this.SqlReady();
//            //if (IsAccountSettled(acc.ID)) return true;//如果该账户已经结算过，则直接返回

//            //1.插入某账户的结算信息(当前财务信息)平仓盈亏,持仓盈亏,手续费,入金,出金,昨日权益,当前权益
//            string sql = String.Format("Insert into settlement (`account`,`settleday`,`realizedpl`,`unrealizedpl`,`commission`,`cashin`,`cashout`,`lastequity`,`nowequity`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", acc.ID, DateTime.Today.ToString(), acc.RealizedPL.ToString(), acc.UnRealizedPL.ToString(), acc.Commission.ToString(), acc.CashIn.ToString(), acc.CashOut.ToString(), acc.LastEquity.ToString(), acc.NowEquity.ToString());
//            cmd.CommandText = sql;
//            bool ex = (cmd.ExecuteNonQuery() > 0);

//            //2.将账户持仓插入到持仓列表
//            DelPosition(acc.ID);//插入当前持仓列表前需要删除以前账户的所有持仓信息
//            foreach (Position p in acc.Positions)
//            {
//                if (!p.isFlat)
//                    ex = ex && InsertPosition(p);
//            }
//            //3.更新账户表中的上期权益数据 以及 结算时间
//            //ex = ex && UpdateAccountInfo(acc.ID, acc.NowEquity, DateTime.Now);
            
//            ex = ex && UpdateAccountNowEquity(acc.ID, acc.NowEquity);
//            //4.更新账户结算时间,以后计算就只需要读取该账户这个时间段之后的交易信息并在当前权益基础上进行权益计算。
//            ex = ex && UpdateAccountSettlementTime(acc.ID, DateTime.Now);
//            return ex;
//        }
//        /// <summary>
//        /// 将账户信息回滚到某个交易日
//        /// false代表没有回滚成功
//        /// true代表回滚成功
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="day"></param>
//        /// <returns></returns>
//        public bool RollbackAccount(IAccount acc,DateTime day)
//        {
//            this.SqlReady();
//            //MessageBox.Show("1");
//            //检查该账户在该日有没有结算信息,没有结算信息则返回false
//            if (!IsAccountSettled(acc.ID, day)) return false;
//            //如果有结算信息则 将当前结算信息修改成该天
//            //MessageBox.Show("2");
//            //2.
//            decimal equity = getSettlementEquity(acc.ID, day);//获得该日的权益
//            if (equity > 0)
//            {
//                //1.输出该日期以后的所有结算信息
//                DelAccountSettlement(acc, day);
//                UpdateAccountNowEquity(acc.ID, equity);
//                UpdateAccountSettlementTime(acc.ID, day);
//                return true;
//            }
//            else
//                return false;
//        }

//        /// <summary>
//        /// 将账户信息回滚到某个交易日
//        /// 回滚时要判断账户是否盈利,比如盈利不回滚,亏损回滚
//        /// isproft true 回滚盈利账户 false 回滚亏损账户
//        /// 
//        /// keepsettleday true 保留结算日期，回滚后结算日期为当前日期, false为更新为回滚到那天
//        /// 
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="day"></param>
//        /// <returns></returns>
//        public bool RollbackAccount(IAccount acc, DateTime day,bool isprofit,bool keepsettledeay)
//        {
//            this.SqlReady();
//            //MessageBox.Show("1");
//            //检查该账户在该日有没有结算信息,没有结算信息则返回false
//            if (!IsAccountSettled(acc.ID, day)) return false;
//            //如果有结算信息则 将当前结算信息修改成该天
//            //2.
//            decimal equity = getSettlementEquity(acc.ID, day);//获得该日的权益
//            //MessageBox.Show("以前权益:" + equity.ToString());
//            DateTime now = DateTime.Now;
//            decimal nowequity = getSettlementEquity(acc.ID,new DateTime(now.Year,now.Month,now.Day));//获得当天的权益
//            //MessageBox.Show("当前权益:" + nowequity.ToString());

//            if (equity > 0 && nowequity>0)
//            {
//                //isprofit true 回滚盈利账户
//                if (isprofit )
//                {
//                    if (nowequity > equity)//盈利账户
//                    {
//                        //1.输出该日期以后的所有结算信息
//                        DelAccountSettlement(acc, day);
//                        UpdateAccountNowEquity(acc.ID, equity);
//                        if(!keepsettledeay)
//                            UpdateAccountSettlementTime(acc.ID, day);
//                        return true;
//                    }
//                    return true;
//                }
//                else
//                {
//                    if (nowequity < equity)//亏损账户
//                    {
//                        //MessageBox.Show("比较后发现亏损，删除结算，并更新当前权益:" + equity.ToString());
//                        //if(acc.RaceStatus == QSEnumAccountRaceStatus.ELIMINATE)
                        
//                        //1.输出该日期以后的所有结算信息
//                        DelAccountSettlement(acc, day);
//                        UpdateAccountNowEquity(acc.ID, equity);
//                        if (!keepsettledeay)
//                            UpdateAccountSettlementTime(acc.ID, day);
//                        return true;
//                    }
//                    return true;    
//                }
//            }
//            else
//                return false;
//        }

//        /// <summary>
//        /// 在确认某天有结算信息的情况下获取该结算日的计算金额
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="day"></param>
//        /// <returns></returns>
//        decimal getSettlementEquity(string account, DateTime day)
//        {
//            string sql = String.Format("select nowequity from settlement  where `account` = '{0}' and `settleday` = '{1}'", account, day.ToString());
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDecimal("nowequity");
//            }
//            catch (Exception ex)
//            {
//                return -1;
//            }
//            finally
//            {
//                myReader.Close();
//            }

        
//        }
//        /// <summary>
//        /// 将某日以后的结算信息删除
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="day"></param>
//        /// <returns></returns>
//        public bool DelAccountSettlement(IAccount acc, DateTime day)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from settlement  where account ='{0}' and  settleday > '{1}'",acc.ID,day.ToString());
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
        
//        }


//        //账户结算后 需要更新Account表中的昨日权益值,以及结算日期
//        bool UpdateAccountInfo(string account, decimal lastequity, DateTime datetime)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET lastequity = '{0}' ,settledatetime= '{1}' WHERE account = '{2}'", lastequity.ToString(),datetime.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        /// <summary>
//        /// 更新某个交易账户的结算后权益
//        /// </summary>
//        /// <returns></returns>
//        private bool UpdateAccountNowEquity(string account, decimal lastequity)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET lastequity = '{0}' WHERE account = '{1}'", lastequity.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }



//        /// <summary>
//        /// 获取某个交易账户上次结算时间
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public DateTime getAccountSettleTime(string account)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT  settledatetime FROM accounts where account = '{0}'", account);
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDateTime("settledatetime");
//            }
//            catch (Exception ex)
//            {
//                return new DateTime(1977, 1, 1, 1, 1, 1);
//            }
//            finally
//            {
//                myReader.Close();
//            }
//        }
//        /// <summary>
//        /// 获得结算系统中最早的一次账户结算,当从数据库恢复数据时候需要从最早结算时间为
//        /// 时间戳来读取交易数据。每个账户的数据恢复时根据每个账户的结算时间来读取数据的
//        /// </summary>
//        /// <returns></returns>
//        public DateTime getLastSettleTime()
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT min(settledatetime)  as settledatetime FROM accounts");
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDateTime("settledatetime");
//            }
//            catch (Exception ex)
//            {
//                return new DateTime(1977, 1, 1, 1, 1, 1);
//            }
//            finally
//            {
//                myReader.Close();
//            }
//        }

//        /// <summary>
//        /// 更新某个账户的结算时间
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="datetime"></param>
//        /// <returns></returns>
//        private bool UpdateAccountSettlementTime(string account, DateTime datetime)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET settledatetime= '{0}' WHERE account = '{1}'", datetime.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }

//        /// <summary>
//        /// //结算账户的时候当我们将持仓盈亏结算到当日权益中去后，持仓盈亏则变成0,持仓成本为结算当时的价格。这个时候我们插入持仓的时候用最新价格更新到持仓成本
//        /// </summary>
//        /// <param name="p"></param>
//        /// <returns></returns>
//        bool InsertPosition(Position p)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into positionhold (`account`,`symbol`,`size`,`price`,`date`) values('{0}','{1}','{2}','{3}','{4}')", p.Account, p.Symbol, p.Size, p.LastPrice, DateTime.Today.ToString());
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }

//        /// <summary>
//        /// 删除持仓列表中某个交易账户的所有持仓信息 用于结算时更新当前持仓信息
//        /// </summary>
//        /// <param name="accountID"></param>
//        /// <returns></returns>
//        private bool DelPosition(string accountID)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from positionhold where account ='{0}'", accountID);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }

//        public bool DelPosition()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from positionhold ");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        #endregion

//        #region 账户类相关操作

//        public int getAccountUserID(string account)
//        {
//            this.SqlReady();
//            string sql = String.Format("select user_id from accounts where `account` = '{0}'",account);

//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetInt32("user_id");

//            }
//            catch (Exception ex)
//            {
//                return -1;
//            }
//            finally
//            {
//                myReader.Close();

//            }
//        }
//        /// <summary>
//        /// 获得账户数据集
//        /// </summary>
//        /// <returns></returns>
//        public DataSet getAccounts()
//        {
//            this.SqlReady();
//            string sql = "select * from accounts";
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "accounts");
//            return retSet;

//        }
//        //
//        public DataSet getAccounts(string accID)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from accounts where account='{0}'", accID);
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "accounts");
//            return retSet;

//        }


//        /// <summary>
//        /// 验证账户请求
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="pass"></param>
//        /// <returns></returns>
//        public bool validAccount(string acc, string pass)
//        {
//            if (_reconecting) return false;//数据库重连 直接返回false
//            this.SqlReady();
//            string sql = String.Format("select * from accounts where `account` = '{0}'", acc);

//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                while (myReader.Read())
//                {
//                    return pass.Equals(myReader.GetString("pass"));
//                }
//            }
//            finally
//            {
//                myReader.Close();

//            }
//            return false;
//        }
//        /// <summary>
//        /// 更改账户密码
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="pass"></param>
//        /// <returns></returns>
//        public bool ChangeAccountPass(string acc, string pass)
//        {
//            if (_reconecting) return false;//数据库重连 直接返回false
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET pass = '{0}' WHERE account = '{1}'", pass, acc);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);


//        }

//        /// <summary>
//        /// 更新账户类型
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="acctype"></param>
//        /// <returns></returns>
//        public bool UpdateAccountRouterTransferType(string account, QSEnumOrderTransferType acctype)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET order_route_type = '{0}' WHERE account = '{1}'", acctype.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        /// <summary>
//        /// 更新账户类别 交易员，配资
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="ca"></param>
//        /// <returns></returns>
//        public bool UpdateAccountCategory(string account, QSEnumAccountCategory ca)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET account_category = '{0}' WHERE account = '{1}'", ca.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
            
//        }
//        /// <summary>
//        /// 更新账户购买乘数 用于配资客户
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="buypower"></param>
//        /// <returns></returns>
//        public bool UpdateAccountBuyMultiplier(string account, int buymultiplier)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET buymultiplier = '{0}' WHERE account = '{1}'", buymultiplier.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
            
//        }
//        /// <summary>
//        /// 更新账户类型
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="acctype"></param>
//        /// <returns></returns>
//        public bool UpdateAccountInterday(string account,bool intraday )
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET intraday = '{0}' WHERE account = '{1}'", intraday.ToString(), account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        /// <summary>
//        /// 更新账户代理编号
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="agentcode"></param>
//        /// <returns></returns>
//        public bool UpdateAccountAgentCode(string account,string agentcode)
//        {
//            this.SqlReady();
//            string sql = String.Format("UPDATE accounts SET agent_code = '{0}' WHERE account = '{1}'",agentcode, account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }

//        #endregion



//        //对操盘手的账户进行入金与出金操作
//        #region 现金操作
//        /// <summary>
//        /// 现金操作
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="ammount"></param>
//        /// <param name="comment"></param>
//        /// <returns></returns>
//        public bool CashOperation(string account, decimal ammount, string comment)
//        {
//            if (_reconecting) return false;//数据库重连 直接返回false
//            this.SqlReady();
//            string sql = String.Format("Insert into transactions (`datetime`,`ammount`,`comment`,`account`) values('{0}','{1}','{2}','{3}')", DateTime.Now.ToString(), ammount.ToString(), comment, account.ToString());
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        public bool IsCashOperationIDExit(string refid)
//        {
//            if (_reconecting) return true;//数据库无法访问时,我们认为该refid是存在的,处于安全考虑
//            this.SqlReady();

//            string sql = String.Format("SELECT * FROM transactions where comment ='{0}'", refid);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        /// <summary>
//        /// 获得某个时间段内的出入金总和系统重置资金操作除外
//        /// 出入金用于
//        /// 1.调整账户权益
//        /// 2.分红系统日后再进行设计
//        /// </summary>
//        /// <param name="accId"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public decimal TotalCash(string accId, DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT Sum(ammount) as total FROM transactions where datetime >='{0}'and datetime <= '{1}' and account='{2}' and comment!='System Reset'", start.ToString(), end.ToString(), accId);
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDecimal("total");
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//            finally
//            {
//                myReader.Close();
//            }
//        }

//        /// <summary>
//        /// 获得结算以来的入金
//        /// </summary>
//        /// <param name="accId"></param>
//        /// <param name="start"></param>
//        /// <returns></returns>
//        public decimal CashIn(string accId, DateTime start)
//        {
//            return CashIn(accId, start, DateTime.Now);
//        }

//        public decimal CashIn(string accId, DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT Sum(ammount) as total FROM transactions where datetime >='{0}'and datetime <= '{1}' and account='{2}' and ammount>0", start.ToString(), end.ToString(), accId);
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDecimal("total");
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//            finally
//            {
//                myReader.Close();
//            }
//        }

//        /// <summary>
//        /// 获得结算以来的出金
//        /// </summary>
//        /// <param name="accID"></param>
//        /// <param name="start"></param>
//        /// <returns></returns>
//        public decimal CashOut(string accID, DateTime start)
//        {
//            return CashOut(accID, start, DateTime.Now);
//        }
//        public decimal CashOut(string accId, DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT Sum(ammount) as total FROM transactions where datetime >='{0}'and datetime <= '{1}' and account='{2}' and ammount<0", start.ToString(), end.ToString(), accId);
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetDecimal("total");
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//            finally
//            {
//                myReader.Close();
//            }


//        }

//        #endregion
//        //执行web过程 插入新的账户
//        #region 插入新的账户
//        /*
//        public string addAccount()
//        {
//            this.SqlReady();
//            int acref = getLiveAccountRef();
//            if (acref < 0) return null;
//            string accID = (acref + 1).ToString();
//            //if (haveRequested(user_id)) return null;
//            string sql = String.Format("Insert into accounts (`account`,`createdtime`) values('{0}','{1}')", accID,DateTime.Now.ToString());
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return accID;
//            else
//                return null;
//        }**/


//        /// <summary>
//        /// 某个User_id增加一个什么类型的帐号,并且密码设置为pass
//        /// </summary>
//        /// <param name="user_id"></param>
//        /// <param name="pass"></param>
//        /// <param name="category"></param>
//        /// <returns></returns>
//        public string AddAccount(string user_id = "0", string pass = "123456", QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
//        {
//            this.SqlReady();
//            int acref = getAccountRef(category);
//            if (acref < 0) return null;
//            string accID = (acref + 1).ToString();
//            //如果user_id为非0编号 表明是由前端web网站调用的添加帐号,因此需要检查user_id是否已经申请过帐号
//            if (user_id != "0" && haveRequested(user_id,category)) return null;

//            string sql = String.Format("Insert into accounts (`account`,`user_id`,`createdtime`,`pass`,`account_category`,`settledatetime`) values('{0}','{1}','{2}','{3}','{4}','{5}')", accID, user_id.ToString(), DateTime.Now.ToString(), pass, category, DateTime.Now-new TimeSpan(1,0,0,0,0));
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return accID;
//            else
//                return null;
//        }
//        /// <summary>
//        /// 增加交易员帐号,交易员帐号可以是公司内部的交易帐号,也可以是对外的模拟帐号
//        /// </summary>
//        /// <param name="user_id"></param>
//        /// <param name="pass"></param>
//        /// <returns></returns>
//        public string addNewAccount(string user_id="0",string pass="123456")
//        {
//            this.SqlReady();
//            int acref = getAccountRef(QSEnumAccountCategory.DEALER);
//            if (acref < 0) return null;
//            string accID = (acref + 1).ToString();
//            //如果user_id为非0编号 表明是由前端web网站调用的添加帐号,因此需要检查user_id是否已经申请过帐号
//            if (user_id!="0" && haveRequested(user_id)) return null;
//            string sql = String.Format("Insert into accounts (`account`,`user_id`,`createdtime`,pass) values('{0}','{1}','{2}','{3}')", accID, user_id.ToString(), DateTime.Now.ToString(),pass);
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return accID;
//            else
//                return null;
//        }

//        /// <summary>
//        /// 增加配资帐号
//        /// </summary>
//        /// <param name="user_id"></param>
//        /// <param name="pass"></param>
//        /// <returns></returns>
//        public string addNewFinAccount(string user_id = "0", string pass = "123456")
//        {
//            this.SqlReady();
//            int acref = getAccountRef(QSEnumAccountCategory.LOANEE);
//            if (acref < 0) return null;
//            string accID = (acref + 1).ToString();
//            if (user_id!="0" && haveRequested(user_id,QSEnumAccountCategory.LOANEE)) return null;
//            string sql = String.Format("Insert into accounts (`account`,`user_id`,`createdtime`,`pass`,`account_category`) values('{0}','{1}','{2}','{3}','{4}')", accID, user_id.ToString(), DateTime.Now.ToString(), pass, "LOANEE");
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return accID;
//            else
//                return null;
//        }

//        public string addSeasonAccount(string user_id = "0", string pass = "123456")
//        {
//            this.SqlReady();
//            int acref = getAccountRef(QSEnumAccountCategory.SEASON);
//            if (acref < 0) return null;
//            string accID = (acref + 1).ToString();
//            if (user_id != "0" && haveRequested(user_id, QSEnumAccountCategory.SEASON)) return null;
//            string sql = String.Format("Insert into accounts (`account`,`user_id`,`createdtime`,`pass`,`account_category`) values('{0}','{1}','{2}','{3}','{4}')", accID, user_id.ToString(), DateTime.Now.ToString(), pass, "SEASON");
//            cmd.CommandText = sql;
//            if (cmd.ExecuteNonQuery() > 0)
//                return accID;
//            else
//                return null;
//        }
    
    
//        public bool delAccount(string account)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from accounts where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            sql = String.Format("delete from finservices where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            sql = String.Format("delete from log_orders where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            sql = String.Format("delete from orders where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();


//            sql = String.Format("delete from log_trades where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            sql = String.Format("delete from trades where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            sql = String.Format("delete from log_postransactions where accounts='{0}'", account);
//            cmd.CommandText = sql;
//            cmd.ExecuteNonQuery();

//            return true;
//        }
//        /// <summary>
//        /// 查找某个账户是否已经有了某个类型的帐号
//        /// 即单个帐号只允许申请1个模拟帐号 和 一个 配资帐号
//        /// </summary>
//        /// <param name="user_id"></param>
//        /// <param name="category"></param>
//        /// <returns></returns>
//        bool haveRequested(string user_id,QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
//        {
//            this.SqlReady();
//            string sql = String.Format("select user_id from accounts where user_id='{0}' and account_category='{1}'", user_id,category.ToString());
//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                int uid = int.Parse(myReader.GetString("user_id"));

//                return true;

//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//            finally
//            {
//                myReader.Close();

//            }
//        }

//        /// <summary>
//        /// 获得某个帐户类别的最大帐户编号引用 用于新增帐户时进行编号自动递增
//        /// </summary>
//        /// <param name="category"></param>
//        /// <returns></returns>
//        int getAccountRef(QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
//        {

//            this.SqlReady();
//            int prefix = SqlGlobal.GetAccountPrefix(category);
//            string sql = String.Format("select max(account) as account from accounts where account_category='{0}'",category.ToString());// and account REGEXP '{1}'",category.ToString() ,"^" +prefix.ToString());

//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return int.Parse(myReader.GetString("account"));

//            }
//            catch (Exception ex)
//            {
//                return prefix * ((int)Math.Pow(10, SqlGlobal.GetAccountPowerLength(category)));
//            }
//            finally
//            {
//                myReader.Close();

//            }
//        }
//        #endregion

//        #region customer的相关操作

//        /// <summary>
//        /// 返回某个customer信息
//        /// </summary>
//        /// <param name="accID"></param>
//        /// <returns></returns>
//        public DataSet getCustomer(string customer)
//        {
//            this.SqlReady();
//            string sql = String.Format("select * from customers where customer='{0}'", customer);
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "customers");
//            return retSet;
//        }
//        /// <summary>
//        /// 验证customer
//        /// </summary>
//        /// <param name="customer"></param>
//        /// <param name="pass"></param>
//        /// <returns></returns>
//        public bool validCustomer(string customer, string pass)
//        {
//            if (_reconecting) return false;//数据库重连 直接返回false
//            this.SqlReady();
//            string sql = String.Format("select * from customers where `customer` = '{0}'", customer);

//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                while (myReader.Read())
//                {
//                    return pass.Equals(myReader.GetString("pass"));
//                }
//            }
//            finally
//            {
//                myReader.Close();

//            }
//            return false;
//        }

//        #endregion

//        #region 获得账户每日盈利 用于计算折算收益
//        /// <summary>
//        /// 删除重复的结算信息
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public bool delReduplicateSettle(DateTime d)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from settlement  WHERE settleday = '{0}'", d.ToString("yyyy-MM-dd"));
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);

//        }

//        /// <summary>
//        /// 获得账户每日结算盈亏序列,时间04-02结算日，时间部分是00:00:00, 进入比赛日期04-02 09:35,则4月2日当天结算收益不被采用计算,因为查询时时间>=04-02 09:35 参赛以后的收益信息。因此报名当天冻结账户
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public DataSet ReTotalDaily(string account, DateTime start, DateTime end)
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT account,settleday,realizedpl,unrealizedpl,commission,if(realizedpl+unrealizedpl-commission>25000 AND settleday >= '2013-07-29',25000,realizedpl+unrealizedpl-commission) as netprofit FROM settlement where account='{0}' and settleday>='{1}' and settleday<='{2}' and commission>0;", account, start.ToString(), end.ToString());
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "settlement");
//            return retSet;
//        }

//        #region 比赛账户统计数据
//        /// <summary>
//        /// 获得所有比赛统计数据
//        /// </summary>
//        /// <returns></returns>
//        public DataSet GetRaceStatistics()
//        {
//            this.SqlReady();
//            string sql = String.Format("SELECT * FROM viewprofit_account_summary");
//            // 创建一个适配器
//            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
//            // 创建DataSet，用于存储数据.
//            DataSet retSet = new DataSet();
//            // 执行查询，并将数据导入DataSet.
//            adapter.Fill(retSet, "racestatistics");
//            return retSet;
//        }
//        /// <summary>
//        /// 插入账户比赛统计信息
//        /// </summary>
//        public bool InsertRaceStatistics(string account,string race_id,string race_status,DateTime entry_time,int race_day,
//            decimal nowequity,decimal obverseequity,decimal commission,int pr_num,int winnum,int lossnum,
//            decimal avg_profit,decimal avg_loss,int winday,int seqwinday,int lossday,int seqlossday,decimal avg_postransperday,decimal avg_posholdtime,decimal totalperformance ,decimal entryperformance,decimal exitperformance,decimal winpercent,decimal profitfactor)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into race_statistics(`account`,`race_id`,`race_status`,`race_entrytime`,`race_day`,`now_equity`,`obverse_equity`,`commission`,`postrans_num`,`win_num`,`loss_num`,`avg_profit`,`avg_loss`,`win_day`,`seqwin_day`,`loss_day`,`seqloss_day`,`avg_postransperday`,`avg_posholdtime`,`total_performance`,`entry_performance`,`exit_performance`,`winpercent`,`profitfactor`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')", account, race_id, race_status, entry_time, race_day, nowequity, obverseequity, commission, pr_num, winnum, lossnum, avg_profit, avg_loss, winday, seqwinday, lossday, seqlossday, avg_postransperday, avg_posholdtime, totalperformance, entryperformance, exitperformance, winpercent, profitfactor);
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));
//        }


//        /// <summary>
//        /// 情况比赛状态
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearRaceStatistics()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from race_statistics");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        #endregion
//        #endregion

//    }
//}
