using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using NetMQ;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public class LogServer
    {


        public LogServer()
        { 
        
        }

        public void NeLog(string msg)
        {
            logcache.Write(msg);
        }
        RingBuffer<string> logcache = new RingBuffer<string>(1000);

        bool _loggo = false;
        Thread logthread = null;
        void LogProcess()
        {
            using (var ctx = NetMQContext.Create())
            {
                using (var pub = ctx.CreatePublisherSocket())
                {
                    pub.Bind("tcp://*:4469");
                    while (_loggo)
                    {
                        while (logcache.hasItems)
                        {
                            string msg = logcache.Read();
                            pub.Send(msg, Encoding.UTF8, NetMQ.zmq.SendReceiveOptions.None);
                        }
                        Util.sleep(10);
                    }
                }

            }
        }

        public void Start()
        {
            if (_loggo) return;
            _loggo = true;
            logthread = new Thread(LogProcess);
            logthread.IsBackground = true;
            logthread.Start();
        }

        public void Stop()
        { 
            
        }
    }
}
