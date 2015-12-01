using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;


using NHttp;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.APIService
{
    public class HttpAPIServer
    {


        public void Start()
        {
            InitServer();
        }

        public void Stop()
        { 
        
        }


        Thread _httpthread = null;
        NHttp.HttpServer _server = null;

        void InitServer()
        {
            _server = new NHttp.HttpServer();
            _server.EndPoint = new IPEndPoint(IPAddress.Loopback, 9070);
            _server.RequestReceived += (s, e) =>
            {
                using (var writer = new StreamWriter(e.Response.OutputStream))
                {
                    Console.WriteLine(string.Format("HttpMethod:{0} RawUrl:{1} Url:{2}", e.Request.HttpMethod, e.Request.RawUrl, e.Request.Url));
                    writer.Write("Hello world!");
                }
            };

            _server.Start();

        }
        void Process()
        {
            using (var server = new NHttp.HttpServer())
            {
                server.EndPoint = new IPEndPoint(IPAddress.Loopback, 9070);

                server.RequestReceived += (s, e) =>
                {
                    using (var writer = new StreamWriter(e.Response.OutputStream))
                    {
                        writer.Write("Hello world!");
                    }
                };

                
                server.Start();

                //Process.Start(String.Format("http://{0}/", server.EndPoint));

                //Console.WriteLine("Press any key to continue...");
                //Console.ReadKey();
            }
        }
    }
}
