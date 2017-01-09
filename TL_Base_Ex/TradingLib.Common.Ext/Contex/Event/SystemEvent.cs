using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 系统事件类参数
    /// </summary>
    public class SystemEventArgs : EventArgs
    {
        public SystemEventArgs()
        {
            
        }
    }

    /// <summary>
    /// 系统类事件
    /// 比如开启交易中心,执行数据转储,执行结算,结算完毕等
    /// 结算：保存当日交易记录到历史记录表，保存当日过夜持仓数据，将当日交易财务结果生成结算记录插入并更新当前最新状态
    /// 结算重置：重新设定当前交易日，从数据库加载最新的Balance并价值该交易日内的持仓记录，交易记录，出入金记录形成当前最新的交易状态
    /// </summary>
    public class SystemEvent
    {

        #region 结算与重置事件
        /// <summary>
        /// 结算前事件 在结算前触发
        /// 比如结算前执行相关费用收取
        /// </summary>
        public event EventHandler<SystemEventArgs> BeforeSettleEvent;

        /// <summary>
        /// 结算后事件 在系统结算完毕后触发
        /// 结算后执行相关数据统计或通知等
        /// </summary>
        public event EventHandler<SystemEventArgs> AfterSettleEvent;


        /// <summary>
        /// 结算重置事件 在结算重置时触发
        /// </summary>
        public event EventHandler<SystemEventArgs> SettleResetEvent;

        internal void FireBeforeSettleEvent(object sender,SystemEventArgs args)
        {
            if (BeforeSettleEvent != null)
                BeforeSettleEvent(sender, args);
        }


        internal void FireAfterSettleEvent(object sender, SystemEventArgs args)
        {
            if (AfterSettleEvent != null)
                AfterSettleEvent(sender, args);
        }


        internal void FireSettleResetEvet(object sender, SystemEventArgs args)
        {
            if (SettleResetEvent != null)
                SettleResetEvent(sender, args);
        }
        #endregion


        #region 任务调度事件

        /// <summary>
        /// 定时任务事件
        /// </summary>
        public event EventHandler<TaskEventArgs> SpecialTimeTaskEvent;


        /// <summary>
        /// 执行任务异常事件
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskErrorEvent;


        internal void FireTaskErrorEvent(object sender, TaskEventArgs args)
        {
            if (TaskErrorEvent != null)
            {
                TaskErrorEvent(sender, args);
            }
        }

        internal void FireSpecialTimeEvent(object sender, TaskEventArgs args)
        {
            if (SpecialTimeTaskEvent != null)
            {
                SpecialTimeTaskEvent(sender, args);
            }
        }

        #endregion


        public event EventHandler<PacketEventArgs> PacketEvent;

        internal void FirePacketEvent(object sender, PacketEventArgs args)
        {
            if (PacketEvent != null)
                PacketEvent(sender, args);
        }



        //#region Position 强平事件

        ///// <summary>
        ///// 强平成功事件
        ///// </summary>
        //public event EventHandler<PositionFlatEventArgs> PositionFlatEvent;

        //internal void FirePositionFlatEvent(object sender,PositionFlatEventArgs args)
        //{
        //    if (PositionFlatEvent != null)
        //    {
        //        PositionFlatEvent(sender,args);
        //    }
        //}
        //#endregion


        #region 出入金操作状态
        /// <summary>
        /// 出入金请求 状态改变22:59
        /// </summary>
        public event EventHandler<CashOperationEventArgs> CashOperationRequest = delegate { };

        /// <summary>
        /// 通过事件中继 向系统推送资金处理事件
        /// 在每个调用到出入金操作状态改变的地方进行触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventType"></param>
        /// <param name="cashOperation"></param>
        public void FireCashOperation(object sender, QSEnumCashOpEventType eventType, JsonWrapperCashOperation cashOperation)
        {
            CashOperationEventArgs arg = new CashOperationEventArgs(eventType, cashOperation);
            CashOperationRequest(sender, arg);
        }
        #endregion


        #region 其他事件

        public event EventHandler<ManagerNotifyEventArgs> ManagerNotifyEvent = delegate { };

        public void FireManagerNotifyEvent(object sender, ManagerNotifyEventArgs arg)
        {
            ManagerNotifyEvent(sender, arg);
        }
        #endregion


        #region 底层交易接口出入金回报事件

        public event EventHandler<BrokerTransferEventArgs> BrokerTransferEvent = delegate { };
        /// <summary>
        /// 主帐户出入金操作回报事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void FireBrokerTransferEvent(object sender, BrokerTransferEventArgs arg)
        {
            BrokerTransferEvent(sender, arg);
        }

        public event EventHandler<BrokerAccountInfoEventArgs> BrokerAccountInfoEvent = delegate { };
        /// <summary>
        /// 主帐户交易帐户回报事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void FireBrokerAccountInfoEvent(object sender, BrokerAccountInfoEventArgs arg)
        {
            BrokerAccountInfoEvent(sender, arg);
        }

        #endregion

    }
}
