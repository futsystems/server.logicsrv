using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    /// <summary>
    /// 全局事件订阅
    /// </summary>
    public partial class Globals
    {

        /// <summary>
        /// 
        /// </summary>
        public static void Reset()
        {

            EnvReady = false;    
        }

        /// <summary>
        /// 当前状态是否就绪
        /// 用于初始化过程中过滤界面的相关操作
        /// 比如在tlclient未初始化时候进行请求操作等
        /// </summary>
        public static bool EnvReady = false;

        static event VoidDelegate InitFinishedEvent;

        /// <summary>
        /// 触发初始化完成事件 用于通知常驻界面资源进行界面更新
        /// 初始化分水岭 在执行OnInitFinished之前生成的对象 订阅OnInitFinished会得到触发，同时由于通过判断EnvReady来进行订阅的，即初始化之后的产生的对象 会通过EnvReady进行订阅
        /// 初始化成功后 会调用OnInitFinished
        /// </summary>
        public static void OnInitFinished()
        {
            if (InitFinishedEvent != null)
                InitFinishedEvent();
            Globals.EnvReady = true;
        }

        /// <summary>
        /// 注册初始化完成事件回调
        /// 如果核心初始化过程没有完成 则注册成回调函数,当初始化完成后再次执行
        /// 如果初始化过程已经完成，则直接执行该函数
        /// </summary>
        /// <param name="callback"></param>
        public static void RegInitCallback(VoidDelegate callback)
        {
            if (!Globals.EnvReady)
            {
                InitFinishedEvent += new VoidDelegate(callback);
            }
            else
            {
                callback();
            }
        }


        /// <summary>
        /// 注册EventHandler用于执行事件注册与延迟加载
        /// </summary>
        /// <param name="control"></param>
        public static void RegIEventHandler(object control)
        {
            if (control is UserControl)
            {
                if (control is IEventBinder)
                {
                    IEventBinder h = control as IEventBinder;
                    //注册初始化完成事件响应函数 用于响应初始化完成事件 当对象在初始化完成之前创建 需要在完成初始化后 加载基础数据
                    Globals.RegInitCallback(h.OnInit);
                    //将组件销毁的事件与对应的注销函数进行绑定
                    (control as UserControl).Disposed += (s, e) => { h.OnDisposed(); };
                }
            }

            if (control is ComponentFactory.Krypton.Toolkit.KryptonForm)
            {
                if (control is IEventBinder)
                {
                    IEventBinder h = control as IEventBinder;
                    //注册初始化完成事件响应函数 用于响应初始化完成事件 当对象在初始化完成之前创建 需要在完成初始化后 加载基础数据
                    Globals.RegInitCallback(h.OnInit);
                    //将组件销毁的事件与对应的注销函数进行绑定
                    (control as ComponentFactory.Krypton.Toolkit.KryptonForm).Disposed += (s, e) => { h.OnDisposed(); };
                }
            }




        }
    }
}
