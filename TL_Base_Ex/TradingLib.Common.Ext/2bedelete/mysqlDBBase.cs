//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MySql.Data.MySqlClient;
//using System.Threading;
//using System.IO;
//using TradingLib.API;
//using TradingLib.Common;
//using System.Data;

//namespace TradingLib.MySql
//{
    
//    /// <summary>
//    /// 数据库连接的基类,用于生成不同的数据库连接,执行不同的的数据库操作
//    /// </summary>
//    public class mysqlDBBase:IDisposable
//    {
//        public event DebugDelegate SendDebugEvent;
//        protected void debug(string msg)
//        {
//            if (SendDebugEvent != null)
//                SendDebugEvent(msg);
//        }
//        protected string connectionString = string.Empty; //= "server=127.0.0.1;user id=root; password=lemtone2005; database=quantshop; pooling=false;charset=utf8";
//        protected MySqlConnection conn;
//        protected MySqlCommand cmd;
//        //增加容错机制,保证数据正确的记录到数据库
        
//        protected string _server = "114.84.192.87";
//        public string Server { get { return _server; } set { _server = value; } }
//        protected string _user = "qs_qianbo";
//        public string User { get { return _user; } set { _user = value; } }
//        protected string _pass = "lemtone2005";
//        public string Pass { get { return _pass; } set { _pass = value; } }
//        protected int  _port = 3306;
//        public int Port { get { return _port; } set { _port = value; } }
//        protected string _dbnanme = "quantshop";
//        public string DBName { get { return _dbnanme; } set { _dbnanme=value; } }





//        #region 类实例方法
//        public void Init(string server, string user, string pass, string dbname = "quantshop", int port = 3306)
//        {

//            _server = server;
//            _user = user;
//            _pass = pass;
//            _dbnanme = dbname;
//            _port = port;

//            connectionString = "server=" + _server + ";user id=" + _user + "; password=" + _pass + "; port=" + _port.ToString() + "; database=" + _dbnanme + "; pooling=false;charset=utf8";
//            TLCtxHelper.Debug("******************************mysqlDBBase str:" + connectionString);
//            conn = new MySqlConnection(connectionString);
//            cmd = conn.CreateCommand();
//            conn.Open();

//        }
//        public MySqlConnection Connection { get { return conn; } }
//        public void Dispose()
//        {
//            try
//            {
//                //IDbConnection dbconn = conn as IDbConnection;
//                conn.Close();
//            }
//            catch (Exception ex)
//            {
//                debug("关闭连接时发生错误:" + ex.ToString());
//            }
//        }
//        public bool isConnOk()
//        {
//            //debug("connection state:"+conn.State.ToString());
//            if (conn.State == System.Data.ConnectionState.Open)
//                return true;
//            return false;
//        }

//        int _retrydelay = 10;
//        protected bool _reconecting = false;
//        protected bool SqlReady()
//        {
//            if (isConnOk())
//            {
//                //_reconecting = false;
//                return true;
//            }
//            else
//            {
//                while (!isConnOk())
//                {
//                    _reconecting = true;
//                    //debug("connection retry");
//                    try
//                    {
//                        //debug("connection close");
//                        conn.Close();
//                    }
//                    catch (Exception ex)
//                    {
//                        //debug("connection close fail:" + ex.ToString());
//                    }
//                    try
//                    {
//                        //debug("connection reconnect to database");
//                        conn = new MySqlConnection(connectionString);
//                        cmd = conn.CreateCommand();
//                        conn.Open();
//                        _reconecting = false;
//                        return true;
//                    }
//                    catch (Exception ex)
//                    {
//                        //debug("reconnection fail:" + ex.ToString());
//                    }

//                    //debug("sleep for " + _retrydelay.ToString() + "secends");
//                    Thread.Sleep(_retrydelay * 1000);
//                }
//            }
//            return false;
//        }

//        #endregion



//        /// <summary>
//        /// 执行sql语句初始化数据表
//        /// </summary>
//        /// <param name="varFileName"></param>
//        /// <param name="server"></param>
//        /// <param name="user"></param>
//        /// <param name="pass"></param>
//        /// <param name="dbname"></param>
//        /// <param name="port"></param>
//        /// <returns></returns>
//        public static bool ExecuteSqlFile(string varFileName, string server, string user, string pass, string dbname = "quantshop", int port = 3306)
//        {
//            using (StreamReader reader = new StreamReader(varFileName, System.Text.Encoding.GetEncoding("utf-8")))
//            {
//                string constr = "server=" + server + ";user id=" + user + "; password=" + pass + "; port=" + port.ToString() + "; database=" + dbname + "; pooling=false;charset=utf8";
//                MySqlCommand command;
//                MySqlConnection Connection = new MySqlConnection(constr);
//                Connection.Open();
//                try
//                {
//                    string line = "";
//                    string l;
//                    while (true)
//                    {
//                        // 如果line被使用，则设为空  
//                        if (line.EndsWith(";"))
//                            line = "";

//                        l = reader.ReadLine();

//                        // 如果到了最后一行，则退出循环  
//                        if (l == null) break;
//                        // 去除空格  
//                        l = l.TrimEnd();
//                        // 如果是空行，则跳出循环  
//                        if (l == "") continue;
//                        // 如果是注释，则跳出循环  
//                        if (l.StartsWith("--")) continue;

//                        // 行数加1   
//                        line += l;
//                        // 如果不是完整的一条语句，则继续读取  
//                        if (!line.EndsWith(";")) continue;
//                        if (line.StartsWith("/*!"))
//                        {
//                            continue;
//                        }

//                        //执行当前行  
//                        command = new MySqlCommand(line, Connection);
//                        command.ExecuteNonQuery();
//                    }
//                }
//                finally
//                {
//                    Connection.Close();
//                }
//            }

//            return true;
//        }

//        public static bool DropDataBase(string server, string user, string pass, string dbname, int port = 3306)
//        {
//            bool re = false;
//            string constr = "server=" + server + ";user id=" + user + "; password=" + pass + "; port=" + port.ToString() + "; pooling=false;charset=utf8";

//            MySqlConnection conn = new MySqlConnection(constr);
//            try
//            {
//                MySqlCommand cmd = new MySqlCommand("DROP TABLE IF EXISTS  " + dbname + ";", conn);
//                conn.Open();
//                cmd.ExecuteNonQuery();
//                re = true;
//            }
//            catch (Exception ex)
//            {
//                re = false;
//            }
//            finally
//            {
//                conn.Close();
//            }
//            return re;
//        }
//        public static bool CreateDatabase(string server, string user, string pass, string dbname, int port = 3306)
//        {
//            bool re=false;
//            string constr = "server=" + server + ";user id=" + user + "; password=" + pass + "; port=" + port.ToString() + "; pooling=false;charset=utf8";
                
//            MySqlConnection conn = new MySqlConnection(constr);
//            try
//            {
//                MySqlCommand cmd = new MySqlCommand("CREATE DATABASE " + dbname + ";", conn);
//                conn.Open();
//                cmd.ExecuteNonQuery();
//                re = true;
//            }
//            catch (Exception ex)
//            {
//                re = false;
//            }
//            finally
//            {
//                conn.Close();
//            }
//            return re;
//        }
//        /// <summary>
//        /// 检查提供的用户名与密码是否正确
//        /// </summary>
//        /// <param name="server"></param>
//        /// <param name="user"></param>
//        /// <param name="pass"></param>
//        /// <param name="port"></param>
//        /// <returns></returns>
//        public static bool ConnectionTest_User(string server, string user, string pass, int port = 3306)
//        {
//            return ConnectionTest(server, user, pass, "test", port);
//        }
//        /// <summary>
//        /// 检测数据库连接是否有效
//        /// </summary>
//        /// <param name="server"></param>
//        /// <param name="user"></param>
//        /// <param name="pass"></param>
//        /// <param name="dbname"></param>
//        /// <param name="port"></param>
//        /// <returns></returns>
//        public static bool ConnectionTest(string server,string user,string pass,string dbname="quantshop",int port=3306)
//        {
//            bool canconnection = false;
//            string constr= "server=" + server + ";user id=" + user + "; password=" + pass + "; port=" + port.ToString() + "; database=" + dbname + "; pooling=false;charset=utf8";
//            MySqlConnection tconn = new MySqlConnection(constr);
//            try
//            {
//                tconn.Open();
//                canconnection = true;
//            }
//            catch (Exception ex)
//            {
//                canconnection = false;
//            }
//            finally
//            {
//                tconn.Close();
//            }
//            return canconnection;
//        }


//    }
//}
