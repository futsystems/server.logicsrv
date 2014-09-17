using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;



namespace TradingLib.Common
{
    /// <summary>
    /// 单例对写
    /// </summary>
    public class PluginHelper
    {
        private static PluginHelper DefaultInstance;
        private PluginFinderWrapper wrapper;



        static PluginHelper()
        {
            DefaultInstance = new PluginHelper();
        }
        public PluginHelper()
        {
            this.wrapper = new PluginFinderWrapper();
        }



        public static PluginFinderWrapper GetWrapper()
        {
            if (DefaultInstance.wrapper == null)
                DefaultInstance.wrapper = new PluginFinderWrapper();
            return DefaultInstance.wrapper;
        }


        /// <summary>
        /// 按类名(全称)获得某个扩展模块插件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IContribPlugin LoadContribPlugin(string id)
        {
            return GetWrapper().LoadContribPlugin(id);
        }

        /// <summary>
        /// 获得所有扩展模块插件
        /// </summary>
        /// <returns></returns>
        public static List<IContribPlugin> LoadContribList()
        {
            return GetWrapper().LoadContribList();
        }

        /// <summary>
        /// 根据某个类名全称 生成扩展模块对象
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IContrib ConstructContrib(string className)
        {
            return GetWrapper().ConstructContrib(className);
        }

        /// <summary>
        /// 获得扩展命令列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<ContribCommandInfo> FindContribCommand(object obj)
        {
            return GetWrapper().FindContribCommand(obj);
        }

        /// <summary>
        /// 获得扩展模块的事件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<ContribEventInfo> FindContribEvent(object obj)
        {
            return GetWrapper().FindContribEvent(obj);
        }

        /// <summary>
        /// 获得扩展任务列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<TaskInfo> FindContribTask(object obj)
        {
            return GetWrapper().FindContribTask(obj);
        }


        public static List<PropertyInfo> FindProperty<T>(object obj) where T : Attribute
        {
            return GetWrapper().FindProperty<T>(obj);
        }

        public static List<PropertyInfo> FindProperty<T>(Type type) where T : Attribute
        {
            return GetWrapper().FindProperty<T>(type);
        }


        /// <summary>
        /// 获得某个方法的参数列表
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static List<MethodArgument> GetArgumentList(MethodInfo mi)
        {
            return GetWrapper().GetArgumentList(mi);
        }

        /// <summary>
        /// 将参数列表中的值转换成函数参数object[]供函数调用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object[] ParseMethodArgs(List<MethodArgument> args)
        {
            return GetWrapper().ParseMethodArgs(args);
        }


        /// <summary>
        /// 获得所有行情接口类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> LoadDataFeedType()
        {
            return GetWrapper().LoadDataFeedType();
        }

        /// <summary>
        /// 获得所有成交接口类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> LoadBrokerType()
        {
            return GetWrapper().LoadBrokerType();
        }


        public static List<Type> LoadAccountType()
        {
            return GetWrapper().LoadAccountType();
        }


        public static List<Type> LoadAccountRule()
        {
            return GetWrapper().LoadAccountRule();
        }

        public static List<Type> LoadOrderRule()
        {
            return GetWrapper().LoadOrderRule();
        }

        public static IList<Type> GetImplementors(string path, Type needtype)
        {
            return GetWrapper().GetImplementors(path, needtype);
        }
        public static void Release()
        {
            DefaultInstance.wrapper = null;
            
        }
    }
}
