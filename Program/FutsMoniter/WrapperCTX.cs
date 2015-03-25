using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    /// <summary>
    /// 
    /// </summary>
    public class WrapperCTX:ICTX
    {
        /// <summary>
        /// 获得登入的全局管理员对象
        /// </summary>
        public ManagerSetting Manager
        { 
            get
            {
                return Globals.Manager;
            }
        }

        public bool EnvReady
        {
            get
            {
                return Globals.EnvReady;
            }
        }

        public IBasicInfoTracker BasicInfoTracker
        {
            get
            {
                return Globals.BasicInfoTracker as IBasicInfoTracker;
            }
        }


        #region 对象和回调函数注册
        public void RegIEventHandler(IEventBinder obj)
        {
            Globals.RegIEventHandler(obj);
        }

        public void RegisterCallback(string module, string cmd, Action<string, bool> handler)
        {
            Globals.LogicEvent.RegisterCallback(module, cmd, handler);
        }

        public void UnRegisterCallback(string module, string cmd, Action<string, bool> handler)
        {
            Globals.LogicEvent.UnRegisterCallback(module, cmd, handler);
        }

        public void RegisterNotifyCallback(string module, string cmd, Action<string> handler)
        {
            Globals.LogicEvent.RegisterNotifyCallback(module, cmd, handler);
        }

        public void UnRegisterNotifyCallback(string module, string cmd, Action<string> handler)
        {
            Globals.LogicEvent.UnRegisterNotifyCallback(module, cmd, handler);
        }
        #endregion

        public void Request(string module, string cmd, string args)
        {
            Globals.TLClient.ReqContribRequest(module, cmd, args);
        }
    }
}
