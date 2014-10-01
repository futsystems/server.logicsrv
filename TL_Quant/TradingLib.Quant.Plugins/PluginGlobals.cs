using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Plugin
{
    public static class PluginGlobals
    {
        public static string UserAppDataPath = "";
        public static string PluginDirectory = "plugins";//组件目录
        public static PluginManager PluginManager = null;
        public static PluginSettings DataStoreSetting = null;

        public static PluginSettings OptimizationSetting = null;
    }
}
