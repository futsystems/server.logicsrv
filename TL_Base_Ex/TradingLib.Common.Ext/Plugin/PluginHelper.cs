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
    public class PluginHelper:IDisposable
    {
        private static PluginHelper DefaultInstance;
        private PluginFinderWrapper wrapper;



        static PluginHelper()
        {
            DefaultInstance = new PluginHelper();
        }
        private PluginHelper()
        {
            this.wrapper = new PluginFinderWrapper();
        }

        public void Dispose()
        { 
            
        }

        public static void DisposeInstance()
        {
            //if (DefaultInstance != null)
            //{
            //    DefaultInstance.wrapper = null;
            //    DefaultInstance.Dispose();
            //    DefaultInstance = null;
            //}
        }

        static PluginFinderWrapper GetWrapper()
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
        public static IContribPlugin LoadContribPlugin(string classname)
        {
            return GetWrapper().LoadContribPlugin(classname);
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

        /// <summary>
        /// 获得帐户类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> LoadAccountType()
        {
            return GetWrapper().LoadAccountType();
        }

        /// <summary>
        /// 获得帐户风控规则类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> LoadAccountRule()
        {
            return GetWrapper().LoadAccountRule();
        }

        /// <summary>
        /// 获得委托风控规则类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> LoadOrderRule()
        {
            return GetWrapper().LoadOrderRule();
        }

        /// <summary>
        /// 从某个文件夹加载实现某个接口的类型
        /// 比如在配资扩展模块中 有多个配资服务计划,且实现了统一的接口，则需要自定义加载该类型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="needtype"></param>
        /// <returns></returns>
        public static IList<Type> GetImplementors(string path, Type needtype)
        {
            return GetWrapper().GetImplementors(path, needtype);
        }

    }
}
