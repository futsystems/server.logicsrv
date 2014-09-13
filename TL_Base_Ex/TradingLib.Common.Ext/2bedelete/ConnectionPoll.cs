//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
//using System.Data;
//using TradingLib.Common;

//namespace TradingLib.MySql
//{
//    public sealed class ConnectionPoll<T>:ObjectPool
//        where T:mysqlDBBase,new()
//    {
//        string _server;
//        string _user;
//        string _pass;
//        string _dbname;
//        int _port;
//        public ConnectionPoll(string server, string user, string pass,string dbname="quantshop",int port=3306)
//        {
//            _server = server;
//            _user = user;
//            _pass = pass;
//            _dbname = dbname;
//            _port = port;   
//        }
//        /// <summary>
//        /// 新建clearcentre mysaldeb 实例
//        /// </summary>
//        /// <returns></returns>
//        protected override object Create()
//        {
//            T t = new T();
//            t.Init(_server, _user, _pass, _dbname, _port);
//            return t;
//        }

//        protected override bool Validate(object o)
//        {
//            try
//            {
//                T dbclear = (T)o;
//                return dbclear.isConnOk();
//            }
//            catch (SqlException)
//            {
//                return false;
//            }
//        }

//        protected override void Expire(object o)
//        {
//            try
//            {
//                T dbclear = (T)o;
//                dbclear.Dispose();
//                dbclear = null;

//            }
//            catch (SqlException) { }
//        }

//        public T BorrowDBConnection()
//        {
//            try
//            {
//                return (T)base.GetObjectFromPool();
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }

//        public void ReturnDBConnection(T conn)
//        {
//            base.ReturnObjectToPool(conn);
//        }

//        public T mysqlDB { get { return this.BorrowDBConnection(); } }
//        public void Return(T mysqldb)
//        {
//            this.ReturnDBConnection(mysqldb);
//        }
//    }
//    /*
//    /// <summary>
//    /// 对象池化的 清算中心数据库连接,用于并发相应清算中心数据库查询
//    /// </summary>
//    public sealed class ClearCentreDBHelper : ObjectPool
//    {
//        public ClearCentreDBHelper(string srv, string u, string p)
//        {
//            server = srv;
//            user = u;
//            pass = p;
//        }
//        string server="127.0.0.1";
//        string user = "clearcentre";
//        string pass = "lemtone2005";
        

//        /// <summary>
//        /// 新建clearcentre mysaldeb 实例
//        /// </summary>
//        /// <returns></returns>
//        protected override object Create()
//        {
//            return new mysqlDBClearCentre(server, user, pass, null);
//        }

//        protected override bool Validate(object o)
//        {
//            try
//            {
//                mysqlDBClearCentre dbclear = (mysqlDBClearCentre)o;
//                return dbclear.isConnOk();
//            }
//            catch (SqlException)
//            {
//                return false;
//            }
//        }

//        protected override void Expire(object o)
//        {
//            try
//            {
//                mysqlDBClearCentre dbclear = (mysqlDBClearCentre)o;
//                dbclear.Dispose();
//                dbclear = null;

//            }
//            catch (SqlException) { }
//        }

//        public mysqlDBClearCentre BorrowDBConnection()
//        {
//            try
//            {
//                return (mysqlDBClearCentre)base.GetObjectFromPool();
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//        }

//        public void ReturnDBConnection(mysqlDBClearCentre mysqldb)
//        {
//            base.ReturnObjectToPool(mysqldb);
//        }

//        public mysqlDBClearCentre mysqlDB { get { return this.BorrowDBConnection(); } }
//        public void Return(mysqlDBClearCentre mysqldb)
//        {
//            this.ReturnDBConnection(mysqldb);
//        }
        
//    }**/
//}
