using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Mixins.DataBase
{
    public sealed class DBConnectionPoll<T> : ObjectPool
            where T : SQLBase, new()
    {
        string _server;
        string _user;
        string _pass;
        string _dbname;
        int _port;
        public DBConnectionPoll(string server, string user, string pass, string dbname = "quantshop", int port = 3306)
        {
            _server = server;
            _user = user;
            _pass = pass;
            _dbname = dbname;
            _port = port;
        }
        /// <summary>
        /// 创建对象池对象 数据库连接对象
        /// 通过Init传入参数
        /// </summary>
        /// <returns></returns>
        protected override object Create()
        {
            T t = new T();
            t.Init(_server, _user, _pass, _dbname, _port);
            return t;
        }

        /// <summary>
        /// 检查数据库连接对象是否有效
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool Validate(object o)
        {
            try
            {
                T dbclear = (T)o;
                return dbclear.IsLive;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        /// <param name="o"></param>
        protected override void Expire(object o)
        {
            try
            {
                T dbclear = (T)o;
                dbclear.Dispose();//调用Dispose手动释放数据库对象
                dbclear = null;

            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 从对象池获得数据库连接对象
        /// </summary>
        /// <returns></returns>
        public T BorrowDBConnection()
        {
            try
            {
                return (T)base.GetObjectFromPool();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将数据库连接对象归还给对象池
        /// </summary>
        /// <param name="conn"></param>
        public void ReturnDBConnection(T conn)
        {
            try
            {
                base.ReturnObjectToPool(conn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}