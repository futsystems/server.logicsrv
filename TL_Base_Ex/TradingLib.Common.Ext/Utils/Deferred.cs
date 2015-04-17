using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace TradingLib.Common
{
    /// <summary>
    /// 延迟回调委托
    /// </summary>
    /// <param name="obj"></param>
    public delegate void DeferredCallBack(IDeferredResult obj);

    /// <summary>
    /// 延迟调用委托
    /// </summary>
    /// <param name="objs"></param>
    /// <returns></returns>
    public delegate object[] DeferredCall(object[] objs);

    /// <summary>
    /// 延迟调用结果
    /// </summary>
    public interface IDeferredResult
    {
        /// <summary>
        /// 延迟调用过程产生的结果
        /// </summary>
        object[] Result { get; }

        /// <summary>
        /// 延迟调用过程是否有异常
        /// </summary>
        bool AnyError { get; }


    }

    /// <summary>
    /// 延迟调用对象
    /// </summary>
    public class Deferred:IDeferredResult
    {
        DeferredCall _call = null;
        object[] _args = null;

        public Deferred(DeferredCall call,object[] args=null)
        {
            _call= call;
            _args = args;
        }



        void DeferredDone(IAsyncResult async)
        {
            //async中包装了异步方法执行的结果
            //从操作结果async中还原委托
            DeferredCall proc = ((AsyncResult)async).AsyncDelegate as DeferredCall;
            _result = proc.EndInvoke(async);
            Util.Debug("Deferred Done");
            if (_anyerror)
            {
                try
                {
                    ErrorEvent(this);
                }
                catch (Exception ex)
                {
                    Util.Debug("Deferred Handle Error error:" + ex.ToString());
                }
            }
            else
            {
                try
                {
                    SuccessEvent(this);
                }
                catch (Exception ex)
                {
                    Util.Debug("Deferred Handle Success error:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 提供延迟调用参数 并运行延迟调用
        /// </summary>
        /// <param name="arg"></param>
        public void Run(object[] arg)
        {
            _args = arg;
            this.Run();
        }

        /// <summary>
        /// 执行延迟调用
        /// </summary>
        public void Run()
        {
            DeferredCall deleg = new DeferredCall(WrapperCall);
            deleg.BeginInvoke(_args, DeferredDone, null);
            Util.Debug("Deferred Return");
            
        }

        public bool AnyError { get { return _anyerror; } }
        bool _anyerror = false;
        /// <summary>
        /// 执行方法
        /// </summary>
        object[] WrapperCall(object[] objs)
        {
            try
            {
                //执行函数
                return _call(_args);
            }
            catch (Exception ex)
            {
                Util.Error("Deferred run error:" + ex.ToString());
                _anyerror = true;
                return null;
            }
            
        }

        event DeferredCallBack SuccessEvent = delegate { };

        event DeferredCallBack ErrorEvent = delegate { };

        public object[] Result { get { return _result; } }
        public object[] _result = null;
        /// <summary>
        /// 传入执行成功的回调函数
        /// </summary>
        /// <returns></returns>
        public Deferred OnSuccess(DeferredCallBack cb)
        {
            //绑定正常回调
            SuccessEvent+=new DeferredCallBack(cb);
            return this;
        }

        /// <summary>
        /// 将某个延迟调用作为执行成功的回调
        /// </summary>
        /// <param name="defer"></param>
        /// <returns></returns>
        public Deferred OnSuccess(Deferred defer)
        {
            SuccessEvent += (r) => { defer.Run(r.Result); };
            return this;
        }


        public Deferred OnError(DeferredCallBack cb)
        {
            //绑定错误回调
            ErrorEvent +=new DeferredCallBack(cb);
            return this;
        }

        public Deferred OnError(Deferred defer)
        {
            ErrorEvent += (r) => { defer.Run(); };
            return this;
        }
    }


    //将某个查询封装成带延迟的同步调用，这样就可以将该调用封装如延迟调用对象进行异步调用，并触发成功或失败对应的回调

    
}
