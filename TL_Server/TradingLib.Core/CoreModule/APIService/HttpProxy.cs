using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Common.Logging;
using ReverseProxy;

namespace TradingLib.Core
{

    public class HttpProxy
    {
        System.Net.HttpListener _proxyListener = null;
        ILog logger = LogManager.GetLogger("HttpProxy");

        ReverseProxy.ReverseProxy proxy = null;

        string remoteIp = "127.0.0.1";
        int localPort = 80;
        public HttpProxy(int port,string ip)
        {
            remoteIp = ip;
            localPort = port;
        }

        bool _go = false;
        void Process()
        {
            proxy = new ReverseProxy.ReverseProxy();
            proxy.AddForwarder(localPort, new IPEndPoint[] { new IPEndPoint(IPAddress.Parse(remoteIp), 80) });
        }
        public void Start()
        {
            _go = true;
            new Thread(Process).Start();
        }
    }

}
