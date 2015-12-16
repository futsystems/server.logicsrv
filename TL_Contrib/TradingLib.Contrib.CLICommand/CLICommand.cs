using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.CLICommand
{
    [ContribAttr("CommandCabinet.ContribName", "命令行工具箱", "用于提telnet命令操作,内核部分只包含部分必须用的命令行,该模块扩展了命令集")]
    public partial class CommandCabinet : ContribSrvObject, IContrib
    {
        const string ContribName = "CommandCabinet";
        public CommandCabinet()
            : base(CommandCabinet.ContribName)
        { }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad() 
        {
            logger.Info("CommandCabinet loaded....");
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() { }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start() { }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() { }


        

    }
}
