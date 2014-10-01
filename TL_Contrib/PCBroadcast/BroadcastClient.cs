using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;

namespace Message.Broadcast
{
    /// <summary>
    /// 交易客户端消息推送组件
    /// 用于向交易客户端推送消息
    /// 路径:UCenter->NotifyCentre->TraderMessageServer->BroadcastClient->MessageExchnage->Client
    /// UCenter访问接口调用NotifyCentre界面的发送消息函数,NotifyCentre找到对应的NotifyTrunk【TraderMessageServer】通道进行发送。BroadcastClient收到消息然后调用ctx界面进行消息发送
    /// NotifyCentre-TraderMessageServer push-pull,
    /// TraderMessageServer-BroadcastClient pub-sub,实现Ucenter通知trunk到交易平台的发送
    /// </summary>
    [ContribAttr(BroadcastClient.ContribName, "PC客户端消息推送", "用于向某个客户端发送消息或者向某组客户端发送消息")]
    public partial class BroadcastClient : ContribSrvObject, IContrib
    {
        TimeSpan PollerTimeOut = new TimeSpan(0, 0, 5);
        const string ContribName = "BroadcastClient";

        ConfigDB _cfgdb;
        string ucaddress = "uc_dev.huiky.com";
        int ucpubport = 5555;

        public BroadcastClient()
            : base(BroadcastClient.ContribName)
        {

        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            _cfgdb = new ConfigDB(BroadcastClient.ContribName);
            if (!_cfgdb.HaveConfig("ucaddress"))
            {
                _cfgdb.UpdateConfig("ucaddress", QSEnumCfgType.String, "uc_dev.huiky.com", "UCenter NotfiyServer地址");
            }
            if (!_cfgdb.HaveConfig("ucpubport"))
            {
                _cfgdb.UpdateConfig("ucpubport", QSEnumCfgType.Int, 5555, "UCenter NotfiyServer下发端口");
            }
            ucaddress = _cfgdb["ucaddress"].AsString();
            ucpubport = _cfgdb["ucpubport"].AsInt();

        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            base.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("PCBroadcast启动............", QSEnumDebugLevel.INFO);
            if (pcgo) return;
            pcgo = true;

            pcthread = new Thread(process);
            pcthread.IsBackground = true;
            pcthread.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (!pcgo) return;
            pcgo = false;
            Util.WaitThreadStop(pcthread);

        }



        void HandleMessage(string message)
        {
            //debug("Got Broadcast Message:" + message,QSEnumDebugLevel.INFO);
            messagebuf.Write(message);
        }

        
        bool pcgo = false;
        Thread pcthread = null;

        void process()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket subscriber = ctx.CreateSocket(SocketType.SUB))
                {
                    //Generate printable identity for the client
                    string cstr = "tcp://"+ucaddress +":"+ucpubport.ToString();
                    debug(PROGRAME + ":Connect to UCenter PubServer :" + cstr, QSEnumDebugLevel.MUST);
                    subscriber.Connect(cstr);
                    subscriber.SubscribeAll();//订阅所有数据

                    //订阅者数据到到处理
                    subscriber.ReceiveReady += (s, e) =>
                    {

                        string message = subscriber.Receive(Encoding.UTF8);
                        HandleMessage(message);
                    };
                    var poller = new Poller(new List<ZmqSocket> { subscriber });


                    while (pcgo)
                    {
                        try
                        {
                            poller.Poll(PollerTimeOut);
                            if (!pcgo)
                            {
                                subscriber.Close();
                            
                            }
                        }
                        catch (Exception ex)
                        {
                            debug("PCBroadcast Error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                        }
                    }
                    debug("PCBroadcast exit...", QSEnumDebugLevel.INFO);
                }
            }
        }

    }
}
