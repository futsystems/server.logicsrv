using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins;
using System.Data;


namespace TradingLib.Mixins.DataBase
{
    /// <summary>
    /// 纯虚数据库对象
    /// 当使用数据库时候 集成该类，实现纯虚函数 Init 用于初始化数据库连接
    /// </summary>
    public abstract class SQLBase : IDisposable
    {

        public event DebugDelegate SendDebugEvent;
        protected void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        protected string connectionString = string.Empty;
        protected IDbConnection conn;
        protected IDbCommand cmd;

        /// <summary>
        /// 纯虚函数 用于初始化数据库连接对象
        /// 提供建立数据库连接所必须的参数
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="dbname"></param>
        /// <param name="port"></param>
        public abstract void Init(string server, string user, string pass, string dbname = "quantshop", int port = 3306);

        /*
         * MySQL数据库连接创建
        {
            connectionString = "server=" + _server + ";user id=" + _user + "; password=" + _pass + "; port=" + _port.ToString() + "; database=" + _dbnanme + "; pooling=false;charset=utf8";
            conn = new MySqlConnection(connectionString);
            cmd = conn.CreateCommand();
            conn.Open();

        }**/

        /// <summary>
        /// 返回数据库连接对象 用于执行相关数据库操作
        /// </summary>
        public virtual IDbConnection Connection { get { return conn; } }


        /// <summary>
        /// 用于释放数据库连接
        /// </summary>
        public virtual void Dispose()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                debug("关闭连接时发生错误:" + ex.ToString());
            }
        }

        /// <summary>
        /// 查询当前连接是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsLive
        {
            get
            {
                if (conn == null) return false;
                if (conn.State == System.Data.ConnectionState.Open)
                    return true;
                return false;
            }
        }
    }
}