using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /* 关于委托状态的跟踪与维护
     * 委托状态维护机制
     * 1.通过本地的ordertracker 采集order,trade,cancel的回报来进行状态维护
     * 通过setsize和fillsize来维护fill和partfill的状态，通过cancel来维护取消状态
     * 通过ordererror产生的order reject来维护reject的状态
     * 
     * 2.通过brokersize进行维护 在与broker通讯过程中 维护了委托的映射 本地委托到 brokerside的委托
     * 当brokerside的委托状态发生更新时，我们更新本地的委托 同时通过brokerrouter传递到系统
     * 系统内部的ordertracker只是用于记录和核对的作用
     * 
     * 
     * 
     * 
     * 
     * 风控中心用于处理客户端提交上来的委托以及其他操作的检查
     * 清算中心用于记录有路由返回过来的交易回报 生成实时的交易状态 清算中心会根据接收到的信息进行账户交易信息记录与财务计算
     * 后续事件的订阅者必须无阻塞的执行操作否则会阻塞主线程的事件回报 因此该事件处理函数必须无异常，否则会影响其他后续组件的对该事件的订阅
     * 
     * 
     * 
     * */
    public partial class MsgExchServer
    {
        /// <summary>
        /// 手工插入委托返回对应的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void ManualInsertOrder(Order o)
        {
            AssignOrderID(ref o);
            long ordid = o.id;
            //设定委托相关编号
            o.OrderSysID = o.OrderSeq.ToString();
            o.BrokerRemoteOrderID = o.OrderSysID;
            OnOrderEvent(o);
            debug("insert ordre manual .....", QSEnumDebugLevel.INFO);
            //return ordid;

        }

        public void ManualInsertTrade(Trade t)
        {
            OnFillEvent(t);
            debug("insert trade manual ....", QSEnumDebugLevel.INFO);
        }
    }
}
