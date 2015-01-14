using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class CoreCommandAttr : ContribCommandAttr
    {
        /// <summary>
        /// 消息处理命令,用于标注核心模块的命令特性
        /// 然后暴露给命令行或者其他相关消息交换
        /// </summary>
        /// <param name="source">处理消息来源</param>
        /// <param name="cmd">命令操作码 标识了该命令</param>
        /// <param name="help">帮助</param>
        /// <param name="description">描述</param>
        public CoreCommandAttr(QSEnumCommandSource source, string cmd, string help, string description)
            : base(source, cmd, help, description)
        {

        }
    }
}
