using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using ZeroMQ;
using System.Threading;
using TradingLib.Common;

namespace TradingLib.Contrib
{
    /// <summary>
    /// 建立web页面信息发布 publisher
    /// </summary>
    public class WebInfoPub
    {
        public event DebugDelegate SendDebugEvent;

        string _add = "127.0.0.1";
        int _port = 9999;
        public WebInfoPub(string address, int port)
        {
            _add = address;
            _port = port;
            
        }
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        void send(string s)
        {
            _socket.Send(s, Encoding.UTF8);
        }

        void notify(WebSocketNotifyType type, JSONMessage jmsg)
        {
            string jstr = new WebSocketMessage(type, jmsg).ToJSONString();
            send(jstr);
        }

        /// <summary>
        /// 发送session信息
        /// </summary>
        /// <param name="msg"></param>
        public void notify_sessioninfo(TradingLib.Common.Message msg)
        {
            //检验消息列别,不是sessioninfo则直接返回
            if (msg.Type != MessageTypes.LOGINUPDATE) return;
            JSONMessage jmessage = new JSONMessage(msg);//将message包装成可序列化为JSONString的格式
            notify(WebSocketNotifyType.notify_sessioninfo, jmessage);
        }
        /// <summary>
        /// 发送市场行情数据
        /// </summary>
        /// <param name="msg"></param>
        public void notify_tick(TradingLib.Common.Message msg)
        {
            if (msg.Type != MessageTypes.TICKNOTIFY) return;
            JSONMessage jmessage = new JSONMessage(msg);//将message包装成可序列化为JSONString的格式
            notify(WebSocketNotifyType.notify_tick, jmessage);
        }

        /// <summary>
        /// 发送总体统计数据
        /// </summary>
        /// <param name="tradernum"></param>
        /// <param name="ordernum"></param>
        /// <param name="margin"></param>
        public void notify_statistic(long tradernum, long ordernum, long margin)
        {
            JSONMessage sta = new JSONMessage(tradernum, ordernum, margin);
            notify(WebSocketNotifyType.notify_all, sta);
        }



        bool _started = false;
        bool _mainthreadready = false;

        public void Start()
        {
            if (_started)
                return;
            debug("Start WebSocketRoute Transport Server....");
            //启动主服务线程

            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(WebSocketRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "WebInfoPub Thread";
            _srvThread.Start();
            ThreadTracker.Register(_srvThread);

            int _wait = 0;
            //用于等待线程中的相关服务启动完毕 这样函数返回时候服务已经启动完毕 相当于阻塞了线程
            //防止过早返回 服务没有启动造成的程序混乱
            //这里是否可以用thread.join来解决？
            while ((_mainthreadready != true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "websocketServer is starting.....");
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
            _srvThread.Abort();

            int _wait = 0;
            while ((_srvThread.IsAlive == true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  #mainthread:" + _srvThread.IsAlive.ToString() + "  websocketServer client is stoping.....");
                Thread.Sleep(1000);
            }
            //当主线程与名成查询线程全部启动时候我们认为服务启动完毕否则我们抛出异常
            if (!_srvThread.IsAlive)
                _started = false;
            else
                throw new QSAsyncServerError();

        }

        Thread _srvThread = null;
        bool _srvgo = false;
        ZmqSocket _socket;
        private void WebSocketRoute()
        {
            using (var context = ZmqContext.Create())
            {   //当server端返回信息时,我们同样需要借助一定的设备完成
                using (ZmqSocket socket = context.CreateSocket(SocketType.PUB))//, adminpublisher = context.Socket(SocketType.PUB),adminfrontend = context.Socket(SocketType.ROUTER), adminbackend = context.Socket(SocketType.DEALER))
                {
                    socket.Bind("tcp://"+_add+":"+_port.ToString());
                    _socket = socket;
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
