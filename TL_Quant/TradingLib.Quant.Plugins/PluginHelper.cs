using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;

using TradingLib.Quant;

using TradingLib.Quant.Base;
namespace TradingLib.Quant.Plugin
{
    /// <summary>
    /// 单例函数
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
            return DefaultInstance.wrapper;
        }
        /// <summary>
        /// 构造指标实例
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ISeries ConstructIndicator(string indicatorClassName, List<ConstructorArgument> args)
        {
            return ConstructIndicator(indicatorClassName, args, true);
        }
        public static ISeries ConstructIndicator(string indicatorClassName, List<ConstructorArgument> args, bool createWrapper)
        {
            return GetWrapper().ConstructIndicator(indicatorClassName, args, createWrapper);
        }

        /// <summary>
        /// 加载indicatorplugin
        /// </summary>
        /// <returns></returns>
        public static List<IIndicatorPlugin> LoadIndicatorList()
        {
            return GetWrapper().LoadIndicatorList();
        }

        /// <summary>
        /// 获得指标构造参数
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <returns></returns>
        public static List<ConstructorArgument> GetIndicatorArgumentList(string indicatorClassName)
        {
            return GetWrapper().GetIndicatorArgumentList(indicatorClassName);
        }

        /// <summary>
        /// 获得指标输入参数
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <returns></returns>
        public static List<SeriesInputAttribute> GetIndicatorInputList(string indicatorClassName)
        {
            return GetWrapper().GetIndicatorInputList(indicatorClassName);
        }
        /// <summary>
        /// 加载某个indicator
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static IIndicatorPlugin LoadIndicatorPlugin(string Id)
        {
            return GetWrapper().LoadIndicatorPlugin(Id);
        }


        /// <summary>
        /// 加载回测报告插件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BackTestReportAttribute LoadBackTestReport(string id)
        {
            return GetWrapper().LoadBackTestReport(id);
        }


 

 




        public static string pluginpath()
        {
            return GetWrapper().PluginFullPath;
        }

        public static string GetFullPath(string path)
        {
            //如果以\开始则加入Application.StartupPath
            if (!path.StartsWith(@"\") && (path.IndexOf(':') < 0))
            {
                path = Application.StartupPath+ "\\"+ path;
            }
            //检查是否有\结尾
            if (!path.EndsWith(@"\"))
            {
                path = path + @"\";
            }
            return path;
        }
        public static string GetFullFileName(string localfilename)
        {
            //如果以\开始则加入Application.StartupPath
            if (!localfilename.StartsWith(@"\") && (localfilename.IndexOf(':') < 0))
            {
                localfilename = Application.StartupPath + "\\" + localfilename;
            }

            return localfilename;
        }

        

 


    }
}
