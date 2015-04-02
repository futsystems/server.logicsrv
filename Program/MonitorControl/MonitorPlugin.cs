using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using System.Windows.Forms;

namespace TradingLib.MoniterControl
{
    public static class MoniterPlugin
    {

        /// <summary>
        /// 获得某个对象的MoniterControlAttr标注
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MoniterControlAttr GetMoniterControlAttr(Type type)
        {
            return (MoniterControlAttr)Attribute.GetCustomAttribute(type, typeof(MoniterControlAttr));
        }



        static Dictionary<string, Type> typemap = new Dictionary<string, Type>();


        /// <summary>
        /// 菜单类型map 用于实现UUID和对应菜单对象的映射
        /// </summary>
        static Dictionary<string, Type> commandMap = new Dictionary<string, Type>();
        /// <summary>
        /// 对应的特性映射
        /// </summary>
        static Dictionary<string, MoniterCommandAttr> commandAttrMap = new Dictionary<string, MoniterCommandAttr>();

        /// <summary>
        /// 判断某个UUID对应的菜单是否存在
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        static bool IsExistCommand(string uuid)
        {
            if (commandMap.Keys.Contains(uuid)) return true;
            return false;
        }

        /// <summary>
        /// 获得所有MoniterControl类型
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetMoniterControlTypes()
        {
            return typemap.Values;
        }
        /// <summary>
        /// 获得某个类型的控件
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public static Type GetMoniterControl(string typename)
        {
            if (!typemap.Keys.Contains(typename))
            {
                return null;
            }
            return typemap[typename];
        }
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

        /// <summary>
        /// 创建管理端菜单对象
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static MoniterCommand CreateMoniterCommand(string uuid)
        {
            if (!commandMap.Keys.Contains(uuid))
            {
                return null;
            }
            return (MoniterCommand)Activator.CreateInstance(commandMap[uuid]);
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

            foreach (string key in commandMap.Keys)
            {
                Util.Debug("       " + key + " " + commandMap[key].FullName);
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
                //直接加载该dll并获得程序集 这里不能使用LoadFile LoadFile会强制在目录下加载所引用的程序集 LoadFrom则会按照一定策略去加载程序集 会通过在内存中加载依赖项
                Assembly a = Assembly.LoadFrom(dllfile); 
                try
                {
                    foreach (Type type in a.GetExportedTypes())
                    {
                        Util.Debug("type:" + type.FullName);
                       
                        //获得所有导出类型并判断是否继承了对应的接口
                        if (!type.IsAbstract && type.GetInterface(typeof(IMoniterControl).FullName) != null)
                        {
                            dictionary[a.GetType(type.FullName)] = true;//标记该类型被加载
                        }

                        //判断是否是管理端命令对象
                        if (!type.IsAbstract && type.GetInterface(typeof(IMonterCommand).FullName) != null)
                        {

                            //dictionary[a.GetType(type.FullName)] = true;//标记该类型被加载
                            Type target = a.GetType(type.FullName);
                            MoniterCommandAttr attr = (MoniterCommandAttr)Attribute.GetCustomAttribute(target, typeof(MoniterCommandAttr));
                            if (attr != null && !IsExistCommand(attr.UUID))
                            {
                                commandMap.Add(attr.UUID, target);
                                commandAttrMap.Add(attr.UUID, attr);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.Debug("load plugin error:" + ex.ToString());
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
