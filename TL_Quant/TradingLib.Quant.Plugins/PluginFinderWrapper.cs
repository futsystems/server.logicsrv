using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TradingLib.API;
using TradingLib.Quant.Base;
namespace TradingLib.Quant.Plugin
{
    public class PluginFinderWrapper
    {

        public static string PluginDirectory = "plugins";//QuantGlobals.PluginDirectory;
        public PluginFinder finder;
        private AppDomain finderDomain;
        private bool _createDomain = false;

        string _fullpath = string.Empty;
        public string PluginFullPath
        {
            get {
                
                CreateFinder();
                return _fullpath;
            }
        }
        private void CreateFinder()
        {
            if (this.finder == null)
            {
                if (PluginDirectory == null)
                {
                    throw new QSQuantError("Plugin directory not set");
                }
                string fullPath = PluginHelper.GetFullPath(PluginDirectory);
                _fullpath = fullPath;
                if (this._createDomain)
                {
                    AppDomainSetup info = new AppDomainSetup
                    {
                        ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                        PrivateBinPath = fullPath
                    };
                    string text1 = "App base:    " + info.ApplicationBase + "\r\nPrivate bin: " + info.PrivateBinPath;
                    this.finderDomain = AppDomain.CreateDomain("Plugin Finder AppDomain", null, info);
                    this.finder = (PluginFinder)this.finderDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginFinder).FullName);
                    this.finder.SetSearchPath(fullPath);
                }
                else
                {
                    this.finder = new PluginFinder(false);
                    this.finder.SetSearchPath(fullPath);
                }
            }
        }

        /// <summary>
        /// 创建indicator 实例
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <param name="args"></param>
        /// <param name="createWrapper"></param>
        /// <returns></returns>
        public ISeries ConstructIndicator(string indicatorClassName, List<ConstructorArgument> args, bool createWrapper)
        {
            this.CreateFinder();
            return this.finder.ConstructIndicator(indicatorClassName, args, createWrapper);
        }

        /// <summary>
        /// 加载指标列表
        /// </summary>
        /// <returns></returns>
        public List<IIndicatorPlugin> LoadIndicatorList()
        {
            this.CreateFinder();
            return this.finder.LoadIndicatorList();
        }


        /// <summary>
        /// 获得指标参数
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <returns></returns>
        public List<ConstructorArgument> GetIndicatorArgumentList(string indicatorClassName)
        {
            this.CreateFinder();
            return this.finder.GetArgumentList(typeof(ISeries), indicatorClassName);
        }
        /// <summary>
        /// 获得指标输入序列
        /// </summary>
        /// <param name="indicatorClassName"></param>
        /// <returns></returns>
        public List<SeriesInputAttribute> GetIndicatorInputList(string indicatorClassName)
        {
            this.CreateFinder();
            return this.finder.GetSeriesInputs(typeof(ISeries), indicatorClassName);
        }

        /// <summary>
        /// 记载某个指标
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IIndicatorPlugin LoadIndicatorPlugin(string Id)
        {
            this.CreateFinder();
            return this.finder.LoadIndicatorPlugin(Id);
        }

        /// <summary>
        /// 加载某个id的回测报告插件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BackTestReportAttribute LoadBackTestReport(string id)
        {
            return this.finder.LoadBackTestPlugin(id);
        }

 

        

 

 

 


 


 

 

    }
}
