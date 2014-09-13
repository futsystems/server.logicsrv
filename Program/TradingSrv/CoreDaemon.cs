using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using TradingLib.LitJson;
using System.Threading;

namespace TraddingSrvCLI
{
    public enum QSEnumCoreThreadStatus
    { 
        Started,
        Starting,
        Stopping,
        Stopped,
    }
    internal struct CoreThreadStatus
    {
        public string Status;
        public CoreThreadStatus(QSEnumCoreThreadStatus status)
        {
            Status = status.ToString();
        }
    }
    internal class CoreDaemon
    {

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        int _port = 4080;

        CoreThread corethread = null;
        public CoreDaemon()
        {
            corethread = new CoreThread();
            corethread.SendDebugEvent +=new DebugDelegate(debug);
        }

        public void Start()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket rep = ctx.CreateSocket(SocketType.REP))
                {
                    rep.Bind("tcp://*:" + _port.ToString());

                    rep.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            string str = rep.Receive(Encoding.UTF8);
                            debug("web taks message is:" + str);
                            string re = HandleWebTask(str);
                            rep.Send(re, Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            debug("deal wektask error:" + ex.ToString());
                        }

                    };
                    var poller = new Poller(new List<ZmqSocket> { rep });
                    //让线程一直获取由socket发报过来的信息
                    corethread.Start();
                    while (true)
                    {
                        try
                        {
                            poller.Poll();
                        }
                        catch (ZmqException e)
                        {
                            debug("%%%%main server message error" + e.ToString());
                        }
                    }
                }
            }
        }

        public string HandleWebTask(string msg)
        {

            debug("handle...........");
            TradingLib.Mixins.LitJson.JsonData request = TradingLib.Mixins.JsonRequest.ToObject(msg);
            string method = request["Method"].ToString().ToUpper();
            switch (method)
            { 
                case "START":
                    if (corethread.Status == QSEnumCoreThreadStatus.Stopped)
                    {
                        corethread.Start();
                        return JsonReply.GenericSuccess(ReplyType.Success, "启动核心服务成功").ToJson();
                    }
                    else
                    {
                        return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于停止状态").ToJson();
                    }
                case "STOP":
                    if (corethread.Status == QSEnumCoreThreadStatus.Started)
                    {
                        new Thread(corethread.Stop).Start();//放入后台线程进行执行
                        //corethread.Stop();
                        return JsonReply.GenericSuccess(ReplyType.Success, "停止核心服务成功").ToJson();
                    }
                    else
                    {
                        return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于运行状态状态").ToJson();
                    }
                case "STATUS":
                    return new TradingLib.Mixins.ReplyWriter().Start().FillReply(TradingLib.Mixins.JsonReply.GenericSuccess("")).Fill(new CoreThreadStatus(corethread.Status),"Playload").End().ToString();
                default:
                    return JsonReply.GenericError(ReplyType.Error, "未支持命令").ToJson();
            }
        }


        
        

    }
}
