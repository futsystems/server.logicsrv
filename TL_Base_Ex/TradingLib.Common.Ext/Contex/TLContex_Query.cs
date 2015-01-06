using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public partial class TLContext
    {

        ///// <summary>
        ///// 返回扩展模块信息列表 用于生成json对象
        ///// </summary>
        ///// <returns></returns>
        //internal List<JsonWrapperContrib> QryContrib()
        //{
        //    List<JsonWrapperContrib> list = new List<JsonWrapperContrib>();

        //    foreach (string uuid in contribIDUUIDMap.Values)
        //    {
        //        list.Add(new JsonWrapperContrib(baseSrvObjectMap[uuid]));
        //    }
        //    return list;
        //}

        ///// <summary>
        ///// 返回核心模块列表 用于生成json对象
        ///// </summary>
        ///// <returns></returns>
        //internal List<JsonWrapperCore> QryCore()
        //{
        //    List<JsonWrapperCore> list = new List<JsonWrapperCore>();
        //    foreach (string uuid in coreIdUUIDMap.Values)
        //    {
        //        list.Add(new JsonWrapperCore(baseSrvObjectMap[uuid]));
        //    }
        //    return list;
        //}

        //internal List<JsonWrapperCLICmd> QryCLICmd()
        //{
        //    List<JsonWrapperCLICmd> list = new List<JsonWrapperCLICmd>();
        //    foreach (string cmdkey in messageCLICmdMap.Keys)
        //    {
        //        list.Add(new JsonWrapperCLICmd(messageCLICmdMap[cmdkey]));
        //    }
        //    return list;
        //}

        //internal List<JsonWrapperTask> QryTask()
        //{
        //    List<JsonWrapperTask> list = new List<JsonWrapperTask>();
        //    foreach (ITask t in taskList)
        //    {
        //        list.Add(new JsonWrapperTask(t));
        //    }
        //    return list;
        //}
    }

    //internal class JsonWrapperTask
    //{
    //    ITask _task;
    //    public JsonWrapperTask(ITask task)
    //    {
    //        _task = task;
    //    }

    //    public string TaskName {get{return _task.TaskName;}}

    //    public string TaskType {get{return _task.TypeName;}}

    //    public string TaskTime {get{return _task.TimeStr;}}
    //}



    //internal class JsonWrapperCLICmd
    //{

    //    ContribCommand _cmd;
    //    public JsonWrapperCLICmd(ContribCommand cmd)
    //    {
    //        _cmd = cmd;
    //    }
    //    public string CMDStr { get { return _cmd.Command; } }
    //    public string CMDHelp { get { return _cmd.CommandHelp; } }
    //    public string CMDDesp { get { return _cmd.CommandDescription; } }
    //}

    //internal class JsonWrapperContrib
    //{
    //    BaseSrvObject _obj;
    //    IContribPlugin _plugin;
    //    public JsonWrapperContrib(BaseSrvObject obj)
    //    {
    //        _obj = obj;
    //        _plugin = PluginHelper.LoadContribPlugin(_obj.GetType().FullName);
    //    }



    //    public string ContribID { get { return _plugin.ContribID; } }
    //    public string ClassName { get { return _plugin.ContribClassName; } }
    //    public string Name { get { return _plugin.Name; } }
    //    public string UUID { get { return _obj.UUID; } }
    //}
    ///// <summary>
    ///// used for prase into  json object string ,then show in website
    ///// </summary>
    //internal class JsonWrapperCore
    //{
    //    BaseSrvObject _obj;
    //    public JsonWrapperCore(BaseSrvObject obj)
    //    {
    //        _obj = obj;
            
    //    }
    //    public string CoreID { get { return (_obj as ICore).CoreId; } }
    //    public string ClassName { get { return _obj.GetType().FullName; } }
    //    public string UUID { get { return _obj.UUID; } }
    //}

}
