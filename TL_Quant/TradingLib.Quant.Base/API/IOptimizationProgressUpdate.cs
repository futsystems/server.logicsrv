using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public interface IOptimizationProgressUpdate
    {
        // Methods
        void UpdateProgress(List<OptimizationPlugin.ProgressItem> progressItems,int currentitem);
    }

    public delegate void UpdateProgressDel(List<OptimizationPlugin.ProgressItem> progressItems,int currentitem);
 

 

}
