using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 通道管理接口
    /// 通道管理器 负责维护通道连接，加载所有行情和成交通道
    /// </summary>
    public interface IConnectorManager : IServiceManager, IRouterManager
    {

    }
}
