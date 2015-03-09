using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 核心模块管理接口
    /// 负责加载所有核心模块
    /// </summary>
    public interface ICoreManager : IServiceManager, IDisposable
    {

        void WireCtxEvent();
    }
}
