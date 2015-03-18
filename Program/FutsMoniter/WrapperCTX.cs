using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    /// <summary>
    /// MoniterControl所用的ICTX实现 
    /// 用于向MoniterControl注入底层操作
    /// </summary>
    public class WrapperCTX:ICTX
    {
        /// <summary>
        /// 当前登入的管理员对象
        /// </summary>
        public ManagerSetting Manager 
        {
            get 
            {
                return Globals.Manager;
            } 
        }


        /// <summary>
        /// 底层环境是否初始化完毕
        /// </summary>
        public bool EnvReady 
        {
            get
            {
                return Globals.EnvReady;
            }
        }

        /// <summary>
        /// 获得全局基础数据
        /// </summary>
        public IBasicInfoTracker BasicInfoTracker 
        {
            get
            {
                return Globals.BasicInfoTracker as IBasicInfoTracker;
            }
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void RegIEventHandler(IEventBinder obj)
        {
            Globals.RegIEventHandler(obj);
        }

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
        public void RegisterCallback(string module, string cmd, Action<string> handler)
        {
            Globals.LogicEvent.RegisterCallback(module, cmd, handler);
        }

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public void UnRegisterCallback(string module, string cmd, Action<string> handler)
        {
            Globals.LogicEvent.UnRegisterCallback(module, cmd, handler);
        }
    }
}
