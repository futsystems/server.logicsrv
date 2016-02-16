using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using System.Diagnostics;

namespace TradingLib.Common
{
    /// <summary>
    /// 服务端服务对象的基类
    /// 1.对象名称
    /// 2.对象UUID编号
    /// 3.日志功能
    /// </summary>
    public class BaseSrvObject:IDisposable
    {
        protected ILog logger = null;
        /// <summary>
        /// 服务端对象名称
        /// </summary>
        protected string PROGRAME = "BaseSrvObject";

        public string Name { get { return PROGRAME; } }
        public string UUID { get { return _uuid; } }
        protected string _uuid; 

        public BaseSrvObject(string programe="BaseSrvObject")
        {
            try
            {
                PROGRAME = programe;
                logger = LogManager.GetLogger(PROGRAME);
                _uuid = System.Guid.NewGuid().ToString();

                //将对象注册到全局CTX实现自动绑定
                TLCtxHelper.Ctx.Register(this);
            }
            catch (Exception ex)
            {
                logger.Error("BaseSrvObject init error:" + ex.ToString());
            }
        }



        bool disposed = false;

        public virtual void Dispose()
        {
            try
            {
                TLCtxHelper.Ctx.Unregister(this);
            }
            finally
            {
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        ~BaseSrvObject()
        {
            Console.WriteLine("descruct called");
            Dispose();
        }
    }
}
