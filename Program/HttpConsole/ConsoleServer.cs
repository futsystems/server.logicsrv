using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ServiceProcess;
using Common.Logging;
using NHttp;

namespace HttpConsole
{
    public class ConsoleServer
    {
        ILog logger = LogManager.GetLogger("HttpServer");
        int _port = 8080;
        NHttp.HttpServer _server = null;

        public ConsoleServer(int port)
        {
            _port = port;
            srvlist.Add("127.0.0.1");
            srvlist.Add("47.89.14.9");
            srvlist.Add("47.91.250.139");
        }

        List<string> srvlist = new List<string>();
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
                object ret = HandleRequest(arg.Request, arg.Response);
                writer.Write(ret is string ? ret : Newtonsoft.Json.JsonConvert.SerializeObject(ret));
            }
        }

        object HandleRequest(HttpRequest request, HttpResponse response)
        {
            string userHost = request.UserHostAddress;

            if (request.RawUrl == "/favicon.ico")
            {
                return "";
            }

            if (!srvlist.Contains(userHost))
            {
                return new { Code = 1, Message = "Not Authorized" };
            }

            logger.Info("url:" + request.RawUrl);
            bool ret = false;
            if (request.RawUrl == "/stop")
            {
                ret = StopService("logicsrv");
            }
            if (request.RawUrl == "/start")
            {
                ret = StartService("logicsrv");
            }
            if (request.RawUrl == "/restart")
            {
                ret = RestartService("logicsrv");
            }

            return ret ? new { Code = 0, Message = "Success" } : new { Code = 1, Message = "Fail" };
        }

        public bool StartService(string serviceName, int timeoutMilliseconds = 5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = 0;
                TimeSpan timeout;

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec1));
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                logger.Info(string.Format("{0} started", serviceName));

                return true;
            }
            catch (Exception e)
            {
                logger.Error("start error:"+e.Message);
                return false;
            }
        }
        public bool StopService(string serviceName, int timeoutMilliseconds = 5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = 0;
                TimeSpan timeout;
                if (service.Status == ServiceControllerStatus.Running)
                {
                    millisec1 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    logger.Info(string.Format("{0} stopped", serviceName));
                }
                return true;

            }
            catch (Exception e)
            {
                logger.Error("stop error:"+e.Message);
                return false;
            }
        }
        public  bool RestartService(string serviceName, int timeoutMilliseconds=5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = 0;
                TimeSpan timeout;
                if (service.Status == ServiceControllerStatus.Running)
                {
                    millisec1 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    logger.Info(string.Format("{0} stopped", serviceName));
                }
                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                logger.Info(string.Format("{0} started", serviceName));

                return true;

            }
            catch (Exception e)
            {
                logger.Error("restart error:"+e.Message);
                return false;
            }
        }

    }
}
