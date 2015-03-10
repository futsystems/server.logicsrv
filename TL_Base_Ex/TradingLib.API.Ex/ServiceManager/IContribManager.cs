using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 扩展模块管理接口
    /// 负责加载所有扩展模块
    /// </summary>
    public interface IContribManager : IServiceManager, IDisposable
    {
        void Load();
    }
}
