using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using System.Threading;
using TradingLib.Mixins.Json;

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
        string autofac_section = "counter";

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
            autofac_section = _configFile["Product"].AsString();
            autofac_section = string.IsNullOrEmpty(autofac_section) ? "Product" : autofac_section;

            
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

            corethread = new CoreThread(autofac_section);
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
        /// 对方主机处于启动中 用于记录启动时间
        /// </summary>
        bool _opposite_starting = false;

        /// <summary>
        /// 记录对方开始启动的时间
        /// </summary>
        int _opposite_starting_time = 0;


        /// <summary>
        /// 等待对方启动时间
        /// </summary>
        int _opposite_starting_wait_time = 20;
        

        /// <summary>
        /// 请求次数
        /// </summary>
        int _requestcnt = 0;

        /// <summary>
        /// 请求对端主机状态
        /// </summary>
        /// <returns></returns>
        CoreThreadStatus RequestStatus()
        {
            CoreThreadStatus status = null;
            try
            {
                string request = TradingLib.Mixins.Json.JsonRequest.Request("Status").ToJson();
                reqsockt.Send(request, Encoding.UTF8);
                string rep = reqsockt.Receive(Encoding.UTF8, _reqtimeout);
                //如果返回为空则表明超时
                if (string.IsNullOrEmpty(rep))
                {
                    debug("request opposite's status timeout");
                    //请求状态超时 则累加超时计数器
                    _timoutcnt++;
                    //关闭socket 并重新新建socket
                    reqsockt.Close();
                    reqsockt = ctx.CreateSocket(SocketType.REQ);
                    reqsockt.Connect(_oppositrepaddress);
                }
                else
                {
                    //如果有数据返回则 重置计数器
                    _timoutcnt = 0;
                    TradingLib.Mixins.Json.JsonReply<CoreThreadStatus> reply = TradingLib.Mixins.Json.JsonReply.ParseReply<CoreThreadStatus>(rep);
                    //解析Reply对象
                    //TradingLib.Mixins.JsonReply reply = TradingLib.Mixins.LitJson.JsonMapper.ToObject<TradingLib.Mixins.JsonReply>(rep);
                    //判断Reply是否为正常返回
                    if (reply.Code == 0)
                    {
                        status = reply.Payload;
                        //TradingLib.Mixins.LitJson.JsonData data = TradingLib.Mixins.JsonReply.ParseJsonReplyData(rep);
                        //status = TradingLib.Mixins.JsonReply.ParsePlayload<CoreThreadStatus>(data);
                        debug("reply status:" + status.Status.ToString());

                        //记录回报时间
                        _lasttime = Util.ToTLDateTime();

                        //如果有应答 则解除首次登入标识
                        //如果是首次登入 则
                        if (firstboot) firstboot = !firstboot;//_requestcnt++;

                        //如果请求次数大于0 则首次接触
                        //if (_requestcnt > 0)
                        //{
                        //    firstboot = false;
                        //    _requestcnt = 0;
                        //}


                    }
                    else
                    {
                        debug("opposite server excute query error:" + reply.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                debug("request status error:" + ex.ToString());    
            }
            return status;
            
        }
        /// <summary>
        /// 注意线程内执行的函数均为同步函数，不能将操作丢入线程内，否则会造成快速返回，多次请求的问题
        /// </summary>
        void WatchDogprocess()
        {
            while (keepgo)
            {
                //如果处于从机状态 则向主服务端查询状态,如果状态异常则将核心线程对象启动进入主机模式
                CoreThreadStatus status = RequestStatus();
                //当前处于待机状态
                if (!this.master)
                {
                    if (status != null)
                    {
                        #region 判断对方主机状态
                        //根据远程的状态判断本地操作
                        switch (status.Status)
                        {
                            case QSEnumCoreThreadStatus.Started://对方处于 已启动状态 则本地继续守护
                                {
                                    //如果记录过对方启动开始 输出对方启动时间长短 并将标识复位
                                    if (_opposite_starting)
                                    {
                                        _opposite_starting = false;
                                        int span = Util.FTDIFF(_opposite_starting_time, Util.ToTLTime());
                                        debug(string.Format("opposit server started by {0} secends", span));
                                    }
                                    break;
                                }
                            case QSEnumCoreThreadStatus.Standby://对方处于 待机状态
                                {
                                    //准备协商进行接管和启动 现实中几乎不会遇到同时获得到对方待机的问题，应为系统启动总有先后，后启动的系统会获得到对方待机然后处于立即启动的状态
                                    //系统A启动，处于2次超时检测阶段，此时系统A处于待机状态,此时启动B系统，B系统检测到系统A处于待机 则B系统立马接管系统进入启动状态
                                    //接管系统
                                    debug("opposite server standby,we take over system");
                                    TakeOver();
                                    break;
                                }
                            case QSEnumCoreThreadStatus.Starting://对方处于 启动状态，则等待对方启动成功 需要设置时间差
                                {
                                    WaitOppositStarting();
                                    //等待对方启动成功
                                    break;
                                }
                            case QSEnumCoreThreadStatus.Stopped://对方处于 已停止 进入启动状态
                                {
                                    _opposite_starting = false;
                                    //_opposite_starting_time = Util.ToTLTime();
                                    break;
                                }
                            case QSEnumCoreThreadStatus.Stopping://对方处于 停止中 进入启动状态
                                {
                                    _opposite_starting = false;
                                    break;
                                }
                            case QSEnumCoreThreadStatus.TakingOver://对方处于 接管状态 等待
                                {
                                    WaitOppositStarting();
                                    break;
                                }
                        }
                        #endregion
                    }

                    /* 如果系统处于待机状态,多次请求远端主机超时后判定 远端主机处于宕机状态
                     * 自行启动接管系统
                     * 
                     * 如果是首次启动,减少超时次数
                     * **/
                    #region 请求状态超时 处理
                    if (_timoutcnt >= _maxtimoutcnt || (firstboot && _timoutcnt >= _firstbootmaxtimeoutcnt))
                    {
                        debug(string.Format("reqeust  status {0} times, still no response,we will takeholder the system,", _timoutcnt));
                        //接管系统
                        TakeOver();
                    }
                    #endregion

                    /* 如果远端主机处于启动状态,则判定启动时间是否超过设定的阀值
                     * 如果超过阀值 则远端系统处于异常状态 直接自行启动接管系统
                     * 
                     * **/
                    #region 启动中状态处理
                    //如果对方主机处于启动中 则需要判断时间超过20秒没有正常启动则自己接管系统
                    if (_opposite_starting)
                    {
                        //如果远端主机在设定时间内没有成功启动则接管系统
                        if (Util.FTDIFF(_opposite_starting_time, Util.ToTLTime()) >= _opposite_starting_wait_time)
                        {
                            debug("opposite server is in starting for 20 secends, maybe some error,we will take over");
                            //接管系统
                            TakeOver();
                        }
                    }
                    #endregion

                }
                else//如果是当前是主机模式
                {
                    //如果对端主机超时 则不做处理
                    if (status != null)
                    {
                        //如果对方主机状态正常 则不采取错误 如果状态有冲突 要协商后关闭一方主机
                        switch (status.Status)
                        {
                            case QSEnumCoreThreadStatus.Started://系统同时处于启动状态,则需要按一定规则停掉其中一台
                                {
                                    if (corethread.Status == QSEnumCoreThreadStatus.Started)
                                    {
                                        debug("opposite started, we shut down ourself.");
                                        corethread.Stop();
                                        this.master = false;
                                    }
                                    
                                    break;
                                }
                            case QSEnumCoreThreadStatus.Standby://对方系统处于待机状态 正常
                            case QSEnumCoreThreadStatus.Stopped://对方系统处于停机章太 正常
                            case QSEnumCoreThreadStatus.Stopping://对方系统停止中 正常
                            case QSEnumCoreThreadStatus.Starting://启动中，等待启动完毕后 再处理
                            case QSEnumCoreThreadStatus.TakingOver://接管状态 不处理，等待启动完毕后再处理
                                break;
                            default:
                                debug("opposit status:" + status.Status.ToString());
                                break;
                        }
                    }
                }
                Thread.Sleep(_statusfreq * 1000);  
            }
        }

        /// <summary>
        /// 记录标识等待对方主机启动
        /// </summary>
        void WaitOppositStarting()
        {
            if (!_opposite_starting)
            {
                _opposite_starting = true;
                _opposite_starting_time = Util.ToTLTime();
            }
        }

        /// <summary>
        /// 接管系统
        /// </summary>
        void TakeOver()
        {
            //设定核心线程对象状态为 接管状态
            corethread.Status = QSEnumCoreThreadStatus.TakingOver;

            //处理本地相关工作
            //设置本地主从标识 系统进入主机模式
            this.master = true;

            //启动核心线程对象
            corethread.Start();
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
                            JsonReply re = HandleWebTask(str);
                            if (re != null)
                            {
                                rep.Send(re.ToJson(), Encoding.UTF8);
                            }
                            else
                            {
                                debug("deal wektask error:");
                            }
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

        public JsonReply HandleWebTask(string msg)
        {

            debug("handle...........");
            //TradingLib.Mixins.LitJson.JsonData request = TradingLib.Mixins.JsonRequest.ToObject(msg);
            //string method = request["Method"].ToString().ToUpper();
            JsonRequest request = JsonRequest.ParseRequest(msg);
            switch (request.Method.ToUpper())
            { 
                case "START":
                    if (corethread.Status == QSEnumCoreThreadStatus.Stopped)
                    {
                        corethread.Start();
                        return WebAPIHelper.ReplySuccess("启动核心服务成功");// JsonReply.GenericSuccess(ReplyType.Success, "启动核心服务成功").ToJson();
                    }
                    else
                    {
                        return WebAPIHelper.ReplyError("SERVICE_STARTED_ALREADY");
                        //return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于停止状态").ToJson();
                    }
                case "STOP":
                    if (corethread.Status == QSEnumCoreThreadStatus.Started)
                    {
                        new Thread(corethread.Stop).Start();//放入后台线程进行执行
                        return WebAPIHelper.ReplySuccess("停止核心服务成功");
                        //return JsonReply.GenericSuccess(ReplyType.Success, "停止核心服务成功").ToJson();
                    }
                    else
                    {
                        return WebAPIHelper.ReplyError("SERVICE_STOPPED_ALREADY");
                        //return JsonReply.GenericSuccess(ReplyType.Success, "服务非处于运行状态状态").ToJson();
                    }
                case "STATUS":
                    return WebAPIHelper.ReplyObject(corethread.CoreStatus);// TradingLib.Mixins.JsonReply.SuccessReply(corethread.CoreStatus).ToJson(); //TradingLib.Mixins.ReplyWriter().Start().FillReply(TradingLib.Mixins.JsonReply.GenericSuccess()).Fill(corethread.CoreStatus,"Playload").End().ToString();
                default:
                    return WebAPIHelper.ReplyError("METHOD_NOT_FOUND");
            }
        }


        
        

    }
}
