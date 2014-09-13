using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Globalization;


namespace TradingLib.Quant.Base
{
    public interface IOptimizationServices
    {
        
        // Methods
        void AbortOptimization();//退出优化
        Form CreateDefaultProgressWindow(Action cancelCallback);//创建进度窗体
        OptimizationResult RunSystem(StrategyRunSettings runSettings, StrategyProgressUpdate progressCallback);//运行策略并得到优化结果
        bool ShowDefaultOptimizationSettingsDialog(IWin32Window owner);//显示设定窗口
        void UpdateProgress(List<OptimizationPlugin.ProgressItem> progressItems,int currentitem);//更新进度
        void SetTotalRunNumber(int totalrun);
        // Properties
        List<StrategyParameterInfo> OptimizationParameters { get; }//优化参数
    }

    

 

 

 

}
