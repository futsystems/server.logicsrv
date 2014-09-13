using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using System.Windows.Forms;
using TradingLib.Quant.Loader;
using TradingLib.Quant.GUI;

namespace TradingLib.Quant.Common
{
    /// <summary>
    /// 将全局的一些对象放入MainForm中，quant.组件中有一些需要放在dll中，则需要有一个全局访问接口来规范访问接口
    /// </summary>
    public interface IGlobalAccess
    {
        Form GetMainForm();
        /// <summary>
        /// 获得全局数据读写对象
        /// </summary>
        /// <returns></returns>
        IDataStore GetDataSore();
        /// <summary>
        /// 获得全局服务管理对象
        /// </summary>
        /// <returns></returns>
        ServiceManager GetServiceManager();
        /// <summary>
        /// 获得全局属性显示窗口
        /// </summary>
        /// <returns></returns>
        fmPropertiesWindow GetPropertiesWindow();
        /// <summary>
        /// 获得全局策略管理对象
        /// </summary>
        /// <returns></returns>
        StrategyManager GetStrategyManager();

        /// <summary>
        /// 创建某个合约Chart图表
        /// </summary>
        /// <param name="symbolFreq"></param>
        /// <returns></returns>
        IChartDisplay CreateChartInstance(SecurityFreq symbolFreq);

        /// <summary>
        /// 显示某个合约 某个数据集
        /// </summary>
        /// <param name="symbolFreq"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        IChartDisplay CreateChartInstance(SecurityFreq symbolFreq, IBarData bars);

        /// <summary>
        /// 获得全局数据读写对象
        /// </summary>
        /// <returns></returns>
        IDataStore GetBarDataStoragePlugin();

        /// <summary>
        /// 显示Bar数据
        /// </summary>
        /// <param name="sf"></param>
        void ShowBarData(SecurityFreq sf);
        /// <summary>
        /// 显示Tick数据
        /// </summary>
        /// <param name="symbol"></param>
        void ShowTickData(Security symbol);

    }
}
