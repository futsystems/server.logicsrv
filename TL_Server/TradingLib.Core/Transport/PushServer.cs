using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using ZeroMQ;

namespace TradingLib.Core
{
    public class PushServer
    {

        ILog logger = LogManager.GetLogger("PushServer");
        /// <summary>
        /// 远程推送服务
        /// 用于向远程服务器推送状态信息 中心服务器采集所有运行系统信息
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
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
            using (ZContext ctx = new ZContext())
            {
                using (ZSocket socket = new ZSocket(ctx,ZSocketType.PUSH))
                {
                    string address = string.Format("tcp://{0}:{1}", _server, _port);
                    socket.Connect(address);


                    while (processgo)
                    {
                        while (statusbuffer.hasItems)
                        {
                            try
                            {
                                SendMessage(socket, statusbuffer.Read());
                            }
                            catch (ZException ex)
                            {
                                logger.Error("ZException:" + ex.ToString());
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Exception:" + ex.ToString());
                            }
                        }
                        Thread.Sleep(50);

                    }
                }
            }
        }

        void SendMessage(ZSocket socket,string msg)
        {
            using (ZMessage zmsg = new ZMessage())
            {
                ZError error;
                zmsg.Add(new ZFrame(msg, Encoding.UTF8));

                if (!socket.Send(zmsg, out error))
                {
                    if (error == ZError.ETERM)
                    {
                        logger.Error("got ZError.ETERM,return directly");
                        return;	// Interrupted
                    }
                    throw new ZException(error);
                }
            }
            
        }
    }
}