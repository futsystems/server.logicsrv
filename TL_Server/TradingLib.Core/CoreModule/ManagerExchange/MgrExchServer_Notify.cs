/* 关于服务端的通讯模式
 * 服务端与管理端的通讯模式比较复杂，涉及到请求，查询，操作等需要对这些通讯进行抽象
 * 
 * 请求～特定回报 比如登入请求与登入回报是成对出现的，并且登入回报往往包含登入信息无法用通用回报进行替代
 * 
 * 查询～特定回报 查询必然想获得一个查询结果因此查询都有对应返回结构 
 * 
 * 操作～通用回报 比如设置合约数据，修改帐户信息等，此类操作会改变服务端对象状态，同时这些状态又要返回给管理端
 * 并且这个返回可能是多个管理端的返回，因此这里我们将它抽象成通知，以通知的形式返回到管理端，管理端再响应通知。
 * 同时被操作的这个对象有一个通知列表用笔表明该对象发生变化了需要通知哪些对端。
 * 
 * 在完成所有功能后需要对这些模式进行一个抽象和重构使得代码更加精简和可维护
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Core
{
    /// <summary>
    /// 服务端的通知模块
    /// 1.交易帐户通过notifyresponse发送到管理端
    /// 2.其他管理端的操作 反应在交易帐户上 通过Account来建立通知target进行通知
    /// 3.其他管理端的操作 反应在其他对象上 需要通过建立单独的通知体系来进行通知
    /// </summary>
    public partial class MgrExchServer
    {

        /// <summary>
        /// 初始化服务端的通知
        /// </summary>
        void InitNotifySection()
        {
            TLCtxHelper.EventSystem.CashOperationRequest += new EventHandler<CashOperationEventArgs>(CashOperationEvent_CashOperationRequest);
            TLCtxHelper.EventSystem.ManagerNotifyEvent +=new EventHandler<ManagerNotifyEventArgs>(EventSystem_ManagerNotifyEvent);

        }

        void EventSystem_ManagerNotifyEvent(object sender, ManagerNotifyEventArgs e)
        {
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(e.NotifyPredicate));
            response.ModuleID = CoreName;
            response.CMDStr = "ManagerNotify";
            response.Result = Mixins.Json.JsonMapper.ToJson(e.Notify);

            CachePacket(response);
        }

        /// <summary>
        /// 向某个Manager过滤谓词对应的Manager发送通知
        /// </summary>
        /// <param name="cmdstr"></param>
        /// <param name="module"></param>
        /// <param name="obj"></param>
        /// <param name="predictate"></param>
        public void Notify(string module,string cmdstr,object obj, Predicate<Manager> predictate)
        {
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(predictate));
            response.ModuleID = module;
            response.CMDStr = cmdstr;
            response.Result = Mixins.Json.JsonReply.SuccessReply(obj).ToJson();
            CachePacket(response);
        }


        public void Notify(string module, string cmdstr, object obj, Manager manager)
        {
            if (manager == null) return;
            List<Manager> m = new List<Manager>();
            m.Add(manager);
            Notify(module, cmdstr, obj, m);
        }
        /// <summary>
        /// 向某个管理员列表发送通知
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmdstr"></param>
        /// <param name="obj"></param>
        /// <param name="manager"></param>
        public void Notify(string module, string cmdstr, object obj, IEnumerable<Manager> managers)
        {
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(managers));
            response.ModuleID = module;
            response.CMDStr = cmdstr;
            response.Result = Mixins.Json.JsonReply.SuccessReply(obj).ToJson();
            CachePacket(response);
        }



        void CashOperationEvent_CashOperationRequest(object sender, CashOperationEventArgs e)
        {
            NotifyCashOperation(e.CashOperation);
        }

        /// <summary>
        /// 通过谓词过滤出当前通知地址
        /// 需要提供的参数就是Manager对应的谓词，用于判断是否需要通知该Manager
        /// </summary>
        /// <param name="predictate"></param>
        /// <returns></returns>
        public IEnumerable<ILocation> GetNotifyTargets(Predicate<Manager> predictate)
        {
            if (predictate == null)
            {
                return this.NotifyTarges.Where(c => c.Manager != null).Select(info => info.Location).ToArray();
            }
            //1.过滤没有绑定Manager的custinfoex                2.通过谓词过滤Manager              3.投影成地址
            return this.NotifyTarges.Where(c=>c.Manager!=null).Where(e => predictate(e.Manager)).Select(info => info.Location).ToArray();
        }


        /// <summary>
        /// 获得某个Manager列表对应的在线管理员地址
        /// </summary>
        /// <param name="managers"></param>
        /// <returns></returns>
        public IEnumerable<ILocation> GetNotifyTargets(IEnumerable<Manager> managers)
        {
            
            return this.NotifyTarges.Where(c => managers.Any(m => m.ID == c.Manager.ID)).Select(info => info.Location);
        }

        /// <summary>
        /// 出入金状态通知
        /// </summary>
        /// <param name="op"></param>
        void NotifyCashOperation(JsonWrapperCashOperation op)
        {
            //通知方式 request获得对应的判断谓词 用于判断哪个客户端需要通知，然后再投影获得对应的地址集合
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(op.GetNotifyPredicate()));
            response.ModuleID = CoreName;
            response.CMDStr = "NotifyCashOperation";
            response.Result = Mixins.Json.JsonReply.SuccessReply(op).ToJson();
            CachePacket(response);
        }


        /// <summary>
        /// 管理员更新通知
        /// 向所有有权访问mgr信息的管理员发送mgr变更
        /// </summary>
        /// <param name="mgr"></param>
        void NotifyManagerUpdate(Manager mgr)
        {
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(mgr.GetNotifyPredicate()));
            response.ModuleID = CoreName;
            response.CMDStr = "NotifyManagerUpdate";
            response.Result = Mixins.Json.JsonReply.SuccessReply(mgr).ToJson();
            CachePacket(response);
        }

        /// <summary>
        /// 管理员删除通知
        /// 向所有有权访问mgr信息的管理员发送mgr删除通知
        /// </summary>
        /// <param name="mgr"></param>
        void NotifyManagerDelete(Manager mgr)
        {
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(GetNotifyTargets(mgr.GetNotifyPredicate()));
            response.ModuleID = CoreName;
            response.CMDStr = "NotifyManagerDelete";
            response.Result = Mixins.Json.JsonReply.SuccessReply(mgr).ToJson();
            CachePacket(response);
        }

        
    }
}
