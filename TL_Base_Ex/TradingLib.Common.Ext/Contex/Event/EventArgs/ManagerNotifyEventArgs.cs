using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 向管理端发送通知事件
    /// 接口侧消息通知
    /// </summary>
    public class ManagerNotifyEventArgs : EventArgs
    {
        public ManagerNotifyEventArgs(Predicate<Manager> predicate, ManagerNotify notify)
        {
            this.Notify = notify;
            this.NotifyPredicate = predicate;
        }

        /// <summary>
        /// 通知管理端过滤谓词
        /// </summary>
        public Predicate<Manager> NotifyPredicate { get; set; }

        /// <summary>
        /// 管理端通知
        /// </summary>
        public ManagerNotify Notify { get; set; }
    }
}