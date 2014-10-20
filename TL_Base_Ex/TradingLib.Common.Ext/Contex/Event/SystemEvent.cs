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

        /// <summary>
        /// 结算前事件 在结算前触发
        /// </summary>
        public event EventHandler<SystemEventArgs> BeforeSettleEvent;


        /// <summary>
        /// 结算后事件 在系统结算完毕后触发
        /// </summary>
        public event EventHandler<SystemEventArgs> AfterSettleEvent;

        /// <summary>
        /// 结算重置前事件 在结算重置前触发
        /// </summary>
        public event EventHandler<SystemEventArgs> BeforeSettleResetEvent;


        /// <summary>
        /// 结算重置后事件 在结算重置后触发
        /// </summary>
        public event EventHandler<SystemEventArgs> AfterSettleResetEvent;

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

        internal void FireBeforeSettleResetEvent(object sender, SystemEventArgs args)
        {
            if (BeforeSettleResetEvent != null)
                BeforeSettleResetEvent(sender, args);
        }

        internal void FireAfterSettleResetEvent(object sender, SystemEventArgs args)
        {
            if (AfterSettleResetEvent != null)
                AfterSettleResetEvent(sender, args);
        }
    }
}
