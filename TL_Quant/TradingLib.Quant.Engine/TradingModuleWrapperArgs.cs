using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;



namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 定义了交易模块参数
    /// 
    /// </summary>
    public sealed class TradingModuleWrapperArgs
    {
        /// <summary>
        /// 系统运行中的成交接口
        /// </summary>
        public ServiceAppDomainFactory BrokerFactoryFactory { get; set; }
        /// <summary>
        /// 系统运行的数据读写组件
        /// </summary>
        public PluginSettings DataStoreSettings { get; set; }

        //public IGlobalAccess Acccess { get; set; }
        public Form MainForm { get; set; }
        /// <summary>
        /// 策略类名
        /// </summary>
        public string SystemClassName { get; set; }
        /// <summary>
        /// 策略文件
        /// </summary>
        public string SystemFilename { get; set; }


    }


}
