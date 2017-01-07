using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Contrib.WebPortal
{

    public class WebServer
    {
        private int m_iPort = 0;
        HttpListener m_HttpListener = new HttpListener();
        Dictionary<string, WebPage> m_Pages = new Dictionary<string, WebPage>();

        ILog logger = LogManager.GetLogger("WebServer");

        public WebServer(int iPort, List<WebPage> WebPages)
        {
            m_iPort = iPort;
            m_HttpListener.Prefixes.Add("http://+:" + m_iPort + "/");
            if (WebPages != null)
            {
                foreach (WebPage WP in WebPages)
                {
                    if (!m_Pages.ContainsKey(WP.Page.ToLower()))
                        m_Pages.Add(WP.Page.ToLower(), WP);
                }
            }
        }
        public void Start()
        {
            try
            {
                if (!m_HttpListener.IsListening)
                {
                    m_HttpListener.Start();
                    new Thread(new ThreadStart(runServer)).Start();
                }
            }
            catch { }
        }
        public void Stop() { m_HttpListener.Stop(); }
        public bool IsRunning() { return m_HttpListener.IsListening; }

        string htmlwrapper(string message)
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append("<html lang=\"zh-cn\">");
            sb.Append("<head>");
            sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.Append("</head><body>");
            //sb.Append(ToHTML(message));
            sb.Append(message.ToHtml());
            sb.Append("</body></html>");
            return sb.ToString();
        }
        private void runServer()
        {
            try
            {
                while (m_HttpListener.IsListening)
                {
                    HttpListenerContext Context = m_HttpListener.GetContext();
                    Thread RequestThread = new Thread(new ParameterizedThreadStart(processRequest));
                    KeyValuePair<HttpListenerContext, WebPage>? Param = null;
                    if (m_Pages.Count > 0)
                    {
                        string rawurl = Context.Request.RawUrl.ToLower();
                        logger.Debug("rawurl:" + rawurl);
                        //以api开头的进入api查询系统
                        if (rawurl.StartsWith("/api"))
                        {
                            //去除头尾的"/"分解并得到对应的路径
                            rawurl = rawurl.TrimStart('/');
                            rawurl = rawurl.TrimEnd('/');
                            string[] path = rawurl.Split('/');

                            //如果api根目录
                            if (path.Length ==1)
                            {
                                WebPage tmp = new WebPage("/", htmlwrapper(TLCtxHelper.Ctx.PrintContribList()));
                                Param = new KeyValuePair<HttpListenerContext, WebPage>(Context, tmp);
                            }
                            else if (path.Length == 2)
                            {
                                
                                string cmdkey = path[1];
                                //如果是list是获得所有列表
                                if (cmdkey.ToUpper().Equals("LIST"))
                                {
                                    WebPage tmp = new WebPage("/", htmlwrapper(TLCtxHelper.Ctx.PrintWebMsg()));
                                    Param = new KeyValuePair<HttpListenerContext, WebPage>(Context, tmp);
                                }
                                else
                                {
                                    logger.Debug("访问键值:" + cmdkey.ToString());
                                    WebPage tmp = new WebPage("/", htmlwrapper(TLCtxHelper.Ctx.PrintHttpAPI(cmdkey)));
                                    Param = new KeyValuePair<HttpListenerContext, WebPage>(Context, tmp);
                                    /*
                                    string msg = string.Empty;

                                    try
                                    {
                                        msg = TLCtxHelper.Ctx.PrintContrib(contribid);

                                    }
                                    catch (Exception ex)
                                    {
                                        msg = ex.ToString();
                                    }

                                    WebPage tmp = new WebPage("/", htmlwrapper(msg));
                                    Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                    Context, tmp);**/
                                    //Param = new KeyValuePair<HttpListenerContext, WebPage>(Context, new WebPage("/", "Error: please request as http://120.0.0.1/contrib/race/"));
                                }
                            }
                            else
                            {
                                Param = new KeyValuePair<HttpListenerContext, WebPage>(Context, new WebPage("/", "Error: please request as http://120.0.0.1/contrib/race/"));
                            }
                        }
                        else if (rawurl.StartsWith("/command"))
                        {
                            rawurl = rawurl.TrimStart('/');
                            rawurl = rawurl.TrimEnd('/');
                            string[] path = rawurl.Split('/');
                            if (path.Length == 1)
                            {
                                WebPage tmp = new WebPage("/", htmlwrapper(TLCtxHelper.Ctx.PrintCommandList()));
                                Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                Context, tmp);
                            }
                            else if (path.Length == 2)
                            {
                                //string message = path[1];
                                //Util.Debug("Contrib:" + contribid.ToString());
                                //WebPage tmp = new WebPage("/", htmlwrapper(TLCtxHelper.Ctx.PrintContrib(contribid)));
                                //Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                //Context, tmp);
                            }
                            else
                            {
                                Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                   Context, new WebPage("/", "Error: please request as http://120.0.0.1/command/[mr/mgr/cli/web]"));
                            }
                            
                        }
                        else if (m_Pages.ContainsKey(Context.Request.RawUrl.ToLower()))
                        {
                            Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                Context, m_Pages[Context.Request.RawUrl.ToLower()]);
                        }
                        else if (Context.Request.RawUrl == "/")
                        {
                            Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                Context, new WebPage("/", "Default Page"));
                        }
                        else
                        {
                            Param = new KeyValuePair<HttpListenerContext, WebPage>(
                                Context, new WebPage("", "Page Not Found"));
                        }
                    }
                    else
                    {
                        Param = new KeyValuePair<HttpListenerContext, WebPage>(
                            Context, new WebPage("/", "Default Page"));
                    }
                    if (Param != null)
                        RequestThread.Start(Param);
                }
            }
            catch { }
        }
        private void processRequest(object O)
        {
            KeyValuePair<HttpListenerContext, WebPage>? ContextAndPage
                = O as KeyValuePair<HttpListenerContext, WebPage>?;
            HttpListenerResponse Response = ContextAndPage.Value.Key.Response;
            byte[] PageData = ContextAndPage.Value.Value.GetContent().ToArray();

            Response.StatusCode = (int)HttpStatusCode.OK;
            Response.ContentLength64 = PageData.Length;
            Response.ContentEncoding = Encoding.UTF8;
            Response.OutputStream.Write(PageData, 0, PageData.Length);
            Response.OutputStream.Close();
            Response.Close();
        }
    }

    public class WebPage
    {
        protected string m_sPage = "";
        protected string m_sContent = "";
        public WebPage(string sPage)
        {
            m_sPage = sPage;
        }
        public WebPage(string sPage, string sContent)
        {
            m_sPage = sPage;
            m_sContent = sContent;
        }
        public string Page { get { return m_sPage; } }
        public virtual List<byte> GetContent()
        {
            return new List<byte>(Encoding.UTF8.GetBytes(m_sContent));
        }
    }
}
