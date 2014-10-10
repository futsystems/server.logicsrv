using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace TradingLib.ORM
{
    public class MySqlBase : TradingLib.Mixins.DataBase.SQLBase
    {

        public override void Init(string server, string user, string pass, string dbname = "quantshop", int port = 3306)
        {
            connectionString = "server=" + server + ";user id=" + user + "; password=" + pass + "; port=" + port.ToString() + "; database=" + dbname + "; pooling=false;charset=utf8";
            conn = new MySqlConnection(connectionString);
            cmd = conn.CreateCommand();
            conn.Open();
        }

    }

    /// <summary>
    /// 数据库连接对象封装
    /// 实现IDisposable接口
    /// 用于在using语句中实现自动释放资源
    /// </summary>
    public class DBMySql : IDisposable
    {
        MySqlBase _mysqlbase = null;

        public IDbConnection Connection { get { return _mysqlbase.Connection; } }

        public DBMySql()
        {
            _mysqlbase = DBHelper.BorrowDB();
        }


        public void Dispose()
        {
            DBHelper.ReturDB(_mysqlbase);
        }

    }
}
