using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib
{
    public partial class RaceCentre
    {
        [ContribEventAttr("ContribDemo",
            "demoevent",//事件编码
            "demo event will void(string msg)","")]
        public event AccountIdDel DemoEvent;



        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange, //消息源
            "demo", //命令名称 操作码
            "demo - check the function is called", //帮助
            "we will use this arch do more complex stuff"//描述
            )]
        public void RaceDemo(ISession session)
        {
            Util.Debug("demo call is made");
            //向客户端发送消息
            //Send(session, "hello user");//race demo
            if (DemoEvent != null)
                DemoEvent(session.ContirbID + "|" + session.CMDStr);
        }
    }
}
