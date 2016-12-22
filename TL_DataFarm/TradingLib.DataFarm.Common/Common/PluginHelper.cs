using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;

namespace TradingLib.DataFarm.Common
{
    public class Plugin
    {
        /// <summary>
        /// 从某个目录加载某个类型的插件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<T> LoadPlugins<T>(string path, string filter = "*")
        {
            Type type = typeof(T);
            List<T> list = new List<T>();

            string[] aDLLs = null;

            try
            {
                aDLLs = Directory.GetFiles(path, "*.dll");
            }
            catch (Exception ex)
            {
                //logger.Error(string.Format("Load {0} Error:{1}", typeof(T), ex));
            }
            if (aDLLs.Length == 0)
                return new List<T>();

            foreach (string item in aDLLs)
            {
                Assembly aDLL = Assembly.UnsafeLoadFrom(item);
                Type[] types = aDLL.GetTypes();

                foreach (Type t in types)
                {
                    try
                    {
                        //connection service must support IDataServerServiceHost interface
                        if (t.GetInterface(type.FullName) != null)
                        {
                            if (filter == "*" || filter == t.FullName)
                            {
                                T o = (T)Activator.CreateInstance(t);
                                list.Add(o);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //logger.Error("Load plugin error:" + ex.ToString());
                    }
                }
            }
            return list;
        }
    }
}
