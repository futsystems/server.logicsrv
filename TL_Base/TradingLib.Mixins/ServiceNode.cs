using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Mixins
{

    public class ServiceNode : IServiceNode
    {
        /// <summary>
        /// 服务节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 服务节点编号
        /// </summary>
        public string NodeID { get; set; }

        /// <summary>
        /// 服务节点UUID
        /// </summary>
        public string NodeUUID { get; set; }

        /// <summary>
        /// 服务节点IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 当前节点时间
        /// </summary>
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// 运行操作系统名称
        /// </summary>
        public string OS { get; set; }

        /// <summary>
        /// 上次启动时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 当前运行配置
        /// </summary>
        public string Config { get; set; }


        /// <summary>
        /// 运行时扩展状态
        /// </summary>
        public object Status { get; set; }
    }
    /// <summary>
    /// 服务节点接口,用于指定实现,用于汇报服务节点的状态
    /// </summary>
    public interface IServiceNode
    {
        /// <summary>
        /// 服务节点名称
        /// </summary>
        string NodeName { get;}

        /// <summary>
        /// 服务节点编号
        /// </summary>
        string NodeID { get; }

        /// <summary>
        /// 服务节点UUID
        /// </summary>
        string NodeUUID { get;}

        /// <summary>
        /// 服务节点IP地址
        /// </summary>
        string IPAddress { get;}

        /// <summary>
        /// 当前节点时间
        /// </summary>
        DateTime CurrentTime { get; }

        /// <summary>
        /// 运行操作系统名称
        /// </summary>
        string OS { get; }

        /// <summary>
        /// 上次启动时间
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 当前运行配置
        /// </summary>
        string Config { get; }


        /// <summary>
        /// 运行时扩展状态
        /// </summary>
        object Status { get; }

    }
}
