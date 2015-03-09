using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 服务管理接口
    /// 系统包含
    /// 1.核心服务管理
    /// 2.数据成交路由通道管理
    /// 3.扩展模块管理
    /// </summary>
    public interface IServiceManager
    {
        string ServiceMgrName { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
