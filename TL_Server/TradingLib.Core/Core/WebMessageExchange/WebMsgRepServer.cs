using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using System.Threading;


namespace TradingLib.Core
{
    /// <summary>
    /// repserver主服务,用于响应其他调用端的命令调用
    /// 
    /// </summary>
    public class WebMsgRepServer :BaseSrvObject,IService
    {

        public event WebMessageDel GotWebMessageEvent;
        TimeSpan PollerTimeOut = new TimeSpan(0, 0, 5);
        string _address = "*";
        int _port = 5000;

        public WebMsgRepServer(string address, int port, bool istnetstring = false) :
            base("WebMsgRepServer")
        {
            _address = address;
            _port = port;
            _istnetstring = istnetstring;
        }

        bool _istnetstring = false;
        string handleWebTask(string task)
        {
            if (GotWebMessageEvent != null)
                return GotWebMessageEvent(task,_istnetstring);
            return ReplyHelper.Error_WebMsgHandlerNotBind;
        }



        bool _started = false;
        bool _mainthreadready = false;

        public bool IsLive
        {
            get
            {
                return _started;
            }
        }
        public void Start()
        {
            if (_started)
                return;
            debug("Start " + PROGRAME, QSEnumDebugLevel.INFO);
            //启动主服务线程

            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(WebTaskRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "WebMsgRep hand request from website";
            _srvThread.Start();
            ThreadTracker.Register(_srvThread);

            int _wait = 0;
            //用于等待线程中的相关服务启动完毕 这样函数返回时候服务已经启动完毕 相当于阻塞了线程
            //防止过早返回 服务没有启动造成的程序混乱
            //这里是否可以用thread.join来解决？
            while ((_mainthreadready != true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "webchannelServer is starting.....");
                Thread.Sleep(500);
            }

            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
            if (_mainthreadready)
                _started = true;
            else
                throw new QSAsyncServerError();

        }

        public void Stop()
        {
            if (!_started) return;

            _srvgo = false;
            ThreadTracker.Unregister(_srvThread);

            int _wait = 0;
            while ((_srvThread.IsAlive == true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  #mainthread:" + _srvThread.IsAlive.ToString() + "  webchannelServer is stoping.....");
                Thread.Sleep(1000);
            }
            _srvThread.Abort();
            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
            if (!_srvThread.IsAlive)
            {
                _started = false;
                debug("WebMsgRepServer Stopped successfull", QSEnumDebugLevel.INFO);
            }
            else
                throw new QSAsyncServerError();

        }

        Thread _srvThread = null;
        bool _srvgo = false;

        private void WebTaskRoute()
        {
            using (var context = ZmqContext.Create())
            {   //当server端返回信息时,我们同样需要借助一定的设备完成
                using (ZmqSocket rep = context.CreateSocket(SocketType.REP))
                {
                    //rep用于监听从web服务端过来的任务请求
                    rep.Bind("tcp://" + _address + ":" + _port.ToString());

                    rep.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            string str = rep.Receive(Encoding.UTF8);
                            debug("web taks message is:" + str, QSEnumDebugLevel.INFO);
                            string re = handleWebTask(str);
                            rep.Send(re, Encoding.UTF8);

                        }
                        catch (Exception ex)
                        {
                            debug("deal wektask error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }

                    };
                    var poller = new Poller(new List<ZmqSocket> { rep });

                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            poller.Poll(PollerTimeOut);
                            if(!_srvgo)
                            {
                                debug("main thread stopped,stop socket", QSEnumDebugLevel.INFO);
                                rep.Close();
                            
                            }
                        }
                        catch (ZmqException e)
                        {
                            debug("%%%%main server message error" + e.ToString());
                        }

                    }

                }


            }

        }




    }
}
