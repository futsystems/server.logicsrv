using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 外围功能模块插件,用于保存该插件的相关信息,从而可以通过插件系统进行创建，加载该插件
    /// </summary>
    public interface IContribPlugin : IPlugin
    {
        string ContribClassName { get; }

        string ContribID { get; }
    }
}
