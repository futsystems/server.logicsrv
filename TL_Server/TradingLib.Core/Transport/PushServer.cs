using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;

namespace TradingLib.Core
{
    public class PushServer
    {

        public PushServer(string address, int port)
        {
            _server = address;
            _port = port;
        }
        string _server = "127.0.0.1";
        int _port = 8870;


        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (processgo) return;
            processgo = true;
            processThread = new Thread(PubProcess);
            processThread.IsBackground = true;

            processThread.Start();

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (processgo)
            {
                processgo = false;
                int wait = 0;
                while (processThread.IsAlive && wait < 10)
                {
                    Thread.Sleep(1000);
                    wait++;
                }
                processThread.Abort();
                processThread = null;
            }
        }

        public void Push(string message)
        {
            statusbuffer.Write(message);
        }

        /// <summary>
        /// 将某个对象序列化成json字符串后发出
        /// </summary>
        /// <param name="obj"></param>
        public void Push(object obj)
        {
            statusbuffer.Write(TradingLib.Mixins.Json.JsonMapper.ToJson(obj));
        }

        RingBuffer<string> statusbuffer = new RingBuffer<string>(1000);

        Thread processThread = null;
        bool processgo = false;
        void PubProcess()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket socket = ctx.CreateSocket(SocketType.PUSH))
                {
                    string address = string.Format("tcp://{0}:{1}", _server, _port);
                    socket.Connect(address);
               

                    while (processgo)
                    {
                        while (statusbuffer.hasItems)
                        {
                            socket.Send(statusbuffer.Read(), Encoding.UTF8);
                        }
                        Thread.Sleep(50);
                    
                    }
                }
            }
        }
    }
}
