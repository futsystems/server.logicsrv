using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

using ZeroMQ;

namespace TradingLib.Core
{
    /// <summary>
    /// 交易数据流发布系统
    /// 
    /// </summary>
    public class TradeFollow:BaseSrvObject,ICore
    {

        public string CoreId { get { return PROGRAME; } }
        public TradeFollow()
            : base("TradeFollow")
        { 

           
            
        }

        bool messageGo = false;
        ZmqSocket _publisher;
        Thread _pubthread;

        public void Start()
        {
            if (messageGo) return;
            messageGo = true;
            _pubthread = new Thread(MessageOut);
            _pubthread.Name = "TradeFollowThread";
            ThreadTracker.Register(_pubthread);
            _pubthread.Start();
        }

        public void Stop()
        {
            if (!messageGo) return;
            messageGo = false;
            _pubthread.Abort();
            _pubthread = null;
        }



        void MessageOut()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB))
                {
                    _publisher = pub;
                    pub.Bind("tcp://*:6868");
                    //循环等待 pub将消息进行分发
                    while (messageGo)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            
        }

        object sendlock = new object();
        /// <summary>
        /// 通过publisher发送一条trade数据
        /// 通过加锁实现线程安全
        /// </summary>
        /// <param name="t"></param>
        public void GotTrade(Trade t)
        {

            lock (sendlock)
            {
                string message = TradeImpl.Serialize(t);
                if (messageGo && _publisher != null)
                {
                    _publisher.Send(message, Encoding.UTF8);
                }
            }
        }
    }
}
