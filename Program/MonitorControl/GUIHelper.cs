using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TradingLib.GUI
{
    public class GUIHelper
    {
        /// <summary>
        /// 获得某个对象的Callback函数列表
        /// 需要用CallbackAttr标注
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<HandlerInfo> FindHandler(object obj)
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

    }
}
