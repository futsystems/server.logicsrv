using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 插件基础接口
    /// 系统其他插件需要继承该接口并扩展不同插件所需要实现的功能
    /// 系统可以从IXXXXPlugin通过PluginHelper生成对应的对象,并通过IPlugin所提供的信息进行加载使其正常工作
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 姓名
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 公司
        /// </summary>
        string Compnay { get; }

        /// <summary>
        /// 类全名
        /// </summary>
        string Id { get; }
    }
}
