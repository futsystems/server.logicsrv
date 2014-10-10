using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using ZeroMQ;

namespace TradingLib.Mixins
{
    /// <summary>
    /// 状态服务
    /// 用于连接到对应的中心监控节点,汇报服务节点的配置状态和服务状态
    /// 采用的是定时汇报,汇报格式采用Json格式
    /// </summary>
    public class StatusServer
    {

        public event StatusDelegate GetStatusEvent;
        

        string nodekeeperaddress = "nodekeeper.huiky.com";
        int port = 10001;
        int freq = 5;
        public StatusServer(string address = "nodekeeper.huiky.com", int keeperport = 10001,int statusfreq=5)
        {
            nodekeeperaddress = address;
            port = keeperport;
            freq = statusfreq;
        }

        bool statgo = false;

        Thread statusthread = null;
        void ProcessStatus()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            { 
                using(ZmqSocket socket = ctx.CreateSocket(SocketType.PUB))
                {
                    string add = "tcp://" + nodekeeperaddress + ":" + port.ToString();
                    socket.Connect(add);
                    while (statgo)
                    {
                        if (GetStatusEvent != null)
                        {
                            string status = GetStatusEvent();
                            socket.Send(status, Encoding.UTF8);
                        }
                        if (!statgo)
                        {
                            socket.Close();
                        }
                        Thread.Sleep(freq*1000);
                    }
                    
                
                }
            }
        }

        public void Start()
        {
            if (statgo) return;
            statgo = true;
            statusthread = new Thread(ProcessStatus);
            statusthread.IsBackground = true;
            statusthread.Start();
        }

        public void Stop()
        {
            if (!statgo) return;
            statgo = false;
            int wait = 0;
            while (statusthread.IsAlive && wait < 10)
            {
                Thread.Sleep(1000);
            }
            statusthread.Abort();
            statusthread = null;
        }
    }
}
