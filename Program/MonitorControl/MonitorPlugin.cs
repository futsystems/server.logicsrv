using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.MoniterControl
{
    public static class MoniterPlugin
    {
        static Dictionary<string, Type> typemap = new Dictionary<string, Type>();


        /// <summary>
        /// 创建指定类型名称的MoniterControl
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public static MonitorControl CreateMoniterControl(string typename)
        {
            if (!typemap.Keys.Contains(typename))
            {
                return null;
            }
            return (MonitorControl)Activator.CreateInstance(typemap[typename]);
        }
        static MoniterPlugin()
        {
            //从插件目录加载类型
            LoadMoniterTypes();
            Util.Debug("MoniterControl list:");
            foreach (string key in typemap.Keys)
            {
                Util.Debug("      " + key);
            }
        }
        static  void LoadMoniterTypes()
        {
            List<Type> monitertypes = new List<Type>();
            List<string> dllfilelist = new List<string>();
            //遍历搜索路径 获得所有dll文件
            string path = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "plugin" });
            dllfilelist.AddRange(Directory.GetFiles(path, "*.dll"));

            Dictionary<Type, bool> dictionary = new Dictionary<Type, bool>();
            foreach (string dllfile in dllfilelist)
            {
                Util.Debug("dll file:" + dllfile, QSEnumDebugLevel.INFO);
                var assembly = Assembly.ReflectionOnlyLoadFrom(dllfile);
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllfile);

                //加载所有依赖的dll 如果不存在则在插件目录下寻找
                foreach (var an in assembly.GetReferencedAssemblies())
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoad(an.FullName);
                    }
                    catch
                    {
                        Assembly.ReflectionOnlyLoadFrom(Path.Combine(Path.GetDirectoryName(dllfile), an.Name + ".dll"));
                    }
                }
                
                foreach (Type type in assembly.GetExportedTypes())
                {
                    Util.Debug("type:" + type.FullName);
                    //bool x = type.GetInterface(typeof(IMoniterControl).FullName) != null;
                    //程序集中的type不是抽象函数并且其实现了needType接口,则标记为有效
                    Type c = typeof(MonitorControl);
                    bool issub = typeof(IMoniterControl).IsAssignableFrom(type);
                    bool issub2 = typeof(MonitorControl).IsAssignableFrom(type);
                    bool issub3 = type.GetInterface(typeof(IMoniterControl).FullName) != null;
                    if (!type.IsAbstract && issub3)
                    {
                        Assembly a = Assembly.Load(assemblyName);
                        dictionary[a.GetType(type.FullName)] = true;//标记该类型被加载
                    }
                }
            }
            foreach (Type t in dictionary.Keys)
            {
                monitertypes.Add(t);
                typemap.Add(t.FullName, t);
            }
        }
    }
}
