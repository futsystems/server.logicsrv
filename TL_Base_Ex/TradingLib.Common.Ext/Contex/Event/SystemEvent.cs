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
    }
}
