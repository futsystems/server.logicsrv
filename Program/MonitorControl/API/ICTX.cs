using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public interface ICTX
    {
        /// <summary>
        /// 当前登入的管理员对象
        /// </summary>
        ManagerSetting Manager { get; }


        /// <summary>
        /// 底层环境是否初始化完毕
        /// </summary>
        bool EnvReady { get; }

        /// <summary>
        /// 获得全局基础数据
        /// </summary>
        IBasicInfoTracker BasicInfoTracker { get; }




        /// <summary>
        /// 将某个对象注册到系统
        /// 界面对象由于加载时间的问题 可能在系统初始化完成前加载也有可能在系统初始化完成后加载
        /// 完成后加载的则立即调用初始化函数 用于获得底层基本数据 填充界面
        /// 完成前加载的则响应系统底层初始化完成时间 用于延迟获得底层基本数据 填充界面
        /// </summary>
        /// <param name="obj"></param>
        void RegIEventHandler(IEventBinder obj);

        /**
         * 在一些基础组件上 我们需要动态的响应回调函数，因此需要在控件内部进行注册和注销回调
         * 这个过程需要用到注册和注销回调函数的入口
         * 
         * 
         * 
         * **/
        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void RegisterCallback(string module, string cmd, Action<string,bool> handler);

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void UnRegisterCallback(string module, string cmd, Action<string,bool> handler);

        /// <summary>
        /// 注册通知类回调
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void RegisterNotifyCallback(string module, string cmd, Action<string> handler);


        /// <summary>
        /// 注销通知类回调
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void UnRegisterNotifyCallback(string module, string cmd, Action<string> handler);


        /// <summary>
        /// 提交某个请求
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        void Request(string module, string cmd, string args);
    }
}
