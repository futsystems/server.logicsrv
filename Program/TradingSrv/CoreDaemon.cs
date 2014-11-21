using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using TradingLib.LitJson;
using System.Threading;

namespace TraddingSrvCLI
{
    public enum QSEnumCoreThreadStatus
    { 
        /// <summary>
        /// 已启动
        /// </summary>
        Started=100,

        /// <summary>
        /// 启动中
        /// </summary>
        Starting=101,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping=102,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped=103,

        /// <summary>
        /// 待机状态
        /// </summary>
        Standby=104,

        /// <summary>
        /// 系统接管中
        /// </summary>
        TakingOver=105,

        /// <summary>
        /// 状态未知
        /// </summary>
        Unknown=110,


    }

    
    internal class CoreThreadStatus
    {
        public CoreThreadStatus()
        {
            this.Status = QSEnumCoreThreadStatus.Standby;
        }
        public QSEnumCoreThreadStatus Status{get;set;}
    }
    /// <summary>
    /// 核心守护开启一个监听端口 用于监听外部命令
    /// 执行相关操作
    /// </summary>
    internal class CoreDaemon
    {

        static void debug(string message)
        {
            Console.WriteLine(message);
        }

        int _port = 4080;

        CoreThread corethread = null;

        string _oppositeAddress = string.Empty;
        int _corePort = 0;
        TimeSpan _reqtimeout = new TimeSpan(0, 0, 2);
        string _oppositrepaddress = string.Empty;
        /// <summary>
        /// 是否启用冗余机制
        /// </summary>
        bool _opposited = false;
        public CoreDaemon()
        {
            ConfigFile _configFile = ConfigFile.GetConfigFile();
            _oppositeAddress = _configFile["OppositAddress"].AsString();
            _corePort = _configFile["CorePort"].AsInt();
            if (!string.IsNullOrEmpty(_oppositeAddress))
            { 
                _opposited=true;
                debug(string.Format("Opposit Server Setted Address:{0} Port:{1}", _oppositeAddress, _corePort));
                _oppositrepaddress = string.Format("tcp://{0}:{1}", _oppositeAddress, _corePort);
            }
            if (_opposited)
            {
                StartWatchDog();
            }

            corethread = new CoreThread();
            //corethread.SendDebugEvent +=new DebugDelegate(debug);
            if (!_opposited)
            {    
                //如果没有设置opposited  启动核心线程
                debug("Opposit Server is not setted, start corethread directly");
                corethread.Start();
            }
        }
        Thread watchDogThread = null;
        bool keepgo = false;
        ZmqContext ctx;
        ZmqSocket reqsockt;
        void StartWatchDog()
        {
            if (keepgo) return;
            debug("start opposite server watchdog process....");
            ctx = ZmqContext.Create();
            reqsockt = ctx.CreateSocket(SocketType.REQ);
            reqsockt.Connect(_oppositrepaddress);
            keepgo = true;
            watchDogThread = new Thread(WatchDogprocess);
            watchDogThread.IsBackground = true;
            watchDogThread.Start();
        }


        /// <summary>
        /// 运行过程中 最大超过几次超时，则认为opposite server 已经发生异常，准备接受系统工作
        /// </summary>
        int _maxtimoutcnt = 3;

        /// <summary>
        /// 开机启动过程中 最大超过几次超时，则认为主机没有启动，直接进入主机模式
        /// </summary>
        int _firstbootmaxtimeoutcnt = 2;

        /// <summary>
        /// 查询状态周期 3秒
        /// </summary>
        int _statusfreq = 3;
        /// <summary>
        /// 请求镜像主机状态超时次数
        /// </summary>
        int _timoutcnt = 0;

        /// <summary>
        /// 最近获得正常回报的时间 表明网络通讯正常
        /// </summary>
        long _lasttime = 0;

        /// <summary>
        /// 是否处于Master状态
        /// </summary>
        bool master = false;

        /// <summary>
        /// 如果刚开始服务程序 则不用等待5次超时检查，
        /// </summary>
        bool firstboot = true;

        /// <summary>
        /// 请求次数
        /// </summary>
        int _requestcnt = 0;
        void WatchDogprocess()
        {
            while (keepgo)
            {
                //如果处于从机状态 则向主服务端查询状态,如果状态异常则将核心线程对象启动进入主机模式
                if (!this.master)
                {
                    string request = TradingLib.Mixins.JsonRequest.Request("Status").ToJson();
                    reqsockt.Send(request, Encoding.UTF8);
                    string rep = reqsockt.Receive(Encoding.UTF8, _reqtimeout);
                    CoreThreadStatus status;
                    if (string.IsNullOrEmpty(rep))
                    {
                        //debug("send request:"+request +" timeout!");
                        //debug("Close bad req socket and init it");
                        reqsockt.Close();
                        reqsockt = ctx.CreateSocket(SocketType.REQ);
                        reqsockt.Connect(_oppositrepaddress);
                        //请求状态超时 则累加超时计数器
                        debug("request opposite's status timeout");
                        _timoutcnt++;
                    }
                    else
                    {
                        //如果有数据返回则 重置计数器
                        _timoutcnt = 0;
                        TradingLib.Mixins.JsonReply reply = TradingLib.Mixins.LitJson.JsonMapper.ToObject<TradingLib.Mixins.JsonReply>(rep);
                        if (reply.Code == 0)
                        {
                            TradingLib.Mixins.LitJson.JsonData data = TradingLib.Mixins.JsonReply.ParseJsonReplyData(rep);
                            status = TradingLib.Mixins.JsonReply.ParsePlayload<CoreThreadStatus>(data);
                            debug("reply status:" + status.Status.ToString());

                            //记录回报时间
                            _lasttime = Util.ToTLDateTime();
                            if (firstboot)
                            {
                                _requestcnt++;
                            }
                            if (_requestcnt > 0)
                            {
                                firstboot = false;
                                _requestcnt = 0;
                            }
                        }
                        else
                        {
                            debug("opposite server excute query error:" + reply.Message);
                        }
                    }

                    if (_timoutcnt >= _maxtimoutcnt || (firstboot && _timoutcnt>=_firstbootmaxtimeoutcnt))
                    {
                        debug(string.Format("reqeust  status {0} times, still no response,we will takeholder the system,", _timoutcnt));
                        //设定核心线程对象状态为 接管状态
                        corethread.Status = QSEnumCoreThreadStatus.TakingOver;

                        //处理本地相关工作
                        //设置本地主从标识 系统进入主机模式
                        this.master = true;

                        //启动核心线程对象
                        corethread.Start();

                    }
                }


                Thread.Sleep(_statusfreq*1000);
            }
        }


        public void Start()
        {
            using (ZmqContext ctx = ZmqContext.Create())
            {
                using (ZmqSocket rep = ctx.CreateSocket(SocketType.REP))
                {
                    rep.Bind("tcp://*:" + _port.ToString());

                    rep.ReceiveReady += (s, e) =>
                    {
                        try
                        {
                            string str = rep.Receive(Encoding.UTF8);
                            debug("web taks message is:" + str);
                            string re = HandleWebTask(str);
                            rep.Send(re, Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            debug("deal wektask error:" + ex.ToString());
                        }

                    };
                    var poller = new Poller(new List<ZmqSocket> { rep });
                    //让线程一直获取由socket发报过来的信息
                    while (true)
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

        public string HandleWebTask(string msg)
        {

            debug("handle...........");
            TradingLib.Mixins.LitJson.JsonData request = TradingLib.Mixins.JsonRequest.ToObject(msg);
            string method = request["Method"].ToString().ToUpper();
            switch (method)
            { 
                case "START":
                    if (corethread.Status == QSEnumCoreThreadStatus.Stopped)
                    {
                        corethread.Start();
                        return JsonReply.GenericSuccess(ReplyType.Success, "启动核心服务成功").ToJson();
                    }
                    else
                    {
                        return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于停止状态").ToJson();
                    }
                case "STOP":
                    if (corethread.Status == QSEnumCoreThreadStatus.Started)
                    {
                        new Thread(corethread.Stop).Start();//放入后台线程进行执行
                        //corethread.Stop();
                        return JsonReply.GenericSuccess(ReplyType.Success, "停止核心服务成功").ToJson();
                    }
                    else
                    {
                        return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于运行状态状态").ToJson();
                    }
                case "STATUS":
                    return new TradingLib.Mixins.ReplyWriter().Start().FillReply(TradingLib.Mixins.JsonReply.GenericSuccess()).Fill(corethread.CoreStatus,"Playload").End().ToString();
                default:
                    return JsonReply.GenericError(ReplyType.Error, "未支持命令").ToJson();
            }
        }


        
        

    }
}
