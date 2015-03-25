﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.MoniterControl;

namespace FutsMoniter
{
    internal class HandlerWrapper
    {

        public HandlerWrapper(HandlerInfo info)
        {
            this.Handler = info;
            this.Control = (MoniterControl)info.Target;
        }

        public MoniterControl Control { get; private set; }
        public HandlerInfo Handler { get; private set; }

        public void HandlerResponse(string result,bool islast)
        {
            //执行调用
            this.Handler.MethodInfo.Invoke(this.Handler.Target, new object[] { result,islast });
        }

        public void HandeNotify(string result)
        {
            this.Handler.MethodInfo.Invoke(this.Handler.Target, new object[] { result });
        }

    }
    public class MoniterControlHelper
    {

        public static void RegisterControl(MoniterControl contrl)
        {
            //注册控件回调函数
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
        static void RegisterCallback(MoniterControl control)
        {
            Util.Info("register callback for control:" + control.ToString());
            List<HandlerInfo> handlers = FindHandler(control);
            foreach (HandlerInfo h in handlers)
            {
                RegisterCallback(h);
            }
        }

        static Dictionary<MoniterControl, List<HandlerWrapper>> responsemap = new Dictionary<MoniterControl, List<HandlerWrapper>>();
        static Dictionary<MoniterControl, List<HandlerWrapper>> notifymap = new Dictionary<MoniterControl, List<HandlerWrapper>>();
        
        /// <summary>
        /// 将MoniterControl的Handler封装后 注册到核心回调系统
        /// </summary>
        /// <param name="info"></param>
        static void RegisterCallback(HandlerInfo info)
        {
            HandlerWrapper wrapper = new HandlerWrapper(info);

            if (info.Attr.Type == QSEnumCallbackTypes.Response)
            {
                Globals.LogicEvent.RegisterCallback(info.Attr.Module, info.Attr.Cmd, wrapper.HandlerResponse);

                if (!responsemap.Keys.Contains(wrapper.Control))
                {
                    responsemap.Add(wrapper.Control, new List<HandlerWrapper>());
                }
                responsemap[wrapper.Control].Add(wrapper);
            }
            else if (info.Attr.Type == QSEnumCallbackTypes.Notify)
            {
                Globals.LogicEvent.RegisterNotifyCallback(info.Attr.Module, info.Attr.Cmd, wrapper.HandeNotify);
                if (!notifymap.Keys.Contains(wrapper.Control))
                {
                    notifymap.Add(wrapper.Control, new List<HandlerWrapper>());
                }
                notifymap[wrapper.Control].Add(wrapper);
            }
        }

        /// <summary>
        /// 将某个control的回调函数注销
        /// </summary>
        /// <param name="control"></param>
        static void UnRegisterCallback(MoniterControl control)
        {
            if (responsemap.Keys.Contains(control))
            {
                foreach (HandlerWrapper w in responsemap[control])
                {
                    Globals.LogicEvent.UnRegisterCallback(w.Handler.Attr.Module, w.Handler.Attr.Cmd, w.HandlerResponse);
                }

                responsemap.Remove(control);
            }
            if (notifymap.Keys.Contains(control))
            {
                foreach (HandlerWrapper w in notifymap[control])
                {
                    Globals.LogicEvent.UnRegisterNotifyCallback(w.Handler.Attr.Module, w.Handler.Attr.Cmd, w.HandeNotify);
                }
                notifymap.Remove(control);
            }
        }

    }
}