using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    [ContribAttr(ResponseServer.ContribName, "策略托管宿主服务", "用于运行服务端策略,执行程序化交易")]
    public partial class ResponseServer : ContribSrvObject, IContrib
    {
        const string ContribName = "ResponseHost";
        public ResponseServer()
            : base(ResponseServer.ContribName)
        { 
            
        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("ResponseServer loaded....", QSEnumDebugLevel.INFO);

            Tracker.ResponseTemplateTracker.LoadResponseTemplate();
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
