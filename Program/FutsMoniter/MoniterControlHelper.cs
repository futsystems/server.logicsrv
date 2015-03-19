using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.GUI;

namespace FutsMoniter
{
    internal class HandlerWrapper
    {

        public HandlerWrapper(HandlerInfo info)
        {
            this.Handler = info;
            this.Control = (MonitorControl)info.Target;
        }

        public MonitorControl Control {get; private set;}
        public HandlerInfo Handler{get;private set;}

        public void HandlerResponse(string result)
        {
            //执行调用
            this.Handler.MethodInfo.Invoke(this.Handler.Target, new object[]{result});
        }

        public void HandeNotify(string result)
        {
            this.Handler.MethodInfo.Invoke(this.Handler.Target, new object[]{result});
        }

    }
    public class MonitorControlHelper
    {

        public static void RegisterControl(MonitorControl contrl)
        {
            RegisterCallback(contrl);
            //对象销毁时注销回调函数
            contrl.Disposed += (s, e) => { UnRegisterCallback(contrl); };
        }
        /// <summary>
        /// 获得某个对象的Callback函数列表
        /// 需要用CallbackAttr标注
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static List<HandlerInfo> FindHandler(object obj)
        {
            List<HandlerInfo> list = new List<HandlerInfo>();
            Type type = obj.GetType();
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo mi in methodInfos)
            {
                CallbackAttr[] attrs = (CallbackAttr[])Attribute.GetCustomAttributes(mi, typeof(CallbackAttr));
                if (attrs != null && attrs.Length >= 1)
                {
                    foreach (CallbackAttr attr in attrs)
                    {
                        list.Add(new HandlerInfo(mi, attr, obj));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 将某个MonitorControl的回调自动注册回调中心
        /// </summary>
        /// <param name="control"></param>
        public static void RegisterCallback(MonitorControl control)
        {
            Util.Debug("register callback for control:" + control.ToString(), QSEnumDebugLevel.INFO);
            List<HandlerInfo> handlers = FindHandler(control);
            foreach (HandlerInfo h in handlers)
            {
                RegisterCallback(h);
            }
        }

        static Dictionary<MonitorControl, List<HandlerWrapper>> handlermap = new Dictionary<MonitorControl, List<HandlerWrapper>>();
        static void RegisterCallback(HandlerInfo info)
        { 
            HandlerWrapper wrapper = new HandlerWrapper(info);
            Globals.LogicEvent.RegisterCallback(info.Attr.Module,info.Attr.Cmd,wrapper.HandlerResponse);

            if (!handlermap.Keys.Contains(wrapper.Control))
            {
                handlermap.Add(wrapper.Control, new List<HandlerWrapper>());
            }
            handlermap[wrapper.Control].Add(wrapper);
        }

        /// <summary>
        /// 将某个control的回调函数注销
        /// </summary>
        /// <param name="control"></param>
        public static void UnRegisterCallback(MonitorControl control)
        { 
            //如果没有该对象则直接返回
            if(!handlermap.Keys.Contains(control)) return;
            foreach(HandlerWrapper w in handlermap[control])
            {
                Globals.LogicEvent.UnRegisterCallback(w.Handler.Attr.Module, w.Handler.Attr.Cmd, w.HandlerResponse);
            }

            handlermap.Remove(control);
        }

    }
}
