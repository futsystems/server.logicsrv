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
        /// 
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
        void RegisterCallback(string module, string cmd, Action<string> handler);

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        void UnRegisterCallback(string module, string cmd, Action<string> handler);
    }
}
