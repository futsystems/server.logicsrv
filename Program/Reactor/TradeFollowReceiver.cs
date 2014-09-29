using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;

namespace Reactor
{
    public class TradeFollowReceiver
    {

        public TradeFollowReceiver()
        { 
        
        }

        string server = "logic_dev.huiky.com";
        int port = 6868;

        public event DebugDelegate SendDebugEvent;
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
            //1.判断日志级别,然后调用日志输出 比如向控件或者屏幕输出显示
            //if (_debugEnable && (int)level <= (int)_debuglevel)
            msgdebug("[" + level.ToString() + "] " + "TF" + ":" + msg);
        }

        void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        bool tradego = false;
        Thread tradethread;

        public void Start()
        {
            if (tradego) return;
            tradego = true;
            tradethread = new Thread(ReceiveProc);
            tradethread.IsBackground = true;
            tradethread.Start();

        }

        public void Stop()
        {
            if (!tradego) return;
            tradego = false;
            tradethread.Abort();
            tradethread = null;
        }
        
        void ReceiveProc()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket receiver = ctx.CreateSocket(SocketType.SUB))
                {

                    string cstr = "tcp://" + server.ToString() + ":" + port.ToString();
                    receiver.Connect(cstr);
                    receiver.SubscribeAll();


                    receiver.ReceiveReady += (s, e) =>
                    {

                        string fill = receiver.Receive(Encoding.UTF8);
                        //string tickstr = subscriber.Receive(Encoding.UTF8);
                        //debug("tickstr:" + tickstr.ToString());
                        debug(fill);
                        
                    };
                    var poller = new Poller(new List<ZmqSocket> { receiver });

                    //_tickreceiveruning = true;
                    while (tradego)
                    {
                        try
                        {
                            poller.Poll();
                        }
                        catch (ZmqException ex)
                        {
                            debug("Tick Sock错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                            receiver.Dispose();
                            ctx.Dispose();
                        }
                        catch (System.Exception ex)
                        {
                            debug("Tick数据处理错误:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    //_tickreceiveruning = false;

                }
            }
        }

    }
}
