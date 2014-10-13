using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Reflection;
using HttpServer;
using HttpServer.BodyDecoders;
using HttpServer.Logging;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Routing;



using HttpListener = HttpServer.HttpListener;



namespace TradingLib.Contrib.RechargeOnLine
{
    public class RechargeHttpServer
    {
        const string PROGRAME = "HttpServer";
        public event DebugDelegate SendDebugEvent;

        int _httpPort = 8085;
        string _httpDirectory = "RechargeGateway";
        //string _pagepath = string.Empty;
        //string _notifypath = string.Empty;

        public RechargeHttpServer(int httpport, string httpresource)
        { 
            _httpPort = httpport;
            _httpDirectory = httpresource;
        }


        void msgdebug(string message)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(message);
        }

        bool _debugEnable = true;
        /// <summary>
        /// 是否输出日志
        /// 如果禁用日志 则所有日志将不对外发送
        /// </summary>
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }

        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        /// <summary>
        /// 日志输出级别
        /// </summary>
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if (_debugEnable && (int)level <= (int)_debuglevel)
            {
                msgdebug(PROGRAME +"["+level.ToString()+"] "+msg);
            }
        }



        Thread httpthread = null;
        bool httpgo = false;
        public void Start()
        {
            debug("Start HttpServer....", QSEnumDebugLevel.INFO);
            if (httpgo) return;
            httpgo = true;
            httpthread = new Thread(httpproc);
            httpthread.IsBackground = true;
            httpthread.Start();
        }

        public void Stop()
        {
            debug("Stop HttpServer....", QSEnumDebugLevel.INFO);
            if (!httpgo) return;
            httpgo = false;
            Util.WaitThreadStop(httpthread, 10);

        }

        TemplateHelper _tmphelper = null;
        void httpproc()
        {
            //日志
            //var filter = new LogFilter();
            //filter.AddStandardRules();
            //LogFactory.Assign(new ConsoleLogFactory(filter));

            // create a server.
            var server = new Server();

            //添加文件访问模块
            var module = new FileModule();
            string path = Util.GetResourceDirectory(_httpDirectory);
            debug("Http Resource path:" + path, QSEnumDebugLevel.INFO);
            module.Resources.Add(new FileResources("/",path));
            server.Add(module);

            server.Add(new MultiPartDecoder());

            // use one http listener.
            server.Add(HttpListener.Create(IPAddress.Any, _httpPort));
            debug("HttpServer listen at port:" + _httpPort.ToString(), QSEnumDebugLevel.INFO);

            //默认根目录首页跳转
            server.Add(new SimpleRouter("/", "/index.html"));
            //添加充值处理模块
            server.Add(new RouterRecharge("/recharge"));
            //添加通知处理模块
            server.Add(new RouterPaymentNotify(GWGlobals.GWInfo.LocalURLInfo.PagePath, GWGlobals.GWInfo.LocalURLInfo.NotifyPath));


            server.Add(new SimpleRouter("/error", "/error.html"));

            
            server.RequestReceived += OnRequest;

            // start server, can have max 5 pending accepts.
            server.Start(5);
        }

        private  void OnRequest(object sender, RequestEventArgs e)
        {
            //debug("got http request: url:" + e.Request.Uri + " method:" + e.Request.Method + " " + e.Request.IsAjax.ToString());
            if (e.Request.Method == Method.Post)
            {
                //访问的绝对路径 用于判断对应的操作
                string localpath = e.Request.Uri.LocalPath;
                debug("got http request: url:" + e.Request.Uri + " method:" + e.Request.Method + " " + e.Request.IsAjax.ToString() +" path:");
            }
        }

    }
}
