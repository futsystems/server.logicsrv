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
    /// 服务端服务对象的基类,实现日志输出,邮件通知,以及服务端对象名称标识
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
                logger = LogManager.GetLogger(programe);

                PROGRAME = programe;
                _uuid = System.Guid.NewGuid().ToString();
                TLCtxHelper.Ctx.Register(this);
            }
            catch (Exception ex)
            {
                Util.Debug("BaseSrvObject init error:" + ex.ToString());
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
