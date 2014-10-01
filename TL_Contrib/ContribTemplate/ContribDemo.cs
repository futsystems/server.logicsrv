using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace ContribTemplate
{
    [ContribAttr("ContribDemo","演示扩展模块","用于演示如何构建一个可以被系统调用的扩展模块")]
    public class ContribDemo:ContribSrvObject,IContrib
    {
        public ContribDemo()
            : base("ContribDemo")
        {  }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad(){}
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory(){}
        /// <summary>
        /// 启动
        /// </summary>
        public void Start(){}

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop(){}


        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "demorequest",
            "demorequest - make a demo contrib command request",
            "make a demo contrib command request")]
        public void DemoContribRequest(ISession session, string name, decimal amount)
        {
            TLCtxHelper.Debug("democontribrequest is called");
            //Send(session, new { Name = name, Amount = amount });
        }
        [ContribCommandAttr(
            "Race",//扩展模块标识
            "demoevent",//事件名称
            "demoeventhandler" ,//操作标识
            "demoeventhandler - check the function is called",//帮助
            "we will use this arch do more complex stuff"//说明
            )]
        public void demoeventhandler(string message)
        {
            TLCtxHelper.Debug("EventHandler:"+message);
            //Send(session, "contrib response");
        }

    }
}
