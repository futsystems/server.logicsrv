using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.TNetStrings;


namespace Message.Broadcast
{
    public partial class BroadcastClient
    {
        RingBuffer<string> messagebuf = new RingBuffer<string>(1000);

        [TaskAttr("每2秒执行消息推送检查", 2, "消息推送检查")]
        public void CTE_SendMessage()
        {
            while (messagebuf.hasItems)
            {
                string message = messagebuf.Read();
                string addfilter = "ALL";
                string msgsend = "";
                //调用内层访问接口推送消息
                //debug("got message:" + message, QSEnumDebugLevel.INFO);

                //1.转换成byte
                byte[] bytemsg = Encoding.UTF8.GetBytes(message);
                ArraySegment<byte> codemsg = new ArraySegment<byte>(bytemsg);

                //2.解析netstring
                List<string> arglist = new List<string>();
                var res = codemsg.TParse();
                arglist.Add(res.Data.ToString());
                while (res.Remain.Count > 0)
                {
                    res = res.Remain.TParse();
                    arglist.Add(res.Data.ToString());
                }

                //3.获取我们需要的参数
                if (arglist.Count == 4)
                {
                    addfilter = arglist[1];
                    msgsend = arglist[3];
                }

                debug("addfilter:" + addfilter + " message:" + msgsend,QSEnumDebugLevel.INFO);
                
                
                //this.Broadcast(msgsend, MessageTypes.NOTIFYMESSAGE, addfilter);
            }
        
        }

       
    }
}
