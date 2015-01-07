using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.Json;


namespace TradingLib.Common
{
    /*
     * 关于架构内的消息引擎
     * 每个模块相互独立,模块可以向系统注册以下几种操作
     * 1.向MessageRouter暴露功能函数,用于被客户端调用  调用格式 ContribID|CMDStr|Parameters
     * 2.向MessageMgr暴露管理函数,用于被管理端调用     调用格式 ContribID|CMDStr|Parameters
     * 3.向MessageWeb暴露web端调用函数,用于被Web端调用
     * 4.向CLIServer暴露命令行调用函数,用于被命令行调用   调用格式CMDStr|Parameters
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * */
   

    //将系统底层的事件进行分类,然后统一暴露到全局ctx中,用于扩展模块方便的获得底层系统时间
    //1.交易基本信息事件
    //2.客户端登入 注销 以及登入后回报事件
    //3.清算中心 帐户添加,账户冻结,帐户出入金事件
    //4.风控中心 
    //在Service部分,我们通过将底层核心组件绑定到对应的FireXXXXEvent函数上来实现底层事件向外围扩展模块的暴露



    /// <summary>
    /// 全局上下文,用于提供核心组件的全局引用
    /// </summary>
    public partial class TLContext
    {

        #region 命令列表结构
        //核心交易消息中处理的命令列表 每个模块对应一个命令列表，命令列表中通过操作编码与对应的执行对象进行映射
        //命令我们考虑使用contrib-cmdstr来进行储存与映射
        ConcurrentDictionary<string, ContribCommand> messageRouterCmdMap = new ConcurrentDictionary<string, ContribCommand>();
        //管理消息路由中处理的命令列表
        ConcurrentDictionary<string, ContribCommand> messageMgrCmdMap = new ConcurrentDictionary<string, ContribCommand>();
        //web交互模块中处理的命令列表
        ConcurrentDictionary<string, ContribCommand> messageWebCmdMap = new ConcurrentDictionary<string, ContribCommand>();
        //CLI交互模块中处理的命令列表
        ConcurrentDictionary<string, ContribCommand> messageCLICmdMap = new ConcurrentDictionary<string, ContribCommand>();

        //系统内部定义的扩展事件和与之对应的ContribCommand Event-<Contrib-CmdStr,ContribCommand>
        //系统内某个ContribEvent触发后,找到与之对应的命令映射 然后遍历该映射字典执行调用的方法 Event
        ConcurrentDictionary<string, ContribEventInfo> contribEventMap = new ConcurrentDictionary<string, ContribEventInfo>();
        ConcurrentDictionary<string, ConcurrentDictionary<string, ContribCommandInfo>> contribEventHandlerMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, ContribCommandInfo>>();

        //无需登入便可以执行的命令
        //ThreadSafeList<string> bypasscmds = new ThreadSafeList<string>();
        #endregion

        #region 定时任务列表

        ThreadSafeList<ITask> taskList = new ThreadSafeList<ITask>();
        /// <summary>
        /// 全局任务列表
        /// </summary>
        public ThreadSafeList<ITask> TaskList { get { return taskList; } }
        #endregion

        #region 对象引用列表
        //记录了系统内的所有BaseObject
        //uuid->BaseSrvObject映射
        ConcurrentDictionary<string, BaseSrvObject> baseSrvObjectMap = new ConcurrentDictionary<string, BaseSrvObject>();
        //记录了ContribID与其UUID对应的关系
        ConcurrentDictionary<string, string> contribIDUUIDMap = new ConcurrentDictionary<string, string>();

        //记录了CoreID与其UUID对应关系
        ConcurrentDictionary<string, string> coreIdUUIDMap = new ConcurrentDictionary<string, string>();

        //记录了服务管理模块与其UUID对应关系
        ConcurrentDictionary<string, string> serviceMgrIdUUIDMap = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 通过ContribId找到其对应的UUID
        /// </summary>
        /// <param name="contribId"></param>
        /// <returns></returns>
        string ContirbID2UUID(string contribId)
        {
            string uuid = "";
            if (contribIDUUIDMap.TryGetValue(contribId.ToUpper(), out uuid))
            {
                return uuid;
            }
            return null;
        }

        /// <summary>
        /// 检查某个Contrib是否注册到系统
        /// </summary>
        /// <param name="contribId"></param>
        /// <returns></returns>
        bool IsContribRegisted(string contribId)
        {
            return (contribIDUUIDMap.Keys.Contains(contribId.ToUpper()));
        }

        bool IsCoreRegisted(string coreid)
        {
            return (coreIdUUIDMap.Keys.Contains(coreid.ToUpper()));
        }

        bool IsServiceManagerRegisted(string srvmgrid)
        {
            return (serviceMgrIdUUIDMap.Keys.Contains(srvmgrid.ToUpper()));
        }
        /// <summary>
        /// 通过contribId找到其对应的IContirb插件对象
        /// </summary>
        /// <param name="contribid"></param>
        /// <returns></returns>
        IContrib ContribFinderName(string contribid)
        {
            string cid = contribid.ToUpper();
            if (contribIDUUIDMap.Keys.Contains(cid))
            {
                return baseSrvObjectMap[contribIDUUIDMap[cid]] as IContrib;
            }
            return null;
        }
        #endregion

        #region internal 暴露对象
        IClearCentreSrv _clearCentreSrv = null;
        /// <summary>
        /// 清算中心
        /// </summary>
        internal IClearCentreSrv ClearCentre
        {
            get
            {
                if (_clearCentreSrv == null)
                {
                    debug("Error-ClearCentre not valid");
                }
                return _clearCentreSrv;
            }
        }

        IRiskCentre _riskcentre = null;
        /// <summary>
        /// 风控中心
        /// </summary>
        internal IRiskCentre RiskCentre
        {
            get
            {
                if (_riskcentre == null)
                {
                    debug("Error-RiskCentre not valid");
                }
                return _riskcentre;
            }
        }

        ISettleCentre _settlecentre = null;
        /// <summary>
        /// 清算中心
        /// </summary>
        internal ISettleCentre SettleCentre
        {
            get
            {
                if (_settlecentre == null)
                {
                    debug("Error-SettleCentre not valid");
                }
                return _settlecentre;
                
            }
        }

        IMessageExchange _messageExchange = null;
        internal IMessageExchange MessageExchange
        {
            get
            {
                if (_messageExchange == null)
                {
                    debug("Error-MessageRouter not valid");
                }
                return _messageExchange;
            }
        }

        IMessageMgr _messagemgr = null;
        internal IMessageMgr MessageMgr
        {
            get
            {
                if (_messagemgr == null)
                {
                    debug("Error-MessageMgr not valid");
                }
                return _messagemgr;
            }
        }


        IRouterManager _routermanager = null;
        internal IRouterManager RouterManager
        {
            get
            {
                if (_routermanager == null)
                {
                    debug("Error-RotuerManager not valid");
                }
                return _routermanager;
            }
        }
        #endregion


        public TLContext()
        {

        }

        public void debug(string msg)
        {
            Util.Debug(">>>>Context:" + msg);
        }

        public void debug(string msg, QSEnumDebugLevel level)
        {
            Util.Debug(">>>>Context:" + msg);
        }

        void objlog(ILogItem item)
        {
            Util.Log(item);
        }
        void objemail(IEmail email)
        {
            TLCtxHelper.Email(email);
        }



        #region 命令解析与调用

        string ContribCommandKey(string contrib, string cmdstr)
        {
            return contrib.ToUpper() + "-" + cmdstr.ToUpper();
        }

        string ContribEventKey(string contrib, string eventstr)
        {
            return contrib.ToUpper() + "-" + eventstr.ToUpper();
        }
        /// <summary>
        /// 核心交易信息交换所使用的命令解析处理函数
        /// 用于处理从客户端发送过来的消息
        /// </summary>
        /// <param name="session">代表了某个客户端会话</param>
        /// <param name="message">客户端提交上来的消息</param>
        public void MessageExchangeHandler(ISession session, string message)
        {
            CmdHandler(session, message, messageRouterCmdMap);
        }
        public void MessageExchangeHandler(ISession session, ContribRequest request)
        {
            Util.Debug("****handle contribrequest:" + request.ToString());
            CmdHandler(session, request.ModuleID, request.CMDStr, request.Parameters, messageRouterCmdMap);
        }

        public void MessageMgrHandler(ISession session, string message)
        {
            
            CmdHandler(session, message, messageMgrCmdMap);
        }

        public void MessageMgrHandler(ISession session,MGRContribRequest request)
        {
            try
            {
                Util.Debug("****handle mgr contribrequest:" + request.ToString());
                CmdHandler(session, request.ModuleID, request.CMDStr, request.Parameters, messageMgrCmdMap);
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
            catch (QSCommandError ex)
            {
                TLCtxHelper.Ctx.debug(ex.Label + "\r\n reason@" + ex.Reason + "\r\n RawException:" + ex.RawException.Message.ToString(), QSEnumDebugLevel.ERROR);
                session.OperationError(new FutsRspError(ex));
            }
            catch (Exception ex)
            {
                session.OperationError(new FutsRspError(ex));
            }
        }
        /// <summary>
        /// 处理web message exchange消息调用
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmdstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JsonReply MessageWebHandler(JsonRequest request,bool istnetstring = false)
        {
            //string module, string cmdstr, string parameters
            string key = ContribCommandKey(request.Module, request.Method);
            Util.Debug("Handler webmessage, cmdkey:" + key);
            ContribCommand cmd=null;
            if(messageWebCmdMap.TryGetValue(key,out cmd))
            {
                try
                {
                    object obj = cmd.ExecuteCmd(null,request.Args,istnetstring);
                    //根据ContribCommand执行结果进行返回
                    //1.执行没有任何返回 比如进行一个无返回信息的操作请求
                    if (obj == null)
                    {
                        return WebAPIHelper.ReplySuccess(string.Format("CMD:{0} Execute Successful", key));
                    }
                    //2.返回JsonReply对象
                    else if (obj is JsonReply)
                    {
                        return obj as JsonReply;
                    }
                    else
                    {
                        return WebAPIHelper.ReplyObject(obj);
                    }
                }
                catch (QSCommandError ex)
                {
                    TLCtxHelper.Ctx.debug(ex.Label + "\r\n reason@" + ex.Reason + "\r\n RawException:" + ex.RawException.Message.ToString(), QSEnumDebugLevel.ERROR);
                    return WebAPIHelper.ReplyError("COMMAND_EXECUTE_ERROR");
                }
                catch (Exception ex)
                {
                    TLCtxHelper.Ctx.debug("ExectueCmd Error:\r\n" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    return WebAPIHelper.ReplyError("SERVER_SIDE_ERROR");
                }

            }
            else
            {
                return WebAPIHelper.ReplyError("METHOD_NOT_FOUND");
            }
        }

        /// <summary>
        /// 命令行处理命令调用
        /// </summary>
        /// <param name="cmdstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string MessageCLIHandler(string cmdstr, string parameters)
        {
            string cmdkey = cmdstr.ToUpper();
            ContribCommand cmd = null;
            if (messageCLICmdMap.TryGetValue(cmdkey, out cmd))
            { 
                try
                {
                    object obj =  cmd.ExecuteCmd(null, parameters);
                    if (obj == null)
                        return "";
                    else
                        return obj.ToString();
                }
                catch (QSCommandError ex)
                {
                    TLCtxHelper.Ctx.debug(ex.Label + "\r\n reason@" + ex.Reason + "\r\n RawException:" + ex.RawException.Message.ToString(), QSEnumDebugLevel.ERROR);
                    return "";// ExCommand.DEFAULT_REP_WRONG;
                }
                catch (Exception ex)
                {
                    TLCtxHelper.Ctx.debug("ExectueCmd Error:\r\n" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    return "";// ExCommand.DEFAULT_REP_WRONG;
                }
            }
            return "CLICommand:" + cmd + " Not Found\r\n";
        }


        /// <summary>
        /// message 为 contribid|cmdstr|parameters的格式
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        /// <param name="cmdmap"></param>
        void CmdHandler(ISession session, string message, ConcurrentDictionary<string, ContribCommand> cmdmap)
        {

            string[] p = message.Split('|');
            if (p.Length < 3)
            {
                debug("Error:ContribCommand message type: contribid|cmdstr|parameters");
                return;
            }
            session.ContirbID = p[0];
            session.CMDStr = p[1];
            CmdHandler(session, p[0], p[1], p[2], cmdmap);
        }

        
        /// <summary>
        /// 命令调用逻辑
        /// </summary>
        /// <param name="session">该消息来自于哪个会话(Front id,session id)</param>
        /// <param name="contribid">该消息发给哪个扩展模块</param>
        /// <param name="cmd">该消息调用扩展模块的哪个命令</param>
        /// <param name="parameters">调用该扩展模块所附带的参数(除session参数以为),所有扩展模块的函数调用均要包含session入口</param>
        void CmdHandler(ISession session, string contribid, string cmd, string parameters, ConcurrentDictionary<string, ContribCommand> cmdmap)
        {
            //Util.Debug("Contirb:" + contribid + " cmd:" + cmd + " args:" + parameters);
            string cmdkey = ContribCommandKey(contribid, cmd);

            if (!IsContribRegisted(contribid) && !IsCoreRegisted(contribid) &&!IsServiceManagerRegisted(contribid))
            {
                debug("Error:Module[" + contribid + "] do not registed");
                return;
            }
            if (!cmdmap.Keys.Contains(cmdkey))
            {
                debug("Error:Contrib[" + contribid + "] do not support Command[" + cmd + "]");
                return;
            }
            cmdmap[cmdkey].ExecuteCmd(session, parameters);
            
        }

        internal void BindContribEvent()
        {
            Util.StatusSection("CTX", "CONTRIBEVENT", QSEnumInfoColor.INFOGREEN, true);
            debug("Binding ContribEvent with ContribCommand(Handler)");
            foreach (string key in contribEventMap.Keys)
            {
                if (contribEventHandlerMap.Keys.Contains(key))
                {
                    foreach (ContribCommandInfo cmd in contribEventHandlerMap[key].Values)
                    {
                        BindEventWithHandler(contribEventMap[key], cmd);
                    }
                }
            }
        }
        void BindEventWithHandler(ContribEventInfo ev, ContribCommandInfo h)
        {
            try
            {
                EventInfo _event = ev.EventInfo;
                MethodInfo _method = h.MethodInfo;
                Delegate d = Delegate.CreateDelegate(_event.EventHandlerType,h.Target,_method.Name);
                _event.AddEventHandler(ev.Target, d);
            }
            catch (Exception ex)
            {
                debug("ContribCommand can not math EventHander:"+ex.ToString()+System.Environment.NewLine);
                debug("Event:" + ev.EventInfo.Name);
                debug("EventHandlerType:" + ev.EventInfo.EventHandlerType.FullName);
                debug("Handler:" + h.MethodInfo.Name);
            }
        }
        #endregion

        #region 注册BaseSrvObject


        public void Unregister(object obj)
        {
            if (obj is BaseSrvObject)
            { 
                BaseSrvObject srvobj = obj as BaseSrvObject;

                //2.将BaseSrvObject的日志输出时间绑定到全局日志输出组件
                //srvobj -= new DebugDelegate(objdebug);
                srvobj.SendLogItemEvent -= new ILogItemDel(objlog);
                //3.将BaseSrvObject的邮件发送事件banding到全局邮件发送函数
                srvobj.SendEmailEvent -= new EmailDel(objemail);

                //4.将BaseSrvObject的log事件绑定到全局日志发送函数
                //srvobj.SendLogEvent -= new LogDelegate(objlog);


                UnParseTaskInfo(srvobj);//注销任务


                BaseSrvObject objremoved = null;
                string uuidremoved = null;
                if (baseSrvObjectMap.Keys.Contains(srvobj.UUID))
                {

                    baseSrvObjectMap.TryRemove(srvobj.UUID, out objremoved);
                }









                if (obj is IClearCentreSrv)
                {
                    debug("ClearCentre unregisted from ctx");
                    _clearCentreSrv = null;
                }
                if (obj is IRiskCentre)
                {
                    debug("RiskCentre unregisted from ctx");
                    _riskcentre = null;
                    //return;
                }
                if (obj is ISettleCentre)
                {
                    debug("SettleCentre unregsited from ctx");
                    _settlecentre = null;
                }
                if (obj is IMessageExchange)
                {
                    debug("MessageRouter(TradingServer) unregisted from ctx");
                    _messageExchange = null;
                }
                if (obj is IMessageMgr)
                {
                    debug("MessageMgr(ManagerSrv) unregisted from ctx");
                    _messagemgr = null;
                }

                if (obj is IServiceManager)
                { 
                    IServiceManager mgr = obj as IServiceManager;
                    serviceMgrIdUUIDMap.TryRemove(mgr.ServiceMgrName.ToUpper(), out uuidremoved);
                    UnParseCommandInfo(obj, mgr.ServiceMgrName);

                    if (obj is IRouterManager)
                    {
                        _routermanager = null;
                    }
                }

                //1.检查是否是核心模块
                if (obj is ICore)
                {
                    ICore core = obj as ICore;
                    coreIdUUIDMap.TryRemove(core.CoreId.ToUpper(), out uuidremoved);

                    UnParseCommandInfo(obj, core.CoreId);
                }

                //2.检查是否是是扩展模块
                if (obj is IContrib)//.GetType().IsAssignableFrom(typeof(IContrib)))
                {
                    IContribPlugin plugin = PluginHelper.LoadContribPlugin(obj.GetType().FullName);

                    contribIDUUIDMap.TryRemove(plugin.ContribID.ToUpper(), out uuidremoved);
                    //查找该对象所支持模块命令列表
                    UnParseCommandInfo(obj, plugin.ContribID);

                    //查找对象暴露的事件
                    UnparseContribEventInfo(obj, plugin.ContribID);

                }
            }
        }

        /// <summary>
        /// 注册清算中心到全局上下文
        /// 通过在BaseSrvObject基类中统一向TLContext进行注册,则ctx可以获得全局所有继承自BaseSrvObject的对象
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void Register(object obj)
        {
            //1.将baseobject登记到全局对象列表
            if (obj is BaseSrvObject)
            {
                //1.记录系统生成的BaseSrvObject
                BaseSrvObject srvobj = obj as BaseSrvObject;
                baseSrvObjectMap.TryAdd(srvobj.UUID, srvobj);

                //2.将BaseSrvObject的日志输出时间绑定到全局日志输出组件
                //srvobj.SendDebugEvent += new DebugDelegate(objdebug);
                srvobj.SendLogItemEvent += new ILogItemDel(objlog) ;
                //3.将BaseSrvObject的邮件发送事件banding到全局邮件发送函数
                srvobj.SendEmailEvent += new EmailDel(objemail);

                //4.将BaseSrvObject的log事件绑定到全局日志发送函数
                //srvobj.SendLogEvent += new LogDelegate(objlog);

                //4.查找该对象所支持模块任务列表
                ParseTaskInfo(srvobj);


                if (obj is IClearCentreSrv)
                {
                    //debug("ClearCentre registed to ctx");
                    _clearCentreSrv = obj as IClearCentreSrv;
                    //return;
                }
                if (obj is ISettleCentre)
                {
                    _settlecentre = obj as ISettleCentre ;
                }

                if (obj is IRiskCentre)
                {
                    //debug("RiskCentre registed to ctx");
                    _riskcentre = obj as IRiskCentre;
                    //return;
                }
                if (obj is IMessageExchange)
                {
                    //debug("MessageRouter(TradingServer) registed to ctx");
                    _messageExchange = obj as IMessageExchange;
                }
                if (obj is IMessageMgr)
                {
                    //debug("MessageMgr(ManagerSrv) regsited to ctx");
                    _messagemgr = obj as IMessageMgr;
                }

                //0.检查是否是服务模块管理
                if (obj is IServiceManager)
                {
                    IServiceManager mgr = obj as IServiceManager;
                    serviceMgrIdUUIDMap.TryAdd(mgr.ServiceMgrName.ToUpper(), srvobj.UUID);
                    ParseCommandInfo(obj, mgr.ServiceMgrName);
                    if (obj is IRouterManager)
                    {
                        _routermanager = obj as IRouterManager;
                    }
                }

                //1.检查是否是核心模块
                if (obj is ICore)
                {
                    ICore core = obj as ICore;
                    coreIdUUIDMap.TryAdd(core.CoreId.ToUpper(), srvobj.UUID);
                    
                    ParseCommandInfo(obj, core.CoreId);
                }

                //2.检查是否是是扩展模块
                if (obj is IContrib)
                {
                    IContribPlugin plugin = PluginHelper.LoadContribPlugin(obj.GetType().FullName);

                    contribIDUUIDMap.TryAdd(plugin.ContribID.ToUpper(), srvobj.UUID);
                    //查找该对象所支持模块命令列表
                    ParseCommandInfo(obj, plugin.ContribID);

                    //查找对象暴露的事件
                    ParseContribEventInfo(obj, plugin.ContribID);

                }
            }

        }

        #region 注册扩展组件的Task函数


        /// <summary>
        /// 手工注入TaskProc 
        /// 注意在生成TaskProc时需要制定对象的UUID 从而实现当对象注销时自动通过uuid进行任务释放
        /// 
        /// </summary>
        /// <param name="proc"></param>
        public void InjectTask(TaskProc proc)
        {

            taskList.Add(proc);
        }
        /// <summary>
        /// 解析任务
        /// </summary>
        /// <param name="obj"></param>
        void ParseTaskInfo(BaseSrvObject obj)
        {
            List<TaskInfo> list = PluginHelper.FindContribTask(obj);

            foreach (TaskInfo info in list)
            {
                ITask task = TaskInfo2ITask(obj, info);
                if (task != null)
                {
                    //Util.Debug("注册任务:" + info.Attr.Name);
                    //将任务标识为某个BaseSrvObject对象,对象销毁时要自动注销任务
                    taskList.Add(task);
                }
            }
        }

        

        void UnParseTaskInfo(BaseSrvObject obj)
        {
            List<ITask> deletelist = new List<ITask>();
            foreach(ITask t in taskList)
            {
                if (t.UUID == obj.UUID)
                {
                    deletelist.Add(t);
                }
            }
            foreach (ITask t in deletelist)
            {
                taskList.Remove(t);
            }

        }

        ITask TaskInfo2ITask(BaseSrvObject obj, TaskInfo info)
        {
            switch (info.Attr.TaskType)
            {
                case QSEnumTaskType.CIRCULATE:
                    return new TaskProc(obj.UUID,info.Attr.Name, new TimeSpan(0, 0, info.Attr.IntervalSecends), delegate() { info.MethodInfo.Invoke(obj, null); });
                case QSEnumTaskType.SPECIALTIME:
                    return new TaskProc(obj.UUID,info.Attr.Name, info.Attr.Hour, info.Attr.Minute, info.Attr.Secend, delegate() { info.MethodInfo.Invoke(obj, null); });
                default:
                    return null;
            }
        }
        #endregion

        #region 解析注册ContribEvent
        /// <summary>
        /// 解析事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="componentId"></param>
        void ParseContribEventInfo(object obj, string componentId)
        {
            //Util.Debug("~~~~~~~~~~~~~~~~~~Parse Event");
            List<ContribEventInfo> list = PluginHelper.FindContribEvent(obj);
            foreach (ContribEventInfo info in list)
            {

                string eventky = ContribEventKey(componentId, info.Attr.EventStr);
                //Util.Debug("Event:" + eventky);
                contribEventMap.TryAdd(eventky, info);
            }
        }

        void UnparseContribEventInfo(object obj, string componentId)
        {
            List<ContribEventInfo> list = PluginHelper.FindContribEvent(obj);
            ContribEventInfo inforemoved = null;
            foreach (ContribEventInfo info in list)
            {

                string eventky = ContribEventKey(componentId, info.Attr.EventStr);
                //Util.Debug("Event:" + eventky);
                contribEventMap.TryRemove(eventky, out inforemoved);
            }
        }

        #endregion

        #region 解析注册对象的扩展模块命令
        /// <summary>
        /// 解析并处理模块命令列表,通过ContribCommandInfo生成对应的命令并填充到列表中
        /// 客户端通过命令contrib|cmdstr|message来发送消息
        /// 命令行则只有单一的命令可以解析,因此命令行命令用单一的cmdstr来进行解析
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        void ParseCommandInfo(object obj, string componentId)
        {
            List<ContribCommandInfo> list = PluginHelper.FindContribCommand(obj);
            //获得模块ID
            string contribid = componentId.ToUpper();
            
            foreach (ContribCommandInfo info in list)
            {
                //处理消息的扩展命令
                if (info.Attr.HandlerType == QSContribCommandHandleType.MessageHandler)
                {
                    //生成命令的键值
                    string cmdkey = ContribCommandKey(componentId, info.Attr.CmdStr);
                    switch (info.Attr.Source)
                    {
                        case QSEnumCommandSource.MessageExchange:
                            {
                                messageRouterCmdMap.TryAdd(cmdkey, new ContribCommand(obj, info));
                                break;
                            }
                        case QSEnumCommandSource.MessageMgr:
                            {
                                messageMgrCmdMap.TryAdd(cmdkey, new ContribCommand(obj, info));
                                break;
                            }
                        case QSEnumCommandSource.MessageWeb:
                            {
                                messageWebCmdMap.TryAdd(cmdkey, new ContribCommand(obj, info));
                                break;
                            }
                        case QSEnumCommandSource.CLI:
                            {
                                messageCLICmdMap.TryAdd(info.Attr.CmdStr.ToUpper(), new ContribCommand(obj, info));
                                break;
                            }
                        default:
                            debug("Error:can not recognize CommandSource");
                            break;
                    }

                    //如果该命令不用客户端登入授权后执行，则加入白名单列表
                    //if (!info.Attr.NeedAuth)
                    //{
                    //    bypasscmds.Add(cmdkey);
                    //}

                }
                else if(info.Attr.HandlerType == QSContribCommandHandleType.EventHandler) //事件
                {
                    //Util.Debug("!!!!!!!!!!!!!!!!!try to register event halder");
                    //源头source
                    string eventky = ContribEventKey(info.Attr.SourceContrib, info.Attr.EventStr);
                    //标识cmd
                    string cmdky = ContribCommandKey(componentId, info.Attr.CmdStr);

                    if(!contribEventHandlerMap.Keys.Contains(eventky))
                    {
                        contribEventHandlerMap.TryAdd(eventky,new ConcurrentDictionary<string,ContribCommandInfo>());
                    }

                    if (contribEventHandlerMap[eventky].Keys.Contains(cmdky))
                    {
                        debug("EventHandler: " + eventky + "  " + cmdky + " existed!!");
                    }
                    else
                    {
                        contribEventHandlerMap[eventky].TryAdd(cmdky, info);
                    }
                    
                }
            }
        }


        void UnParseCommandInfo(object obj, string componentId)
        {
            List<ContribCommandInfo> list = PluginHelper.FindContribCommand(obj);
            //获得模块ID
            string contribid = componentId.ToUpper();
            ContribCommand cmdremoved = null;
            ConcurrentDictionary<string, ContribCommandInfo> eventlistremoved = null;
            ContribCommandInfo cmdinforemoved = new ContribCommandInfo();
            foreach (ContribCommandInfo info in list)
            {

                //处理消息的扩展命令
                if (info.Attr.HandlerType == QSContribCommandHandleType.MessageHandler)
                {
                    //生成命令的键值
                    string cmdkey = ContribCommandKey(componentId, info.Attr.CmdStr);
                    switch (info.Attr.Source)
                    {
                        case QSEnumCommandSource.MessageExchange:
                            {
                                messageRouterCmdMap.TryRemove(cmdkey, out cmdremoved);
                                break;
                            }
                        case QSEnumCommandSource.MessageMgr:
                            {
                                messageMgrCmdMap.TryRemove(cmdkey, out cmdremoved);
                                break;
                            }
                        case QSEnumCommandSource.MessageWeb:
                            {
                                messageWebCmdMap.TryRemove(cmdkey, out cmdremoved);
                                break;
                            }
                        case QSEnumCommandSource.CLI:
                            {
                                messageCLICmdMap.TryRemove(info.Attr.CmdStr.ToUpper(),out cmdremoved);
                                break;
                            }
                        default:
                            debug("Error:can not recognize CommandSource");
                            break;
                    }

                    //如果该命令不用客户端登入授权后执行，则加入白名单列表
                    //if (!info.Attr.NeedAuth)
                    //{
                    //    bypasscmds.Remove(cmdkey);
                    //}

                }
                else if (info.Attr.HandlerType == QSContribCommandHandleType.EventHandler) //事件
                {
                    //Util.Debug("!!!!!!!!!!!!!!!!!try to register event halder");
                    //源头source
                    string eventky = ContribEventKey(info.Attr.SourceContrib, info.Attr.EventStr);
                    //标识cmd
                    string cmdky = ContribCommandKey(componentId, info.Attr.CmdStr);


                    if (contribEventHandlerMap.Keys.Contains(eventky))
                    {
                        
                        if (contribEventHandlerMap[eventky].Keys.Contains(cmdky))
                        {
                            contribEventHandlerMap[eventky].TryRemove(cmdky, out cmdinforemoved);
                        }
                        
                    }

                   

                }
            }
            //contribEventHandlerMap.TryRemove(eventky, out eventlistremoved);
        }


        /// <summary>
        /// 检查扩展模块是否已经在全局注册了命令列表
        /// 如果已经注册了命令列表则不能进行正常注册
        /// 原因
        /// 1.模块标识重复,修改扩展模块的ContribID
        /// 2.系统启动逻辑异常,导致重复生成了某个模块的多个实例
        /// </summary>
        /// <param name="contribid"></param>
        /// <returns></returns>
        //bool IsContribRegisted(string contribid)
        //{
        //    if (messageMgrCmdMap.Keys.Contains(contribid.ToUpper()))
        //        return true;
        //    return false;
        //}

        /// <summary>
        /// 初始化扩展模块的命令列表
        /// 用于在命令列表中形成该扩展模块的映射记录
        /// </summary>
        /// <param name="contribid"></param>
        //void InitContribCommandMap(string contribid)
        //{

        //    messageRouterCmdMap.TryAdd(contribid.ToUpper(), new ConcurrentDictionary<string, ContribCommand>());
        //    messageMgrCmdMap.TryAdd(contribid.ToUpper(), new ConcurrentDictionary<string, ContribCommand>());
        //    messageWebCmdMap.TryAdd(contribid.ToUpper(), new ConcurrentDictionary<string, ContribCommand>());
        //    messageCLICmdMap.TryAdd(contribid.ToUpper(), new ConcurrentDictionary<string, ContribCommand>());
        //}
        /// <summary>
        /// 销毁扩展模块命令列表
        /// </summary>
        /// <param name="contribid"></param>
        //void DestoryContribCommandMap(string contribid)
        //{ 
        //    ConcurrentDictionary<string, ContribCommand> tmp;
        //     messageRouterCmdMap.TryRemove(contribid.ToUpper(), out tmp);
        //     messageMgrCmdMap.TryRemove(contribid.ToUpper(), out tmp);
        //     messageWebCmdMap.TryRemove(contribid.ToUpper(), out tmp);
        //     messageCLICmdMap.TryRemove(contribid.ToUpper(), out tmp);

        //}
        #endregion




        #endregion

        #region print section
        

        string PluginInfo(IPlugin plugin)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Author:".PadRight(20, ' ') + plugin.Author + System.Environment.NewLine);
            sb.Append("Company:".PadRight(20, ' ') + plugin.Compnay + System.Environment.NewLine);
            sb.Append("Version:".PadRight(20, ' ') + plugin.Version + System.Environment.NewLine);
            sb.Append("Description:" + System.Environment.NewLine);
            sb.Append(plugin.Description + System.Environment.NewLine);
            return sb.ToString();
        }


        /// <summary>
        /// 输出系统内服务对象列表
        /// </summary>
        /// <returns></returns>
        public string PrintBaseObjectList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CliUtils.SectionHeader("BaseSrbObjectList"));
            foreach (BaseSrvObject obj in baseSrvObjectMap.Values)
            {
                sb.Append(obj.UUID.PadRight(40, ' ') + obj.Name + System.Environment.NewLine);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 输出扩展模块信息
        /// </summary>
        /// <returns></returns>
        public string PrintContribList()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(CliUtils.SectionHeader("ContribList"));
            foreach (string uuid in contribIDUUIDMap.Values)
            {
                IContrib c = baseSrvObjectMap[uuid] as IContrib;
                IContribPlugin plugin = PluginHelper.LoadContribPlugin(c.GetType().FullName);
                sb.Append(plugin.ContribID.PadRight(12, ' ') + c.GetType().FullName.PadRight(40, ' ') + plugin.Name + System.Environment.NewLine);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 输出核心底层模块列表
        /// </summary>
        /// <returns></returns>
        public string PrintCoreList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CliUtils.SectionHeader("CoreList"));
            foreach (string uuid in coreIdUUIDMap.Values)
            {
                ICore core = baseSrvObjectMap[uuid] as ICore;

                sb.Append(core.CoreId.PadRight(20, ' ') + core.GetType().FullName.PadRight(40, ' ') + System.Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 输出某个contrib的所有信息
        /// </summary>
        /// <param name="contrib"></param>
        /// <returns></returns>
        public string PrintContrib(string contrib)
        {
            Util.Debug("try to print contrib:" + contrib + " information");
            StringBuilder sb = new StringBuilder();
            IContrib c = ContribFinderName(contrib.ToUpper());
            if (c == null)
            {
                return "Contrib:" + contrib + " Not Exist" + System.Environment.NewLine;
            }
            IContribPlugin plugin = PluginHelper.LoadContribPlugin(c.GetType().FullName);
            if (plugin == null)
            {
                return "Contrib:" + contrib + " Not Exist" + System.Environment.NewLine;
            }

            sb.Append(CliUtils.SectionHeader("Contrib:" + contrib));
            sb.Append("UUID:".PadRight(20, ' ') + (c as BaseSrvObject).UUID + System.Environment.NewLine);
            sb.Append("ContribID:".PadRight(20, ' ') + plugin.ContribID + System.Environment.NewLine);
            sb.Append("Name:".PadRight(20, ' ') + plugin.Name + System.Environment.NewLine);
            sb.Append(PluginInfo(plugin));
            sb.Append("ContribCommand:" + System.Environment.NewLine);

            List<ContribCommandInfo> contribcommandlist = PluginHelper.FindContribCommand(c);
            foreach (ContribCommandInfo info in contribcommandlist)
            {
                sb.Append(PrintCommandAPI(contrib, info.Attr.CmdStr));
            }
            sb.Append("TaskCommand:" + System.Environment.NewLine);
            Util.Debug("it is run to here for print contrib");
            return sb.ToString();
        }
        /// <summary>
        /// 获得某个模块,某个命令的相信信息
        /// </summary>
        /// <param name="contrib"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string PrintCommandAPI(string contrib, string cmd)
        {
            string bigcontrib = contrib.ToUpper();
            string bigcmd = cmd.ToUpper();
            string cmdkey = ContribCommandKey(contrib, cmd);
            if (!IsContribRegisted(contrib))
                return (CliUtils.SECPRIFX+ " Can Not Find Command:" + contrib + "-" + cmd).PadRight(CliUtils.SECNUM, CliUtils.SECCHAR);

            string re = "";
            if (messageRouterCmdMap.Keys.Contains(cmdkey))
            {
                re += messageRouterCmdMap[cmdkey].ContribCommandAPI;
            }
            if (messageMgrCmdMap.Keys.Contains(cmdkey))
            {
                re += messageMgrCmdMap[cmdkey].ContribCommandAPI;
            }
            if (messageWebCmdMap.Keys.Contains(cmdkey))
            {
                re += messageWebCmdMap[cmdkey].ContribCommandAPI;
            }
            if (messageCLICmdMap.Keys.Contains(bigcmd))
            {
                re += messageCLICmdMap[cmdkey].ContribCommandAPI;
            }

            return re;
        }

        /// <summary>
        /// 输出某个访问键值的访问信息
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <returns></returns>
        public string PrintHttpAPI(string cmdkey)
        {
            try
            {
                cmdkey = cmdkey.ToUpper();
                if (messageWebCmdMap.Keys.Contains(cmdkey))
                {
                    return messageWebCmdMap[cmdkey].ContribCommandAPI;
                }
                else
                {
                    return string.Format("访问键值:{0} 不存在,请确认是否拼写正确",cmdkey);
                }
            }
            catch (Exception ex)
            {
                Util.Debug("服务端查询访问键值异常:" + ex.ToString());
            }
            return "服务端访问异常";
        }

        /// <summary>
        /// 输出所有命令行列表
        /// </summary>
        /// <returns></returns>
        public string PrintCLICommandList()
        {
            string cmdlist = "";
            foreach (string cmdkey in messageCLICmdMap.Keys)
            {
                cmdlist += messageCLICmdMap[cmdkey].CommandHelp;
            }
            return cmdlist;
        }


        /// <summary>
        /// 输出所有事件列表
        /// </summary>
        /// <returns></returns>
        public string PrintEventList()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CliUtils.SectionHeader("ContribEvent List"));
            foreach (string key in contribEventMap.Keys)
            {
                sb.Append("Event:" + key + " " + contribEventMap[key].EventInfo.Name+System.Environment.NewLine);
            }

            return sb.ToString();

        }

        
        /// <summary>
        /// 输出事件处理器列表
        /// </summary>
        /// <returns></returns>
        public string PrintEventHandler()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CliUtils.SectionHeader("Event Handler"));

            foreach (string eventstr in contribEventHandlerMap.Keys)
            {
                sb.Append("Event:" + eventstr + System.Environment.NewLine);
                foreach (string cmdky in contribEventHandlerMap[eventstr].Keys)
                {
                    sb.Append(("Command[" + cmdky + "]").PadRight(25, ' ') + contribEventHandlerMap[eventstr][cmdky].Attr.CmdStr + System.Environment.NewLine);
                }
            }
            return sb.ToString();
        }




        /// <summary>
        /// 输出命令列表
        /// </summary>
        /// <returns></returns>
        public string PrintCommandList()
        {
            string s = "";
            s += PrintCommandMap("MessageRouer", messageRouterCmdMap);
            s += PrintCommandMap("MessageMgr", messageMgrCmdMap);
            s += PrintCommandMap("MessageWeb", messageWebCmdMap);
            s += PrintCommandMap("MessageCLI", messageCLICmdMap);
            return s;
        }

        string PrintCommandMap(string title, ConcurrentDictionary<string, ContribCommand> map)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((CliUtils.SECPRIFX + " " + title + "").PadRight(CliUtils.SECNUM, CliUtils.SECCHAR) + System.Environment.NewLine);
            foreach (string key in map.Keys)
            {
                sb.Append(("  Command[" + key + "] ").PadRight(50, ' ') + map[key].Source.ToString().PadRight(20, ' ') + map[key].CommandHelp);
            }
            return sb.ToString();
        }

        public string PrintWebMsg()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in messageWebCmdMap.Keys)
            {
                sb.Append(key.PadRight(30, ' ') + messageWebCmdMap[key].ContribCommandDesp+ Environment.NewLine);
            }
            return sb.ToString();
        }
        #endregion


    }
}
