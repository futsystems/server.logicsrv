using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace WebGate
{

    [ContribAttr("WebGate", "WebGate服务", "WebGate服务用于提供web端的业务调用,新增帐号,设置等")]
    public class WebAPIGate:BaseSrvObject,IContrib
    {

        WebAPIServer _srv = null;
        WebRequestHandler _handler = null;
        public WebAPIGate()
            : base("WebGate")
        { 
            
        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("WebGate 进行初始化......",QSEnumDebugLevel.INFO);
            //1.加载配置文件
            ConfigFile cfg = ConfigFile.GetConfigFile("contribcfg/wegate.cfg");
            string srvadd = cfg["WebGateAddress"].AsString();
            int srvport = cfg["WebGatePort"].AsInt();

            _srv = new WebAPIServer(srvadd, srvport);

            _handler = new WebRequestHandler();

            _srv.GotWebTaskEvent += new WebTaskDel(_handler.HandleRequest);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("WebGate 启动......", QSEnumDebugLevel.INFO);
            _srv.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

    }
}
