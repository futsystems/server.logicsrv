using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.Plugin;
using TradingLib.Quant;
using TradingLib.Quant.Common;
using TradingLib.API;

namespace TradingLib.Quant
{
    public static class QuantGlobals
    {
        public static string BuildNumber = "48";
        public static string UserAppDataPath = "";
        public static string DATAPATH = "D:\\QSData\\Data";//数据目录
        public static string CONFIGPATH = "config";
        public static string AppDataDirectory = "appdata";//程序生成的数据目录
        public static string ProjectConfigDirectory = "Projects";//策略配置数据存放目录
        public static string PluginDirectory = "plugins";//组件目录
        public static string StrategyDirectory = @"D:\QuantShop\TradingLib.Quant\TradingLib.Quant.Test\bin\Release\strategys";//策略文件夹
        public static string StrategyConfigFileName = "StrategySettings.xml";//策略StrategySetup xml
        public static string SYMBOLCONFIGFILENAME = "SymbolWatchListConfig.xml"; //


        public static string PriceChartName = "Price Pane";
        public static string VolumeChartName = "Volume Pane";
        public static string PluginCacheFile = "pluginCache";

        public static string PaperBroker = "PAPERBROKER";
        public static PluginManager PluginManager = null;
        public static PluginSettings DataStoreSetting = null;

        public static PluginSettings OptimizationSetting = null;

        public static DebugDelegate GDebug = null;
        //通过将MainForm进行全局设定，系统可以得全局对象 储存/服务等
        public static IGlobalAccess Access = null;
        //public static StrategyManager StrategyManager = null;



    }
}
