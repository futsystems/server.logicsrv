using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;

namespace TradingLib.Contrib.TradeFlow
{
    [ContribAttr(TradeFlowCentre.ContribName, "TradeFlow扩展", "TradeFlow用将系统的成交流通过pub广播出去")]
    public class TradeFlowCentre:ContribSrvObject, IContrib
    {
        const string ContribName = "TradeFlowCentre";

        public TradeFlowCentre()
            : base(TradeFlowCentre.ContribName)
        { 
            
        }



        bool messageGo = false;
        ZmqSocket _publisher;
        Thread _pubthread;
        ConfigDB _cfgdb;

        int _pubport = 6868;

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad() 
        {

            _cfgdb = new ConfigDB(ContribName);
            if (!_cfgdb.HaveConfig("PubPort"))
            {
                _cfgdb.UpdateConfig("PubPort", QSEnumCfgType.Int,6868, "成交流广播端口");
            }
            _pubport = _cfgdb["PubPort"].AsInt();
            TLCtxHelper.EventIndicator.GotFillEvent += new FillDelegate(EventIndicator_GotFillEvent);
        }

        void EventIndicator_GotFillEvent(Trade t)
        {
            this.GotTrade(t);
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() 
        {
        
        
        }

        void MessageOut()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB))
                {
                    _publisher = pub;
                    pub.Bind("tcp://*:"+_pubport.ToString());
                    //循环等待 pub将消息进行分发
                    debug("TradeFlow publisher bind at:" + _pubport.ToString(),QSEnumDebugLevel.INFO);
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
        void GotTrade(Trade t)
        {
            debug("got new trade ,will pubish it...",QSEnumDebugLevel.INFO);
            lock (sendlock)
            {
                string message = TradeImpl.Serialize(t);
                if (messageGo && _publisher != null)
                {
                    //debug("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`", QSEnumDebugLevel.INFO);
                    _publisher.Send(message, Encoding.UTF8);
                }
            }
        }


        /// <summary>
        /// 启动
        /// </summary>
        public void Start() 
        {
            if (messageGo) return;
            messageGo = true;
            _pubthread = new Thread(MessageOut);
            _pubthread.Name = "TradeFollowThread";
            ThreadTracker.Register(_pubthread);
            _pubthread.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() 
        {
            if (!messageGo) return;
            messageGo = false;
            _pubthread.Abort();
            Util.WaitThreadStop(_pubthread);
            _pubthread = null;
        
        }

    }
}
