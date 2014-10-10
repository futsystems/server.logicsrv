using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.WebPortal
{
    [ContribAttr(WebPortal.ContribName, "Web服务", "Web服务用于提供一个Web入口,方便的查看扩展模块或函数的API")]
    public class WebPortal:BaseSrvObject,IContrib
    {
        const string ContribName = "WebPortal";

        WebServer _webserver;
        ConfigDB _cfgdb;
        int _port = 7070;
        public WebPortal()
            : base(WebPortal.ContribName)
        { 
                        
        }
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            _cfgdb = new ConfigDB(WebPortal.ContribName);
            if (!_cfgdb.HaveConfig("webport"))
            {
                _cfgdb.UpdateConfig("webport", QSEnumCfgType.Int, 7070, "Web端访问端口");
            }


            _port = _cfgdb["webport"].AsInt();
            
            List<WebPage> Pages = new List<WebPage>(new WebPage[] 
            { 
                new WebPage("/test1.html", "<html><body><h1>Hola Dude</h1></body></html>"),
                new WebPage("/test2.html", "Just some plane\nText To Display"),
                new WebPage("/test3.html", "<html><body><a href='/test1.html'>Test Page</a></body></html>"),
                new WebPage("/contrib.html", TLCtxHelper.Ctx.PrintContribList()),
                new WebPage("/contrib.html", TLCtxHelper.Ctx.PrintContribList()),
                //new WebPage("/API", TLCtxHelper.Ctx.PrintWebMsg()),
            });
            string sPages = "<h2>Main Page</h2>";
            Pages.ForEach(delegate(WebPage WP) { sPages += "<a href=" + WP.Page + ">" + WP.Page + "</a><br>"; });
            Pages.Add(new WebPage("/", "<html><body>" + sPages + "</body></html>"));

            _webserver = new WebServer(_port, Pages);
            
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("Start webportal at port:" + _port.ToString(), QSEnumDebugLevel.INFO);
            _webserver.Start();
        
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        { 
        
        }
    }
}
