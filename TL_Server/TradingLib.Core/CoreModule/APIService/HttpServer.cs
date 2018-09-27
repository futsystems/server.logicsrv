﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

using NHttp;
using TradingLib.API;
using TradingLib.Common;
using DotLiquid;
using Common.Logging;

namespace TradingLib.Contrib.APIService
{
    



    public class HttpServer
    {
        ILog logger = LogManager.GetLogger("HttpServer");

        Dictionary<string, RequestHandler> handlerMap = new Dictionary<string, RequestHandler>();
        int _port = 8080;
        string _address = "127.0.0.1";

        NHttp.HttpServer _server = null;

        public HttpServer(string address,int port)
        {
            _address = address;
            _port = port;

            //初始化Handler
            RequestHandler tmp= null;
            tmp = new CashHandler();
            handlerMap.Add(tmp.Module, tmp);
            tmp = new APIHandler();
            handlerMap.Add(tmp.Module, tmp);

        }

        public void Start()
        {
            _server = new NHttp.HttpServer();
            _server.EndPoint = new IPEndPoint(IPAddress.Any, _port);
            _server.RequestReceived += (s, e) =>
            {
                HandleHttpRequest(e);
            };
            _server.Start();
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
            }
        }

        void HandleHttpRequest(HttpRequestEventArgs arg)
        {
            using (var writer = new StreamWriter(arg.Response.OutputStream))
            {
                //Console.WriteLine(string.Format("HttpMethod:{0} RawUrl:{1} Url:{2}", arg.Request.HttpMethod, arg.Request.RawUrl, arg.Request.Url));
                object ret = HandleRequest(arg.Request,arg.Response);
                writer.Write(ret is string?ret:ret.SerializeObject()); 
            }
        }

        object HandleRequest(HttpRequest request,HttpResponse response)
        {
            if (request.RawUrl == "/favicon.ico")
            {
                //logger.Info("ssssssssssssssss");
            }
            //response.StatusCode = 404;
            //response.StatusDescription = "xxxxxxxxxxx";
            //return "33";
            string[] path = request.RawUrl.Split('/');
            if (path.Length < 2)
            {
                return "Not Support";
            }
            string module = path[1].ToUpper();//获得一级地址作为模块
            RequestHandler handler = null;
            if (handlerMap.TryGetValue(module, out handler))
            {
                return handler.Process(request);
            }
            else
            {
                return string.Format("Module:{0} Not Support", module);
            }
        }



        

    }
}