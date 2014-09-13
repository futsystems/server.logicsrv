using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;

/*
 * web前端与tradingsrv通讯协议
 * 1.RequstTradingAccount|user_id   请求交易帐号
 * 2.ReqFinService|account,ammount,type,discount,agent          为某个交易帐号请求生成配资服务
 * 3.ReqUpdateFinService|account,param_name,value               为某个账户的配资服务修改参数,提高或者降低融资金额,修改计费类别等
 * 
 * 
 * 
 * */
namespace TradingLib.Contrib
{
    public class WebTaskRepServer:IService
    {
        const string PROGRAME = "WebTaskRepSrv";
        public event DebugDelegate SendDebugEvent;
        public event WebTaskDel GotWebTaskEvent;
        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }
        //debug输出
        void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        string _address = "*";
        int _port = 5000;
        public WebTaskRepServer(string address, int port)
        {
            _address = address;
            _port = port;
        }


        string handleWebTask(string task)
        {
            if (GotWebTaskEvent != null)
                return GotWebTaskEvent(task);
            return string.Empty;
        }

        

        bool _started = false;
        bool _mainthreadready = false;

        public bool IsLive
        {
            get {
                return _started;
            }
        }
        public void Start()
        {
            if (_started)
                return;
            debug("Start "+PROGRAME,QSEnumDebugLevel.INFO);
            //启动主服务线程

            _srvgo = true;
            _srvThread = new Thread(new ThreadStart(WebTaskRoute));
            _srvThread.IsBackground = true;
            _srvThread.Name = "WebManager Collect Message From website";
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
            _srvThread.Abort();

            int _wait = 0;
            while ((_srvThread.IsAlive == true) && (_wait++ < 5))
            {
                debug("#:" + _wait.ToString() + "  #mainthread:" + _srvThread.IsAlive.ToString()  +"  webchannelServer is stoping.....");
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
                            debug("web taks message is:" + str,QSEnumDebugLevel.INFO);
                            string re = handleWebTask(str);
                            rep.Send(re, Encoding.UTF8);

                        }
                        catch (Exception ex)
                        {
                            debug("deal wektask error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                        }

                    };
                    var poller = new Poller(new List<ZmqSocket> { rep });

                    //让线程一直获取由socket发报过来的信息
                    _mainthreadready = true;
                    while (_srvgo)
                    {
                        try
                        {
                            poller.Poll();
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
