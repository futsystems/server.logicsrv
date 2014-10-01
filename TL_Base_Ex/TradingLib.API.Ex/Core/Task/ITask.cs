using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface ITask
    {
        string UUID { get; }
        void CheckTask(DateTime signaltime);
        string TaskMemo { get; }

        string TaskName { get; }
        string TypeName { get; }
        string TimeStr { get; }
    }
}
