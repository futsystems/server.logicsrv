using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.DataBase;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.ORM
{
    public class DBHelper
    {
        static ILog logger = LogManager.GetLogger("DBHelper");

        /// <summary>
        /// 初始化数据库全局连接信息
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        public static void InitDBConfig(string address, int port, string name, string user, string pass)
        {

            logger.Info(string.Format("{0}Address:{1} Port:{2} DBName:{3} UserName:{4} Password:{5}", Util.GlobalPrefix, address, port, name, user, pass));
            DBAddress = address;
            DBPort= port;
            DBName = name;
            UserName = user;
            PassWord = pass;
            
        }

        static string DBAddress="127.0.0.1";
        static int DBPort=3306;
        static string DBName="demo";
        static string UserName="root";
        static string PassWord="123456";


        private static DBHelper defaultInsantance;

        private TradingLib.Mixins.DataBase.DBConnectionPoll<MySqlBase> dbpoll = null;

        static DBHelper()
        {
            defaultInsantance = new DBHelper();

        }

        private DBHelper()
        {
            
        }

        /// <summary>
        /// 获得数据库封装对象
        /// </summary>
        /// <returns></returns>
        public static MySqlBase BorrowDB()
        {
            //延迟数据库连接池的初始化
            if (defaultInsantance.dbpoll == null)
            {
                defaultInsantance.dbpoll = new TradingLib.Mixins.DataBase.DBConnectionPoll<MySqlBase>(DBAddress, UserName, PassWord, DBName, DBPort);
            }
            return defaultInsantance.dbpoll.BorrowDBConnection();
        }

        /// <summary>
        /// 返回数据库封装对象
        /// </summary>
        /// <param name="db"></param>
        public static void ReturDB(MySqlBase db)
        {
            defaultInsantance.dbpoll.ReturnDBConnection(db);
        }
    }
}
